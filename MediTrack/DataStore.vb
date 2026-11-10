' ============================================================================
'  DataStore.vb  -  the "backend" of MediTrack
'
'  * Creates the Access tables the first time. The First-Time Setup window
'    then creates the pharmacy details and the first administrator
'    (there are no built-in demo passwords).
'  * Loads every table into lists when someone signs in (fast screens).
'  * Every change is written to the Access database FIRST, then to the lists,
'    so the data is saved permanently.
'  * A sale and a write-off use a TRANSACTION: either every part is saved,
'    or nothing is (the stock can never be half-updated).
'  * Passwords and the admin access code are stored as SHA-256 hashes,
'    never as plain text.
' ============================================================================
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.OleDb
Imports System.Security.Cryptography
Imports System.Text

Public Module DataStore

    ' ---------------- Data loaded from the database ----------------
    Public Drugs As New List(Of Drug)
    Public Suppliers As New List(Of Supplier)
    Public History As New List(Of HistoryEntry)
    Public Sales As New List(Of SaleItem)        ' every sold line (used by the dashboard)
    Public SaleHeaders As New List(Of Sale)      ' every receipt
    Public Losses As New List(Of LossRecord)
    Public Users As New List(Of AppUser)
    Public Settings As New Dictionary(Of String, String)
    Public VoidedSaleIds As New HashSet(Of Integer)   ' receipts that were cancelled / refunded

    ''' <summary>True if the receipt was voided (its money and items do not count any more).</summary>
    Public Function IsVoided(saleId As Integer) As Boolean
        Return VoidedSaleIds.Contains(saleId)
    End Function

    ''' <summary>Minimum length for passwords and the admin access code.</summary>
    Public Const MinimumPasswordLength As Integer = 6

    ' ---------------- Who is signed in ----------------
    Public CurrentUser As String = ""         ' full name shown on screens and receipts
    Public CurrentUsername As String = ""     ' login name (empty when signed in with the access code)
    Public CurrentRole As String = ""        ' "Admin" or "Staff"

    Public Function IsAdmin() As Boolean
        Return CurrentRole = "Admin"
    End Function

    ' ========================================================================
    '  START-UP
    ' ========================================================================

    ''' <summary>Opens (or creates) the database, then loads everything into the lists.</summary>
    Public Sub Initialise()
        Dim isNew As Boolean = Database.OpenOrCreate()
        If isNew OrElse Not TableExists("Drugs") Then
            CreateTables()                ' empty tables - First-Time Setup fills them
        ElseIf HasBrokenReceipts() Then
            ' A database made by the earlier build saved receipts with number 0.
            If System.Windows.Forms.MessageBox.Show(
                    "This database was made by an earlier test version and some receipts were saved with the wrong number (0)." &
                    vbCrLf & vbCrLf & "Reset the database now? (Recommended)" & vbCrLf &
                    "All test sales, drugs and users in it will be removed and First-Time Setup will start.",
                    "MediTrack - Repair Database", System.Windows.Forms.MessageBoxButtons.YesNo,
                    System.Windows.Forms.MessageBoxIcon.Question) = System.Windows.Forms.DialogResult.Yes Then
                For Each table As String In {"SaleItems", "Sales", "History", "Losses", "Drugs", "Suppliers", "Settings", "Users"}
                    Try
                        Database.Execute("DROP TABLE [" & table & "]")
                    Catch
                        ' table already missing
                    End Try
                Next
                CreateTables()
            End If
        End If
        UpgradeTables()
        LoadAll()
    End Sub

    ''' <summary>True when the database has no accounts yet, so First-Time Setup must run.</summary>
    Public Function NeedsSetup() As Boolean
        Return Convert.ToInt32(Database.Scalar("SELECT COUNT(*) FROM Users")) = 0
    End Function

    ''' <summary>
    ''' Saves what the owner typed in First-Time Setup: the pharmacy details, the
    ''' first administrator and the admin access code - all in one transaction.
    ''' Sample drugs, suppliers and sales are added only if asked for.
    ''' </summary>
    Public Sub CompleteSetup(pharmacyName As String, address As String, phone As String,
                             adminFullName As String, adminUsername As String, adminPassword As String,
                             accessCode As String, includeSamples As Boolean,
                             Optional logoText As String = "", Optional themeHex As String = "")
        Using cn As OleDbConnection = Database.OpenConnection()
            Dim tx As OleDbTransaction = cn.BeginTransaction()
            Try
                Database.ExecuteOn(cn, tx, "DELETE FROM Settings")
                Database.ExecuteOn(cn, tx, "INSERT INTO Users (Username, PasswordHash, FullName, Role, IsActive, CreatedOn) VALUES (?, ?, ?, ?, ?, ?)",
                                   adminUsername.Trim().ToLower(), HashText(adminUsername.Trim(), adminPassword), adminFullName.Trim(),
                                   "Admin", True, Date.Now)
                For Each pair As String() In New String()() {
                        New String() {"AdminCodeHash", HashText("admin-code", accessCode.Trim())},
                        New String() {"PharmacyName", pharmacyName},
                        New String() {"PharmacyAddress", address},
                        New String() {"PharmacyPhone", phone},
                        New String() {"ReceiptFooter", "Thank you. Get well soon!"},
                        New String() {"PharmacyEmail", ""},
                        New String() {"LogoImage", logoText},
                        New String() {"ThemeColor", themeHex},
                        New String() {"SetupDate", Date.Now.ToString("yyyy-MM-dd HH:mm")}}
                    Database.ExecuteOn(cn, tx, "INSERT INTO Settings (SettingKey, SettingValue) VALUES (?, ?)", pair(0), pair(1))
                Next
                If includeSamples Then AddSampleData(cn, tx, adminFullName.Trim())
                tx.Commit()
            Catch
                tx.Rollback()
                Throw
            End Try
        End Using
        MarkChanged()
        LoadAll()
    End Sub

    ''' <summary>Adds columns that newer versions need to a database made by an older version.</summary>
    Private Sub UpgradeTables()
        For Each sql As String In {"ALTER TABLE Sales ADD COLUMN Discount CURRENCY",
                                   "ALTER TABLE Sales ADD COLUMN PaymentMethod TEXT(30)",
                                   "ALTER TABLE Sales ADD COLUMN Voided YESNO",
                                   "ALTER TABLE Sales ADD COLUMN VoidReason TEXT(200)"}
            Try
                Database.Execute(sql)
            Catch
                ' the column is already there
            End Try
        Next
    End Sub

    ''' <summary>True if any receipt line was saved without a real receipt number.</summary>
    Private Function HasBrokenReceipts() As Boolean
        Try
            Return Convert.ToInt32(Database.Scalar("SELECT COUNT(*) FROM SaleItems WHERE SaleID = 0")) > 0
        Catch
            Return False
        End Try
    End Function

    Private Function TableExists(name As String) As Boolean
        Try
            Database.Scalar("SELECT COUNT(*) FROM [" & name & "]")
            Return True
        Catch
            Return False
        End Try
    End Function

    Private Sub CreateTables()
        Database.Execute("CREATE TABLE Users (UserID COUNTER PRIMARY KEY, Username TEXT(50) NOT NULL, " &
                         "PasswordHash TEXT(100), FullName TEXT(100), Role TEXT(20), IsActive YESNO, CreatedOn DATETIME)")
        Database.Execute("CREATE TABLE Settings (SettingKey TEXT(50) PRIMARY KEY, SettingValue MEMO)")
        Database.Execute("CREATE TABLE Suppliers (SupplierID COUNTER PRIMARY KEY, SupplierName TEXT(100) NOT NULL, " &
                         "ContactPerson TEXT(100), Phone TEXT(30), Email TEXT(100), Address MEMO, SuppliedDrugs MEMO)")
        Database.Execute("CREATE TABLE Drugs (DrugID COUNTER PRIMARY KEY, DrugName TEXT(100) NOT NULL, Category TEXT(50), " &
                         "Quantity LONG, UnitPrice CURRENCY, BatchNo TEXT(50), ExpiryDate DATETIME, " &
                         "SupplierName TEXT(100), SupplierContact TEXT(30), ReorderLevel LONG)")
        Database.Execute("CREATE TABLE History (HistoryID COUNTER PRIMARY KEY, LogDate DATETIME, DrugName TEXT(100), " &
                         "ActionType TEXT(30), QtyChange LONG, Party TEXT(100), BatchNo TEXT(50), UserName TEXT(100))")
        Database.Execute("CREATE TABLE Sales (SaleID COUNTER PRIMARY KEY, SaleDate DATETIME, Customer TEXT(100), " &
                         "SoldBy TEXT(100), Total CURRENCY, AmountPaid CURRENCY, Discount CURRENCY, PaymentMethod TEXT(30), " &
                         "Voided YESNO, VoidReason TEXT(200))")
        Database.Execute("CREATE TABLE SaleItems (ItemID COUNTER PRIMARY KEY, SaleID LONG, DrugID LONG, " &
                         "DrugName TEXT(100), BatchNo TEXT(50), Quantity LONG, UnitPrice CURRENCY)")
        Database.Execute("CREATE TABLE Losses (LossID COUNTER PRIMARY KEY, LossDate DATETIME, DrugName TEXT(100), " &
                         "BatchNo TEXT(50), Quantity LONG, LossValue CURRENCY, Reason TEXT(50))")
    End Sub

    ''' <summary>Sample suppliers, drugs, sales and history (only when chosen in First-Time Setup).</summary>
    Private Sub AddSampleData(cn As OleDbConnection, tx As OleDbTransaction, sellerName As String)
        Dim stampNow As Date = Date.Now

        ' ---- Suppliers ----
        Dim sup As String()() = {
            New String() {"MediPharm Ltd", "Kwame Mensah", "024 456 7890", "sales@medipharm.com", "12 Ring Road, Accra", "Paracetamol, Ibuprofen"},
            New String() {"HealthPlus", "Ama Owusu", "020 333 4444", "orders@healthplus.com", "5 Oxford Street, Osu", "Amoxicillin, Ciprofloxacin"},
            New String() {"PharmaCare", "Yaw Boateng", "024 987 6543", "info@pharmacare.com", "Spintex Road, Accra", "Metformin, Amlodipine"},
            New String() {"Wellness Supplies", "Efua Asante", "020 111 2233", "hello@wellness.com", "Adum, Kumasi", "Vitamin C, Multivitamins"},
            New String() {"Global Pharma", "Kofi Adjei", "027 555 1212", "contact@globalpharma.com", "Tema Industrial Area", "Artemether, Cough syrups"}}
        For Each s As String() In sup
            Database.ExecuteOn(cn, tx, "INSERT INTO Suppliers (SupplierName, ContactPerson, Phone, Email, Address, SuppliedDrugs) VALUES (?, ?, ?, ?, ?, ?)",
                               s(0), s(1), s(2), s(3), s(4), s(5))
        Next

        ' ---- Drugs (expiry dates are set from today so the demo always shows every colour) ----
        Dim t As Date = Date.Today
        Dim names As String() = {"Paracetamol 500mg", "Amoxicillin 500mg", "Metformin 500mg", "Ibuprofen 400mg", "Vitamin C 1000mg",
                                 "Artemether/Lumefantrine", "Amlodipine 5mg", "Ciprofloxacin 500mg", "Cough Syrup 100ml", "Multivitamin Tablets"}
        Dim cats As String() = {"Analgesic", "Antibiotic", "Antidiabetic", "Analgesic", "Supplement",
                                "Antimalarial", "Antihypertensive", "Antibiotic", "Cough & Cold", "Supplement"}
        Dim qtys As Integer() = {120, 14, 60, 12, 100, 40, 75, 30, 8, 55}
        Dim prices As Decimal() = {2.5D, 4.2D, 3.8D, 2.8D, 3.5D, 18D, 6.5D, 5.2D, 15D, 12D}
        Dim batches As String() = {"PCT2456", "AMX7781", "MTF1223", "IBU6677", "VTC9832", "ART5520", "AML3301", "CIP1001", "CSY0909", "MVT7710"}
        Dim expiries As Date() = {t.AddMonths(7), t.AddMonths(10), t.AddDays(45), t.AddMonths(9), t.AddMonths(15),
                                  t.AddDays(20), t.AddMonths(18), t.AddDays(70), t.AddDays(-12), t.AddMonths(12)}
        Dim supIndex As Integer() = {0, 1, 2, 0, 3, 4, 2, 1, 4, 3}
        Dim reorders As Integer() = {20, 15, 10, 15, 25, 10, 15, 10, 5, 10}
        Dim drugIds(9) As Integer
        For i As Integer = 0 To 9
            drugIds(i) = Database.InsertOn(cn, tx,
                "INSERT INTO Drugs (DrugName, Category, Quantity, UnitPrice, BatchNo, ExpiryDate, SupplierName, SupplierContact, ReorderLevel) " &
                "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)",
                names(i), cats(i), qtys(i), prices(i), batches(i), expiries(i),
                sup(supIndex(i))(0), sup(supIndex(i))(2), reorders(i))
        Next

        ' ---- Past sales (so the dashboard and Sales Records have something to show) ----
        Dim picks As Integer() = {0, 4, 1, 6, 0, 5, 3, 9, 0, 7, 4, 2}
        Dim sold As Integer() = {2, 1, 1, 2, 3, 1, 2, 1, 1, 2, 2, 1}
        Dim monthStart As New Date(t.Year, t.Month, 1)
        Dim times As New List(Of Date)
        Dim pickIndex As New List(Of Integer)
        For day As Integer = 0 To t.Day - 1
            For k As Integer = 0 To day Mod 3
                times.Add(monthStart.AddDays(day).AddHours(9 + k * 3).AddMinutes((day * 7) Mod 50))
                pickIndex.Add((day * 3 + k) Mod picks.Length)
            Next
        Next
        For day As Integer = 0 To 19
            times.Add(monthStart.AddMonths(-1).AddDays(day).AddHours(11))
            pickIndex.Add((day * 5) Mod picks.Length)
        Next
        Dim payMethods As String() = {"Cash", "Mobile Money", "Cash", "Card"}
        For n As Integer = 0 To times.Count - 1
            Dim di As Integer = picks(pickIndex(n))
            Dim q As Integer = sold(pickIndex(n))
            Dim total As Decimal = q * prices(di)
            Dim saleId As Integer = Database.InsertOn(cn, tx,
                "INSERT INTO Sales (SaleDate, Customer, SoldBy, Total, AmountPaid, Discount, PaymentMethod) VALUES (?, ?, ?, ?, ?, ?, ?)",
                times(n), "Walk-in", sellerName, total, total, 0D, payMethods(n Mod 4))
            Database.ExecuteOn(cn, tx,
                "INSERT INTO SaleItems (SaleID, DrugID, DrugName, BatchNo, Quantity, UnitPrice) VALUES (?, ?, ?, ?, ?, ?)",
                saleId, drugIds(di), names(di), batches(di), q, prices(di))
        Next

        ' ---- One earlier loss and a few history lines ----
        Database.ExecuteOn(cn, tx, "INSERT INTO Losses (LossDate, DrugName, BatchNo, Quantity, LossValue, Reason) VALUES (?, ?, ?, ?, ?, ?)",
                           t.AddDays(-6), "Ciprofloxacin 500mg", "CIP0802", 10, 52D, "Expired")
        Dim hist As Object()() = {
            New Object() {stampNow.AddDays(-6), "Ciprofloxacin 500mg", "Written Off", -10, "HealthPlus", "CIP0802"},
            New Object() {stampNow.AddDays(-5), "Vitamin C 1000mg", "Stock In", 30, "Wellness Supplies", "VTC9832"},
            New Object() {stampNow.AddDays(-4), "Metformin 500mg", "Updated", 0, "PharmaCare", "MTF1223"},
            New Object() {stampNow.AddDays(-3), "Ibuprofen 400mg", "Stock Out", -10, "-", "IBU6677"},
            New Object() {stampNow.AddDays(-1), "Amoxicillin 500mg", "Stock In", 50, "HealthPlus", "AMX7781"},
            New Object() {stampNow.AddHours(-2), "Multivitamin Tablets", "Added", 55, "Wellness Supplies", "MVT7710"}}
        For Each h As Object() In hist
            Database.ExecuteOn(cn, tx, "INSERT INTO History (LogDate, DrugName, ActionType, QtyChange, Party, BatchNo, UserName) VALUES (?, ?, ?, ?, ?, ?, ?)",
                               h(0), h(1), h(2), h(3), h(4), h(5), "System")
        Next
    End Sub

    ' ========================================================================
    '  LOADING EVERYTHING FROM THE DATABASE INTO THE LISTS
    ' ========================================================================
    Public Sub LoadAll()
        Drugs.Clear()
        For Each r As DataRow In Database.GetTable("SELECT * FROM Drugs ORDER BY DrugName").Rows
            Dim d As New Drug()
            d.ID = ToInt(r("DrugID"))
            d.DrugName = Txt(r("DrugName"))
            d.Category = Txt(r("Category"))
            d.Quantity = ToInt(r("Quantity"))
            d.UnitPrice = ToMoney(r("UnitPrice"))
            d.BatchNo = Txt(r("BatchNo"))
            d.ExpiryDate = ToDate(r("ExpiryDate"))
            d.SupplierName = Txt(r("SupplierName"))
            d.SupplierContact = Txt(r("SupplierContact"))
            d.ReorderLevel = ToInt(r("ReorderLevel"))
            Drugs.Add(d)
        Next

        Suppliers.Clear()
        For Each r As DataRow In Database.GetTable("SELECT * FROM Suppliers ORDER BY SupplierName").Rows
            Dim s As New Supplier()
            s.ID = ToInt(r("SupplierID"))
            s.SupplierName = Txt(r("SupplierName"))
            s.ContactPerson = Txt(r("ContactPerson"))
            s.Phone = Txt(r("Phone"))
            s.Email = Txt(r("Email"))
            s.Address = Txt(r("Address"))
            s.SuppliedDrugs = Txt(r("SuppliedDrugs"))
            Suppliers.Add(s)
        Next

        History.Clear()
        For Each r As DataRow In Database.GetTable("SELECT * FROM History ORDER BY LogDate").Rows
            Dim h As New HistoryEntry()
            h.LogDate = ToDate(r("LogDate"))
            h.DrugName = Txt(r("DrugName"))
            h.ActionType = Txt(r("ActionType"))
            h.QtyChange = ToInt(r("QtyChange"))
            h.Party = Txt(r("Party"))
            h.BatchNo = Txt(r("BatchNo"))
            h.UserName = Txt(r("UserName"))
            History.Add(h)
        Next

        SaleHeaders.Clear()
        VoidedSaleIds.Clear()
        Dim dateById As New Dictionary(Of Integer, Date)
        For Each r As DataRow In Database.GetTable("SELECT * FROM Sales ORDER BY SaleDate").Rows
            Dim s As New Sale()
            s.SaleID = ToInt(r("SaleID"))
            s.SaleDate = ToDate(r("SaleDate"))
            s.Customer = Txt(r("Customer"))
            s.SoldBy = Txt(r("SoldBy"))
            s.Total = ToMoney(r("Total"))
            s.AmountPaid = ToMoney(r("AmountPaid"))
            s.Discount = If(r.Table.Columns.Contains("Discount"), ToMoney(r("Discount")), 0D)
            s.PaymentMethod = If(r.Table.Columns.Contains("PaymentMethod"), Txt(r("PaymentMethod")), "")
            If s.PaymentMethod = "" Then s.PaymentMethod = "Cash"
            s.Voided = r.Table.Columns.Contains("Voided") AndAlso Not IsDBNull(r("Voided")) AndAlso CBool(r("Voided"))
            s.VoidReason = If(r.Table.Columns.Contains("VoidReason"), Txt(r("VoidReason")), "")
            SaleHeaders.Add(s)
            If s.Voided Then VoidedSaleIds.Add(s.SaleID)
            dateById(s.SaleID) = s.SaleDate
        Next

        Sales.Clear()
        For Each r As DataRow In Database.GetTable("SELECT * FROM SaleItems ORDER BY ItemID").Rows
            Dim item As New SaleItem()
            item.SaleID = ToInt(r("SaleID"))
            item.DrugID = ToInt(r("DrugID"))
            item.DrugName = Txt(r("DrugName"))
            item.BatchNo = Txt(r("BatchNo"))
            item.Quantity = ToInt(r("Quantity"))
            item.UnitPrice = ToMoney(r("UnitPrice"))
            If dateById.ContainsKey(item.SaleID) Then item.SaleDate = dateById(item.SaleID)
            Sales.Add(item)
        Next

        Losses.Clear()
        For Each r As DataRow In Database.GetTable("SELECT * FROM Losses ORDER BY LossDate").Rows
            Dim l As New LossRecord()
            l.LossDate = ToDate(r("LossDate"))
            l.DrugName = Txt(r("DrugName"))
            l.BatchNo = Txt(r("BatchNo"))
            l.Quantity = ToInt(r("Quantity"))
            l.Value = ToMoney(r("LossValue"))
            l.Reason = Txt(r("Reason"))
            Losses.Add(l)
        Next

        Users.Clear()
        For Each r As DataRow In Database.GetTable("SELECT UserID, Username, FullName, Role, IsActive FROM Users ORDER BY Username").Rows
            Dim u As New AppUser()
            u.ID = ToInt(r("UserID"))
            u.Username = Txt(r("Username"))
            u.FullName = Txt(r("FullName"))
            u.Role = Txt(r("Role"))
            u.IsActive = Not IsDBNull(r("IsActive")) AndAlso CBool(r("IsActive"))
            Users.Add(u)
        Next

        Settings.Clear()
        For Each r As DataRow In Database.GetTable("SELECT SettingKey, SettingValue FROM Settings").Rows
            Settings(Txt(r("SettingKey"))) = Txt(r("SettingValue"))
        Next
        LoadedVersion = GetSetting("DataVersion", "")
    End Sub

    ' ========================================================================
    '  LIVE UPDATES BETWEEN WINDOWS / COMPUTERS
    '  Every save writes a new 'DataVersion' stamp in the Settings table.
    '  Each open window checks the stamp every few seconds; if it is not the
    '  one it loaded, someone else saved something, so it reloads.
    ' ========================================================================
    Public LoadedVersion As String = ""

    ''' <summary>Writes a new version stamp so other open windows know to reload.</summary>
    Private Sub MarkChanged()
        Dim stamp As String = Date.Now.Ticks.ToString() & "-" & Guid.NewGuid().ToString("N").Substring(0, 8)
        If Database.Execute("UPDATE Settings SET SettingValue = ? WHERE SettingKey = ?", stamp, "DataVersion") = 0 Then
            Database.Execute("INSERT INTO Settings (SettingKey, SettingValue) VALUES (?, ?)", "DataVersion", stamp)
        End If
        Settings("DataVersion") = stamp
        LoadedVersion = stamp
    End Sub

    ''' <summary>True if another window or computer has saved changes since this one last loaded.</summary>
    Public Function HasChangedElsewhere() As Boolean
        Dim v As Object = Database.Scalar("SELECT SettingValue FROM Settings WHERE SettingKey = ?", "DataVersion")
        Dim current As String = If(v Is Nothing OrElse IsDBNull(v), "", v.ToString())
        Return current <> LoadedVersion
    End Function

    ' ========================================================================
    '  SIGNING IN
    ' ========================================================================

    ''' <summary>SHA-256 hash of a password. The username is mixed in so equal passwords give different hashes.</summary>
    Public Function HashText(salt As String, text As String) As String
        Using sha As SHA256 = SHA256.Create()
            Dim bytes As Byte() = sha.ComputeHash(Encoding.UTF8.GetBytes("MediTrack|" & salt.ToLower() & "|" & text))
            Return Convert.ToBase64String(bytes)
        End Using
    End Function

    ''' <summary>Checks a username and password. Returns the user, or Nothing if wrong or disabled.</summary>
    Public Function CheckLogin(username As String, password As String) As AppUser
        Dim t As DataTable = Database.GetTable("SELECT UserID, Username, FullName, Role, IsActive, PasswordHash FROM Users WHERE Username = ?",
                                               username.Trim().ToLower())
        If t.Rows.Count = 0 Then Return Nothing
        Dim r As DataRow = t.Rows(0)
        If Txt(r("PasswordHash")) <> HashText(username.Trim(), password) Then Return Nothing
        If IsDBNull(r("IsActive")) OrElse Not CBool(r("IsActive")) Then Return Nothing
        Dim u As New AppUser()
        u.ID = ToInt(r("UserID"))
        u.Username = Txt(r("Username"))
        u.FullName = Txt(r("FullName"))
        u.Role = Txt(r("Role"))
        u.IsActive = True
        Return u
    End Function

    Public Function CheckAdminCode(code As String) As Boolean
        Dim stored As Object = Database.Scalar("SELECT SettingValue FROM Settings WHERE SettingKey = ?", "AdminCodeHash")
        Return stored IsNot Nothing AndAlso Txt(stored) = HashText("admin-code", code.Trim())
    End Function

    ''' <summary>
    ''' Security check for databases made by older test versions: lists the
    ''' accounts (and the access code) that still use a well-known test password.
    ''' </summary>
    Public Function WeakDefaultLogins() As List(Of String)
        Dim found As New List(Of String)
        For Each pair As String() In New String()() {New String() {"staff", "1234"}, New String() {"pharmacist", "1234"},
                                                     New String() {"manager", "admin123"}}
            Dim stored As Object = Database.Scalar("SELECT PasswordHash FROM Users WHERE Username = ? AND IsActive = True", pair(0))
            If stored IsNot Nothing AndAlso Txt(stored) = HashText(pair(0), pair(1)) Then found.Add("user '" & pair(0) & "'")
        Next
        If CheckAdminCode("ADMIN123") Then found.Add("the admin access code")
        Return found
    End Function

    Public Sub ChangeAdminCode(newCode As String)
        SaveSetting("AdminCodeHash", HashText("admin-code", newCode.Trim()))
    End Sub

    ' ========================================================================
    '  SETTINGS
    ' ========================================================================
    Public Function GetSetting(key As String, Optional fallback As String = "") As String
        If Settings.ContainsKey(key) AndAlso Settings(key) <> "" Then Return Settings(key)
        Return fallback
    End Function

    Public Sub SaveSetting(key As String, value As String)
        If Database.Execute("UPDATE Settings SET SettingValue = ? WHERE SettingKey = ?", value, key) = 0 Then
            Database.Execute("INSERT INTO Settings (SettingKey, SettingValue) VALUES (?, ?)", key, value)
        End If
        Settings(key) = value
        MarkChanged()
    End Sub

    ''' <summary>Lets a signed-in user change their own password. Returns False if the old password is wrong.</summary>
    Public Function ChangeOwnPassword(username As String, oldPassword As String, newPassword As String) As Boolean
        If CheckLogin(username, oldPassword) Is Nothing Then Return False
        Database.Execute("UPDATE Users SET PasswordHash = ? WHERE Username = ?", HashText(username, newPassword), username.Trim().ToLower())
        MarkChanged()
        Return True
    End Function

    ''' <summary>
    ''' Daily automatic backup: once a day, copies the database to the Backups folder
    ''' next to the program and keeps the 14 newest copies.
    ''' </summary>
    Public Function AutoBackup() As String
        Try
            Dim folder As String = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Backups")
            System.IO.Directory.CreateDirectory(folder)
            Dim ext As String = System.IO.Path.GetExtension(Database.DatabasePath)
            Dim target As String = System.IO.Path.Combine(folder, "MediTrack_" & Date.Today.ToString("yyyy-MM-dd") & ext)
            If Not System.IO.File.Exists(target) Then Database.BackupTo(target)
            Dim copies As String() = System.IO.Directory.GetFiles(folder, "MediTrack_*" & ext)
            Array.Sort(copies)
            For i As Integer = 0 To copies.Length - 15
                System.IO.File.Delete(copies(i))
            Next
            Return target
        Catch
            Return ""     ' a backup problem must never stop the pharmacy from working
        End Try
    End Function

    ' ========================================================================
    '  USERS
    ' ========================================================================
    Public Function FindUser(username As String) As AppUser
        For Each u As AppUser In Users
            If u.Username.ToLower() = username.Trim().ToLower() Then Return u
        Next
        Return Nothing
    End Function

    Public Function FindUserById(id As Integer) As AppUser
        For Each u As AppUser In Users
            If u.ID = id Then Return u
        Next
        Return Nothing
    End Function

    Public Function AddUser(username As String, fullName As String, password As String, role As String, active As Boolean) As AppUser
        Dim u As New AppUser()
        u.Username = username.Trim().ToLower()
        u.FullName = fullName.Trim()
        u.Role = role
        u.IsActive = active
        u.ID = Database.InsertAndGetId("INSERT INTO Users (Username, PasswordHash, FullName, Role, IsActive, CreatedOn) VALUES (?, ?, ?, ?, ?, ?)",
                                       u.Username, HashText(u.Username, password), u.FullName, u.Role, u.IsActive, Date.Now)
        Users.Add(u)
        MarkChanged()
        Return u
    End Function

    ''' <summary>How many active Admin accounts there are, not counting the given user.</summary>
    Public Function OtherActiveAdmins(userId As Integer) As Integer
        Return Convert.ToInt32(Database.Scalar("SELECT COUNT(*) FROM Users WHERE Role = 'Admin' AND IsActive = True AND UserID <> ?", userId))
    End Function

    ''' <summary>Saves a changed user. The password only changes when newPassword is not empty.</summary>
    Public Sub UpdateUser(u As AppUser, newPassword As String)
        If (u.Role <> "Admin" OrElse Not u.IsActive) AndAlso OtherActiveAdmins(u.ID) = 0 Then
            Throw New InvalidOperationException("This is the only active Admin account. Keep it active as Admin, " &
                                                "or make another user an Admin first.")
        End If
        Database.Execute("UPDATE Users SET FullName = ?, Role = ?, IsActive = ? WHERE UserID = ?", u.FullName, u.Role, u.IsActive, u.ID)
        If newPassword <> "" Then
            Database.Execute("UPDATE Users SET PasswordHash = ? WHERE UserID = ?", HashText(u.Username, newPassword), u.ID)
        End If
        MarkChanged()
    End Sub

    ' ========================================================================
    '  DRUGS
    ' ========================================================================
    Public Function AddDrug(name As String, category As String, qty As Integer, price As Decimal, batch As String,
                            expiry As Date, supplier As String, contact As String, reorder As Integer) As Drug
        Dim d As New Drug()
        d.DrugName = name
        d.Category = category
        d.Quantity = qty
        d.UnitPrice = price
        d.BatchNo = batch
        d.ExpiryDate = expiry.Date
        d.SupplierName = supplier
        d.SupplierContact = contact
        d.ReorderLevel = reorder
        d.ID = Database.InsertAndGetId(
            "INSERT INTO Drugs (DrugName, Category, Quantity, UnitPrice, BatchNo, ExpiryDate, SupplierName, SupplierContact, ReorderLevel) " &
            "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)",
            d.DrugName, d.Category, d.Quantity, d.UnitPrice, d.BatchNo, d.ExpiryDate, d.SupplierName, d.SupplierContact, d.ReorderLevel)
        Drugs.Add(d)
        MarkChanged()
        Return d
    End Function

    ''' <summary>
    ''' Saves a drug's details. The stock is changed by qtyChange (not overwritten),
    ''' so a sale made on another computer at the same moment is never lost.
    ''' </summary>
    Public Sub UpdateDrug(d As Drug, qtyChange As Integer)
        Dim changed As Integer = Database.Execute(
            "UPDATE Drugs SET DrugName = ?, Category = ?, Quantity = Quantity + ?, UnitPrice = ?, BatchNo = ?, ExpiryDate = ?, " &
            "SupplierName = ?, SupplierContact = ?, ReorderLevel = ? WHERE DrugID = ? AND Quantity + ? >= 0",
            d.DrugName, d.Category, qtyChange, d.UnitPrice, d.BatchNo, d.ExpiryDate.Date,
            d.SupplierName, d.SupplierContact, d.ReorderLevel, d.ID, qtyChange)
        If changed = 0 Then
            Throw New InvalidOperationException("The stock of " & d.DrugName & " has changed (it may have just been sold " &
                                                "on another computer) or the drug was deleted. Check the quantity and try again.")
        End If
        MarkChanged()
    End Sub

    Public Sub DeleteDrug(d As Drug)
        Database.Execute("DELETE FROM Drugs WHERE DrugID = ?", d.ID)
        Dim id As Integer = d.ID
        Drugs.RemoveAll(Function(x) x.ID = id)
        MarkChanged()
    End Sub

    ' ========================================================================
    '  SUPPLIERS
    ' ========================================================================
    Public Function AddSupplier(name As String, person As String, phone As String, email As String,
                                address As String, supplied As String) As Supplier
        Dim s As New Supplier()
        s.SupplierName = name
        s.ContactPerson = person
        s.Phone = phone
        s.Email = email
        s.Address = address
        s.SuppliedDrugs = supplied
        s.ID = Database.InsertAndGetId("INSERT INTO Suppliers (SupplierName, ContactPerson, Phone, Email, Address, SuppliedDrugs) VALUES (?, ?, ?, ?, ?, ?)",
                                       s.SupplierName, s.ContactPerson, s.Phone, s.Email, s.Address, s.SuppliedDrugs)
        Suppliers.Add(s)
        MarkChanged()
        Return s
    End Function

    ''' <summary>Saves a changed supplier, and updates the drugs that used its old name.</summary>
    Public Sub UpdateSupplier(s As Supplier, oldName As String)
        Database.Execute("UPDATE Suppliers SET SupplierName = ?, ContactPerson = ?, Phone = ?, Email = ?, Address = ?, SuppliedDrugs = ? WHERE SupplierID = ?",
                         s.SupplierName, s.ContactPerson, s.Phone, s.Email, s.Address, s.SuppliedDrugs, s.ID)
        Database.Execute("UPDATE Drugs SET SupplierName = ?, SupplierContact = ? WHERE SupplierName = ?", s.SupplierName, s.Phone, oldName)
        For Each d As Drug In Drugs
            If d.SupplierName = oldName Then
                d.SupplierName = s.SupplierName
                d.SupplierContact = s.Phone
            End If
        Next
        MarkChanged()
    End Sub

    Public Sub DeleteSupplier(s As Supplier)
        Database.Execute("DELETE FROM Suppliers WHERE SupplierID = ?", s.ID)
        Dim id As Integer = s.ID
        Suppliers.RemoveAll(Function(x) x.ID = id)
        MarkChanged()
    End Sub

    ' ========================================================================
    '  HISTORY
    ' ========================================================================
    Public Sub AddHistory(logTime As Date, drugName As String, action As String, qtyChange As Integer,
                          party As String, batch As String)
        AddHistoryOn(Nothing, Nothing, logTime, drugName, action, qtyChange, party, batch)
        MarkChanged()
    End Sub

    Private Sub AddHistoryOn(cn As OleDbConnection, tx As OleDbTransaction, logTime As Date, drugName As String, action As String,
                             qtyChange As Integer, party As String, batch As String)
        Dim h As New HistoryEntry()
        h.LogDate = logTime
        h.DrugName = drugName
        h.ActionType = action
        h.QtyChange = qtyChange
        h.Party = If(String.IsNullOrWhiteSpace(party), "-", party)
        h.BatchNo = batch
        h.UserName = If(CurrentUser = "", "System", CurrentUser)
        Dim sql As String = "INSERT INTO History (LogDate, DrugName, ActionType, QtyChange, Party, BatchNo, UserName) VALUES (?, ?, ?, ?, ?, ?, ?)"
        If cn Is Nothing Then
            Database.Execute(sql, h.LogDate, h.DrugName, h.ActionType, h.QtyChange, h.Party, h.BatchNo, h.UserName)
        Else
            Database.ExecuteOn(cn, tx, sql, h.LogDate, h.DrugName, h.ActionType, h.QtyChange, h.Party, h.BatchNo, h.UserName)
        End If
        History.Add(h)
    End Sub

    ' ========================================================================
    '  SALES  (one transaction: all saved, or nothing saved)
    ' ========================================================================

    ''' <summary>
    ''' Saves a sale: the receipt, each line, takes the quantities out of stock
    ''' and writes "Sold" history lines. Returns the new receipt number.
    ''' </summary>
    Public Function RecordSale(cart As List(Of SaleItem), customer As String, amountPaid As Decimal,
                               discount As Decimal, paymentMethod As String) As Sale
        Dim total As Decimal = 0
        For Each item As SaleItem In cart
            total += item.Total
        Next
        total -= discount
        Dim sale As New Sale()
        sale.SaleDate = Date.Now
        sale.Customer = customer
        sale.SoldBy = CurrentUser
        sale.Total = total
        sale.AmountPaid = amountPaid
        sale.Discount = discount
        sale.PaymentMethod = paymentMethod

        Dim newHistory As New List(Of HistoryEntry)
        Using cn As OleDbConnection = Database.OpenConnection()
            Dim tx As OleDbTransaction = cn.BeginTransaction()
            Try
                sale.SaleID = Database.InsertOn(cn, tx, "INSERT INTO Sales (SaleDate, Customer, SoldBy, Total, AmountPaid, Discount, PaymentMethod) VALUES (?, ?, ?, ?, ?, ?, ?)",
                                                sale.SaleDate, sale.Customer, sale.SoldBy, sale.Total, sale.AmountPaid, sale.Discount, sale.PaymentMethod)
                For Each item As SaleItem In cart
                    ' Take it out of stock - only if there is still enough
                    Dim changed As Integer = Database.ExecuteOn(cn, tx,
                        "UPDATE Drugs SET Quantity = Quantity - ? WHERE DrugID = ? AND Quantity >= ?",
                        item.Quantity, item.DrugID, item.Quantity)
                    If changed = 0 Then Throw New InvalidOperationException("Not enough " & item.DrugName & " in stock.")
                    Database.ExecuteOn(cn, tx, "INSERT INTO SaleItems (SaleID, DrugID, DrugName, BatchNo, Quantity, UnitPrice) VALUES (?, ?, ?, ?, ?, ?)",
                                       sale.SaleID, item.DrugID, item.DrugName, item.BatchNo, item.Quantity, item.UnitPrice)
                    Database.ExecuteOn(cn, tx, "INSERT INTO History (LogDate, DrugName, ActionType, QtyChange, Party, BatchNo, UserName) VALUES (?, ?, ?, ?, ?, ?, ?)",
                                       sale.SaleDate, item.DrugName, "Sold", -item.Quantity, customer, item.BatchNo, CurrentUser)
                Next
                tx.Commit()
            Catch
                tx.Rollback()
                Throw
            End Try
        End Using

        ' Saved: now update the lists the screens use
        SaleHeaders.Add(sale)
        For Each item As SaleItem In cart
            item.SaleID = sale.SaleID
            item.SaleDate = sale.SaleDate
            Sales.Add(item)
            Dim d As Drug = FindDrugById(item.DrugID)
            If d IsNot Nothing Then d.Quantity -= item.Quantity
            Dim h As New HistoryEntry()
            h.LogDate = sale.SaleDate
            h.DrugName = item.DrugName
            h.ActionType = "Sold"
            h.QtyChange = -item.Quantity
            h.Party = customer
            h.BatchNo = item.BatchNo
            h.UserName = CurrentUser
            History.Add(h)
        Next
        MarkChanged()
        Return sale
    End Function

    Public Function ItemsOfSale(saleId As Integer) As List(Of SaleItem)
        Dim result As New List(Of SaleItem)
        For Each item As SaleItem In Sales
            If item.SaleID = saleId Then result.Add(item)
        Next
        Return result
    End Function

    Public Function FindSale(saleId As Integer) As Sale
        For Each s As Sale In SaleHeaders
            If s.SaleID = saleId Then Return s
        Next
        Return Nothing
    End Function

    ' ========================================================================
    '  VOID / REFUND A SALE  (one transaction)
    '  The receipt is kept (for the audit trail) but marked VOIDED, the items
    '  go back into stock and a "Returned" history line is written for each.
    ' ========================================================================
    Public Sub VoidSale(saleId As Integer, reason As String)
        Dim stamp As Date = Date.Now
        Using cn As OleDbConnection = Database.OpenConnection()
            Dim tx As OleDbTransaction = cn.BeginTransaction()
            Try
                Dim changed As Integer = Database.ExecuteOn(cn, tx,
                    "UPDATE Sales SET Voided = True, VoidReason = ? WHERE SaleID = ? AND (Voided = False OR Voided IS NULL)",
                    reason, saleId)
                If changed = 0 Then Throw New InvalidOperationException("Receipt #" & saleId & " was already voided.")
                For Each item As SaleItem In ItemsOfSale(saleId)
                    ' Put the stock back (if the drug was deleted since, there is nothing to put back)
                    Database.ExecuteOn(cn, tx, "UPDATE Drugs SET Quantity = Quantity + ? WHERE DrugID = ?", item.Quantity, item.DrugID)
                    Database.ExecuteOn(cn, tx, "INSERT INTO History (LogDate, DrugName, ActionType, QtyChange, Party, BatchNo, UserName) VALUES (?, ?, ?, ?, ?, ?, ?)",
                                       stamp, item.DrugName, "Returned", item.Quantity, "Receipt #" & saleId, item.BatchNo, CurrentUser)
                Next
                tx.Commit()
            Catch
                tx.Rollback()
                Throw
            End Try
        End Using
        MarkChanged()
    End Sub

    ' ========================================================================
    '  WRITE OFF EXPIRED STOCK  (one transaction)
    ' ========================================================================
    Public Sub WriteOff(expired As List(Of Drug))
        Dim stamp As Date = Date.Now
        Using cn As OleDbConnection = Database.OpenConnection()
            Dim tx As OleDbTransaction = cn.BeginTransaction()
            Try
                For Each d As Drug In expired
                    ' Use the quantity in the database right now (another PC may have changed it)
                    Dim cmd As New OleDbCommand("SELECT Quantity FROM Drugs WHERE DrugID = ?", cn, tx)
                    cmd.Parameters.AddWithValue("@p0", d.ID)
                    Dim current As Object = cmd.ExecuteScalar()
                    d.Quantity = If(current Is Nothing OrElse IsDBNull(current), 0, Convert.ToInt32(current))
                    If d.Quantity <= 0 Then Continue For
                    Database.ExecuteOn(cn, tx, "INSERT INTO Losses (LossDate, DrugName, BatchNo, Quantity, LossValue, Reason) VALUES (?, ?, ?, ?, ?, ?)",
                                       stamp, d.DrugName, d.BatchNo, d.Quantity, d.Quantity * d.UnitPrice, "Expired")
                    Database.ExecuteOn(cn, tx, "UPDATE Drugs SET Quantity = 0 WHERE DrugID = ?", d.ID)
                    Database.ExecuteOn(cn, tx, "INSERT INTO History (LogDate, DrugName, ActionType, QtyChange, Party, BatchNo, UserName) VALUES (?, ?, ?, ?, ?, ?, ?)",
                                       stamp, d.DrugName, "Written Off", -d.Quantity, If(d.SupplierName = "", "-", d.SupplierName), d.BatchNo, CurrentUser)
                Next
                tx.Commit()
            Catch
                tx.Rollback()
                Throw
            End Try
        End Using

        For Each d As Drug In expired
            Dim l As New LossRecord()
            l.LossDate = stamp
            l.DrugName = d.DrugName
            l.BatchNo = d.BatchNo
            l.Quantity = d.Quantity
            l.Value = d.Quantity * d.UnitPrice
            l.Reason = "Expired"
            Losses.Add(l)
            Dim h As New HistoryEntry()
            h.LogDate = stamp
            h.DrugName = d.DrugName
            h.ActionType = "Written Off"
            h.QtyChange = -d.Quantity
            h.Party = If(d.SupplierName = "", "-", d.SupplierName)
            h.BatchNo = d.BatchNo
            h.UserName = CurrentUser
            History.Add(h)
            d.Quantity = 0
        Next
        MarkChanged()
    End Sub

    ' ========================================================================
    '  FINDING RECORDS IN THE LISTS
    ' ========================================================================
    Public Function FindDrugById(id As Integer) As Drug
        For Each d As Drug In Drugs
            If d.ID = id Then Return d
        Next
        Return Nothing
    End Function

    Public Function FindDrug(name As String, batch As String) As Drug
        For Each d As Drug In Drugs
            If d.DrugName.ToLower() = name.ToLower() AndAlso d.BatchNo.ToLower() = batch.ToLower() Then Return d
        Next
        Return Nothing
    End Function

    Public Function FindSupplierById(id As Integer) As Supplier
        For Each s As Supplier In Suppliers
            If s.ID = id Then Return s
        Next
        Return Nothing
    End Function

    Public Function FindSupplier(name As String) As Supplier
        For Each s As Supplier In Suppliers
            If s.SupplierName.ToLower() = name.ToLower() Then Return s
        Next
        Return Nothing
    End Function

    ''' <summary>Days from today until the drug expires (negative = already expired).</summary>
    Public Function DaysLeft(d As Drug) As Integer
        Return (d.ExpiryDate.Date - Date.Today).Days
    End Function

    ' ---------------- Reading database values safely ----------------
    Private Function Txt(v As Object) As String
        If v Is Nothing OrElse IsDBNull(v) Then Return ""
        Return v.ToString()
    End Function

    Private Function ToInt(v As Object) As Integer
        If v Is Nothing OrElse IsDBNull(v) Then Return 0
        Return Convert.ToInt32(v)
    End Function

    Private Function ToMoney(v As Object) As Decimal
        If v Is Nothing OrElse IsDBNull(v) Then Return 0D
        Return Convert.ToDecimal(v)
    End Function

    Private Function ToDate(v As Object) As Date
        If v Is Nothing OrElse IsDBNull(v) Then Return Date.Today
        Return Convert.ToDateTime(v)
    End Function

End Module
