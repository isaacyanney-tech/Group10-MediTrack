<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ChangePasswordForm
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
        Me.lblCpTitle = New System.Windows.Forms.Label()
        Me.lblCpOld = New System.Windows.Forms.Label()
        Me.txtCpOld = New System.Windows.Forms.TextBox()
        Me.lblCpNew = New System.Windows.Forms.Label()
        Me.txtCpNew = New System.Windows.Forms.TextBox()
        Me.lblCpConfirm = New System.Windows.Forms.Label()
        Me.txtCpConfirm = New System.Windows.Forms.TextBox()
        Me.chkCpShow = New System.Windows.Forms.CheckBox()
        Me.lblCpError = New System.Windows.Forms.Label()
        Me.btnCpSave = New System.Windows.Forms.Button()
        Me.btnCpCancel = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblCpTitle
        '
        Me.lblCpTitle.Location = New System.Drawing.Point(16, 14)
        Me.lblCpTitle.Name = "lblCpTitle"
        Me.lblCpTitle.Text = "Change your password"
        Me.lblCpTitle.AutoSize = True
        Me.lblCpTitle.Font = New System.Drawing.Font("Segoe UI", 12!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCpTitle.ForeColor = System.Drawing.Color.FromArgb(CType(CType(15, Byte), Integer), CType(CType(76, Byte), Integer), CType(CType(129, Byte), Integer))
        '
        'lblCpOld
        '
        Me.lblCpOld.Location = New System.Drawing.Point(16, 57)
        Me.lblCpOld.Name = "lblCpOld"
        Me.lblCpOld.Text = "Current password:"
        Me.lblCpOld.AutoSize = True
        Me.lblCpOld.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCpOld.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'txtCpOld
        '
        Me.txtCpOld.Location = New System.Drawing.Point(150, 54)
        Me.txtCpOld.Name = "txtCpOld"
        Me.txtCpOld.Size = New System.Drawing.Size(200, 25)
        Me.txtCpOld.TabIndex = 0
        Me.txtCpOld.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCpOld.UseSystemPasswordChar = True
        '
        'lblCpNew
        '
        Me.lblCpNew.Location = New System.Drawing.Point(16, 91)
        Me.lblCpNew.Name = "lblCpNew"
        Me.lblCpNew.Text = "New password:"
        Me.lblCpNew.AutoSize = True
        Me.lblCpNew.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCpNew.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'txtCpNew
        '
        Me.txtCpNew.Location = New System.Drawing.Point(150, 88)
        Me.txtCpNew.Name = "txtCpNew"
        Me.txtCpNew.Size = New System.Drawing.Size(200, 25)
        Me.txtCpNew.TabIndex = 1
        Me.txtCpNew.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCpNew.UseSystemPasswordChar = True
        '
        'lblCpConfirm
        '
        Me.lblCpConfirm.Location = New System.Drawing.Point(16, 125)
        Me.lblCpConfirm.Name = "lblCpConfirm"
        Me.lblCpConfirm.Text = "Confirm new:"
        Me.lblCpConfirm.AutoSize = True
        Me.lblCpConfirm.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCpConfirm.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'txtCpConfirm
        '
        Me.txtCpConfirm.Location = New System.Drawing.Point(150, 122)
        Me.txtCpConfirm.Name = "txtCpConfirm"
        Me.txtCpConfirm.Size = New System.Drawing.Size(200, 25)
        Me.txtCpConfirm.TabIndex = 2
        Me.txtCpConfirm.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCpConfirm.UseSystemPasswordChar = True
        '
        'chkCpShow
        '
        Me.chkCpShow.Location = New System.Drawing.Point(150, 152)
        Me.chkCpShow.Name = "chkCpShow"
        Me.chkCpShow.TabIndex = 3
        Me.chkCpShow.Text = "Show passwords"
        Me.chkCpShow.AutoSize = True
        Me.chkCpShow.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkCpShow.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        '
        'lblCpError
        '
        Me.lblCpError.Location = New System.Drawing.Point(16, 180)
        Me.lblCpError.Name = "lblCpError"
        Me.lblCpError.Size = New System.Drawing.Size(340, 18)
        Me.lblCpError.Text = ""
        Me.lblCpError.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCpError.ForeColor = System.Drawing.Color.FromArgb(CType(CType(198, Byte), Integer), CType(CType(40, Byte), Integer), CType(CType(40, Byte), Integer))
        '
        'btnCpSave
        '
        Me.btnCpSave.Location = New System.Drawing.Point(150, 204)
        Me.btnCpSave.Name = "btnCpSave"
        Me.btnCpSave.Size = New System.Drawing.Size(96, 32)
        Me.btnCpSave.TabIndex = 4
        Me.btnCpSave.Text = "&Save"
        Me.btnCpSave.BackColor = System.Drawing.Color.FromArgb(CType(CType(21, Byte), Integer), CType(CType(101, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnCpSave.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnCpSave.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(21, Byte), Integer), CType(CType(101, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.btnCpSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCpSave.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCpSave.ForeColor = System.Drawing.Color.White
        Me.btnCpSave.UseVisualStyleBackColor = False
        '
        'btnCpCancel
        '
        Me.btnCpCancel.Location = New System.Drawing.Point(254, 204)
        Me.btnCpCancel.Name = "btnCpCancel"
        Me.btnCpCancel.Size = New System.Drawing.Size(96, 32)
        Me.btnCpCancel.TabIndex = 5
        Me.btnCpCancel.Text = "Cancel"
        Me.btnCpCancel.BackColor = System.Drawing.Color.FromArgb(CType(CType(248, Byte), Integer), CType(CType(250, Byte), Integer), CType(CType(252, Byte), Integer))
        Me.btnCpCancel.Cursor = System.Windows.Forms.Cursors.Hand
        Me.btnCpCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(184, Byte), Integer), CType(CType(194, Byte), Integer), CType(CType(207, Byte), Integer))
        Me.btnCpCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCpCancel.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCpCancel.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        Me.btnCpCancel.UseVisualStyleBackColor = False
        '
        'ChangePasswordForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(372, 250)
        Me.Controls.Add(Me.lblCpTitle)
        Me.Controls.Add(Me.lblCpOld)
        Me.Controls.Add(Me.txtCpOld)
        Me.Controls.Add(Me.lblCpNew)
        Me.Controls.Add(Me.txtCpNew)
        Me.Controls.Add(Me.lblCpConfirm)
        Me.Controls.Add(Me.txtCpConfirm)
        Me.Controls.Add(Me.chkCpShow)
        Me.Controls.Add(Me.lblCpError)
        Me.Controls.Add(Me.btnCpSave)
        Me.Controls.Add(Me.btnCpCancel)
        Me.Font = New System.Drawing.Font("Segoe UI", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AcceptButton = Me.btnCpSave
        Me.BackColor = System.Drawing.Color.White
        Me.CancelButton = Me.btnCpCancel
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Change Password"
        Me.Name = "ChangePasswordForm"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblCpTitle As System.Windows.Forms.Label
    Friend WithEvents lblCpOld As System.Windows.Forms.Label
    Friend WithEvents txtCpOld As System.Windows.Forms.TextBox
    Friend WithEvents lblCpNew As System.Windows.Forms.Label
    Friend WithEvents txtCpNew As System.Windows.Forms.TextBox
    Friend WithEvents lblCpConfirm As System.Windows.Forms.Label
    Friend WithEvents txtCpConfirm As System.Windows.Forms.TextBox
    Friend WithEvents chkCpShow As System.Windows.Forms.CheckBox
    Friend WithEvents lblCpError As System.Windows.Forms.Label
    Friend WithEvents btnCpSave As System.Windows.Forms.Button
    Friend WithEvents btnCpCancel As System.Windows.Forms.Button
End Class
