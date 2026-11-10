<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SetupForm
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
        Me.pnlSetupHeader = New System.Windows.Forms.Panel()
        Me.lblSetupTitle = New System.Windows.Forms.Label()
        Me.lblSetupSub = New System.Windows.Forms.Label()
        Me.grpSetupPharmacy = New System.Windows.Forms.GroupBox()
        Me.lblSetupName = New System.Windows.Forms.Label()
        Me.txtSetupName = New System.Windows.Forms.TextBox()
        Me.lblSetupAddress = New System.Windows.Forms.Label()
        Me.txtSetupAddress = New System.Windows.Forms.TextBox()
        Me.lblSetupPhone = New System.Windows.Forms.Label()
        Me.txtSetupPhone = New System.Windows.Forms.TextBox()
        Me.grpSetupAdmin = New System.Windows.Forms.GroupBox()
        Me.lblSetupFullName = New System.Windows.Forms.Label()
        Me.txtSetupFullName = New System.Windows.Forms.TextBox()
        Me.lblSetupUser = New System.Windows.Forms.Label()
        Me.txtSetupUser = New System.Windows.Forms.TextBox()
        Me.lblSetupPass = New System.Windows.Forms.Label()
        Me.txtSetupPass = New System.Windows.Forms.TextBox()
        Me.lblSetupPass2 = New System.Windows.Forms.Label()
        Me.txtSetupPass2 = New System.Windows.Forms.TextBox()
        Me.grpSetupCode = New System.Windows.Forms.GroupBox()
        Me.lblSetupCode = New System.Windows.Forms.Label()
        Me.txtSetupCode = New System.Windows.Forms.TextBox()
        Me.lblSetupCode2 = New System.Windows.Forms.Label()
        Me.txtSetupCode2 = New System.Windows.Forms.TextBox()
        Me.chkSetupShow = New System.Windows.Forms.CheckBox()
        Me.grpSetupBrand = New System.Windows.Forms.GroupBox()
        Me.picSetupLogo = New System.Windows.Forms.PictureBox()
        Me.btnSetupLogo = New System.Windows.Forms.Button()
        Me.pnlSetupSwatch = New System.Windows.Forms.Panel()
        Me.btnSetupColour = New System.Windows.Forms.Button()
        Me.lblSetupBrandHint = New System.Windows.Forms.Label()
        Me.chkSampleData = New System.Windows.Forms.CheckBox()
        Me.lblSetupError = New System.Windows.Forms.Label()
        Me.btnSetupFinish = New System.Windows.Forms.Button()
        Me.btnSetupCancel = New System.Windows.Forms.Button()
        Me.pnlSetupHeader.SuspendLayout()
        Me.grpSetupPharmacy.SuspendLayout()
        Me.grpSetupAdmin.SuspendLayout()
        Me.grpSetupCode.SuspendLayout()
        Me.grpSetupBrand.SuspendLayout()
        Me.pnlSetupSwatch.SuspendLayout()
        CType(Me.picSetupLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pnlSetupHeader
        '
        Me.pnlSetupHeader.Controls.Add(Me.lblSetupTitle)
        Me.pnlSetupHeader.Controls.Add(Me.lblSetupSub)
        Me.pnlSetupHeader.Location = New System.Drawing.Point(0, 0)
        Me.pnlSetupHeader.Name = "pnlSetupHeader"
        Me.pnlSetupHeader.Size = New System.Drawing.Size(560, 76)
        Me.pnlSetupHeader.TabIndex = 0
        Me.pnlSetupHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(76, Byte), Integer), CType(CType(129, Byte), Integer))
        Me.pnlSetupHeader.Dock = System.Windows.Forms.DockStyle.Top
        '
        'lblSetupTitle
        '
        Me.lblSetupTitle.Location = New System.Drawing.Point(20, 12)
        Me.lblSetupTitle.Name = "lblSetupTitle"
        Me.lblSetupTitle.Text = "Welcome to MediTrack"
        Me.lblSetupTitle.AutoSize = True
        Me.lblSetupTitle.Font = New System.Drawing.Font("Segoe UI", 15!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSetupTitle.ForeColor = System.Drawing.Color.White
        '
        'lblSetupSub
        '
        Me.lblSetupSub.Location = New System.Drawing.Point(22, 46)
        Me.lblSetupSub.Name = "lblSetupSub"
        Me.lblSetupSub.Text = "Set up your pharmacy. This is done once, on the main computer."
        Me.lblSetupSub.AutoSize = True
        Me.lblSetupSub.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSetupSub.ForeColor = System.Drawing.Color.FromArgb(CType(CType(207, Byte), Integer), CType(CType(227, Byte), Integer), CType(CType(247, Byte), Integer))
        '
        'grpSetupPharmacy
        '
        Me.grpSetupPharmacy.Controls.Add(Me.lblSetupName)
        Me.grpSetupPharmacy.Controls.Add(Me.txtSetupName)
        Me.grpSetupPharmacy.Controls.Add(Me.lblSetupAddress)
        Me.grpSetupPharmacy.Controls.Add(Me.txtSetupAddress)
        Me.grpSetupPharmacy.Controls.Add(Me.lblSetupPhone)
        Me.grpSetupPharmacy.Controls.Add(Me.txtSetupPhone)
        Me.grpSetupPharmacy.Location = New System.Drawing.Point(16, 90)
        Me.grpSetupPharmacy.Name = "grpSetupPharmacy"
        Me.grpSetupPharmacy.Size = New System.Drawing.Size(528, 128)
        Me.grpSetupPharmacy.TabIndex = 1
        Me.grpSetupPharmacy.Text = "1.  Pharmacy details (printed on receipts)"
        Me.grpSetupPharmacy.BackColor = System.Drawing.Color.White
        Me.grpSetupPharmacy.Font = New System.Drawing.Font("Segoe UI", 10.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grpSetupPharmacy.ForeColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(76, Byte), Integer), CType(CType(129, Byte), Integer))
        Me.grpSetupPharmacy.Padding = New System.Windows.Forms.Padding(8, 6, 8, 8)
        '
        'lblSetupName
        '
        Me.lblSetupName.Location = New System.Drawing.Point(14, 29)
        Me.lblSetupName.Name = "lblSetupName"
        Me.lblSetupName.Text = "Pharmacy name:"
        Me.lblSetupName.AutoSize = True
        Me.lblSetupName.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSetupName.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'txtSetupName
        '
        Me.txtSetupName.Location = New System.Drawing.Point(160, 26)
        Me.txtSetupName.Name = "txtSetupName"
        Me.txtSetupName.Size = New System.Drawing.Size(350, 25)
        Me.txtSetupName.TabIndex = 2
        Me.txtSetupName.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        '
        'lblSetupAddress
        '
        Me.lblSetupAddress.Location = New System.Drawing.Point(14, 61)
        Me.lblSetupAddress.Name = "lblSetupAddress"
        Me.lblSetupAddress.Text = "Address:"
        Me.lblSetupAddress.AutoSize = True
        Me.lblSetupAddress.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSetupAddress.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'txtSetupAddress
        '
        Me.txtSetupAddress.Location = New System.Drawing.Point(160, 58)
        Me.txtSetupAddress.Name = "txtSetupAddress"
        Me.txtSetupAddress.Size = New System.Drawing.Size(350, 25)
        Me.txtSetupAddress.TabIndex = 3
        Me.txtSetupAddress.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        '
        'lblSetupPhone
        '
        Me.lblSetupPhone.Location = New System.Drawing.Point(14, 93)
        Me.lblSetupPhone.Name = "lblSetupPhone"
        Me.lblSetupPhone.Text = "Phone number(s):"
        Me.lblSetupPhone.AutoSize = True
        Me.lblSetupPhone.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSetupPhone.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'txtSetupPhone
        '
        Me.txtSetupPhone.Location = New System.Drawing.Point(160, 90)
        Me.txtSetupPhone.Name = "txtSetupPhone"
        Me.txtSetupPhone.Size = New System.Drawing.Size(350, 25)
        Me.txtSetupPhone.TabIndex = 4
        Me.txtSetupPhone.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        '
        'grpSetupAdmin
        '
        Me.grpSetupAdmin.Controls.Add(Me.lblSetupFullName)
        Me.grpSetupAdmin.Controls.Add(Me.txtSetupFullName)
        Me.grpSetupAdmin.Controls.Add(Me.lblSetupUser)
        Me.grpSetupAdmin.Controls.Add(Me.txtSetupUser)
        Me.grpSetupAdmin.Controls.Add(Me.lblSetupPass)
        Me.grpSetupAdmin.Controls.Add(Me.txtSetupPass)
        Me.grpSetupAdmin.Controls.Add(Me.lblSetupPass2)
        Me.grpSetupAdmin.Controls.Add(Me.txtSetupPass2)
        Me.grpSetupAdmin.Location = New System.Drawing.Point(16, 228)
        Me.grpSetupAdmin.Name = "grpSetupAdmin"
        Me.grpSetupAdmin.Size = New System.Drawing.Size(528, 160)
        Me.grpSetupAdmin.TabIndex = 5
        Me.grpSetupAdmin.Text = "2.  Administrator account (the owner / manager)"
        Me.grpSetupAdmin.BackColor = System.Drawing.Color.White
        Me.grpSetupAdmin.Font = New System.Drawing.Font("Segoe UI", 10.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grpSetupAdmin.ForeColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(76, Byte), Integer), CType(CType(129, Byte), Integer))
        Me.grpSetupAdmin.Padding = New System.Windows.Forms.Padding(8, 6, 8, 8)
        '
        'lblSetupFullName
        '
        Me.lblSetupFullName.Location = New System.Drawing.Point(14, 29)
        Me.lblSetupFullName.Name = "lblSetupFullName"
        Me.lblSetupFullName.Text = "Full name:"
        Me.lblSetupFullName.AutoSize = True
        Me.lblSetupFullName.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSetupFullName.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'txtSetupFullName
        '
        Me.txtSetupFullName.Location = New System.Drawing.Point(160, 26)
        Me.txtSetupFullName.Name = "txtSetupFullName"
        Me.txtSetupFullName.Size = New System.Drawing.Size(350, 25)
        Me.txtSetupFullName.TabIndex = 6
        Me.txtSetupFullName.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        '
        'lblSetupUser
        '
        Me.lblSetupUser.Location = New System.Drawing.Point(14, 61)
        Me.lblSetupUser.Name = "lblSetupUser"
        Me.lblSetupUser.Text = "Username:"
        Me.lblSetupUser.AutoSize = True
        Me.lblSetupUser.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSetupUser.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'txtSetupUser
        '
        Me.txtSetupUser.Location = New System.Drawing.Point(160, 58)
        Me.txtSetupUser.Name = "txtSetupUser"
        Me.txtSetupUser.Size = New System.Drawing.Size(350, 25)
        Me.txtSetupUser.TabIndex = 7
        Me.txtSetupUser.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        '
        'lblSetupPass
        '
        Me.lblSetupPass.Location = New System.Drawing.Point(14, 93)
        Me.lblSetupPass.Name = "lblSetupPass"
        Me.lblSetupPass.Text = "Password:"
        Me.lblSetupPass.AutoSize = True
        Me.lblSetupPass.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSetupPass.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'txtSetupPass
        '
        Me.txtSetupPass.Location = New System.Drawing.Point(160, 90)
        Me.txtSetupPass.Name = "txtSetupPass"
        Me.txtSetupPass.Size = New System.Drawing.Size(350, 25)
        Me.txtSetupPass.TabIndex = 8
        Me.txtSetupPass.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSetupPass.UseSystemPasswordChar = True
        '
        'lblSetupPass2
        '
        Me.lblSetupPass2.Location = New System.Drawing.Point(14, 125)
        Me.lblSetupPass2.Name = "lblSetupPass2"
        Me.lblSetupPass2.Text = "Confirm password:"
        Me.lblSetupPass2.AutoSize = True
        Me.lblSetupPass2.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSetupPass2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'txtSetupPass2
        '
        Me.txtSetupPass2.Location = New System.Drawing.Point(160, 122)
        Me.txtSetupPass2.Name = "txtSetupPass2"
        Me.txtSetupPass2.Size = New System.Drawing.Size(350, 25)
        Me.txtSetupPass2.TabIndex = 9
        Me.txtSetupPass2.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSetupPass2.UseSystemPasswordChar = True
        '
        'grpSetupCode
        '
        Me.grpSetupCode.Controls.Add(Me.lblSetupCode)
        Me.grpSetupCode.Controls.Add(Me.txtSetupCode)
        Me.grpSetupCode.Controls.Add(Me.lblSetupCode2)
        Me.grpSetupCode.Controls.Add(Me.txtSetupCode2)
        Me.grpSetupCode.Controls.Add(Me.chkSetupShow)
        Me.grpSetupCode.Location = New System.Drawing.Point(16, 398)
        Me.grpSetupCode.Name = "grpSetupCode"
        Me.grpSetupCode.Size = New System.Drawing.Size(528, 118)
        Me.grpSetupCode.TabIndex = 10
        Me.grpSetupCode.Text = "3.  Admin access code (quick admin sign-in)"
        Me.grpSetupCode.BackColor = System.Drawing.Color.White
        Me.grpSetupCode.Font = New System.Drawing.Font("Segoe UI", 10.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grpSetupCode.ForeColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(76, Byte), Integer), CType(CType(129, Byte), Integer))
        Me.grpSetupCode.Padding = New System.Windows.Forms.Padding(8, 6, 8, 8)
        '
        'lblSetupCode
        '
        Me.lblSetupCode.Location = New System.Drawing.Point(14, 29)
        Me.lblSetupCode.Name = "lblSetupCode"
        Me.lblSetupCode.Text = "Access code:"
        Me.lblSetupCode.AutoSize = True
        Me.lblSetupCode.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSetupCode.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'txtSetupCode
        '
        Me.txtSetupCode.Location = New System.Drawing.Point(160, 26)
        Me.txtSetupCode.Name = "txtSetupCode"
        Me.txtSetupCode.Size = New System.Drawing.Size(350, 25)
        Me.txtSetupCode.TabIndex = 11
        Me.txtSetupCode.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSetupCode.UseSystemPasswordChar = True
        '
        'lblSetupCode2
        '
        Me.lblSetupCode2.Location = New System.Drawing.Point(14, 61)
        Me.lblSetupCode2.Name = "lblSetupCode2"
        Me.lblSetupCode2.Text = "Confirm code:"
        Me.lblSetupCode2.AutoSize = True
        Me.lblSetupCode2.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSetupCode2.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'txtSetupCode2
        '
        Me.txtSetupCode2.Location = New System.Drawing.Point(160, 58)
        Me.txtSetupCode2.Name = "txtSetupCode2"
        Me.txtSetupCode2.Size = New System.Drawing.Size(350, 25)
        Me.txtSetupCode2.TabIndex = 12
        Me.txtSetupCode2.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSetupCode2.UseSystemPasswordChar = True
        '
        'chkSetupShow
        '
        Me.chkSetupShow.Location = New System.Drawing.Point(160, 90)
        Me.chkSetupShow.Name = "chkSetupShow"
        Me.chkSetupShow.TabIndex = 13
        Me.chkSetupShow.Text = "Show password and code"
        Me.chkSetupShow.AutoSize = True
        Me.chkSetupShow.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkSetupShow.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'grpSetupBrand
        '
        Me.grpSetupBrand.Controls.Add(Me.picSetupLogo)
        Me.grpSetupBrand.Controls.Add(Me.btnSetupLogo)
        Me.grpSetupBrand.Controls.Add(Me.pnlSetupSwatch)
        Me.grpSetupBrand.Controls.Add(Me.btnSetupColour)
        Me.grpSetupBrand.Controls.Add(Me.lblSetupBrandHint)
        Me.grpSetupBrand.Location = New System.Drawing.Point(16, 526)
        Me.grpSetupBrand.Name = "grpSetupBrand"
        Me.grpSetupBrand.Size = New System.Drawing.Size(528, 82)
        Me.grpSetupBrand.TabIndex = 14
        Me.grpSetupBrand.Text = "4.  Logo and colour (optional - can be changed later)"
        Me.grpSetupBrand.BackColor = System.Drawing.Color.White
        Me.grpSetupBrand.Font = New System.Drawing.Font("Segoe UI", 10.5!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grpSetupBrand.ForeColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(76, Byte), Integer), CType(CType(129, Byte), Integer))
        Me.grpSetupBrand.Padding = New System.Windows.Forms.Padding(8, 6, 8, 8)
        '
        'picSetupLogo
        '
        Me.picSetupLogo.Location = New System.Drawing.Point(14, 24)
        Me.picSetupLogo.Name = "picSetupLogo"
        Me.picSetupLogo.Size = New System.Drawing.Size(100, 48)
        Me.picSetupLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.picSetupLogo.BackColor = System.Drawing.Color.Transparent
        Me.picSetupLogo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picSetupLogo.BackColor = System.Drawing.Color.White
        '
        'btnSetupLogo
        '
        Me.btnSetupLogo.Location = New System.Drawing.Point(124, 24)
        Me.btnSetupLogo.Name = "btnSetupLogo"
        Me.btnSetupLogo.Size = New System.Drawing.Size(120, 28)
        Me.btnSetupLogo.TabIndex = 15
        Me.btnSetupLogo.Text = "Choose Logo..."
        Me.btnSetupLogo.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(252, Byte), Integer))
        Me.btnSetupLogo.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnSetupLogo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(184, Byte), Integer), CType(CType(194, Byte), Integer), CType(CType(207, Byte), Integer))
        Me.btnSetupLogo.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSetupLogo.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSetupLogo.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        Me.btnSetupLogo.UseVisualStyleBackColor = False
        '
        'pnlSetupSwatch
        '
        Me.pnlSetupSwatch.Location = New System.Drawing.Point(270, 24)
        Me.pnlSetupSwatch.Name = "pnlSetupSwatch"
        Me.pnlSetupSwatch.Size = New System.Drawing.Size(40, 28)
        Me.pnlSetupSwatch.TabIndex = 16
        Me.pnlSetupSwatch.BackColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(76, Byte), Integer), CType(CType(129, Byte), Integer))
        Me.pnlSetupSwatch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        '
        'btnSetupColour
        '
        Me.btnSetupColour.Location = New System.Drawing.Point(318, 24)
        Me.btnSetupColour.Name = "btnSetupColour"
        Me.btnSetupColour.Size = New System.Drawing.Size(130, 28)
        Me.btnSetupColour.TabIndex = 17
        Me.btnSetupColour.Text = "Choose Colour..."
        Me.btnSetupColour.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(252, Byte), Integer))
        Me.btnSetupColour.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnSetupColour.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(184, Byte), Integer), CType(CType(194, Byte), Integer), CType(CType(207, Byte), Integer))
        Me.btnSetupColour.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSetupColour.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSetupColour.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        Me.btnSetupColour.UseVisualStyleBackColor = False
        '
        'lblSetupBrandHint
        '
        Me.lblSetupBrandHint.Location = New System.Drawing.Point(124, 56)
        Me.lblSetupBrandHint.Name = "lblSetupBrandHint"
        Me.lblSetupBrandHint.Size = New System.Drawing.Size(400, 18)
        Me.lblSetupBrandHint.Text = "PNG or JPG logo.  The colour is used for the header, tables and login."
        Me.lblSetupBrandHint.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSetupBrandHint.ForeColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        '
        'chkSampleData
        '
        Me.chkSampleData.Location = New System.Drawing.Point(20, 616)
        Me.chkSampleData.Name = "chkSampleData"
        Me.chkSampleData.TabIndex = 18
        Me.chkSampleData.Text = "Add sample drugs, suppliers and sales (for training or a demo)"
        Me.chkSampleData.AutoSize = True
        Me.chkSampleData.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkSampleData.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'lblSetupError
        '
        Me.lblSetupError.Location = New System.Drawing.Point(18, 642)
        Me.lblSetupError.Name = "lblSetupError"
        Me.lblSetupError.Size = New System.Drawing.Size(524, 20)
        Me.lblSetupError.Text = ""
        Me.lblSetupError.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSetupError.ForeColor = System.Drawing.Color.FromArgb(CType(CType(198, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        '
        'btnSetupFinish
        '
        Me.btnSetupFinish.Location = New System.Drawing.Point(294, 668)
        Me.btnSetupFinish.Name = "btnSetupFinish"
        Me.btnSetupFinish.Size = New System.Drawing.Size(130, 36)
        Me.btnSetupFinish.TabIndex = 19
        Me.btnSetupFinish.Text = "&Finish Setup"
        Me.btnSetupFinish.BackColor = System.Drawing.Color.FromArgb(CType(CType(46, Byte), Integer), CType(CType(125, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnSetupFinish.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnSetupFinish.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(46, Byte), Integer), CType(CType(125, Byte), Integer), CType(CType(50, Byte), Integer))
        Me.btnSetupFinish.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSetupFinish.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSetupFinish.ForeColor = System.Drawing.Color.White
        Me.btnSetupFinish.UseVisualStyleBackColor = False
        '
        'btnSetupCancel
        '
        Me.btnSetupCancel.Location = New System.Drawing.Point(432, 668)
        Me.btnSetupCancel.Name = "btnSetupCancel"
        Me.btnSetupCancel.Size = New System.Drawing.Size(112, 36)
        Me.btnSetupCancel.TabIndex = 20
        Me.btnSetupCancel.Text = "Cancel"
        Me.btnSetupCancel.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(252, Byte), Integer))
        Me.btnSetupCancel.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnSetupCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(184, Byte), Integer), CType(CType(194, Byte), Integer), CType(CType(207, Byte), Integer))
        Me.btnSetupCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnSetupCancel.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnSetupCancel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        Me.btnSetupCancel.UseVisualStyleBackColor = False
        '
        'SetupForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(560, 720)
        Me.Controls.Add(Me.pnlSetupHeader)
        Me.Controls.Add(Me.grpSetupPharmacy)
        Me.Controls.Add(Me.grpSetupAdmin)
        Me.Controls.Add(Me.grpSetupCode)
        Me.Controls.Add(Me.grpSetupBrand)
        Me.Controls.Add(Me.chkSampleData)
        Me.Controls.Add(Me.lblSetupError)
        Me.Controls.Add(Me.btnSetupFinish)
        Me.Controls.Add(Me.btnSetupCancel)
        Me.Font = New System.Drawing.Font("Segoe UI", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AcceptButton = Me.btnSetupFinish
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(241, Byte), Integer), CType(CType(245, Byte), Integer), CType(CType(249, Byte), Integer))
        Me.CancelButton = Me.btnSetupCancel
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "MediTrack - First-Time Setup"
        Me.Name = "SetupForm"
        CType(Me.picSetupLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlSetupSwatch.ResumeLayout(False)
        Me.pnlSetupSwatch.PerformLayout()
        Me.grpSetupBrand.ResumeLayout(False)
        Me.grpSetupBrand.PerformLayout()
        Me.grpSetupCode.ResumeLayout(False)
        Me.grpSetupCode.PerformLayout()
        Me.grpSetupAdmin.ResumeLayout(False)
        Me.grpSetupAdmin.PerformLayout()
        Me.grpSetupPharmacy.ResumeLayout(False)
        Me.grpSetupPharmacy.PerformLayout()
        Me.pnlSetupHeader.ResumeLayout(False)
        Me.pnlSetupHeader.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents pnlSetupHeader As System.Windows.Forms.Panel
    Friend WithEvents lblSetupTitle As System.Windows.Forms.Label
    Friend WithEvents lblSetupSub As System.Windows.Forms.Label
    Friend WithEvents grpSetupPharmacy As System.Windows.Forms.GroupBox
    Friend WithEvents lblSetupName As System.Windows.Forms.Label
    Friend WithEvents txtSetupName As System.Windows.Forms.TextBox
    Friend WithEvents lblSetupAddress As System.Windows.Forms.Label
    Friend WithEvents txtSetupAddress As System.Windows.Forms.TextBox
    Friend WithEvents lblSetupPhone As System.Windows.Forms.Label
    Friend WithEvents txtSetupPhone As System.Windows.Forms.TextBox
    Friend WithEvents grpSetupAdmin As System.Windows.Forms.GroupBox
    Friend WithEvents lblSetupFullName As System.Windows.Forms.Label
    Friend WithEvents txtSetupFullName As System.Windows.Forms.TextBox
    Friend WithEvents lblSetupUser As System.Windows.Forms.Label
    Friend WithEvents txtSetupUser As System.Windows.Forms.TextBox
    Friend WithEvents lblSetupPass As System.Windows.Forms.Label
    Friend WithEvents txtSetupPass As System.Windows.Forms.TextBox
    Friend WithEvents lblSetupPass2 As System.Windows.Forms.Label
    Friend WithEvents txtSetupPass2 As System.Windows.Forms.TextBox
    Friend WithEvents grpSetupCode As System.Windows.Forms.GroupBox
    Friend WithEvents lblSetupCode As System.Windows.Forms.Label
    Friend WithEvents txtSetupCode As System.Windows.Forms.TextBox
    Friend WithEvents lblSetupCode2 As System.Windows.Forms.Label
    Friend WithEvents txtSetupCode2 As System.Windows.Forms.TextBox
    Friend WithEvents chkSetupShow As System.Windows.Forms.CheckBox
    Friend WithEvents grpSetupBrand As System.Windows.Forms.GroupBox
    Friend WithEvents picSetupLogo As System.Windows.Forms.PictureBox
    Friend WithEvents btnSetupLogo As System.Windows.Forms.Button
    Friend WithEvents pnlSetupSwatch As System.Windows.Forms.Panel
    Friend WithEvents btnSetupColour As System.Windows.Forms.Button
    Friend WithEvents lblSetupBrandHint As System.Windows.Forms.Label
    Friend WithEvents chkSampleData As System.Windows.Forms.CheckBox
    Friend WithEvents lblSetupError As System.Windows.Forms.Label
    Friend WithEvents btnSetupFinish As System.Windows.Forms.Button
    Friend WithEvents btnSetupCancel As System.Windows.Forms.Button
End Class
