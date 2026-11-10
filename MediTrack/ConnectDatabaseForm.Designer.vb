<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ConnectDatabaseForm
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
        Me.lblDbTitle = New System.Windows.Forms.Label()
        Me.rbLocalDb = New System.Windows.Forms.RadioButton()
        Me.lblLocalHint = New System.Windows.Forms.Label()
        Me.rbSharedDb = New System.Windows.Forms.RadioButton()
        Me.lblSharedHint = New System.Windows.Forms.Label()
        Me.txtDbPath = New System.Windows.Forms.TextBox()
        Me.btnBrowseDb = New System.Windows.Forms.Button()
        Me.btnTestDb = New System.Windows.Forms.Button()
        Me.lblDbTestResult = New System.Windows.Forms.Label()
        Me.btnDbSave = New System.Windows.Forms.Button()
        Me.btnDbCancel = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblDbTitle
        '
        Me.lblDbTitle.Location = New System.Drawing.Point(16, 14)
        Me.lblDbTitle.Name = "lblDbTitle"
        Me.lblDbTitle.Text = "Where is the MediTrack database?"
        Me.lblDbTitle.AutoSize = True
        Me.lblDbTitle.Font = New System.Drawing.Font("Segoe UI", 12!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDbTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(76, Byte), Integer), CType(CType(129, Byte), Integer))
        '
        'rbLocalDb
        '
        Me.rbLocalDb.Location = New System.Drawing.Point(20, 50)
        Me.rbLocalDb.Name = "rbLocalDb"
        Me.rbLocalDb.TabIndex = 0
        Me.rbLocalDb.Text = "On this computer (the main / admin PC)"
        Me.rbLocalDb.AutoSize = True
        Me.rbLocalDb.Checked = True
        Me.rbLocalDb.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rbLocalDb.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        Me.rbLocalDb.TabStop = True
        '
        'lblLocalHint
        '
        Me.lblLocalHint.Location = New System.Drawing.Point(38, 74)
        Me.lblLocalHint.Name = "lblLocalHint"
        Me.lblLocalHint.Text = "The database file is kept next to the program on this PC."
        Me.lblLocalHint.AutoSize = True
        Me.lblLocalHint.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLocalHint.ForeColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        '
        'rbSharedDb
        '
        Me.rbSharedDb.Location = New System.Drawing.Point(20, 104)
        Me.rbSharedDb.Name = "rbSharedDb"
        Me.rbSharedDb.TabIndex = 1
        Me.rbSharedDb.Text = "Shared database on another computer (counter PCs)"
        Me.rbSharedDb.AutoSize = True
        Me.rbSharedDb.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rbSharedDb.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'lblSharedHint
        '
        Me.lblSharedHint.Location = New System.Drawing.Point(38, 128)
        Me.lblSharedHint.Name = "lblSharedHint"
        Me.lblSharedHint.Text = "Example:  \\ADMIN-PC\MediTrack\MediTrack.accdb"
        Me.lblSharedHint.AutoSize = True
        Me.lblSharedHint.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblSharedHint.ForeColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        '
        'txtDbPath
        '
        Me.txtDbPath.Location = New System.Drawing.Point(38, 152)
        Me.txtDbPath.Name = "txtDbPath"
        Me.txtDbPath.Size = New System.Drawing.Size(400, 25)
        Me.txtDbPath.TabIndex = 2
        Me.txtDbPath.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        '
        'btnBrowseDb
        '
        Me.btnBrowseDb.Location = New System.Drawing.Point(446, 150)
        Me.btnBrowseDb.Name = "btnBrowseDb"
        Me.btnBrowseDb.Size = New System.Drawing.Size(96, 29)
        Me.btnBrowseDb.TabIndex = 3
        Me.btnBrowseDb.Text = "Browse..."
        Me.btnBrowseDb.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(252, Byte), Integer))
        Me.btnBrowseDb.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnBrowseDb.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(184, Byte), Integer), CType(CType(194, Byte), Integer), CType(CType(207, Byte), Integer))
        Me.btnBrowseDb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnBrowseDb.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnBrowseDb.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        Me.btnBrowseDb.UseVisualStyleBackColor = False
        '
        'btnTestDb
        '
        Me.btnTestDb.Location = New System.Drawing.Point(38, 190)
        Me.btnTestDb.Name = "btnTestDb"
        Me.btnTestDb.Size = New System.Drawing.Size(150, 32)
        Me.btnTestDb.TabIndex = 4
        Me.btnTestDb.Text = "Test Connection"
        Me.btnTestDb.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(252, Byte), Integer))
        Me.btnTestDb.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnTestDb.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(184, Byte), Integer), CType(CType(194, Byte), Integer), CType(CType(207, Byte), Integer))
        Me.btnTestDb.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnTestDb.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnTestDb.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        Me.btnTestDb.UseVisualStyleBackColor = False
        '
        'lblDbTestResult
        '
        Me.lblDbTestResult.Location = New System.Drawing.Point(196, 198)
        Me.lblDbTestResult.Name = "lblDbTestResult"
        Me.lblDbTestResult.Size = New System.Drawing.Size(346, 20)
        Me.lblDbTestResult.Text = ""
        Me.lblDbTestResult.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDbTestResult.ForeColor = System.Drawing.Color.FromArgb(CType(CType(100, Byte), Integer), CType(CType(116, Byte), Integer), CType(CType(139, Byte), Integer))
        '
        'btnDbSave
        '
        Me.btnDbSave.Location = New System.Drawing.Point(330, 240)
        Me.btnDbSave.Name = "btnDbSave"
        Me.btnDbSave.Size = New System.Drawing.Size(100, 34)
        Me.btnDbSave.TabIndex = 5
        Me.btnDbSave.Text = "&Save"
        Me.btnDbSave.BackColor = System.Drawing.Color.FromArgb(CType(CType(21, Byte), Integer), CType(CType(101, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnDbSave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnDbSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(21, Byte), Integer), CType(CType(101, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnDbSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnDbSave.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDbSave.ForeColor = System.Drawing.Color.White
        Me.btnDbSave.UseVisualStyleBackColor = False
        '
        'btnDbCancel
        '
        Me.btnDbCancel.Location = New System.Drawing.Point(438, 240)
        Me.btnDbCancel.Name = "btnDbCancel"
        Me.btnDbCancel.Size = New System.Drawing.Size(104, 34)
        Me.btnDbCancel.TabIndex = 6
        Me.btnDbCancel.Text = "Cancel"
        Me.btnDbCancel.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(252, Byte), Integer))
        Me.btnDbCancel.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnDbCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(184, Byte), Integer), CType(CType(194, Byte), Integer), CType(CType(207, Byte), Integer))
        Me.btnDbCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnDbCancel.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnDbCancel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        Me.btnDbCancel.UseVisualStyleBackColor = False
        '
        'ConnectDatabaseForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(560, 290)
        Me.Controls.Add(Me.lblDbTitle)
        Me.Controls.Add(Me.rbLocalDb)
        Me.Controls.Add(Me.lblLocalHint)
        Me.Controls.Add(Me.rbSharedDb)
        Me.Controls.Add(Me.lblSharedHint)
        Me.Controls.Add(Me.txtDbPath)
        Me.Controls.Add(Me.btnBrowseDb)
        Me.Controls.Add(Me.btnTestDb)
        Me.Controls.Add(Me.lblDbTestResult)
        Me.Controls.Add(Me.btnDbSave)
        Me.Controls.Add(Me.btnDbCancel)
        Me.Font = New System.Drawing.Font("Segoe UI", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AcceptButton = Me.btnDbSave
        Me.BackColor = System.Drawing.Color.White
        Me.CancelButton = Me.btnDbCancel
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Database Settings"
        Me.Name = "ConnectDatabaseForm"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblDbTitle As System.Windows.Forms.Label
    Friend WithEvents rbLocalDb As System.Windows.Forms.RadioButton
    Friend WithEvents lblLocalHint As System.Windows.Forms.Label
    Friend WithEvents rbSharedDb As System.Windows.Forms.RadioButton
    Friend WithEvents lblSharedHint As System.Windows.Forms.Label
    Friend WithEvents txtDbPath As System.Windows.Forms.TextBox
    Friend WithEvents btnBrowseDb As System.Windows.Forms.Button
    Friend WithEvents btnTestDb As System.Windows.Forms.Button
    Friend WithEvents lblDbTestResult As System.Windows.Forms.Label
    Friend WithEvents btnDbSave As System.Windows.Forms.Button
    Friend WithEvents btnDbCancel As System.Windows.Forms.Button
End Class
