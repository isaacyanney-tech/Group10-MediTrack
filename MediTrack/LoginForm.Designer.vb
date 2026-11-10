<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LoginForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.pnlLoginBrand = New System.Windows.Forms.Panel()
        Me.lblLoginLogo = New System.Windows.Forms.Label()
        Me.picLoginLogo = New System.Windows.Forms.PictureBox()
        Me.lblLoginTitle = New System.Windows.Forms.Label()
        Me.lblLoginTag = New System.Windows.Forms.Label()
        Me.lblLoginFeatures = New System.Windows.Forms.Label()
        Me.lblVersion = New System.Windows.Forms.Label()
        Me.lblSignIn = New System.Windows.Forms.Label()
        Me.lblSignInSub = New System.Windows.Forms.Label()
        Me.rbStaff = New System.Windows.Forms.RadioButton()
        Me.rbAdmin = New System.Windows.Forms.RadioButton()
        Me.pnlStaffLogin = New System.Windows.Forms.Panel()
        Me.lblUsername = New System.Windows.Forms.Label()
        Me.txtUsername = New System.Windows.Forms.TextBox()
        Me.lblPassword = New System.Windows.Forms.Label()
        Me.txtPassword = New System.Windows.Forms.TextBox()
        Me.chkShowPassword = New System.Windows.Forms.CheckBox()
        Me.pnlAdminLogin = New System.Windows.Forms.Panel()
        Me.lblAccessCode = New System.Windows.Forms.Label()
        Me.txtAccessCode = New System.Windows.Forms.TextBox()
        Me.chkShowCode = New System.Windows.Forms.CheckBox()
        Me.lblAccessHint = New System.Windows.Forms.Label()
        Me.lblLoginError = New System.Windows.Forms.Label()
        Me.btnLogin = New System.Windows.Forms.Button()
        Me.btnExit = New System.Windows.Forms.Button()
        Me.lblDbLocation = New System.Windows.Forms.Label()
        Me.btnDbSettings = New System.Windows.Forms.Button()
        Me.pnlLoginBrand.SuspendLayout()
        Me.pnlStaffLogin.SuspendLayout()
        Me.pnlAdminLogin.SuspendLayout()
        CType(Me.picLoginLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlLoginBrand
        '
        Me.pnlLoginBrand.Controls.Add(Me.lblLoginLogo)
        Me.pnlLoginBrand.Controls.Add(Me.picLoginLogo)
        Me.pnlLoginBrand.Controls.Add(Me.lblLoginTitle)
        Me.pnlLoginBrand.Controls.Add(Me.lblLoginTag)
        Me.pnlLoginBrand.Controls.Add(Me.lblLoginFeatures)
        Me.pnlLoginBrand.Controls.Add(Me.lblVersion)
        Me.pnlLoginBrand.Location = New System.Drawing.Point(0, 0)
        Me.pnlLoginBrand.Name = "pnlLoginBrand"
        Me.pnlLoginBrand.Size = New System.Drawing.Size(300, 392)
        Me.pnlLoginBrand.TabIndex = 0
        Me.pnlLoginBrand.BackColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(76, Byte), Integer), CType(CType(129, Byte), Integer))
        Me.pnlLoginBrand.Dock = System.Windows.Forms.DockStyle.Left
        '
        'lblLoginLogo
        '
        Me.lblLoginLogo.Location = New System.Drawing.Point(28, 40)
        Me.lblLoginLogo.Name = "lblLoginLogo"
        Me.lblLoginLogo.Text = "+"
        Me.lblLoginLogo.AutoSize = True
        Me.lblLoginLogo.Font = New System.Drawing.Font("Segoe UI", 40!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLoginLogo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(19, Byte), Integer), CType(CType(168, Byte), Integer), CType(CType(158, Byte), Integer))
        '
        'picLoginLogo
        '
        Me.picLoginLogo.Location = New System.Drawing.Point(30, 34)
        Me.picLoginLogo.Name = "picLoginLogo"
        Me.picLoginLogo.Size = New System.Drawing.Size(160, 70)
        Me.picLoginLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picLoginLogo.BackColor = System.Drawing.Color.Transparent
        Me.picLoginLogo.Visible = False
        '
        'lblLoginTitle
        '
        Me.lblLoginTitle.Location = New System.Drawing.Point(30, 110)
        Me.lblLoginTitle.Name = "lblLoginTitle"
        Me.lblLoginTitle.Size = New System.Drawing.Size(250, 56)
        Me.lblLoginTitle.Text = "MediTrack Pharmacy"
        Me.lblLoginTitle.Font = New System.Drawing.Font("Segoe UI", 16!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLoginTitle.ForeColor = System.Drawing.Color.White
        '
        'lblLoginTag
        '
        Me.lblLoginTag.Location = New System.Drawing.Point(32, 170)
        Me.lblLoginTag.Name = "lblLoginTag"
        Me.lblLoginTag.Text = "Inventory and Point of Sale"
        Me.lblLoginTag.AutoSize = True
        Me.lblLoginTag.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLoginTag.ForeColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(227, Byte), Integer), CType(CType(247, Byte), Integer))
        '
        'lblLoginFeatures
        '
        Me.lblLoginFeatures.Location = New System.Drawing.Point(32, 210)
        Me.lblLoginFeatures.Name = "lblLoginFeatures"
        Me.lblLoginFeatures.Size = New System.Drawing.Size(240, 80)
        Me.lblLoginFeatures.Text = "Counter sales and receipts" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Stock and expiry tracking" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Suppliers and reports" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Admin dashboard"
        Me.lblLoginFeatures.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLoginFeatures.ForeColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(227, Byte), Integer), CType(CType(247, Byte), Integer))
        '
        'lblVersion
        '
        Me.lblVersion.Location = New System.Drawing.Point(32, 356)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Text = "Powered by MediTrack"
        Me.lblVersion.AutoSize = True
        Me.lblVersion.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblVersion.ForeColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(227, Byte), Integer), CType(CType(247, Byte), Integer))
        '
        'lblSignIn
        '
        Me.lblSignIn.Location = New System.Drawing.Point(330, 30)
        Me.lblSignIn.Name = "lblSignIn"
        Me.lblSignIn.Text = "Sign in"
        Me.lblSignIn.AutoSize = True
        Me.lblSignIn.Font = New System.Drawing.Font("Segoe UI", 16!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSignIn.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'lblSignInSub
        '
        Me.lblSignInSub.Location = New System.Drawing.Point(333, 64)
        Me.lblSignInSub.Name = "lblSignInSub"
        Me.lblSignInSub.Text = "Choose how you are signing in."
        Me.lblSignInSub.AutoSize = True
        Me.lblSignInSub.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSignInSub.ForeColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        '
        'rbStaff
        '
        Me.rbStaff.Location = New System.Drawing.Point(334, 96)
        Me.rbStaff.Name = "rbStaff"
        Me.rbStaff.TabIndex = 1
        Me.rbStaff.Text = "User Login"
        Me.rbStaff.AutoSize = True
        Me.rbStaff.Checked = True
        Me.rbStaff.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rbStaff.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        Me.rbStaff.TabStop = True
        '
        'rbAdmin
        '
        Me.rbAdmin.Location = New System.Drawing.Point(480, 96)
        Me.rbAdmin.Name = "rbAdmin"
        Me.rbAdmin.TabIndex = 2
        Me.rbAdmin.Text = "Admin Access Code"
        Me.rbAdmin.AutoSize = True
        Me.rbAdmin.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rbAdmin.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'pnlStaffLogin
        '
        Me.pnlStaffLogin.Controls.Add(Me.lblUsername)
        Me.pnlStaffLogin.Controls.Add(Me.txtUsername)
        Me.pnlStaffLogin.Controls.Add(Me.lblPassword)
        Me.pnlStaffLogin.Controls.Add(Me.txtPassword)
        Me.pnlStaffLogin.Controls.Add(Me.chkShowPassword)
        Me.pnlStaffLogin.Location = New System.Drawing.Point(330, 128)
        Me.pnlStaffLogin.Name = "pnlStaffLogin"
        Me.pnlStaffLogin.Size = New System.Drawing.Size(320, 132)
        Me.pnlStaffLogin.TabIndex = 3
        '
        'lblUsername
        '
        Me.lblUsername.Location = New System.Drawing.Point(2, 2)
        Me.lblUsername.Name = "lblUsername"
        Me.lblUsername.Text = "Username"
        Me.lblUsername.AutoSize = True
        Me.lblUsername.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblUsername.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'txtUsername
        '
        Me.txtUsername.Location = New System.Drawing.Point(4, 22)
        Me.txtUsername.Name = "txtUsername"
        Me.txtUsername.Size = New System.Drawing.Size(310, 27)
        Me.txtUsername.TabIndex = 4
        Me.txtUsername.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        '
        'lblPassword
        '
        Me.lblPassword.Location = New System.Drawing.Point(2, 58)
        Me.lblPassword.Name = "lblPassword"
        Me.lblPassword.Text = "Password"
        Me.lblPassword.AutoSize = True
        Me.lblPassword.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPassword.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'txtPassword
        '
        Me.txtPassword.Location = New System.Drawing.Point(4, 78)
        Me.txtPassword.Name = "txtPassword"
        Me.txtPassword.Size = New System.Drawing.Size(310, 27)
        Me.txtPassword.TabIndex = 5
        Me.txtPassword.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtPassword.UseSystemPasswordChar = True
        '
        'chkShowPassword
        '
        Me.chkShowPassword.Location = New System.Drawing.Point(4, 110)
        Me.chkShowPassword.Name = "chkShowPassword"
        Me.chkShowPassword.TabIndex = 6
        Me.chkShowPassword.Text = "Show password"
        Me.chkShowPassword.AutoSize = True
        Me.chkShowPassword.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkShowPassword.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'pnlAdminLogin
        '
        Me.pnlAdminLogin.Controls.Add(Me.lblAccessCode)
        Me.pnlAdminLogin.Controls.Add(Me.txtAccessCode)
        Me.pnlAdminLogin.Controls.Add(Me.chkShowCode)
        Me.pnlAdminLogin.Controls.Add(Me.lblAccessHint)
        Me.pnlAdminLogin.Location = New System.Drawing.Point(330, 128)
        Me.pnlAdminLogin.Name = "pnlAdminLogin"
        Me.pnlAdminLogin.Size = New System.Drawing.Size(320, 132)
        Me.pnlAdminLogin.TabIndex = 7
        Me.pnlAdminLogin.Visible = False
        '
        'lblAccessCode
        '
        Me.lblAccessCode.Location = New System.Drawing.Point(2, 2)
        Me.lblAccessCode.Name = "lblAccessCode"
        Me.lblAccessCode.Text = "Admin access code"
        Me.lblAccessCode.AutoSize = True
        Me.lblAccessCode.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAccessCode.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'txtAccessCode
        '
        Me.txtAccessCode.Location = New System.Drawing.Point(4, 22)
        Me.txtAccessCode.Name = "txtAccessCode"
        Me.txtAccessCode.Size = New System.Drawing.Size(310, 27)
        Me.txtAccessCode.TabIndex = 8
        Me.txtAccessCode.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAccessCode.UseSystemPasswordChar = True
        '
        'chkShowCode
        '
        Me.chkShowCode.Location = New System.Drawing.Point(4, 54)
        Me.chkShowCode.Name = "chkShowCode"
        Me.chkShowCode.TabIndex = 9
        Me.chkShowCode.Text = "Show code"
        Me.chkShowCode.AutoSize = True
        Me.chkShowCode.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkShowCode.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'lblAccessHint
        '
        Me.lblAccessHint.Location = New System.Drawing.Point(2, 82)
        Me.lblAccessHint.Name = "lblAccessHint"
        Me.lblAccessHint.Size = New System.Drawing.Size(310, 36)
        Me.lblAccessHint.Text = "Gives full access: dashboard, reports," & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "suppliers, users and settings."
        Me.lblAccessHint.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAccessHint.ForeColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        '
        'lblLoginError
        '
        Me.lblLoginError.Location = New System.Drawing.Point(332, 262)
        Me.lblLoginError.Name = "lblLoginError"
        Me.lblLoginError.Size = New System.Drawing.Size(320, 20)
        Me.lblLoginError.Text = ""
        Me.lblLoginError.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLoginError.ForeColor = System.Drawing.Color.FromArgb(CType(CType(198, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        '
        'btnLogin
        '
        Me.btnLogin.Location = New System.Drawing.Point(334, 284)
        Me.btnLogin.Name = "btnLogin"
        Me.btnLogin.Size = New System.Drawing.Size(150, 36)
        Me.btnLogin.TabIndex = 10
        Me.btnLogin.Text = "&Sign In"
        Me.btnLogin.BackColor = System.Drawing.Color.FromArgb(CType(CType(21, Byte), Integer), CType(CType(101, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnLogin.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(21, Byte), Integer), CType(CType(101, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnLogin.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnLogin.ForeColor = System.Drawing.Color.White
        Me.btnLogin.UseVisualStyleBackColor = False
        '
        'btnExit
        '
        Me.btnExit.Location = New System.Drawing.Point(494, 284)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(150, 36)
        Me.btnExit.TabIndex = 11
        Me.btnExit.Text = "E&xit"
        Me.btnExit.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(252, Byte), Integer))
        Me.btnExit.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(184, Byte), Integer), CType(CType(194, Byte), Integer), CType(CType(207, Byte), Integer))
        Me.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnExit.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnExit.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        Me.btnExit.UseVisualStyleBackColor = False
        '
        'lblDbLocation
        '
        Me.lblDbLocation.Location = New System.Drawing.Point(332, 344)
        Me.lblDbLocation.Name = "lblDbLocation"
        Me.lblDbLocation.Size = New System.Drawing.Size(200, 34)
        Me.lblDbLocation.Text = "Database: this computer"
        Me.lblDbLocation.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDbLocation.ForeColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        '
        'btnDbSettings
        '
        Me.btnDbSettings.Location = New System.Drawing.Point(540, 342)
        Me.btnDbSettings.Name = "btnDbSettings"
        Me.btnDbSettings.Size = New System.Drawing.Size(104, 30)
        Me.btnDbSettings.TabIndex = 12
        Me.btnDbSettings.Text = "Database..."
        Me.btnDbSettings.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(252, Byte), Integer))
        Me.btnDbSettings.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnDbSettings.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(184, Byte), Integer), CType(CType(194, Byte), Integer), CType(CType(207, Byte), Integer))
        Me.btnDbSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnDbSettings.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDbSettings.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        Me.btnDbSettings.UseVisualStyleBackColor = False
        '
        'LoginForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(680, 392)
        Me.Controls.Add(Me.pnlLoginBrand)
        Me.Controls.Add(Me.lblSignIn)
        Me.Controls.Add(Me.lblSignInSub)
        Me.Controls.Add(Me.rbStaff)
        Me.Controls.Add(Me.rbAdmin)
        Me.Controls.Add(Me.pnlStaffLogin)
        Me.Controls.Add(Me.pnlAdminLogin)
        Me.Controls.Add(Me.lblLoginError)
        Me.Controls.Add(Me.btnLogin)
        Me.Controls.Add(Me.btnExit)
        Me.Controls.Add(Me.lblDbLocation)
        Me.Controls.Add(Me.btnDbSettings)
        Me.Font = New System.Drawing.Font("Segoe UI", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AcceptButton = Me.btnLogin
        Me.BackColor = System.Drawing.Color.White
        Me.CancelButton = Me.btnExit
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "MediTrack - Sign In"
        Me.Name = "LoginForm"
        CType(Me.picLoginLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlAdminLogin.ResumeLayout(False)
        Me.pnlAdminLogin.PerformLayout()
        Me.pnlStaffLogin.ResumeLayout(False)
        Me.pnlStaffLogin.PerformLayout()
        Me.pnlLoginBrand.ResumeLayout(False)
        Me.pnlLoginBrand.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents pnlLoginBrand As System.Windows.Forms.Panel
    Friend WithEvents lblLoginLogo As System.Windows.Forms.Label
    Friend WithEvents picLoginLogo As System.Windows.Forms.PictureBox
    Friend WithEvents lblLoginTitle As System.Windows.Forms.Label
    Friend WithEvents lblLoginTag As System.Windows.Forms.Label
    Friend WithEvents lblLoginFeatures As System.Windows.Forms.Label
    Friend WithEvents lblVersion As System.Windows.Forms.Label
    Friend WithEvents lblSignIn As System.Windows.Forms.Label
    Friend WithEvents lblSignInSub As System.Windows.Forms.Label
    Friend WithEvents rbStaff As System.Windows.Forms.RadioButton
    Friend WithEvents rbAdmin As System.Windows.Forms.RadioButton
    Friend WithEvents pnlStaffLogin As System.Windows.Forms.Panel
    Friend WithEvents lblUsername As System.Windows.Forms.Label
    Friend WithEvents txtUsername As System.Windows.Forms.TextBox
    Friend WithEvents lblPassword As System.Windows.Forms.Label
    Friend WithEvents txtPassword As System.Windows.Forms.TextBox
    Friend WithEvents chkShowPassword As System.Windows.Forms.CheckBox
    Friend WithEvents pnlAdminLogin As System.Windows.Forms.Panel
    Friend WithEvents lblAccessCode As System.Windows.Forms.Label
    Friend WithEvents txtAccessCode As System.Windows.Forms.TextBox
    Friend WithEvents chkShowCode As System.Windows.Forms.CheckBox
    Friend WithEvents lblAccessHint As System.Windows.Forms.Label
    Friend WithEvents lblLoginError As System.Windows.Forms.Label
    Friend WithEvents btnLogin As System.Windows.Forms.Button
    Friend WithEvents btnExit As System.Windows.Forms.Button
    Friend WithEvents lblDbLocation As System.Windows.Forms.Label
    Friend WithEvents btnDbSettings As System.Windows.Forms.Button
End Class
