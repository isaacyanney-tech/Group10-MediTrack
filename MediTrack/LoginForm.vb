' ============================================================================
'  LoginForm.vb  -  the first window of MediTrack
'
'  On opening, it connects to the Access database (creating it the first time).
'  Two ways to sign in:
'    User Login        : username + password from the Users table
'    Admin Access Code : the code stored (hashed) in the Settings table
'  A brand-new database has no accounts, so First-Time Setup opens first
'  and the owner creates the administrator and the access code.
'  A user whose Role is "Admin" gets the admin portal; "Staff" gets the counter.
' ============================================================================
Imports System.Windows.Forms

Public Class LoginForm

    Private failedAttempts As Integer = 0

    Private Sub LoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not ConnectToDatabase() Then
            Me.BeginInvoke(New MethodInvoker(AddressOf Me.Close))
        End If
    End Sub

    ''' <summary>
    ''' Opens the database (on this PC or the shared one). If it fails, offers
    ''' Database Settings so the user can fix the location, then tries again.
    ''' Returns False if the user gives up.
    ''' </summary>
    Private Function ConnectToDatabase() As Boolean
        Do
            Try
                Cursor = Cursors.WaitCursor
                DataStore.Initialise()
                Cursor = Cursors.Default
                If DataStore.NeedsSetup() Then
                    Dim done As Boolean
                    Using setup As New SetupForm()
                        done = (setup.ShowDialog(Me) = DialogResult.OK)
                    End Using
                    If Not done Then
                        ' A counter PC should use the main computer's database instead of a new one
                        If MessageBox.Show("Setup was not finished." & vbCrLf & vbCrLf &
                                           "Is this a counter computer that should use the database on the main computer?" & vbCrLf &
                                           "Click Yes to choose the shared database, or No to close MediTrack.",
                                           "MediTrack", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                            Return False
                        End If
                        Using f As New ConnectDatabaseForm()
                            If f.ShowDialog(Me) <> DialogResult.OK Then Return False
                        End Using
                        Continue Do              ' open the chosen database and check again
                    End If
                End If
                ApplyBranding()
                ShowDatabaseLocation()
                Return True
            Catch ex As Exception
                Cursor = Cursors.Default
                If MessageBox.Show(ex.Message & vbCrLf & vbCrLf & "Open Database Settings to choose where the database is?",
                                   "MediTrack - Database Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error) = DialogResult.No Then
                    Return False
                End If
                Using f As New ConnectDatabaseForm()
                    If f.ShowDialog(Me) <> DialogResult.OK Then Return False
                End Using
            End Try
        Loop
    End Function

    ''' <summary>The business's own name, logo and colour on the sign-in window.</summary>
    Private Sub ApplyBranding()
        lblLoginTitle.Text = Branding.BusinessName()
        Me.Text = Branding.BusinessName() & "  -  Sign In"
        Branding.ShowLogo(picLoginLogo, lblLoginLogo)
        pnlLoginBrand.BackColor = Branding.ThemeColor()
    End Sub

    Private Sub chkShowPassword_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowPassword.CheckedChanged
        txtPassword.UseSystemPasswordChar = Not chkShowPassword.Checked
    End Sub

    Private Sub chkShowCode_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowCode.CheckedChanged
        txtAccessCode.UseSystemPasswordChar = Not chkShowCode.Checked
    End Sub

    Private Sub ShowDatabaseLocation()
        Dim sharedPath As String = Database.GetSavedLocation()
        lblDbLocation.Text = If(sharedPath = "", "Database: this computer", "Database: " & sharedPath)
    End Sub

    ''' <summary>Database... button: choose this computer or a shared database on the network.</summary>
    Private Sub btnDbSettings_Click(sender As Object, e As EventArgs) Handles btnDbSettings.Click
        Using f As New ConnectDatabaseForm()
            If f.ShowDialog(Me) = DialogResult.OK Then
                If ConnectToDatabase() Then
                    MessageBox.Show("Connected to the database." & vbCrLf & Database.DatabasePath, "Database Settings",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If
        End Using
    End Sub

    Private Sub LoginForm_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        txtUsername.Focus()
    End Sub

    Private Sub Role_CheckedChanged(sender As Object, e As EventArgs) Handles rbStaff.CheckedChanged, rbAdmin.CheckedChanged
        pnlStaffLogin.Visible = rbStaff.Checked
        pnlAdminLogin.Visible = rbAdmin.Checked
        lblLoginError.Text = ""
        If rbStaff.Checked Then txtUsername.Focus() Else txtAccessCode.Focus()
    End Sub

    Private Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Try
            If rbAdmin.Checked Then
                If txtAccessCode.Text.Trim() = "" Then
                    ShowError("Please enter the admin access code.")
                    txtAccessCode.Focus()
                ElseIf DataStore.CheckAdminCode(txtAccessCode.Text) Then
                    OpenMainForm("Administrator", "Admin", "")
                Else
                    WrongDetails("Wrong access code. Please try again.")
                    txtAccessCode.SelectAll()
                    txtAccessCode.Focus()
                End If
            Else
                If txtUsername.Text.Trim() = "" OrElse txtPassword.Text = "" Then
                    ShowError("Please enter your username and password.")
                    Return
                End If
                Dim u As AppUser = DataStore.CheckLogin(txtUsername.Text, txtPassword.Text)
                If u Is Nothing Then
                    WrongDetails("Wrong username or password, or the account is disabled.")
                    txtPassword.Clear()
                    txtPassword.Focus()
                Else
                    OpenMainForm(u.FullName, If(u.Role = "Admin", "Admin", "Staff"), u.Username)
                End If
            End If
        Catch ex As Exception
            MessageBox.Show("Could not check the login." & vbCrLf & vbCrLf & ex.Message, "MediTrack",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>After 5 wrong tries the Login button is locked for 30 seconds.</summary>
    Private Sub WrongDetails(message As String)
        failedAttempts += 1
        ShowError(message)
        If failedAttempts >= 5 Then
            btnLogin.Enabled = False
            ShowError("Too many wrong tries. Please wait 30 seconds.")
            Dim t As New System.Windows.Forms.Timer()
            t.Interval = 30000
            AddHandler t.Tick, Sub()
                                   t.Stop()
                                   t.Dispose()
                                   failedAttempts = 0
                                   btnLogin.Enabled = True
                                   lblLoginError.Text = ""
                               End Sub
            t.Start()
        End If
    End Sub

    Private Sub OpenMainForm(personName As String, role As String, username As String)
        failedAttempts = 0
        DataStore.CurrentUser = personName
        DataStore.CurrentUsername = username
        DataStore.CurrentRole = role
        lblLoginError.Text = ""
        txtPassword.Clear()
        txtAccessCode.Clear()

        DataStore.LoadAll()              ' always start with the latest saved data
        DataStore.AutoBackup()           ' daily safety copy of the database
        Try
            DataStore.AddHistory(Date.Now, "-", "Signed In", 0, If(role = "Admin", "Admin", "Staff"), "-")   ' audit trail
        Catch
        End Try
        Me.Hide()
        Dim main As New MainForm()
        main.ShowDialog()

        Dim loggedOut As Boolean = main.LoggedOut
        main.Dispose()
        DataStore.CurrentUser = ""
        DataStore.CurrentUsername = ""
        DataStore.CurrentRole = ""
        If loggedOut Then
            ApplyBranding()                  ' the name, logo or colour may have been changed
            Me.Show()
            If rbStaff.Checked Then txtUsername.Focus() Else txtAccessCode.Focus()
        Else
            Me.Close()
        End If
    End Sub

    Private Sub ShowError(message As String)
        lblLoginError.Text = message
    End Sub

    Private Sub btnExit_Click(sender As Object, e As EventArgs) Handles btnExit.Click
        Me.Close()
    End Sub

End Class
