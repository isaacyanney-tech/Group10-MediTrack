' ============================================================================
'  SalesReport.vb
'  Prints an A4 "Sales Summary" for any period (for example the end of the
'  day or the end of the month): money taken, payment methods, discounts,
'  voided receipts, best-selling drugs, losses and the stock value.
'  Used by the "Print Summary" button on the Sales Records tab.
' ============================================================================
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Windows.Forms

Public Class SalesReport

    Private ReadOnly fromDate As Date
    Private ReadOnly toDate As Date          ' last day included

    Public Sub New(firstDay As Date, lastDay As Date)
        fromDate = firstDay.Date
        toDate = lastDay.Date
    End Sub

    ''' <summary>Opens the print preview; the user can print from there.</summary>
    Public Sub ShowPreview(owner As IWin32Window)
        Using doc As New PrintDocument()
            doc.DocumentName = "MediTrack Sales Summary"
            AddHandler doc.PrintPage, AddressOf PrintPage
            Using dlg As New PrintPreviewDialog()
                dlg.Document = doc
                dlg.Width = 900
                dlg.Height = 1000
                dlg.UseAntiAlias = True
                dlg.Text = "Sales Summary - Print Preview"
                dlg.ShowDialog(owner)
            End Using
        End Using
    End Sub

    Private Sub PrintPage(sender As Object, e As PrintPageEventArgs)
        Dim g As Graphics = e.Graphics
        Dim left As Single = e.MarginBounds.Left
        Dim width As Single = e.MarginBounds.Width
        Dim y As Single = e.MarginBounds.Top
        Dim endDate As Date = toDate.AddDays(1)

        ' ---------------- work out the figures ----------------
        Dim receipts As Integer = 0, voidedCount As Integer = 0
        Dim netTotal As Decimal = 0, discounts As Decimal = 0, voidedValue As Decimal = 0
        Dim byMethod As New Dictionary(Of String, Decimal)
        Dim byMethodCount As New Dictionary(Of String, Integer)
        For Each m As String In New String() {"Cash", "Mobile Money", "Card"}
            byMethod(m) = 0D
            byMethodCount(m) = 0
        Next
        For Each s As Sale In DataStore.SaleHeaders
            If s.SaleDate < fromDate OrElse s.SaleDate >= endDate Then Continue For
            If s.Voided Then
                voidedCount += 1
                voidedValue += s.Total
                Continue For
            End If
            receipts += 1
            netTotal += s.Total
            discounts += s.Discount
            Dim m As String = If(s.PaymentMethod = "", "Cash", s.PaymentMethod)
            If Not byMethod.ContainsKey(m) Then
                byMethod(m) = 0D
                byMethodCount(m) = 0
            End If
            byMethod(m) += s.Total
            byMethodCount(m) += 1
        Next

        Dim units As Integer = 0
        Dim qtyByDrug As New Dictionary(Of String, Integer)
        Dim moneyByDrug As New Dictionary(Of String, Decimal)
        For Each item As SaleItem In DataStore.Sales
            If item.SaleDate < fromDate OrElse item.SaleDate >= endDate OrElse DataStore.IsVoided(item.SaleID) Then Continue For
            units += item.Quantity
            If Not qtyByDrug.ContainsKey(item.DrugName) Then
                qtyByDrug(item.DrugName) = 0
                moneyByDrug(item.DrugName) = 0D
            End If
            qtyByDrug(item.DrugName) += item.Quantity
            moneyByDrug(item.DrugName) += item.Total
        Next

        Dim lossValue As Decimal = 0
        For Each l As LossRecord In DataStore.Losses
            If l.LossDate >= fromDate AndAlso l.LossDate < endDate Then lossValue += l.Value
        Next
        Dim stockValue As Decimal = 0
        Dim lowStock As Integer = 0
        For Each d As Drug In DataStore.Drugs
            If DataStore.DaysLeft(d) >= 0 Then stockValue += d.Quantity * d.UnitPrice
            If d.Quantity <= d.ReorderLevel Then lowStock += 1
        Next

        ' ---------------- draw the page ----------------
        Using titleFont As New Font("Segoe UI", 18.0F, FontStyle.Bold),
              headFont As New Font("Segoe UI", 11.0F, FontStyle.Bold),
              textFont As New Font("Segoe UI", 10.0F),
              boldFont As New Font("Segoe UI", 10.0F, FontStyle.Bold),
              smallFont As New Font("Segoe UI", 8.5F),
              blue As New SolidBrush(Branding.ThemeColor()),
              ink As New SolidBrush(Color.FromArgb(31, 41, 55)),
              grey As New SolidBrush(Color.FromArgb(100, 116, 139)),
              band As New SolidBrush(Color.FromArgb(232, 240, 250)),
              linePen As New Pen(Color.FromArgb(200, 210, 222)),
              rightAlign As New StringFormat()
            rightAlign.Alignment = StringAlignment.Far

            Dim row = Sub(label As String, value As String, f As Font)
                          g.DrawString(label, f, ink, left + 10, y)
                          g.DrawString(value, f, ink, New RectangleF(left, y, width - 10, f.GetHeight(g) + 2), rightAlign)
                          y += f.GetHeight(g) + 5
                      End Sub
            Dim section = Sub(title As String)
                              y += 10
                              g.FillRectangle(band, left, y, width, headFont.GetHeight(g) + 8)
                              g.DrawString(title, headFont, blue, left + 8, y + 4)
                              y += headFont.GetHeight(g) + 14
                          End Sub

            ' Header
            ' Logo on the right, business name and contacts on the left
            Dim headerTop As Single = y
            Dim logo As Image = Branding.Logo()
            If logo IsNot Nothing Then
                Dim h As Single = Math.Min(70.0F, logo.Height)
                Dim w As Single = Math.Min(200.0F, h * logo.Width / logo.Height)
                h = w * logo.Height / logo.Width
                g.DrawImage(logo, left + width - w, headerTop, w, h)
            End If
            g.DrawString(Branding.BusinessName(), titleFont, blue, left, y)
            y += titleFont.GetHeight(g)
            Dim contact As String = DataStore.GetSetting("PharmacyAddress")
            Dim phones As List(Of String) = Branding.PhoneNumbers()
            If phones.Count > 0 Then contact &= If(contact = "", "", "   |   ") & "Tel: " & String.Join(", ", phones.ToArray())
            Dim email As String = DataStore.GetSetting("PharmacyEmail")
            If email <> "" Then contact &= If(contact = "", "", "   |   ") & email
            If contact <> "" Then
                g.DrawString(contact, textFont, grey, New RectangleF(left, y, width - 210, textFont.GetHeight(g) * 2 + 2))
                y += textFont.GetHeight(g) * If(g.MeasureString(contact, textFont).Width > width - 210, 2, 1)
            End If
            If logo IsNot Nothing Then y = Math.Max(y, headerTop + 74)
            y += 6
            g.DrawString("SALES SUMMARY", headFont, ink, left, y)
            Dim period As String = If(fromDate = toDate, fromDate.ToString("dddd, d MMMM yyyy"),
                                      fromDate.ToString("d MMM yyyy") & "  to  " & toDate.ToString("d MMM yyyy"))
            g.DrawString(period, headFont, ink, New RectangleF(left, y, width, headFont.GetHeight(g) + 2), rightAlign)
            y += headFont.GetHeight(g) + 6
            g.DrawLine(linePen, left, y, left + width, y)
            y += 4

            section("Money taken")
            row("Receipts", receipts.ToString("N0"), textFont)
            row("Gross sales (before discounts)", (netTotal + discounts).ToString("N2"), textFont)
            row("Discounts given", "-" & discounts.ToString("N2"), textFont)
            row("NET SALES (GH₵)", netTotal.ToString("N2"), headFont)
            row("Average per receipt", If(receipts > 0, netTotal / receipts, 0D).ToString("N2"), textFont)
            row("Units sold", units.ToString("N0"), textFont)
            row("Voided / refunded receipts", voidedCount.ToString() & "   (" & voidedValue.ToString("N2") & ")", textFont)

            section("By payment method")
            For Each m As String In byMethod.Keys
                row(m & "  (" & byMethodCount(m).ToString() & " receipts)", byMethod(m).ToString("N2"), textFont)
            Next

            section("Best-selling drugs")
            Dim names As New List(Of String)(qtyByDrug.Keys)
            names.Sort(Function(a, b) moneyByDrug(b).CompareTo(moneyByDrug(a)))
            If names.Count = 0 Then
                g.DrawString("No sales in this period.", textFont, grey, left + 10, y)
                y += textFont.GetHeight(g) + 5
            End If
            For i As Integer = 0 To Math.Min(names.Count, 10) - 1
                row((i + 1).ToString() & ".  " & names(i) & "   x " & qtyByDrug(names(i)).ToString(), moneyByDrug(names(i)).ToString("N2"), textFont)
            Next

            section("Stock")
            row("Expired stock written off in this period", lossValue.ToString("N2"), textFont)
            row("Value of stock on the shelf now (not expired)", stockValue.ToString("N2"), textFont)
            row("Drugs at or below the reorder level now", lowStock.ToString(), textFont)

            ' Footer with signature lines
            y = Math.Max(y + 40, e.MarginBounds.Bottom - 70)
            g.DrawLine(linePen, left, y, left + 220, y)
            g.DrawLine(linePen, left + width - 220, y, left + width, y)
            g.DrawString("Prepared by: " & DataStore.CurrentUser, smallFont, grey, left, y + 4)
            g.DrawString("Checked by (signature)", smallFont, grey, left + width - 220, y + 4)
            g.DrawString("Printed " & Date.Now.ToString("dd/MM/yyyy h:mm tt") & "  -  Powered by MediTrack",
                         smallFont, grey, left, e.MarginBounds.Bottom - 10)
        End Using
        e.HasMorePages = False
    End Sub

End Class
