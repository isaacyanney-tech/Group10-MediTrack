' ============================================================================
'  MainForm.vb  -  MediTrack Pharmacy Inventory System (main window)
'
'  Layout: MainForm.Designer.vb (open MainForm.vb in Design view).
'  Menu bar (File, View, Help) and eight tabs:
'
'    Dashboard          (Admin) sales this month, stock value, losses, chart
'    Counter (Sales)            sell drugs, take them out of stock, print receipt
'    Inventory                  add / update drugs, delete (Admin)
'    Expiry Tracker             drugs sorted by days left, write off (Admin)
'    Suppliers          (Admin) add / update / delete suppliers
'    History Log        (Admin) every action, search and filter
'    Sales Records      (Admin) receipts by date, reprint, export
'    Users and Settings (Admin) user accounts, pharmacy details, access code, backup
'
'  Every change is saved in the Access database through DataStore.vb:
'    1. check the input   2. save it (DataStore)   3. RefreshAll() redraws the tabs
' ============================================================================
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Text
Imports System.Windows.Forms

Public Class MainForm

    ''' <summary>True when the user chose Log Out (the login window then comes back).</summary>
    Public Property LoggedOut As Boolean = False

    Private Const ExpiryWarningDays As Integer = 90
    Private Const ExpiryCriticalDays As Integer = 30

    Private ReadOnly ChartBlue As Color = Color.FromArgb(46, 117, 182)
    Private ReadOnly ChartLight As Color = Color.FromArgb(157, 195, 230)
    Private ReadOnly RowRed As Color = Color.FromArgb(255, 214, 218)
    Private ReadOnly RowOrange As Color = Color.FromArgb(255, 228, 196)
    Private ReadOnly RowYellow As Color = Color.FromArgb(255, 246, 196)
    Private ReadOnly GoodGreen As Color = Color.FromArgb(84, 130, 53)
    Private ReadOnly BadRed As Color = Color.FromArgb(192, 57, 43)
    Private ReadOnly VoidedFont As New Font("Segoe UI", 9.75F, FontStyle.Strikeout)

    Private cart As New List(Of SaleItem)      ' drugs in the sale being made at the counter
    Private selectedDrugId As Integer = 0       ' drug being edited (0 = none)
    Private selectedSupplierId As Integer = 0   ' supplier being edited (0 = none)
    Private selectedUserId As Integer = 0       ' user account being edited (0 = none)
    Private isLoading As Boolean = False        ' stops events firing while we fill controls
    Private formReady As Boolean = False

    Private closeConfirmed As Boolean = False   ' True once the user has agreed to close / log out
    Private syncCounter As Integer = 0         ' counts clock ticks between database checks
    Private syncing As Boolean = False

    Private themeTargets As List(Of Action(Of Color))   ' places painted in the business colour

    Private chartDays(6) As Date
    Private chartValues(6) As Decimal

    ' ========================================================================
    '  FORM LOAD
    ' ========================================================================
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim admin As Boolean = DataStore.IsAdmin()
        Dim roleName As String = If(admin, "Admin / Manager", "Counter Staff")

        lblWelcomeUser.Text = "Welcome, " & DataStore.CurrentUser & "  (" & roleName & ")"
        lblUserInfo.Text = "User: " & DataStore.CurrentUser
        themeTargets = Branding.FindThemed(Me)      ' remember what to repaint in the business colour
        ApplyBranding()

        ' Counter staff only get Counter, Inventory and Expiry Tracker
        If Not admin Then
            For Each page As TabPage In New TabPage() {pgDashboard, pgSuppliers, pgHistory, pgSales, pgAdmin}
                tabMain.TabPages.Remove(page)
            Next
            For Each item As ToolStripItem In New ToolStripItem() {mnuViewDashboard, mnuViewSuppliers, mnuViewHistory,
                                                                    mnuViewSales, mnuViewAdmin, mnuBackup}
                item.Visible = False
            Next
        End If
        btnDeleteDrug.Visible = admin
        btnWriteOff.Visible = admin
        btnExportInventory.Visible = admin
        ' Only people who signed in with a username can change their own password
        mnuChangePassword.Visible = (DataStore.CurrentUsername <> "")

        isLoading = True
        cboCategory.SelectedIndex = 0
        cboSupplier.SelectedIndex = 0
        cboActionFilter.SelectedIndex = 0
        cboExpiryFilter.SelectedIndex = 0
        cboNewRole.SelectedIndex = 0
        cboPayment.SelectedIndex = 0
        For i As Integer = 1 To cboCategory.Items.Count - 1
            cboInvCategory.Items.Add(cboCategory.Items(i))
        Next
        cboInvCategory.SelectedIndex = 0
        dtpExpiry.Value = Date.Today.AddYears(1)
        dtpSalesFrom.Value = New Date(Date.Today.Year, Date.Today.Month, 1)
        dtpSalesTo.Value = Date.Today
        isLoading = False

        AddToolTips()
        formReady = True
        RefreshAll()
        ShowSettings()
        ShowPage(If(admin, pgDashboard, pgCounter))

        UpdateClock()
        tmrClock.Start()
        SetStatus("Signed in as " & DataStore.CurrentUser)
        lblSyncInfo.Text = "Live: on"
    End Sub

    ' ========================================================================
    '  KEYBOARD SHORTCUTS
    ' ========================================================================
    ''' <summary>F2 = new sale (go to the counter search box), F9 = complete the sale.</summary>
    Private Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.F2 Then
            StartNewSale()
            e.Handled = True
        ElseIf e.KeyCode = Keys.F9 Then
            ShowPage(pgCounter)
            btnCompleteSale.PerformClick()
            e.Handled = True
        End If
    End Sub

    Private Sub StartNewSale()
        ShowPage(pgCounter)
        txtCounterSearch.Clear()
        txtCounterSearch.Focus()
    End Sub

    Private Sub mnuShortcuts_Click(sender As Object, e As EventArgs) Handles mnuShortcuts.Click
        MessageBox.Show("F2  -  New sale (go to the counter search box)" & vbCrLf &
                        "Enter in the search box  -  add the first matching drug" & vbCrLf &
                        "F9  -  Complete the sale" & vbCrLf &
                        "Ctrl+1 ... Ctrl+8  -  open a tab" & vbCrLf &
                        "Ctrl+L  -  Log out",
                        "Keyboard Shortcuts", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub mnuChangePassword_Click(sender As Object, e As EventArgs) Handles mnuChangePassword.Click
        Using f As New ChangePasswordForm()
            f.ShowDialog(Me)
        End Using
    End Sub

    Private Sub AddToolTips()
        tipMain.SetToolTip(txtQuantity, "Total units in stock. When updating, type the NEW total.")
        tipMain.SetToolTip(txtReorder, "Warn me when quantity falls to this level or below.")
        tipMain.SetToolTip(dgvCounterDrugs, "Double-click a drug to add it to the sale.")
        tipMain.SetToolTip(txtAmountPaid, "Cash received. The sale will not complete if it is less than the total.")
        tipMain.SetToolTip(btnWriteOff, "Removes all expired stock and records it as a loss.")
        tipMain.SetToolTip(btnBackup, "Saves a copy of the whole database. A copy is also made automatically every day.")
        tipMain.SetToolTip(txtCounterSearch, "Type a drug name or batch number, then press Enter to add it.")
        tipMain.SetToolTip(txtDiscount, "Money to take off the subtotal, e.g. 2.00")
        tipMain.SetToolTip(btnQtyPlus, "Add one more of the selected line")
        tipMain.SetToolTip(btnQtyMinus, "Take one off the selected line")
        tipMain.SetToolTip(txtNewPassword, "For an existing user, leave empty to keep the current password.")
        tipMain.SetToolTip(txtPharmacyPhone, "One or more numbers. Separate several numbers with a comma.")
        tipMain.SetToolTip(btnChangeLogo, "PNG or JPG. It appears in the header, on the sign-in window, receipts and reports.")
        tipMain.SetToolTip(btnChooseColour, "The colour of the header, sign-in panel, table headings and titles.")
        tipMain.SetToolTip(txtSalesSearch, "Type a customer name or a receipt number.")
        tipMain.SetToolTip(btnVoidSale, "Cancels the selected receipt and puts its items back into stock.")
        tipMain.SetToolTip(btnPrintSummary, "Prints a summary of the sales for the dates chosen (e.g. end of day).")
    End Sub

    ' ========================================================================
    '  MENU BAR AND TABS
    ' ========================================================================
    Private Sub mnuView_Click(sender As Object, e As EventArgs) Handles mnuViewDashboard.Click, mnuViewCounter.Click,
            mnuViewInventory.Click, mnuViewExpiry.Click, mnuViewSuppliers.Click, mnuViewHistory.Click,
            mnuViewSales.Click, mnuViewAdmin.Click
        If sender Is mnuViewDashboard Then ShowPage(pgDashboard)
        If sender Is mnuViewCounter Then ShowPage(pgCounter)
        If sender Is mnuViewInventory Then ShowPage(pgInventory)
        If sender Is mnuViewExpiry Then ShowPage(pgExpiry)
        If sender Is mnuViewSuppliers Then ShowPage(pgSuppliers)
        If sender Is mnuViewHistory Then ShowPage(pgHistory)
        If sender Is mnuViewSales Then ShowPage(pgSales)
        If sender Is mnuViewAdmin Then ShowPage(pgAdmin)
    End Sub

    Private Sub ShowPage(page As TabPage)
        If tabMain.TabPages.Contains(page) Then tabMain.SelectedTab = page
    End Sub

    Private Sub tabMain_SelectedIndexChanged(sender As Object, e As EventArgs) Handles tabMain.SelectedIndexChanged
        If Not formReady Then Return
        If tabMain.SelectedTab Is pgDashboard Then ShowDashboard()
        If tabMain.SelectedTab Is pgSales Then ShowSales()
    End Sub

    Private Sub mnuLogOut_Click(sender As Object, e As EventArgs) Handles mnuLogOut.Click
        If cart.Count > 0 Then
            If MessageBox.Show("There is an unfinished sale at the counter. Log out anyway?", "Log Out",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then Return
        End If
        LoggedOut = True
        closeConfirmed = True
        Me.Close()
    End Sub

    Private Sub mnuExit_Click(sender As Object, e As EventArgs) Handles mnuExit.Click
        Dim question As String = If(cart.Count > 0, "There is an unfinished sale at the counter." & vbCrLf & "Close MediTrack anyway?", "Close MediTrack?")
        If MessageBox.Show(question, "Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            LoggedOut = False
            closeConfirmed = True
            Me.Close()
        End If
    End Sub

    ''' <summary>Closing with the X button: do not lose an unfinished sale by accident.</summary>
    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If closeConfirmed OrElse e.CloseReason <> CloseReason.UserClosing Then Return
        If cart.Count > 0 Then
            If MessageBox.Show("There is an unfinished sale at the counter." & vbCrLf & "Close MediTrack anyway?", "MediTrack",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then
                e.Cancel = True
                Return
            End If
        End If
        tmrClock.Stop()
    End Sub

    ''' <summary>Right after signing in: tell the user about expired, expiring and low stock.</summary>
    Private Sub MainForm_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        If DataStore.IsAdmin() Then
            Try
                Dim weak As List(Of String) = DataStore.WeakDefaultLogins()
                If weak.Count > 0 Then
                    MessageBox.Show("Security warning: " & String.Join(", ", weak.ToArray()) &
                                    " still use the old test password from an earlier version." & vbCrLf & vbCrLf &
                                    "Open Users and Settings and change them (or disable those accounts) before the pharmacy goes live.",
                                    "MediTrack - Security", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    ShowPage(pgAdmin)
                End If
            Catch
            End Try
        End If
        Dim expired As Integer = 0, expiring As Integer = 0, outOfStock As Integer = 0, low As Integer = 0
        For Each d As Drug In DataStore.Drugs
            Dim days As Integer = DataStore.DaysLeft(d)
            If d.Quantity > 0 AndAlso days < 0 Then
                expired += 1
            ElseIf d.Quantity > 0 AndAlso days <= ExpiryCriticalDays Then
                expiring += 1
            End If
            If d.Quantity = 0 Then
                outOfStock += 1
            ElseIf d.Quantity <= d.ReorderLevel Then
                low += 1
            End If
        Next
        If expired + expiring + outOfStock + low = 0 Then Return
        Dim lines As New List(Of String)
        If expired > 0 Then lines.Add("  -  " & expired.ToString() & " drug(s) have EXPIRED and must not be sold")
        If expiring > 0 Then lines.Add("  -  " & expiring.ToString() & " drug(s) expire within " & ExpiryCriticalDays.ToString() & " days")
        If outOfStock > 0 Then lines.Add("  -  " & outOfStock.ToString() & " drug(s) are out of stock")
        If low > 0 Then lines.Add("  -  " & low.ToString() & " drug(s) are at or below the reorder level")
        MessageBox.Show("Stock alerts for today:" & vbCrLf & vbCrLf & String.Join(vbCrLf, lines.ToArray()) & vbCrLf & vbCrLf &
                        "See the Expiry Tracker and Inventory tabs for details.", "MediTrack - Stock Alerts",
                        MessageBoxButtons.OK, If(expired > 0, MessageBoxIcon.Warning, MessageBoxIcon.Information))
    End Sub

    Private Sub mnuAbout_Click(sender As Object, e As EventArgs) Handles mnuAbout.Click
        MessageBox.Show("MediTrack Pharmacy Inventory System" & vbCrLf & "Version 9.0" & vbCrLf & vbCrLf &
                        "Set up for: " & Branding.BusinessName() & vbCrLf & vbCrLf &
                        "Visual Basic .NET (Windows Forms) with a Microsoft Access database." & vbCrLf &
                        "Database: " & Database.DatabasePath,
                        "About MediTrack", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ' ========================================================================
    '  REFRESH - redraw every tab from the data
    ' ========================================================================
    ''' <summary>Shows the business name, logo and colour (they can be changed in Users and Settings).</summary>
    Private Sub ApplyBranding()
        Dim name As String = Branding.BusinessName()
        lblAppTitle.Text = name
        Me.Text = name & "  -  " & If(DataStore.IsAdmin(), "Admin Portal", "Counter") & "  |  MediTrack"
        Branding.ShowLogo(picHeaderLogo, lblLogo)
        If themeTargets IsNot Nothing Then Branding.ApplyTheme(themeTargets, Branding.ThemeColor())
    End Sub

    Private Sub RefreshAll()
        ApplyBranding()
        ShowInventory()
        ShowDrugNames()
        ShowSuppliers()
        ShowCounterDrugs()
        ShowCart()
        ShowExpiry()
        ShowHistory()
        UpdateLowStock()
        If DataStore.IsAdmin() Then
            ShowDashboard()
            ShowSales()
            ShowUsers()
        End If
    End Sub

    ''' <summary>
    ''' Runs a save. If the database reports a problem, shows it, reloads the
    ''' data from the database so the screen matches what is really saved,
    ''' and returns False.
    ''' </summary>
    Private Function TrySave(work As Action, what As String) As Boolean
        Try
            Cursor = Cursors.WaitCursor
            work()
            ' Reload so this window shows exactly what is saved, including
            ' anything another computer saved a moment ago.
            Try
                DataStore.LoadAll()
                RefreshAll()
            Catch
            End Try
            Return True
        Catch ex As Exception
            MessageBox.Show("Could not " & what & "." & vbCrLf & vbCrLf & ex.Message, "MediTrack",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Try
                DataStore.LoadAll()
                RefreshAll()
            Catch
            End Try
            SetStatus("Error: could not " & what)
            Return False
        Finally
            Cursor = Cursors.Default
        End Try
    End Function

    ' ========================================================================
    '  DASHBOARD (Admin)
    ' ========================================================================
    Private Sub ShowDashboard()
        Dim today As Date = Date.Today
        Dim monthStart As New Date(today.Year, today.Month, 1)

        Dim todayTotal As Decimal = 0, monthTotal As Decimal = 0
        Dim todayCount As Integer = 0, salesThisMonth As Integer = 0, itemsSold As Integer = 0
        Dim qtyByDrug As New Dictionary(Of String, Integer)
        Dim moneyByDrug As New Dictionary(Of String, Decimal)

        For Each s As Sale In DataStore.SaleHeaders
            If s.Voided Then Continue For
            If s.SaleDate >= monthStart Then
                monthTotal += s.Total
                salesThisMonth += 1
            End If
            If s.SaleDate.Date = today Then
                todayTotal += s.Total
                todayCount += 1
            End If
        Next
        For Each item As SaleItem In DataStore.Sales
            If DataStore.IsVoided(item.SaleID) Then Continue For
            If item.SaleDate >= monthStart Then
                itemsSold += item.Quantity
                If Not qtyByDrug.ContainsKey(item.DrugName) Then
                    qtyByDrug(item.DrugName) = 0
                    moneyByDrug(item.DrugName) = 0
                End If
                qtyByDrug(item.DrugName) += item.Quantity
                moneyByDrug(item.DrugName) += item.Total
            End If
        Next

        Dim stockValue As Decimal = 0, expiredOnShelf As Decimal = 0
        Dim drugsInStock As Integer = 0, expiringSoon As Integer = 0
        Dim reorder As New List(Of Drug)
        For Each d As Drug In DataStore.Drugs
            Dim days As Integer = DataStore.DaysLeft(d)
            If days < 0 Then
                expiredOnShelf += d.Quantity * d.UnitPrice
            Else
                stockValue += d.Quantity * d.UnitPrice
                If d.Quantity > 0 Then drugsInStock += 1
                If d.Quantity > 0 AndAlso days <= ExpiryCriticalDays Then expiringSoon += 1
                If d.Quantity <= d.ReorderLevel Then reorder.Add(d)
            End If
        Next
        Dim writtenOff As Decimal = 0
        For Each l As LossRecord In DataStore.Losses
            writtenOff += l.Value
        Next

        lblKpiTodayValue.Text = Money(todayTotal)
        lblKpiTodayNote.Text = todayCount.ToString() & " receipt(s) today"
        lblKpiMonthSalesValue.Text = Money(monthTotal)
        lblKpiMonthSalesNote.Text = today.ToString("MMMM yyyy")
        lblKpiItemsSoldValue.Text = itemsSold.ToString("N0")
        lblKpiItemsSoldNote.Text = "units sold since " & monthStart.ToString("d MMM")
        lblKpiStockValueValue.Text = Money(stockValue)
        lblKpiStockValueNote.Text = drugsInStock.ToString() & " drugs in stock (not expired)"
        lblKpiTransactionsValue.Text = salesThisMonth.ToString("N0")
        lblKpiTransactionsNote.Text = If(salesThisMonth > 0, "average " & Money(monthTotal / salesThisMonth) & " per receipt", "no sales yet")
        lblKpiLossValue.Text = Money(expiredOnShelf + writtenOff)
        lblKpiLossNote.Text = Money(expiredOnShelf) & " on shelf, " & Money(writtenOff) & " written off"
        lblKpiLowStockValue.Text = reorder.Count.ToString()
        lblKpiLowStockNote.Text = "at or below reorder level"
        lblKpiExpiringValue.Text = expiringSoon.ToString()
        lblKpiExpiringNote.Text = "see the Expiry Tracker"

        Dim names As New List(Of String)(qtyByDrug.Keys)
        names.Sort(Function(a, b) qtyByDrug(b).CompareTo(qtyByDrug(a)))
        dgvTopDrugs.Rows.Clear()
        For i As Integer = 0 To Math.Min(names.Count, 10) - 1
            dgvTopDrugs.Rows.Add(names(i), qtyByDrug(names(i)), moneyByDrug(names(i)))
        Next
        dgvTopDrugs.ClearSelection()

        ' Reorder list: the drugs with the least stock first, with the supplier to call
        reorder.Sort(Function(a, b) a.Quantity.CompareTo(b.Quantity))
        dgvReorder.Rows.Clear()
        For Each d As Drug In reorder
            Dim s As Supplier = DataStore.FindSupplier(d.SupplierName)
            dgvReorder.Rows.Add(d.DrugName, d.Quantity, d.ReorderLevel, d.SupplierName, If(s Is Nothing, d.SupplierContact, s.Phone))
        Next
        dgvReorder.ClearSelection()

        For i As Integer = 0 To 6
            chartDays(i) = today.AddDays(i - 6)
            chartValues(i) = 0
        Next
        For Each s As Sale In DataStore.SaleHeaders
            If s.Voided Then Continue For
            Dim index As Integer = (s.SaleDate.Date - chartDays(0)).Days
            If index >= 0 AndAlso index <= 6 Then chartValues(index) += s.Total
        Next
        pnlChart.Invalidate()
    End Sub

    ''' <summary>Draws the 7-day bar chart.</summary>
    Private Sub pnlChart_Paint(sender As Object, e As PaintEventArgs) Handles pnlChart.Paint
        Dim g As Graphics = e.Graphics
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias
        Dim w As Integer = pnlChart.ClientSize.Width
        Dim h As Integer = pnlChart.ClientSize.Height
        Dim top As Integer = 24
        Dim bottom As Integer = h - 26
        Dim slot As Single = CSng(w / 7)

        Dim maxValue As Decimal = 1
        For Each v As Decimal In chartValues
            If v > maxValue Then maxValue = v
        Next

        Using center As New StringFormat()
            center.Alignment = StringAlignment.Center
            Using smallFont As New Font("Segoe UI", 8.25F), boldFont As New Font("Segoe UI", 8.25F, FontStyle.Bold),
                  barBrush As New SolidBrush(ChartLight), todayBrush As New SolidBrush(ChartBlue),
                  textBrush As New SolidBrush(Color.FromArgb(91, 101, 115)), linePen As New Pen(Color.FromArgb(217, 225, 234))
                g.DrawLine(linePen, 0, bottom, w, bottom)
                For i As Integer = 0 To 6
                    Dim barHeight As Single = CSng(chartValues(i) / maxValue) * (bottom - top)
                    Dim barWidth As Single = slot * 0.55F
                    Dim x As Single = i * slot + (slot - barWidth) / 2
                    g.FillRectangle(If(i = 6, todayBrush, barBrush), x, bottom - barHeight, barWidth, barHeight)
                    g.DrawString(chartValues(i).ToString("N0"), boldFont, textBrush,
                                 New RectangleF(i * slot, bottom - barHeight - 18, slot, 16), center)
                    g.DrawString(If(i = 6, "Today", chartDays(i).ToString("ddd d")), smallFont, textBrush,
                                 New RectangleF(i * slot, bottom + 5, slot, 18), center)
                Next
            End Using
        End Using
    End Sub

    Private Sub pnlChart_Resize(sender As Object, e As EventArgs) Handles pnlChart.Resize
        pnlChart.Invalidate()
    End Sub

    ' ========================================================================
    '  COUNTER (selling drugs)
    ' ========================================================================
    Private Sub ShowCounterDrugs()
        Dim search As String = txtCounterSearch.Text.Trim().ToLower()
        Dim found As New List(Of Drug)
        For Each d As Drug In DataStore.Drugs
            If d.Quantity > 0 AndAlso d.ExpiryDate.Date >= Date.Today Then
                If search = "" OrElse d.DrugName.ToLower().Contains(search) OrElse d.BatchNo.ToLower().Contains(search) Then found.Add(d)
            End If
        Next
        ' Same drug name: the batch that expires first is listed first (sell it first)
        found.Sort(Function(a, b)
                       Dim byName As Integer = String.Compare(a.DrugName, b.DrugName, True)
                       If byName <> 0 Then Return byName
                       Return a.ExpiryDate.CompareTo(b.ExpiryDate)
                   End Function)
        dgvCounterDrugs.Rows.Clear()
        For Each d As Drug In found
            dgvCounterDrugs.Rows.Add(d.ID, d.DrugName, d.BatchNo, d.Quantity, d.UnitPrice, d.ExpiryDate)
        Next
        dgvCounterDrugs.ClearSelection()
        If search <> "" AndAlso dgvCounterDrugs.Rows.Count > 0 Then dgvCounterDrugs.Rows(0).Selected = True
    End Sub

    ''' <summary>Enter in the search box adds the first matching drug (works with barcode scanners too).</summary>
    Private Sub txtCounterSearch_KeyDown(sender As Object, e As KeyEventArgs) Handles txtCounterSearch.KeyDown
        If e.KeyCode = Keys.Enter Then
            e.SuppressKeyPress = True
            If dgvCounterDrugs.Rows.Count = 0 Then
                SetStatus("No drug matches '" & txtCounterSearch.Text.Trim() & "'")
                Return
            End If
            If dgvCounterDrugs.SelectedRows.Count = 0 Then dgvCounterDrugs.Rows(0).Selected = True
            AddSelectedDrugToCart()
            txtCounterSearch.SelectAll()
        ElseIf e.KeyCode = Keys.Down AndAlso dgvCounterDrugs.Rows.Count > 0 Then
            dgvCounterDrugs.Focus()
        End If
    End Sub

    Private Sub txtCounterSearch_TextChanged(sender As Object, e As EventArgs) Handles txtCounterSearch.TextChanged
        If formReady Then ShowCounterDrugs()
    End Sub

    Private Sub btnAddToCart_Click(sender As Object, e As EventArgs) Handles btnAddToCart.Click
        AddSelectedDrugToCart()
    End Sub

    Private Sub dgvCounterDrugs_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvCounterDrugs.CellDoubleClick
        If e.RowIndex >= 0 Then AddSelectedDrugToCart()
    End Sub

    Private Sub AddSelectedDrugToCart()
        If dgvCounterDrugs.SelectedRows.Count = 0 Then
            MessageBox.Show("Click a drug in the list first.", "Counter", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim d As Drug = DataStore.FindDrugById(ToInt(dgvCounterDrugs.SelectedRows(0).Cells(colCdID.Index).Value))
        If d Is Nothing Then Return
        Dim qty As Integer = CInt(nudQty.Value)

        Dim line As SaleItem = Nothing
        For Each item As SaleItem In cart
            If item.DrugID = d.ID Then line = item
        Next
        Dim alreadyInCart As Integer = If(line Is Nothing, 0, line.Quantity)
        If alreadyInCart + qty > d.Quantity Then
            MessageBox.Show("Only " & d.Quantity.ToString() & " of " & d.DrugName & " in stock" &
                            If(alreadyInCart > 0, " (" & alreadyInCart.ToString() & " already in this sale).", "."),
                            "Not Enough Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If line Is Nothing Then
            line = New SaleItem()
            line.DrugID = d.ID
            line.DrugName = d.DrugName
            line.BatchNo = d.BatchNo
            line.UnitPrice = d.UnitPrice
            line.Quantity = qty
            cart.Add(line)
        Else
            line.Quantity += qty
        End If
        nudQty.Value = 1
        ShowCart()
        SetStatus("Added " & qty.ToString() & " x " & d.DrugName & " to the sale")
    End Sub

    Private Sub ShowCart()
        Dim selectedLine As Integer = If(dgvCart.SelectedRows.Count > 0, dgvCart.SelectedRows(0).Index, -1)
        dgvCart.Rows.Clear()
        For Each item As SaleItem In cart
            dgvCart.Rows.Add(item.DrugID, item.DrugName, item.Quantity, item.UnitPrice, item.Total)
        Next
        dgvCart.ClearSelection()
        If selectedLine >= 0 AndAlso selectedLine < dgvCart.Rows.Count Then dgvCart.Rows(selectedLine).Selected = True
        lblSubtotal.Text = Money(CartSubtotal())
        lblCartTotal.Text = Money(AmountDue())
        UpdateChange()
    End Sub

    ''' <summary>The +/- buttons change the quantity of the selected line in the sale.</summary>
    Private Sub CartQty_Click(sender As Object, e As EventArgs) Handles btnQtyPlus.Click, btnQtyMinus.Click
        If dgvCart.SelectedRows.Count = 0 Then
            MessageBox.Show("Click a line in the sale first.", "Change Quantity", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim line As SaleItem = cart(dgvCart.SelectedRows(0).Index)
        If sender Is btnQtyPlus Then
            Dim d As Drug = DataStore.FindDrugById(line.DrugID)
            If d Is Nothing OrElse line.Quantity + 1 > d.Quantity Then
                SetStatus("No more " & line.DrugName & " in stock")
                Return
            End If
            line.Quantity += 1
        ElseIf line.Quantity > 1 Then
            line.Quantity -= 1
        End If
        ShowCart()
    End Sub

    ''' <summary>Total of all lines before any discount.</summary>
    Private Function CartSubtotal() As Decimal
        Dim total As Decimal = 0
        For Each item As SaleItem In cart
            total += item.Total
        Next
        Return total
    End Function

    ''' <summary>The discount typed in (0 if empty or not a number).</summary>
    Private Function CartDiscount() As Decimal
        Dim d As Decimal
        If Decimal.TryParse(txtDiscount.Text.Trim(), d) AndAlso d > 0 Then Return d
        Return 0D
    End Function

    ''' <summary>What the customer has to pay: subtotal minus discount.</summary>
    Private Function AmountDue() As Decimal
        Return Math.Max(0D, CartSubtotal() - CartDiscount())
    End Function

    Private Sub txtDiscount_TextChanged(sender As Object, e As EventArgs) Handles txtDiscount.TextChanged
        lblCartTotal.Text = Money(AmountDue())
        UpdateChange()
    End Sub

    Private Sub cboPayment_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboPayment.SelectedIndexChanged
        ' Mobile Money and Card are paid exactly, so there is no change to give
        Dim cash As Boolean = (cboPayment.SelectedIndex <= 0)
        txtAmountPaid.Enabled = cash
        If Not cash Then txtAmountPaid.Clear()
        UpdateChange()
    End Sub

    Private Sub UpdateChange()
        Dim paid As Decimal
        If Decimal.TryParse(txtAmountPaid.Text.Trim(), paid) AndAlso cart.Count > 0 Then
            Dim change As Decimal = paid - AmountDue()
            If change >= 0 Then
                lblChange.Text = "Change: " & Money(change)
                lblChange.ForeColor = GoodGreen
            Else
                lblChange.Text = "Short by: " & Money(-change)
                lblChange.ForeColor = BadRed
            End If
        Else
            lblChange.Text = "Change: " & Money(0)
            lblChange.ForeColor = GoodGreen
        End If
    End Sub

    Private Sub txtAmountPaid_TextChanged(sender As Object, e As EventArgs) Handles txtAmountPaid.TextChanged
        UpdateChange()
    End Sub

    Private Sub btnRemoveItem_Click(sender As Object, e As EventArgs) Handles btnRemoveItem.Click
        If dgvCart.SelectedRows.Count = 0 Then
            MessageBox.Show("Click a line in the sale first.", "Remove Item", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        cart.RemoveAt(dgvCart.SelectedRows(0).Index)
        ShowCart()
    End Sub

    Private Sub btnClearCart_Click(sender As Object, e As EventArgs) Handles btnClearCart.Click
        If cart.Count = 0 Then Return
        If MessageBox.Show("Remove everything from this sale?", "Clear Sale",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then Return
        cart.Clear()
        txtCustomer.Clear()
        txtAmountPaid.Clear()
        txtDiscount.Clear()
        cboPayment.SelectedIndex = 0
        ShowCart()
    End Sub

    ''' <summary>Saves the sale in the database (stock goes down), then shows/prints the receipt.</summary>
    Private Sub btnCompleteSale_Click(sender As Object, e As EventArgs) Handles btnCompleteSale.Click
        If cart.Count = 0 Then
            MessageBox.Show("The sale is empty. Add a drug first.", "Complete Sale", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim discount As Decimal = CartDiscount()
        If discount > CartSubtotal() Then
            Fail(txtDiscount, "The discount cannot be more than the subtotal (" & Money(CartSubtotal()) & ").")
            Return
        End If
        Dim total As Decimal = AmountDue()
        Dim paid As Decimal = total
        Dim method As String = If(cboPayment.SelectedItem Is Nothing, "Cash", cboPayment.SelectedItem.ToString())
        If method = "Cash" Then
            ' Cash: the amount received must be typed in and must cover the total
            If txtAmountPaid.Text.Trim() = "" Then
                Fail(txtAmountPaid, "Please type the amount of cash received from the customer.")
                Return
            End If
            If Not Decimal.TryParse(txtAmountPaid.Text.Trim(), paid) Then
                Fail(txtAmountPaid, "Amount paid must be a number, e.g. 50.00")
                Return
            End If
            If paid < total Then
                Fail(txtAmountPaid, "Not enough cash. The total is " & Money(total) & " but only " & Money(paid) &
                     " was given." & vbCrLf & "The customer still needs to pay " & Money(total - paid) & ".")
                Return
            End If
        End If
        For Each item As SaleItem In cart
            Dim d As Drug = DataStore.FindDrugById(item.DrugID)
            If d Is Nothing OrElse d.Quantity < item.Quantity Then
                MessageBox.Show("There is not enough " & item.DrugName & " in stock any more. Please remove it from the sale.",
                                "Not Enough Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
        Next

        Dim customer As String = If(txtCustomer.Text.Trim() = "", "Walk-in", txtCustomer.Text.Trim())
        Dim saved As Sale = Nothing
        Dim itemsSold As New List(Of SaleItem)(cart)
        If Not TrySave(Sub() saved = DataStore.RecordSale(itemsSold, customer, paid, discount, method), "save the sale") Then
            ShowCart()       ' nothing was saved; the sale stays on screen so it can be fixed
            Return
        End If

        Dim lowAfterSale As New List(Of String)
        For Each item As SaleItem In itemsSold
            Dim d As Drug = DataStore.FindDrugById(item.DrugID)
            If d IsNot Nothing AndAlso d.Quantity <= d.ReorderLevel Then lowAfterSale.Add(d.DrugName & " (" & d.Quantity.ToString() & " left)")
        Next

        cart = New List(Of SaleItem)
        txtCustomer.Clear()
        txtAmountPaid.Clear()
        txtDiscount.Clear()
        cboPayment.SelectedIndex = 0
        RefreshAll()
        txtCounterSearch.Clear()
        txtCounterSearch.Focus()
        SetStatus("Sale #" & saved.SaleID.ToString() & " saved: " & Money(saved.Total))

        If chkPrintReceipt.Checked Then
            PrintReceipt(saved)
        Else
            MessageBox.Show("Sale #" & saved.SaleID.ToString() & " saved. Total: " & Money(saved.Total) & vbCrLf &
                            "Change: " & Money(saved.AmountPaid - saved.Total), "Sale Completed",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        If lowAfterSale.Count > 0 Then
            MessageBox.Show("These drugs are now low in stock:" & vbCrLf & "  - " &
                            String.Join(vbCrLf & "  - ", lowAfterSale.ToArray()),
                            "Low Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    ' ========================================================================
    '  RECEIPTS  (see ReceiptForm.vb and ReceiptPrinter.vb)
    ' ========================================================================
    ''' <summary>Opens the receipt window, where the receipt can be printed.</summary>
    Private Sub PrintReceipt(s As Sale)
        Using f As New ReceiptForm(s)
            f.ShowDialog(Me)
        End Using
    End Sub

    ' ========================================================================
    '  INVENTORY
    ' ========================================================================
    Private Sub ShowInventory()
        Dim search As String = txtInvSearch.Text.Trim().ToLower()
        Dim category As String = If(cboInvCategory.SelectedIndex <= 0, "", cboInvCategory.Text)
        Dim sorted As New List(Of Drug)(DataStore.Drugs)
        sorted.Sort(Function(a, b) String.Compare(a.DrugName, b.DrugName, True))
        dgvInventory.Rows.Clear()
        Dim totalValue As Decimal = 0
        For Each d As Drug In sorted
            If search <> "" AndAlso Not (d.DrugName.ToLower().Contains(search) OrElse d.BatchNo.ToLower().Contains(search) OrElse
                                         d.SupplierName.ToLower().Contains(search)) Then Continue For
            If category <> "" AndAlso d.Category <> category Then Continue For
            If chkLowOnly.Checked AndAlso d.Quantity > d.ReorderLevel Then Continue For
            dgvInventory.Rows.Add(d.ID, d.DrugName, d.Category, d.Quantity, d.UnitPrice, d.BatchNo,
                                  d.ExpiryDate, d.SupplierName, d.SupplierContact, d.ReorderLevel)
            totalValue += d.Quantity * d.UnitPrice
        Next
        dgvInventory.ClearSelection()
        lblInvCount.Text = "Showing " & dgvInventory.Rows.Count.ToString() & " of " & DataStore.Drugs.Count.ToString() &
                           " drugs   |   Value: " & Money(totalValue)
    End Sub

    Private Sub InventoryFilter_Changed(sender As Object, e As EventArgs) _
            Handles txtInvSearch.TextChanged, cboInvCategory.SelectedIndexChanged, chkLowOnly.CheckedChanged
        If formReady Then ShowInventory()
    End Sub

    Private Sub dgvInventory_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvInventory.CellFormatting
        If e.RowIndex < 0 OrElse e.RowIndex >= dgvInventory.Rows.Count Then Return
        Dim row As DataGridViewRow = dgvInventory.Rows(e.RowIndex)
        If row.Cells(colExpiryDate.Index).Value Is Nothing Then Return
        Dim qty As Integer = ToInt(row.Cells(colQuantity.Index).Value)
        Dim reorder As Integer = ToInt(row.Cells(colReorderLevel.Index).Value)
        Dim expiry As Date = CDate(row.Cells(colExpiryDate.Index).Value)
        If expiry < Date.Today Then
            e.CellStyle.BackColor = RowRed
        ElseIf qty <= reorder Then
            e.CellStyle.BackColor = RowYellow
        ElseIf expiry <= Date.Today.AddDays(ExpiryWarningDays) Then
            e.CellStyle.BackColor = RowOrange
        End If
    End Sub

    Private Sub ShowDrugNames()
        Dim current As String = cboDrugName.Text
        cboDrugName.Items.Clear()
        For Each d As Drug In DataStore.Drugs
            If Not cboDrugName.Items.Contains(d.DrugName) Then cboDrugName.Items.Add(d.DrugName)
        Next
        cboDrugName.Sorted = True
        cboDrugName.Text = current
    End Sub

    Private Sub btnAddDrug_Click(sender As Object, e As EventArgs) Handles btnAddDrug.Click
        Dim qty As Integer, reorder As Integer
        Dim price As Decimal
        If Not ValidateDrugInput(qty, price, reorder) Then Return
        Dim name As String = cboDrugName.Text.Trim()
        Dim batch As String = txtBatch.Text.Trim()
        If DataStore.FindDrug(name, batch) IsNot Nothing Then
            MessageBox.Show("'" & name & "' with batch " & batch & " already exists." & vbCrLf &
                            "Click it in the table and use 'Update / Stock In' instead.",
                            "Duplicate Drug", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim category As String = cboCategory.Text
        Dim supplier As String = cboSupplier.Text
        Dim contact As String = txtSupplierContact.Text.Trim()
        Dim expiry As Date = dtpExpiry.Value.Date

        If Not TrySave(Sub()
                           DataStore.AddDrug(name, category, qty, price, batch, expiry, supplier, contact, reorder)
                           DataStore.AddHistory(Date.Now, name, "Added", qty, supplier, batch)
                       End Sub, "add the drug") Then Return

        ClearDrugFields()
        RefreshAll()
        SetStatus("Drug added: " & name)
        MessageBox.Show(name & " was added to the inventory.", "Drug Added", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub btnStockIn_Click(sender As Object, e As EventArgs) Handles btnStockIn.Click
        Dim qty As Integer, reorder As Integer
        Dim price As Decimal
        If Not ValidateDrugInput(qty, price, reorder) Then Return
        Dim name As String = cboDrugName.Text.Trim()
        Dim batch As String = txtBatch.Text.Trim()
        Dim d As Drug = DataStore.FindDrugById(selectedDrugId)
        If d Is Nothing Then d = DataStore.FindDrug(name, batch)
        If d Is Nothing Then
            MessageBox.Show("Please click the drug you want to update in the table first." & vbCrLf &
                            "(To save a new drug, use 'Add Drug'.)",
                            "No Drug Selected", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim other As Drug = DataStore.FindDrug(name, batch)
        If other IsNot Nothing AndAlso other.ID <> d.ID Then
            MessageBox.Show("Another drug already uses that name and batch number.", "Duplicate Drug",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' The user types the NEW total, so the change = new total - old total
        Dim difference As Integer = qty - d.Quantity
        If Not DataStore.IsAdmin() Then
            ' Counter staff can receive stock, but only an admin can change prices or reduce stock
            If price <> d.UnitPrice Then
                Fail(txtUnitPrice, "Only an admin can change the price of a drug." & vbCrLf &
                     "The current price is " & Money(d.UnitPrice) & ".")
                txtUnitPrice.Text = d.UnitPrice.ToString("0.00")
                Return
            End If
            If difference < 0 Then
                Fail(txtQuantity, "Only an admin can reduce the stock." & vbCrLf &
                     "Type a number of " & d.Quantity.ToString() & " or more, or ask an admin.")
                Return
            End If
        End If
        Dim action As String
        If difference > 0 Then
            action = "Stock In"
        ElseIf difference < 0 Then
            action = "Stock Out"
        Else
            action = "Updated"
        End If

        d.DrugName = name
        d.Category = cboCategory.Text
        d.Quantity = qty
        d.UnitPrice = price
        d.BatchNo = batch
        d.ExpiryDate = dtpExpiry.Value.Date
        d.SupplierName = cboSupplier.Text
        d.SupplierContact = txtSupplierContact.Text.Trim()
        d.ReorderLevel = reorder
        Dim supplierName As String = d.SupplierName
        If Not TrySave(Sub()
                           DataStore.UpdateDrug(d, difference)
                           DataStore.AddHistory(Date.Now, name, action, difference, supplierName, batch)
                       End Sub, "update the drug") Then Return

        ClearDrugFields()
        RefreshAll()
        Dim change As String = If(difference > 0, "+" & difference.ToString(), difference.ToString())
        SetStatus(action & ": " & name & " (" & change & ")")
        MessageBox.Show(name & " was updated." & vbCrLf & "Recorded as: " & action & " (" & change & ")",
                        "Stock Updated", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub btnDeleteDrug_Click(sender As Object, e As EventArgs) Handles btnDeleteDrug.Click
        Dim d As Drug = DataStore.FindDrugById(selectedDrugId)
        If d Is Nothing Then
            MessageBox.Show("Click the drug you want to delete in the table first.", "Delete Drug",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        If MessageBox.Show("Delete " & d.DrugName & " (batch " & d.BatchNo & ") from the inventory?" & vbCrLf &
                           "Past sales of this drug stay in the records.",
                           "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then Return
        If Not TrySave(Sub()
                           DataStore.DeleteDrug(d)
                           DataStore.AddHistory(Date.Now, d.DrugName, "Deleted", -d.Quantity, d.SupplierName, d.BatchNo)
                       End Sub, "delete the drug") Then Return
        ClearDrugFields()
        RefreshAll()
        SetStatus("Deleted: " & d.DrugName)
    End Sub

    Private Function ValidateDrugInput(ByRef qty As Integer, ByRef price As Decimal, ByRef reorder As Integer) As Boolean
        If cboDrugName.Text.Trim() = "" Then Return Fail(cboDrugName, "Please enter the drug name.")
        If cboCategory.SelectedIndex <= 0 Then Return Fail(cboCategory, "Please select a category.")
        If Not Integer.TryParse(txtQuantity.Text.Trim(), qty) OrElse qty < 0 Then
            Return Fail(txtQuantity, "Quantity must be a whole number (0 or more).")
        End If
        If Not Decimal.TryParse(txtUnitPrice.Text.Trim(), price) OrElse price <= 0D Then
            Return Fail(txtUnitPrice, "Unit price must be a number greater than 0, e.g. 2.50")
        End If
        If txtBatch.Text.Trim() = "" Then Return Fail(txtBatch, "Please enter the batch/lot number.")
        If cboSupplier.SelectedIndex <= 0 Then Return Fail(cboSupplier, "Please select a supplier.")
        If Not Integer.TryParse(txtReorder.Text.Trim(), reorder) OrElse reorder < 0 Then
            Return Fail(txtReorder, "Reorder level must be a whole number (0 or more).")
        End If
        If dtpExpiry.Value.Date <= Date.Today Then
            If MessageBox.Show("The expiry date is today or in the past. Save anyway?", "Expiry Date",
                               MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then
                dtpExpiry.Focus()
                Return False
            End If
        End If
        Return True
    End Function

    Private Sub btnClearFields_Click(sender As Object, e As EventArgs) Handles btnClearFields.Click
        ClearDrugFields()
    End Sub

    Private Sub ClearDrugFields()
        isLoading = True
        selectedDrugId = 0
        cboDrugName.Text = ""
        cboCategory.SelectedIndex = 0
        txtQuantity.Clear()
        txtUnitPrice.Clear()
        txtBatch.Clear()
        dtpExpiry.Value = Date.Today.AddYears(1)
        If cboSupplier.Items.Count > 0 Then cboSupplier.SelectedIndex = 0
        txtSupplierContact.Clear()
        txtReorder.Clear()
        dgvInventory.ClearSelection()
        isLoading = False
    End Sub

    Private Sub dgvInventory_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvInventory.CellClick
        If e.RowIndex < 0 Then Return
        LoadDrugIntoFields(ToInt(dgvInventory.Rows(e.RowIndex).Cells(colDrugID.Index).Value))
    End Sub

    Private Sub LoadDrugIntoFields(drugId As Integer)
        Dim d As Drug = DataStore.FindDrugById(drugId)
        If d Is Nothing Then Return
        isLoading = True
        selectedDrugId = d.ID
        cboDrugName.Text = d.DrugName
        If d.Category <> "" AndAlso Not cboCategory.Items.Contains(d.Category) Then cboCategory.Items.Add(d.Category)
        If d.Category <> "" Then cboCategory.SelectedItem = d.Category Else cboCategory.SelectedIndex = 0
        txtQuantity.Text = d.Quantity.ToString()
        txtUnitPrice.Text = d.UnitPrice.ToString("0.00")
        txtBatch.Text = d.BatchNo
        If d.ExpiryDate >= dtpExpiry.MinDate AndAlso d.ExpiryDate <= dtpExpiry.MaxDate Then dtpExpiry.Value = d.ExpiryDate
        If cboSupplier.Items.Contains(d.SupplierName) Then
            cboSupplier.SelectedItem = d.SupplierName
        Else
            cboSupplier.SelectedIndex = 0
        End If
        txtSupplierContact.Text = d.SupplierContact
        txtReorder.Text = d.ReorderLevel.ToString()
        isLoading = False
        SetStatus("Editing: " & d.DrugName & "  -  change values and click 'Update / Stock In'")
    End Sub

    Private Sub cboDrugName_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles cboDrugName.SelectionChangeCommitted
        If cboDrugName.SelectedItem Is Nothing Then Return
        Dim name As String = cboDrugName.SelectedItem.ToString()
        For Each row As DataGridViewRow In dgvInventory.Rows
            If Convert.ToString(row.Cells(colDrugName.Index).Value) = name Then
                dgvInventory.ClearSelection()
                row.Selected = True
                LoadDrugIntoFields(ToInt(row.Cells(colDrugID.Index).Value))
                Exit For
            End If
        Next
    End Sub

    Private Sub cboSupplier_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSupplier.SelectedIndexChanged
        If isLoading Then Return
        If cboSupplier.SelectedIndex <= 0 Then
            txtSupplierContact.Clear()
            Return
        End If
        Dim s As Supplier = DataStore.FindSupplier(cboSupplier.Text)
        If s IsNot Nothing Then txtSupplierContact.Text = s.Phone
    End Sub

    Private Sub btnExportInventory_Click(sender As Object, e As EventArgs) Handles btnExportInventory.Click
        ExportGridToCsv(dgvInventory, "MediTrack_Inventory_" & Date.Today.ToString("yyyyMMdd") & ".csv")
    End Sub

    Private Sub WholeNumber_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtQuantity.KeyPress, txtReorder.KeyPress
        If Not Char.IsControl(e.KeyChar) AndAlso Not Char.IsDigit(e.KeyChar) Then e.Handled = True
    End Sub

    Private Sub Money_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtUnitPrice.KeyPress, txtAmountPaid.KeyPress, txtDiscount.KeyPress
        Dim box As TextBox = CType(sender, TextBox)
        If Char.IsControl(e.KeyChar) OrElse Char.IsDigit(e.KeyChar) Then Return
        If e.KeyChar = "."c AndAlso Not box.Text.Contains(".") Then Return
        e.Handled = True
    End Sub

    ' ========================================================================
    '  EXPIRY TRACKER
    ' ========================================================================
    Private Sub ShowExpiry()
        Dim found As New List(Of Drug)
        Dim expiredCount As Integer = 0, criticalCount As Integer = 0, warningCount As Integer = 0
        Dim valueAtRisk As Decimal = 0

        For Each d As Drug In DataStore.Drugs
            If d.Quantity <= 0 Then Continue For
            Dim days As Integer = DataStore.DaysLeft(d)
            If days < 0 Then
                expiredCount += 1
            ElseIf days <= ExpiryCriticalDays Then
                criticalCount += 1
            ElseIf days <= ExpiryWarningDays Then
                warningCount += 1
            End If
            If days <= ExpiryWarningDays Then valueAtRisk += d.Quantity * d.UnitPrice

            Dim includeRow As Boolean = True
            Select Case cboExpiryFilter.SelectedIndex
                Case 1 : includeRow = (days < 0)
                Case 2 : includeRow = (days >= 0 AndAlso days <= ExpiryCriticalDays)
                Case 3 : includeRow = (days >= 0 AndAlso days <= ExpiryWarningDays)
                Case 4 : includeRow = (days > ExpiryWarningDays)
            End Select
            If includeRow Then found.Add(d)
        Next

        found.Sort(Function(a, b) a.ExpiryDate.CompareTo(b.ExpiryDate))   ' soonest first
        dgvExpiry.Rows.Clear()
        For Each d As Drug In found
            Dim days As Integer = DataStore.DaysLeft(d)
            Dim daysText As String
            If days < 0 Then
                daysText = Math.Abs(days).ToString() & " days ago"
            ElseIf days = 0 Then
                daysText = "Today"
            Else
                daysText = days.ToString() & " days"
            End If
            dgvExpiry.Rows.Add(d.DrugName, d.BatchNo, d.Quantity, d.ExpiryDate, daysText, ExpiryStatus(days),
                               d.Quantity * d.UnitPrice, d.SupplierName)
        Next
        dgvExpiry.ClearSelection()

        lblExpExpiredValue.Text = expiredCount.ToString()
        lblExpExpiredNote.Text = "remove these from the shelves"
        lblExpCriticalValue.Text = criticalCount.ToString()
        lblExpCriticalNote.Text = "sell or return these first"
        lblExpWarningValue.Text = warningCount.ToString()
        lblExpWarningNote.Text = "keep an eye on these"
        lblExpRiskValue.Text = Money(valueAtRisk)
        lblExpRiskNote.Text = "expired or expiring within 90 days"
    End Sub

    Private Function ExpiryStatus(days As Integer) As String
        If days < 0 Then Return "Expired"
        If days <= ExpiryCriticalDays Then Return "Critical (under 30 days)"
        If days <= ExpiryWarningDays Then Return "Warning (under 90 days)"
        Return "OK"
    End Function

    Private Sub dgvExpiry_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvExpiry.CellFormatting
        If e.RowIndex < 0 OrElse e.RowIndex >= dgvExpiry.Rows.Count Then Return
        Dim status As String = Convert.ToString(dgvExpiry.Rows(e.RowIndex).Cells(colExStatus.Index).Value)
        If status.StartsWith("Expired") Then
            e.CellStyle.BackColor = RowRed
        ElseIf status.StartsWith("Critical") Then
            e.CellStyle.BackColor = RowOrange
        ElseIf status.StartsWith("Warning") Then
            e.CellStyle.BackColor = RowYellow
        End If
    End Sub

    Private Sub cboExpiryFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboExpiryFilter.SelectedIndexChanged
        If formReady Then ShowExpiry()
    End Sub

    Private Sub btnWriteOff_Click(sender As Object, e As EventArgs) Handles btnWriteOff.Click
        Dim expired As New List(Of Drug)
        Dim total As Decimal = 0
        For Each d As Drug In DataStore.Drugs
            If d.Quantity > 0 AndAlso DataStore.DaysLeft(d) < 0 Then
                expired.Add(d)
                total += d.Quantity * d.UnitPrice
            End If
        Next
        If expired.Count = 0 Then
            MessageBox.Show("There is no expired stock to write off.", "Write Off", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        If MessageBox.Show("Write off " & expired.Count.ToString() & " expired drug(s) worth " & Money(total) & "?" & vbCrLf &
                           "Their quantity will be set to 0 and the value recorded as a loss.",
                           "Write Off Expired Stock", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.No Then Return
        If Not TrySave(Sub() DataStore.WriteOff(expired), "write off the expired stock") Then Return
        RefreshAll()
        SetStatus("Expired stock written off: " & Money(total))
    End Sub

    ' ========================================================================
    '  SUPPLIERS (Admin)
    ' ========================================================================
    Private Sub ShowSuppliers()
        isLoading = True
        Dim currentSupplier As String = If(cboSupplier.SelectedIndex > 0, cboSupplier.Text, "")
        cboSupplier.Items.Clear()
        cboSupplier.Items.Add("Select Supplier")
        For Each s As Supplier In DataStore.Suppliers
            cboSupplier.Items.Add(s.SupplierName)
        Next
        cboSupplier.SelectedIndex = 0
        If currentSupplier <> "" AndAlso cboSupplier.Items.Contains(currentSupplier) Then cboSupplier.SelectedItem = currentSupplier
        isLoading = False
        ShowSuppliersGrid()
    End Sub

    Private Sub ShowSuppliersGrid()
        Dim search As String = txtSearchSupplier.Text.Trim().ToLower()
        dgvSuppliers.Rows.Clear()
        For Each s As Supplier In DataStore.Suppliers
            If search = "" OrElse s.SupplierName.ToLower().Contains(search) Then
                dgvSuppliers.Rows.Add(s.ID, s.SupplierName, s.ContactPerson, s.Phone, s.Email, s.Address, s.SuppliedDrugs)
            End If
        Next
        dgvSuppliers.ClearSelection()
    End Sub

    Private Sub txtSearchSupplier_TextChanged(sender As Object, e As EventArgs) Handles txtSearchSupplier.TextChanged
        If formReady Then ShowSuppliersGrid()
    End Sub

    Private Sub dgvSuppliers_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvSuppliers.CellClick
        If e.RowIndex < 0 Then Return
        Dim s As Supplier = DataStore.FindSupplierById(ToInt(dgvSuppliers.Rows(e.RowIndex).Cells(colSupID.Index).Value))
        If s Is Nothing Then Return
        selectedSupplierId = s.ID
        txtSupName.Text = s.SupplierName
        txtContactPerson.Text = s.ContactPerson
        txtPhone.Text = s.Phone
        txtEmail.Text = s.Email
        txtAddress.Text = s.Address
        txtSuppliedDrugs.Text = s.SuppliedDrugs
        SetStatus("Supplier selected: " & s.SupplierName & "  -  edit and click Update, or click Delete")
    End Sub

    Private Sub btnAddSupplier_Click(sender As Object, e As EventArgs) Handles btnAddSupplier.Click
        If Not ValidateSupplierInput() Then Return
        Dim name As String = txtSupName.Text.Trim()
        If DataStore.FindSupplier(name) IsNot Nothing Then
            MessageBox.Show("A supplier called '" & name & "' already exists." & vbCrLf &
                            "Click it in the table and click Update to change it.",
                            "Duplicate Supplier", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim person As String = txtContactPerson.Text.Trim(), phone As String = txtPhone.Text.Trim()
        Dim email As String = txtEmail.Text.Trim(), address As String = txtAddress.Text.Trim()
        Dim supplied As String = txtSuppliedDrugs.Text.Trim()
        If Not TrySave(Sub() DataStore.AddSupplier(name, person, phone, email, address, supplied), "add the supplier") Then Return
        ClearSupplierFields()
        ShowSuppliers()
        SetStatus("Supplier added: " & name)
        MessageBox.Show(name & " was added to the supplier list.", "Supplier Added", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub btnUpdateSupplier_Click(sender As Object, e As EventArgs) Handles btnUpdateSupplier.Click
        Dim s As Supplier = DataStore.FindSupplierById(selectedSupplierId)
        If s Is Nothing Then
            MessageBox.Show("Click a supplier in the table first.", "Update Supplier", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        If Not ValidateSupplierInput() Then Return
        Dim newName As String = txtSupName.Text.Trim()
        Dim other As Supplier = DataStore.FindSupplier(newName)
        If other IsNot Nothing AndAlso other.ID <> s.ID Then
            MessageBox.Show("Another supplier is already called '" & newName & "'.", "Duplicate Supplier",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Dim oldName As String = s.SupplierName
        s.SupplierName = newName
        s.ContactPerson = txtContactPerson.Text.Trim()
        s.Phone = txtPhone.Text.Trim()
        s.Email = txtEmail.Text.Trim()
        s.Address = txtAddress.Text.Trim()
        s.SuppliedDrugs = txtSuppliedDrugs.Text.Trim()
        If Not TrySave(Sub() DataStore.UpdateSupplier(s, oldName), "update the supplier") Then Return
        ClearSupplierFields()
        RefreshAll()
        SetStatus("Supplier updated: " & newName)
        MessageBox.Show(newName & " was updated.", "Supplier Updated", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub btnDeleteSupplier_Click(sender As Object, e As EventArgs) Handles btnDeleteSupplier.Click
        Dim s As Supplier = DataStore.FindSupplierById(selectedSupplierId)
        If s Is Nothing Then
            MessageBox.Show("Click a supplier in the table first.", "Delete Supplier", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim used As Integer = 0
        For Each d As Drug In DataStore.Drugs
            If d.SupplierName = s.SupplierName Then used += 1
        Next
        Dim question As String = "Delete supplier '" & s.SupplierName & "'?"
        If used > 0 Then question &= vbCrLf & vbCrLf & used.ToString() & " drug(s) still list this supplier. They will keep the name."
        If MessageBox.Show(question, "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then Return
        If Not TrySave(Sub() DataStore.DeleteSupplier(s), "delete the supplier") Then Return
        ClearSupplierFields()
        ShowSuppliers()
        SetStatus("Supplier deleted: " & s.SupplierName)
    End Sub

    Private Sub btnClearSupplier_Click(sender As Object, e As EventArgs) Handles btnClearSupplier.Click
        ClearSupplierFields()
    End Sub

    Private Function ValidateSupplierInput() As Boolean
        If txtSupName.Text.Trim() = "" Then Return Fail(txtSupName, "Please enter the supplier name.")
        If txtPhone.Text.Trim() = "" Then Return Fail(txtPhone, "Please enter the supplier's phone number.")
        Dim email As String = txtEmail.Text.Trim()
        If email <> "" AndAlso (Not email.Contains("@") OrElse Not email.Contains(".") OrElse email.Contains(" ")) Then
            Return Fail(txtEmail, "Please enter a valid email address, e.g. orders@supplier.com")
        End If
        Return True
    End Function

    Private Sub ClearSupplierFields()
        selectedSupplierId = 0
        txtSupName.Clear()
        txtContactPerson.Clear()
        txtPhone.Clear()
        txtEmail.Clear()
        txtAddress.Clear()
        txtSuppliedDrugs.Clear()
        dgvSuppliers.ClearSelection()
    End Sub

    ' ========================================================================
    '  HISTORY (Admin)
    ' ========================================================================
    Private Sub ShowHistory()
        Dim search As String = txtSearchHistory.Text.Trim().ToLower()
        Dim action As String = If(cboActionFilter.SelectedItem Is Nothing, "All", cboActionFilter.SelectedItem.ToString())
        Dim entries As New List(Of HistoryEntry)(DataStore.History)
        entries.Sort(Function(a, b) b.LogDate.CompareTo(a.LogDate))
        dgvHistory.Rows.Clear()
        For Each h As HistoryEntry In entries
            Dim nameMatches As Boolean = (search = "" OrElse h.DrugName.ToLower().Contains(search))
            Dim actionMatches As Boolean = (action = "All" OrElse h.ActionType = action)
            If nameMatches AndAlso actionMatches Then
                dgvHistory.Rows.Add(h.LogDate, h.DrugName, h.ActionType, h.QtyChange, h.Party, h.BatchNo, h.UserName)
            End If
        Next
        dgvHistory.ClearSelection()
    End Sub

    Private Sub HistoryFilter_Changed(sender As Object, e As EventArgs) _
            Handles txtSearchHistory.TextChanged, cboActionFilter.SelectedIndexChanged
        If formReady Then ShowHistory()
    End Sub

    Private Sub dgvHistory_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvHistory.CellFormatting
        If e.RowIndex < 0 OrElse e.ColumnIndex <> colHisQty.Index OrElse e.Value Is Nothing Then Return
        Dim n As Integer = ToInt(e.Value)
        e.Value = If(n > 0, "+" & n.ToString(), n.ToString())
        If n > 0 Then
            e.CellStyle.ForeColor = GoodGreen
        ElseIf n < 0 Then
            e.CellStyle.ForeColor = BadRed
        End If
        e.FormattingApplied = True
    End Sub

    Private Sub btnExportHistory_Click(sender As Object, e As EventArgs) Handles btnExportHistory.Click
        ExportGridToCsv(dgvHistory, "MediTrack_History_" & Date.Today.ToString("yyyyMMdd") & ".csv")
    End Sub

    ' ========================================================================
    '  SALES RECORDS (Admin)
    ' ========================================================================
    Private Sub ShowSales()
        Dim fromDate As Date = dtpSalesFrom.Value.Date
        Dim toDate As Date = dtpSalesTo.Value.Date.AddDays(1)     ' include the whole "to" day
        Dim search As String = txtSalesSearch.Text.Trim().ToLower()

        Dim itemCount As New Dictionary(Of Integer, Integer)
        For Each item As SaleItem In DataStore.Sales
            If Not itemCount.ContainsKey(item.SaleID) Then itemCount(item.SaleID) = 0
            itemCount(item.SaleID) += item.Quantity
        Next

        Dim shown As New List(Of Sale)
        For Each s As Sale In DataStore.SaleHeaders
            If s.SaleDate >= fromDate AndAlso s.SaleDate < toDate Then
                If search = "" OrElse s.Customer.ToLower().Contains(search) OrElse s.SaleID.ToString() = search.TrimStart("#"c) Then shown.Add(s)
            End If
        Next
        shown.Sort(Function(a, b) b.SaleDate.CompareTo(a.SaleDate))

        Dim total As Decimal = 0
        Dim units As Integer = 0
        Dim receipts As Integer = 0, voidedCount As Integer = 0
        Dim byMethod As New Dictionary(Of String, Decimal)
        dgvSales.Rows.Clear()
        For Each s As Sale In shown
            Dim n As Integer = If(itemCount.ContainsKey(s.SaleID), itemCount(s.SaleID), 0)
            Dim rowIndex As Integer = dgvSales.Rows.Add(s.SaleID, s.SaleDate, s.Customer, s.PaymentMethod, n, s.Total, s.SoldBy,
                                                        If(s.Voided, "VOIDED", "Paid"))
            If s.Voided Then
                ' Voided receipts stay in the list (audit trail) but do not count
                dgvSales.Rows(rowIndex).DefaultCellStyle.ForeColor = BadRed
                dgvSales.Rows(rowIndex).DefaultCellStyle.Font = VoidedFont
                voidedCount += 1
                Continue For
            End If
            receipts += 1
            total += s.Total
            units += n
            If Not byMethod.ContainsKey(s.PaymentMethod) Then byMethod(s.PaymentMethod) = 0D
            byMethod(s.PaymentMethod) += s.Total
        Next
        Dim parts As New List(Of String)
        For Each m As String In New String() {"Cash", "Mobile Money", "Card"}
            parts.Add(m & ": " & Money(If(byMethod.ContainsKey(m), byMethod(m), 0D)))
        Next
        lblSalesPayments.Text = "Payments  -  " & String.Join("     |     ", parts.ToArray()) &
                                If(voidedCount > 0, "     |     Voided: " & voidedCount.ToString(), "")
        dgvSales.ClearSelection()
        dgvSaleItems.Rows.Clear()
        lblSaleInfo.Text = "Click a receipt to see its items."
        lblSaleInfo.ForeColor = Color.FromArgb(100, 116, 139)
        btnVoidSale.Enabled = True

        lblSalesCountValue.Text = receipts.ToString("N0")
        lblSalesCountNote.Text = fromDate.ToString("d MMM") & " to " & dtpSalesTo.Value.ToString("d MMM yyyy")
        lblSalesTotalValue.Text = Money(total)
        lblSalesTotalNote.Text = "for the dates chosen"
        lblSalesItemsValue.Text = units.ToString("N0")
        lblSalesItemsNote.Text = "units"
        lblSalesAverageValue.Text = Money(If(receipts > 0, total / receipts, 0D))
        lblSalesAverageNote.Text = "per receipt"
    End Sub

    Private Sub btnSalesShow_Click(sender As Object, e As EventArgs) Handles btnSalesShow.Click
        If dtpSalesFrom.Value.Date > dtpSalesTo.Value.Date Then
            MessageBox.Show("The 'From' date must be before the 'To' date.", "Sales Records", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        ShowSales()
    End Sub

    Private Sub btnSalesToday_Click(sender As Object, e As EventArgs) Handles btnSalesToday.Click
        dtpSalesFrom.Value = Date.Today
        dtpSalesTo.Value = Date.Today
        ShowSales()
    End Sub

    Private Sub btnSalesMonth_Click(sender As Object, e As EventArgs) Handles btnSalesMonth.Click
        dtpSalesFrom.Value = New Date(Date.Today.Year, Date.Today.Month, 1)
        dtpSalesTo.Value = Date.Today
        ShowSales()
    End Sub

    Private Sub txtSalesSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSalesSearch.TextChanged
        If formReady Then ShowSales()
    End Sub

    Private Sub dgvSales_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvSales.CellClick
        If e.RowIndex < 0 Then Return
        Dim id As Integer = ToInt(dgvSales.Rows(e.RowIndex).Cells(colSaleID.Index).Value)
        Dim s As Sale = DataStore.FindSale(id)
        If s Is Nothing Then Return
        dgvSaleItems.Rows.Clear()
        For Each item As SaleItem In DataStore.ItemsOfSale(id)
            dgvSaleItems.Rows.Add(item.DrugName, item.BatchNo, item.Quantity, item.UnitPrice, item.Total)
        Next
        dgvSaleItems.ClearSelection()
        lblSaleInfo.Text = "Receipt #" & id.ToString() & "  -  " & s.PaymentMethod & ", paid " & Money(s.AmountPaid) &
                           If(s.Discount > 0, ", discount " & Money(s.Discount), "") & ", change " & Money(s.AmountPaid - s.Total) &
                           If(s.Voided, "  -  VOIDED: " & s.VoidReason, "")
        lblSaleInfo.ForeColor = If(s.Voided, BadRed, Color.FromArgb(100, 116, 139))
        btnVoidSale.Enabled = Not s.Voided
    End Sub

    Private Sub btnReprint_Click(sender As Object, e As EventArgs) Handles btnReprint.Click
        If dgvSales.SelectedRows.Count = 0 Then
            MessageBox.Show("Click a receipt in the list first.", "Print Receipt", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim s As Sale = DataStore.FindSale(ToInt(dgvSales.SelectedRows(0).Cells(colSaleID.Index).Value))
        If s IsNot Nothing Then PrintReceipt(s)
    End Sub

    ''' <summary>Void / Refund: cancels a receipt and puts its items back into stock.</summary>
    Private Sub btnVoidSale_Click(sender As Object, e As EventArgs) Handles btnVoidSale.Click
        If dgvSales.SelectedRows.Count = 0 Then
            MessageBox.Show("Click the receipt you want to void in the list first.", "Void / Refund",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim s As Sale = DataStore.FindSale(ToInt(dgvSales.SelectedRows(0).Cells(colSaleID.Index).Value))
        If s Is Nothing Then Return
        If s.Voided Then
            MessageBox.Show("Receipt #" & s.SaleID.ToString() & " is already voided.", "Void / Refund",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim reason As String = Microsoft.VisualBasic.Interaction.InputBox(
            "Void receipt #" & s.SaleID.ToString() & " (" & Money(s.Total) & ", " & s.SaleDate.ToString("dd/MM/yyyy h:mm tt") & ")?" &
            vbCrLf & vbCrLf & "The items go back into stock and the money is taken off the sales totals." &
            vbCrLf & vbCrLf & "Reason (required), e.g. Wrong item, Customer returned goods:", "Void / Refund").Trim()
        If reason = "" Then Return       ' cancelled
        Dim saleId As Integer = s.SaleID
        If Not TrySave(Sub() DataStore.VoidSale(saleId, reason & " (by " & DataStore.CurrentUser & ")"), "void the receipt") Then Return
        RefreshAll()
        SetStatus("Receipt #" & saleId.ToString() & " voided")
        MessageBox.Show("Receipt #" & saleId.ToString() & " was voided." & vbCrLf &
                        "Refund " & Money(s.Total) & " to the customer. The items are back in stock.",
                        "Void / Refund", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ''' <summary>Prints a sales summary for the dates chosen (for example, at the end of the day).</summary>
    Private Sub btnPrintSummary_Click(sender As Object, e As EventArgs) Handles btnPrintSummary.Click
        If dtpSalesFrom.Value.Date > dtpSalesTo.Value.Date Then
            MessageBox.Show("The 'From' date must be before the 'To' date.", "Print Summary", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Try
            Dim report As New SalesReport(dtpSalesFrom.Value.Date, dtpSalesTo.Value.Date)
            report.ShowPreview(Me)
        Catch ex As Exception
            MessageBox.Show("Could not prepare the summary." & vbCrLf & ex.Message, "Print Summary",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnExportSales_Click(sender As Object, e As EventArgs) Handles btnExportSales.Click
        ExportGridToCsv(dgvSales, "MediTrack_Sales_" & dtpSalesFrom.Value.ToString("yyyyMMdd") & "_" &
                        dtpSalesTo.Value.ToString("yyyyMMdd") & ".csv")
    End Sub

    ' ========================================================================
    '  USERS AND SETTINGS (Admin)
    ' ========================================================================
    Private Sub ShowUsers()
        dgvUsers.Rows.Clear()
        For Each u As AppUser In DataStore.Users
            dgvUsers.Rows.Add(u.ID, u.Username, u.FullName, u.Role, If(u.IsActive, "Yes", "No"))
        Next
        dgvUsers.ClearSelection()
    End Sub

    Private Sub dgvUsers_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvUsers.CellClick
        If e.RowIndex < 0 Then Return
        Dim u As AppUser = DataStore.FindUserById(ToInt(dgvUsers.Rows(e.RowIndex).Cells(colUserID.Index).Value))
        If u Is Nothing Then Return
        selectedUserId = u.ID
        txtNewUsername.Text = u.Username
        txtNewUsername.ReadOnly = True            ' the username is the login name; it does not change
        txtNewFullName.Text = u.FullName
        txtNewPassword.Clear()
        cboNewRole.SelectedItem = If(u.Role = "Admin", "Admin", "Staff")
        chkUserActive.Checked = u.IsActive
        SetStatus("User selected: " & u.Username)
    End Sub

    Private Sub btnAddUser_Click(sender As Object, e As EventArgs) Handles btnAddUser.Click
        Dim username As String = txtNewUsername.Text.Trim().ToLower()
        If selectedUserId <> 0 Then
            Fail(txtNewUsername, "This user already exists. Click Update User to save changes, or Clear to add a new one.")
            Return
        End If
        If username = "" OrElse username.Contains(" ") Then
            Fail(txtNewUsername, "Please enter a username with no spaces, e.g. ama")
            Return
        End If
        If DataStore.FindUser(username) IsNot Nothing Then
            Fail(txtNewUsername, "The username '" & username & "' is already taken.")
            Return
        End If
        If txtNewFullName.Text.Trim() = "" Then
            Fail(txtNewFullName, "Please enter the person's full name.")
            Return
        End If
        If txtNewPassword.Text.Length < DataStore.MinimumPasswordLength Then
            Fail(txtNewPassword, "The password must be at least " & DataStore.MinimumPasswordLength.ToString() & " characters.")
            Return
        End If
        Dim fullName As String = txtNewFullName.Text.Trim(), password As String = txtNewPassword.Text
        Dim role As String = cboNewRole.Text, active As Boolean = chkUserActive.Checked
        If Not TrySave(Sub() DataStore.AddUser(username, fullName, password, role, active), "add the user") Then Return
        ClearUserFields()
        ShowUsers()
        SetStatus("User added: " & username)
        MessageBox.Show("User '" & username & "' was added. They can now sign in.", "User Added",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub btnUpdateUser_Click(sender As Object, e As EventArgs) Handles btnUpdateUser.Click
        Dim u As AppUser = DataStore.FindUserById(selectedUserId)
        If u Is Nothing Then
            MessageBox.Show("Click a user in the table first.", "Update User", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        If txtNewFullName.Text.Trim() = "" Then
            Fail(txtNewFullName, "Please enter the person's full name.")
            Return
        End If
        If txtNewPassword.Text <> "" AndAlso txtNewPassword.Text.Length < DataStore.MinimumPasswordLength Then
            Fail(txtNewPassword, "The new password must be at least " & DataStore.MinimumPasswordLength.ToString() & " characters.")
            Return
        End If
        If u.Username = DataStore.CurrentUsername AndAlso (Not chkUserActive.Checked OrElse cboNewRole.Text <> "Admin") Then
            Fail(chkUserActive, "You cannot disable your own account or remove your own Admin role while signed in.")
            Return
        End If
        u.FullName = txtNewFullName.Text.Trim()
        u.Role = cboNewRole.Text
        u.IsActive = chkUserActive.Checked
        Dim password As String = txtNewPassword.Text
        If Not TrySave(Sub() DataStore.UpdateUser(u, password), "update the user") Then Return
        ClearUserFields()
        ShowUsers()
        SetStatus("User updated: " & u.Username)
        MessageBox.Show("User '" & u.Username & "' was updated." & If(password <> "", vbCrLf & "The password was changed.", ""),
                        "User Updated", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub btnClearUser_Click(sender As Object, e As EventArgs) Handles btnClearUser.Click
        ClearUserFields()
    End Sub

    Private Sub ClearUserFields()
        selectedUserId = 0
        txtNewUsername.ReadOnly = False
        txtNewUsername.Clear()
        txtNewFullName.Clear()
        txtNewPassword.Clear()
        cboNewRole.SelectedIndex = 0
        chkUserActive.Checked = True
        dgvUsers.ClearSelection()
    End Sub

    Private Sub ShowSettings()
        txtPharmacyName.Text = DataStore.GetSetting("PharmacyName")
        txtPharmacyAddress.Text = DataStore.GetSetting("PharmacyAddress")
        txtPharmacyPhone.Text = DataStore.GetSetting("PharmacyPhone")
        txtPharmacyEmail.Text = DataStore.GetSetting("PharmacyEmail")
        txtReceiptFooter.Text = DataStore.GetSetting("ReceiptFooter")
        picLogoPreview.Image = Branding.Logo()
        btnRemoveLogo.Enabled = picLogoPreview.Image IsNot Nothing
        pnlThemeSwatch.BackColor = Branding.ThemeColor()
        lblDbPath.Text = Database.DatabasePath
        lblDbCounts.Text = DataStore.Drugs.Count.ToString() & " drugs,  " & DataStore.Suppliers.Count.ToString() & " suppliers,  " &
                           DataStore.SaleHeaders.Count.ToString() & " receipts,  " & DataStore.History.Count.ToString() & " history lines"
        Try
            Dim folder As String = Path.Combine(Application.StartupPath, "Backups")
            Dim copies As String() = If(Directory.Exists(folder), Directory.GetFiles(folder, "MediTrack_*"), New String() {})
            Array.Sort(copies)
            lblLastBackup.Text = If(copies.Length = 0, "No automatic backup yet.",
                                    "Automatic daily backups: " & copies.Length.ToString() & " kept. Latest: " & Path.GetFileName(copies(copies.Length - 1)))
        Catch
            lblLastBackup.Text = ""
        End Try
    End Sub

    Private Sub btnSaveSettings_Click(sender As Object, e As EventArgs) Handles btnSaveSettings.Click
        If txtPharmacyName.Text.Trim() = "" Then
            Fail(txtPharmacyName, "Please enter the business name.")
            Return
        End If
        Dim email As String = txtPharmacyEmail.Text.Trim()
        If email <> "" AndAlso (Not email.Contains("@") OrElse Not email.Contains(".") OrElse email.Contains(" ")) Then
            Fail(txtPharmacyEmail, "Please enter a valid email address, e.g. info@mypharmacy.com, or leave it empty.")
            Return
        End If
        Dim name As String = txtPharmacyName.Text.Trim(), address As String = txtPharmacyAddress.Text.Trim()
        Dim footer As String = txtReceiptFooter.Text.Trim()
        ' Tidy the phone numbers: "024 111 2222 ,020 333 4444" becomes "024 111 2222, 020 333 4444"
        Dim numbers As New List(Of String)
        For Each part As String In txtPharmacyPhone.Text.Split(New Char() {","c, ";"c, "/"c})
            If part.Trim() <> "" Then numbers.Add(part.Trim())
        Next
        Dim phone As String = String.Join(", ", numbers.ToArray())
        If Not TrySave(Sub()
                           DataStore.SaveSetting("PharmacyName", name)
                           DataStore.SaveSetting("PharmacyAddress", address)
                           DataStore.SaveSetting("PharmacyPhone", phone)
                           DataStore.SaveSetting("PharmacyEmail", email)
                           DataStore.SaveSetting("ReceiptFooter", footer)
                       End Sub, "save the business details") Then Return
        ShowSettings()
        SetStatus("Business details saved")
        MessageBox.Show("The business details were saved." & vbCrLf &
                        "They now appear on the screens, receipts and reports on every computer.", "Saved",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    ''' <summary>Change Logo: pick a picture; it is resized and saved in the database.</summary>
    Private Sub btnChangeLogo_Click(sender As Object, e As EventArgs) Handles btnChangeLogo.Click
        Dim logoText As String = Branding.PickLogo(Me)
        If logoText = "" Then Return
        If Not TrySave(Sub() DataStore.SaveSetting("LogoImage", logoText), "save the logo") Then Return
        ShowSettings()
        SetStatus("Logo changed")
    End Sub

    Private Sub btnRemoveLogo_Click(sender As Object, e As EventArgs) Handles btnRemoveLogo.Click
        If MessageBox.Show("Remove the logo? The MediTrack '+' symbol will be shown instead.", "Logo",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then Return
        If Not TrySave(Sub() DataStore.SaveSetting("LogoImage", ""), "remove the logo") Then Return
        ShowSettings()
        SetStatus("Logo removed")
    End Sub

    ''' <summary>Choose Colour: the header, sign-in panel, table headings and titles take this colour.</summary>
    Private Sub btnChooseColour_Click(sender As Object, e As EventArgs) Handles btnChooseColour.Click
        Dim chosen As Color
        If Not Branding.PickColour(Me, Branding.ThemeColor(), chosen) Then Return
        If chosen.GetBrightness() > 0.8F Then
            MessageBox.Show("That colour is too light: white text on it would be hard to read. Please choose a darker colour.",
                            "Theme Colour", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Dim hex As String = Branding.ColorToText(chosen)
        If Not TrySave(Sub() DataStore.SaveSetting("ThemeColor", hex), "save the colour") Then Return
        ShowSettings()
        SetStatus("Theme colour changed")
    End Sub

    Private Sub btnResetColour_Click(sender As Object, e As EventArgs) Handles btnResetColour.Click
        If Not TrySave(Sub() DataStore.SaveSetting("ThemeColor", ""), "reset the colour") Then Return
        ShowSettings()
        SetStatus("Theme colour reset to MediTrack blue")
    End Sub

    ' ---- Show password ticks ----
    Private Sub chkShowUserPassword_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowUserPassword.CheckedChanged
        txtNewPassword.UseSystemPasswordChar = Not chkShowUserPassword.Checked
    End Sub

    Private Sub chkShowCodes_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowCodes.CheckedChanged
        For Each box As TextBox In New TextBox() {txtCurrentCode, txtNewCode, txtConfirmCode}
            box.UseSystemPasswordChar = Not chkShowCodes.Checked
        Next
    End Sub

    Private Sub btnChangeCode_Click(sender As Object, e As EventArgs) Handles btnChangeCode.Click
        If Not DataStore.CheckAdminCode(txtCurrentCode.Text) Then
            Fail(txtCurrentCode, "The current access code is wrong.")
            Return
        End If
        If txtNewCode.Text.Trim().Length < DataStore.MinimumPasswordLength Then
            Fail(txtNewCode, "The new code must be at least " & DataStore.MinimumPasswordLength.ToString() & " characters.")
            Return
        End If
        If txtNewCode.Text <> txtConfirmCode.Text Then
            Fail(txtConfirmCode, "The two new codes do not match.")
            Return
        End If
        Dim newCode As String = txtNewCode.Text
        If Not TrySave(Sub() DataStore.ChangeAdminCode(newCode), "change the access code") Then Return
        txtCurrentCode.Clear()
        txtNewCode.Clear()
        txtConfirmCode.Clear()
        DataStore.AddHistory(Date.Now, "-", "Code Changed", 0, "-", "-")
        MessageBox.Show("The admin access code was changed. Use the new code next time you sign in.", "Access Code",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub BackupDatabase_Click(sender As Object, e As EventArgs) Handles btnBackup.Click, mnuBackup.Click
        Using dlg As New SaveFileDialog()
            Dim ext As String = Path.GetExtension(Database.DatabasePath)
            dlg.Filter = "Access database (*" & ext & ")|*" & ext
            dlg.FileName = "MediTrack_Backup_" & Date.Now.ToString("yyyyMMdd_HHmm") & ext
            If dlg.ShowDialog() <> DialogResult.OK Then Return
            If dlg.FileName.Equals(Database.DatabasePath, StringComparison.OrdinalIgnoreCase) Then
                MessageBox.Show("Choose a different file name for the backup.", "Back Up", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If
            Dim target As String = dlg.FileName
            If TrySave(Sub() Database.BackupTo(target), "back up the database") Then
                SetStatus("Backup saved: " & Path.GetFileName(target))
                MessageBox.Show("Backup saved to:" & vbCrLf & target, "Back Up", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Using
    End Sub

    Private Sub btnOpenDbFolder_Click(sender As Object, e As EventArgs) Handles btnOpenDbFolder.Click
        Try
            Process.Start("explorer.exe", "/select,""" & Database.DatabasePath & """")
        Catch ex As Exception
            MessageBox.Show("Could not open the folder." & vbCrLf & ex.Message, "MediTrack", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnReloadData_Click(sender As Object, e As EventArgs) Handles btnReloadData.Click
        If TrySave(Sub() DataStore.LoadAll(), "reload the data") Then
            RefreshAll()
            ShowSettings()
            SetStatus("Data reloaded from the database")
        End If
    End Sub

    ' ========================================================================
    '  SHARED HELPERS
    ' ========================================================================
    Private Sub UpdateLowStock()
        Dim names As New List(Of String)
        For Each d As Drug In DataStore.Drugs
            If d.Quantity <= d.ReorderLevel AndAlso DataStore.DaysLeft(d) >= 0 Then names.Add(d.DrugName)
        Next
        If names.Count = 0 Then
            lblLowStock.Text = "No low stock"
            lblLowStock.ForeColor = GoodGreen
        Else
            lblLowStock.Text = "Low stock: " & String.Join(", ", names.ToArray())
            lblLowStock.ForeColor = Color.FromArgb(197, 90, 17)
        End If
    End Sub

    ''' <summary>Saves the visible columns of a table as a CSV file that opens in Excel.</summary>
    Private Sub ExportGridToCsv(grid As DataGridView, defaultName As String)
        If grid.Rows.Count = 0 Then
            MessageBox.Show("There is nothing to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        Using dlg As New SaveFileDialog()
            dlg.Filter = "CSV file for Excel (*.csv)|*.csv"
            dlg.FileName = defaultName
            If dlg.ShowDialog() <> DialogResult.OK Then Return
            Try
                Dim sb As New StringBuilder()
                Dim header As New List(Of String)
                For Each c As DataGridViewColumn In grid.Columns
                    If c.Visible Then header.Add(CsvField(c.HeaderText))
                Next
                sb.AppendLine(String.Join(",", header.ToArray()))
                For Each row As DataGridViewRow In grid.Rows
                    Dim cells As New List(Of String)
                    For Each c As DataGridViewColumn In grid.Columns
                        If c.Visible Then cells.Add(CsvField(row.Cells(c.Index).FormattedValue))
                    Next
                    sb.AppendLine(String.Join(",", cells.ToArray()))
                Next
                File.WriteAllText(dlg.FileName, sb.ToString(), New UTF8Encoding(True))
                SetStatus("Exported to " & Path.GetFileName(dlg.FileName))
                If MessageBox.Show("Export complete. Open the file in Excel now?", "Export",
                                   MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
                    Process.Start(dlg.FileName)
                End If
            Catch ex As Exception
                MessageBox.Show("Could not export the file. Is it already open in Excel?" & vbCrLf & vbCrLf & ex.Message,
                                "MediTrack", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Using
    End Sub

    Private Function CsvField(value As Object) As String
        Dim text As String = If(value Is Nothing, "", value.ToString())
        If text.Contains(",") OrElse text.Contains("""") OrElse text.Contains(vbCr) OrElse text.Contains(vbLf) Then
            text = """" & text.Replace("""", """""") & """"
        End If
        Return text
    End Function

    Private Function Fail(ctrl As Control, message As String) As Boolean
        MessageBox.Show(message, "Check Your Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        ctrl.Focus()
        Return False
    End Function

    Private Function Money(amount As Decimal) As String
        Return "GH₵ " & amount.ToString("N2")
    End Function

    Private Function ToInt(value As Object) As Integer
        If value Is Nothing Then Return 0
        Dim n As Integer
        If Integer.TryParse(value.ToString(), n) Then Return n
        Return 0
    End Function

    Private Sub SetStatus(message As String)
        lblSystemStatus.Text = message
    End Sub

    Private Sub tmrClock_Tick(sender As Object, e As EventArgs) Handles tmrClock.Tick
        UpdateClock()
        syncCounter += 1
        If syncCounter >= 3 Then           ' check the database every 3 seconds
            syncCounter = 0
            CheckForChanges()
        End If
    End Sub

    ''' <summary>
    ''' Live updates: if another window (for example the cashier) has saved a sale
    ''' or any other change, reload the data and redraw every tab.
    ''' </summary>
    Private Sub CheckForChanges()
        If syncing OrElse Not formReady Then Return
        syncing = True
        Try
            If DataStore.HasChangedElsewhere() Then
                ' remember what the cashier has selected so it is not lost
                Dim selectedCounterDrug As Integer = 0
                If dgvCounterDrugs.SelectedRows.Count > 0 Then selectedCounterDrug = ToInt(dgvCounterDrugs.SelectedRows(0).Cells(colCdID.Index).Value)

                DataStore.LoadAll()
                RefreshAll()

                If selectedCounterDrug <> 0 Then
                    For Each row As DataGridViewRow In dgvCounterDrugs.Rows
                        If ToInt(row.Cells(colCdID.Index).Value) = selectedCounterDrug Then row.Selected = True
                    Next
                End If
                lblSyncInfo.Text = "Live: updated " & Date.Now.ToString("h:mm:ss tt")
            End If
            lblSyncInfo.ForeColor = Color.FromArgb(84, 130, 53)
        Catch
            ' Database busy or unreachable for a moment: try again on the next check
            lblSyncInfo.Text = "Live: waiting for database..."
            lblSyncInfo.ForeColor = Color.FromArgb(197, 90, 17)
        Finally
            syncing = False
        End Try
    End Sub

    Private Sub UpdateClock()
        lblClock.Text = Date.Now.ToString("h:mm:ss tt")
        lblHeaderDate.Text = Date.Now.ToString("dddd, d MMMM yyyy")
    End Sub

End Class
