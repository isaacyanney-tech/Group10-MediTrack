' ============================================================================
'  Database.vb
'  The connection between MediTrack and its Microsoft Access database.
'
'  - Opens MediTrack.accdb (or MediTrack.mdb) in the program's folder.
'  - If no database exists yet, it CREATES an empty one. DataStore.vb then
'    creates the tables (First-Time Setup then adds the first data).
'  - Small helper functions (GetTable, Execute, Scalar, InsertAndGetId) run SQL
'    with ? parameters, so user input can never break or attack the SQL.
' ============================================================================
Option Strict Off   ' Needed because ADOX / DAO (used to create the file) are late-bound COM objects.

Imports System.Collections.Generic
Imports System.Data
Imports System.Data.OleDb
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows.Forms

Public Module Database

    ''' <summary>The OLE DB connection string used by the whole program.</summary>
    Public ConnectionString As String = ""

    ''' <summary>Full path of the Access file in use.</summary>
    Public DatabasePath As String = ""

    ' Access engines, newest first. ACE = Access 2007+ (.accdb). Jet 4.0 = built into 32-bit Windows (.mdb).
    Private ReadOnly AceProviders As String() = {"Microsoft.ACE.OLEDB.16.0", "Microsoft.ACE.OLEDB.12.0"}
    Private Const JetProvider As String = "Microsoft.Jet.OLEDB.4.0"

    ' Every failed attempt is recorded so an error message can say exactly what went wrong.
    Private ReadOnly Attempts As New System.Text.StringBuilder()

    ' ------------------------------------------------------------------------
    '  OPEN OR CREATE
    ' ------------------------------------------------------------------------

    ''' <summary>
    ''' Opens the database. Returns True if a NEW empty database was created
    ''' (so the tables still have to be made).
    ''' </summary>
    ''' <summary>Small text file next to the program that remembers a shared database path.</summary>
    Public ReadOnly Property LocationFile As String
        Get
            Return Path.Combine(Application.StartupPath, "database_location.txt")
        End Get
    End Property

    ''' <summary>The shared database path chosen in Database Settings, or "" for this computer.</summary>
    Public Function GetSavedLocation() As String
        Try
            If File.Exists(LocationFile) Then Return File.ReadAllText(LocationFile).Trim()
        Catch
        End Try
        Return ""
    End Function

    ''' <summary>Saves the shared database path ("" = use the database on this computer).</summary>
    Public Sub SaveLocation(databaseFile As String)
        If databaseFile.Trim() = "" Then
            If File.Exists(LocationFile) Then File.Delete(LocationFile)
        Else
            File.WriteAllText(LocationFile, databaseFile.Trim())
        End If
    End Sub

    ''' <summary>Checks that a database file can be opened. Returns "" if it works, or the problem.</summary>
    Public Function TestLocation(databaseFile As String) As String
        If Not File.Exists(databaseFile) Then Return "File not found. Check the path, the network and that the folder is shared."
        Dim providers As New List(Of String)(AceProviders)
        providers.Add(JetProvider)
        Dim lastError As String = ""
        For Each prov As String In providers
            Try
                Using cn As New OleDbConnection(BuildConnectionString(prov, databaseFile))
                    cn.Open()
                End Using
                Return ""
            Catch ex As Exception
                lastError = ex.Message
            End Try
        Next
        Return "Could not open it: " & lastError
    End Function

    Public Function OpenOrCreate() As Boolean
        Attempts.Clear()
        ' A shared database on another computer (chosen in Database Settings)
        Dim sharedPath As String = GetSavedLocation()
        If sharedPath <> "" Then
            If Not File.Exists(sharedPath) Then
                Throw New IOException("The shared database could not be found:" & vbCrLf & sharedPath & vbCrLf & vbCrLf &
                                      "Check that the main computer is switched on, connected to the network, and that the folder is shared.")
            End If
            If OpenAny(sharedPath) Then Return False
            Throw New IOException("The shared database could not be opened:" & vbCrLf & sharedPath & vbCrLf & vbCrLf & Details())
        End If

        Dim folder As String = Application.StartupPath
        Dim accdbPath As String = Path.Combine(folder, "MediTrack.accdb")
        Dim mdbPath As String = Path.Combine(folder, "MediTrack.mdb")

        ' 1) An existing .accdb
        If File.Exists(accdbPath) Then
            For Each prov As String In AceProviders
                If TryOpen(prov, accdbPath) Then Return False
            Next
        End If

        ' 2) An existing .mdb
        If File.Exists(mdbPath) Then
            If OpenAny(mdbPath) Then Return False
            Throw New Exception("MediTrack.mdb was found but could not be opened." & vbCrLf & vbCrLf & Details())
        End If

        ' 3) Nothing usable yet: create a new file (prefer .accdb, fall back to .mdb)
        For Each prov As String In AceProviders
            If Not File.Exists(accdbPath) AndAlso TryCreateAdox(prov, accdbPath, "Jet OLEDB:Engine Type=6") AndAlso TryOpen(prov, accdbPath) Then
                Return True
            End If
        Next
        If CreateMdb(mdbPath) Then Return True

        Throw New Exception("The Access database could not be opened or created." & vbCrLf & vbCrLf & Details())
    End Function

    Private Function CreateMdb(mdbPath As String) As Boolean
        If TryCreateAdox(JetProvider, mdbPath, "Jet OLEDB:Engine Type=5") AndAlso OpenAny(mdbPath) Then Return True
        If TryCreateDao(mdbPath) AndAlso OpenAny(mdbPath) Then Return True
        If TryCreateOdbc(mdbPath) AndAlso OpenAny(mdbPath) Then Return True
        Return False
    End Function

    Private Function OpenAny(filePath As String) As Boolean
        If TryOpen(JetProvider, filePath) Then Return True
        For Each prov As String In AceProviders
            If TryOpen(prov, filePath) Then Return True
        Next
        Return False
    End Function

    Private Function Details() As String
        Dim bits As String = If(Environment.Is64BitProcess, "64-bit", "32-bit")
        Return "The program is running as " & bits & "." & vbCrLf &
               "Fix: in Visual Studio open Project > MediTrack Properties > Compile and set 'Target CPU' to x86, " &
               "or install the free 'Microsoft Access Database Engine 2016 Redistributable'." & vbCrLf & vbCrLf &
               "Technical details:" & vbCrLf & Attempts.ToString()
    End Function

    Private Sub Note(what As String, ex As Exception)
        Attempts.AppendLine("- " & what & ": " & ex.Message)
    End Sub

    Private Function BuildConnectionString(provider As String, filePath As String) As String
        Return "Provider=" & provider & ";Data Source=" & filePath & ";Persist Security Info=False;"
    End Function

    Private Function TryOpen(provider As String, filePath As String) As Boolean
        Dim cs As String = BuildConnectionString(provider, filePath)
        Try
            Using cn As New OleDbConnection(cs)
                cn.Open()
            End Using
            ConnectionString = cs
            DatabasePath = filePath
            Return True
        Catch ex As Exception
            Note("Open " & Path.GetFileName(filePath) & " with " & provider, ex)
            Return False
        End Try
    End Function

    Private Sub DeleteQuietly(filePath As String)
        Try
            If File.Exists(filePath) Then File.Delete(filePath)
        Catch
        End Try
    End Sub

    ' Method 1: ADOX
    Private Function TryCreateAdox(provider As String, filePath As String, extra As String) As Boolean
        Dim catalog As Object = Nothing
        Try
            catalog = CreateObject("ADOX.Catalog")
            catalog.Create(BuildConnectionString(provider, filePath) & extra)
            Try
                catalog.ActiveConnection.Close()
            Catch
            End Try
            Return True
        Catch ex As Exception
            Note("ADOX create with " & provider, ex)
            DeleteQuietly(filePath)
            Return False
        Finally
            If catalog IsNot Nothing Then
                Try
                    Marshal.ReleaseComObject(catalog)
                Catch
                End Try
            End If
        End Try
    End Function

    ' Method 2: DAO
    Private Function TryCreateDao(filePath As String) As Boolean
        For Each progId As String In {"DAO.DBEngine.36", "DAO.DBEngine.120"}
            Dim engine As Object = Nothing
            Try
                engine = CreateObject(progId)
                Dim db As Object = engine.CreateDatabase(filePath, ";LANGID=0x0409;CP=1252;COUNTRY=0", 32)
                db.Close()
                Return True
            Catch ex As Exception
                Note("DAO create with " & progId, ex)
                DeleteQuietly(filePath)
            Finally
                If engine IsNot Nothing Then
                    Try
                        Marshal.ReleaseComObject(engine)
                    Catch
                    End Try
                End If
            End Try
        Next
        Return False
    End Function

    <DllImport("odbccp32.dll", CharSet:=CharSet.Unicode, EntryPoint:="SQLConfigDataSourceW")>
    Private Function SQLConfigDataSource(hwndParent As IntPtr, fRequest As Integer, lpszDriver As String, lpszAttributes As String) As Boolean
    End Function

    ' Method 3: ODBC driver
    Private Function TryCreateOdbc(filePath As String) As Boolean
        For Each driver As String In {"Microsoft Access Driver (*.mdb)", "Microsoft Access Driver (*.mdb, *.accdb)"}
            Try
                Dim attrs As String = "CREATE_DB=""" & filePath & """ General" & ChrW(0)
                If SQLConfigDataSource(IntPtr.Zero, 1, driver, attrs) AndAlso File.Exists(filePath) Then Return True
                Attempts.AppendLine("- ODBC create with " & driver & ": driver not available")
            Catch ex As Exception
                Note("ODBC create with " & driver, ex)
            End Try
        Next
        Return False
    End Function

    ' ------------------------------------------------------------------------
    '  RUNNING SQL  (use ? placeholders; values are passed in the same order)
    ' ------------------------------------------------------------------------

    ''' <summary>Opens a new connection (used for transactions).</summary>
    Public Function OpenConnection() As OleDbConnection
        Dim cn As New OleDbConnection(ConnectionString)
        cn.Open()
        Return cn
    End Function

    ''' <summary>Runs a SELECT and returns the rows as a DataTable.</summary>
    Public Function GetTable(sql As String, ParamArray values() As Object) As DataTable
        Dim table As New DataTable()
        Using cn As New OleDbConnection(ConnectionString)
            Using cmd As New OleDbCommand(sql, cn)
                AddParameters(cmd, values)
                Using da As New OleDbDataAdapter(cmd)
                    da.Fill(table)
                End Using
            End Using
        End Using
        Return table
    End Function

    ''' <summary>Runs INSERT / UPDATE / DELETE / CREATE. Returns how many rows changed.</summary>
    Public Function Execute(sql As String, ParamArray values() As Object) As Integer
        Using cn As OleDbConnection = OpenConnection()
            Return ExecuteOn(cn, Nothing, sql, values)
        End Using
    End Function

    ''' <summary>Runs a command on an open connection (and transaction, if given).</summary>
    Public Function ExecuteOn(cn As OleDbConnection, tx As OleDbTransaction, sql As String, ParamArray values() As Object) As Integer
        Using cmd As New OleDbCommand(sql, cn, tx)
            AddParameters(cmd, values)
            Return cmd.ExecuteNonQuery()
        End Using
    End Function

    ''' <summary>Runs an INSERT and returns the new AutoNumber ID.</summary>
    Public Function InsertAndGetId(sql As String, ParamArray values() As Object) As Integer
        Using cn As OleDbConnection = OpenConnection()
            Return InsertOn(cn, Nothing, sql, values)
        End Using
    End Function

    ' The AutoNumber column of each table that InsertOn is used with.
    Private ReadOnly IdColumns As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
        {"Users", "UserID"}, {"Suppliers", "SupplierID"}, {"Drugs", "DrugID"}, {"Sales", "SaleID"},
        {"SaleItems", "ItemID"}, {"Losses", "LossID"}, {"History", "HistoryID"}}

    ''' <summary>
    ''' Runs an INSERT and returns the new AutoNumber ID. First asks Access for
    ''' @@IDENTITY; some Access engines answer 0, so in that case it reads the
    ''' highest ID in the table instead (on the same connection/transaction).
    ''' </summary>
    Public Function InsertOn(cn As OleDbConnection, tx As OleDbTransaction, sql As String, ParamArray values() As Object) As Integer
        ExecuteOn(cn, tx, sql, values)

        Dim id As Integer = 0
        Using cmd As New OleDbCommand("SELECT @@IDENTITY", cn, tx)
            Dim v As Object = cmd.ExecuteScalar()
            If v IsNot Nothing AndAlso Not IsDBNull(v) Then id = Convert.ToInt32(v)
        End Using
        If id > 0 Then Return id

        ' Fallback: highest ID in the table we just inserted into
        Dim m As System.Text.RegularExpressions.Match =
            System.Text.RegularExpressions.Regex.Match(sql, "INSERT\s+INTO\s+\[?(\w+)\]?", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
        Dim table As String = m.Groups(1).Value
        If Not IdColumns.ContainsKey(table) Then Throw New InvalidOperationException("Could not read the new ID for table " & table & ".")
        Using cmd As New OleDbCommand("SELECT MAX(" & IdColumns(table) & ") FROM [" & table & "]", cn, tx)
            Dim v As Object = cmd.ExecuteScalar()
            If v IsNot Nothing AndAlso Not IsDBNull(v) Then id = Convert.ToInt32(v)
        End Using
        If id <= 0 Then Throw New InvalidOperationException("Could not read the new ID for table " & table & ".")
        Return id
    End Function

    ''' <summary>Runs a query that returns one value (e.g. COUNT(*)).</summary>
    Public Function Scalar(sql As String, ParamArray values() As Object) As Object
        Using cn As OleDbConnection = OpenConnection()
            Using cmd As New OleDbCommand(sql, cn)
                AddParameters(cmd, values)
                Return cmd.ExecuteScalar()
            End Using
        End Using
    End Function

    ''' <summary>Copies the database file (for backups).</summary>
    Public Sub BackupTo(destination As String)
        OleDbConnection.ReleaseObjectPool()
        File.Copy(DatabasePath, destination, True)
    End Sub

    Private Sub AddParameters(cmd As OleDbCommand, values() As Object)
        If values Is Nothing Then Return
        Dim i As Integer = 0
        For Each v As Object In values
            i += 1
            Dim p As OleDbParameter
            If v Is Nothing OrElse v Is DBNull.Value Then
                p = cmd.Parameters.Add("p" & i.ToString(), OleDbType.VarWChar)
                p.Value = DBNull.Value
            ElseIf TypeOf v Is Date Then
                Dim d As Date = CDate(v)
                p = cmd.Parameters.Add("p" & i.ToString(), OleDbType.Date)
                p.Value = New Date(d.Year, d.Month, d.Day, d.Hour, d.Minute, d.Second)   ' Access cannot store milliseconds
            ElseIf TypeOf v Is Decimal Then
                p = cmd.Parameters.Add("p" & i.ToString(), OleDbType.Currency)
                p.Value = v
            ElseIf TypeOf v Is Integer Then
                p = cmd.Parameters.Add("p" & i.ToString(), OleDbType.Integer)
                p.Value = v
            ElseIf TypeOf v Is Boolean Then
                p = cmd.Parameters.Add("p" & i.ToString(), OleDbType.Boolean)
                p.Value = v
            Else
                Dim s As String = v.ToString()
                p = cmd.Parameters.Add("p" & i.ToString(), If(s.Length > 255, OleDbType.LongVarWChar, OleDbType.VarWChar))
                p.Value = s
            End If
        Next
    End Sub

End Module
