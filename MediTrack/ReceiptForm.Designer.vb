<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ReceiptForm
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
        Me.pnlReceiptTop = New System.Windows.Forms.Panel()
        Me.lblReceiptTitle = New System.Windows.Forms.Label()
        Me.pnlReceiptButtons = New System.Windows.Forms.Panel()
        Me.btnPrint = New System.Windows.Forms.Button()
        Me.btnPrintPreview = New System.Windows.Forms.Button()
        Me.btnCloseReceipt = New System.Windows.Forms.Button()
        Me.pnlPaperBack = New System.Windows.Forms.Panel()
        Me.pnlPaper = New System.Windows.Forms.Panel()
        Me.pnlReceiptTop.SuspendLayout()
        Me.pnlReceiptButtons.SuspendLayout()
        Me.pnlPaperBack.SuspendLayout()
        Me.pnlPaper.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlReceiptTop
        '
        Me.pnlReceiptTop.Controls.Add(Me.lblReceiptTitle)
        Me.pnlReceiptTop.Location = New System.Drawing.Point(0, 0)
        Me.pnlReceiptTop.Name = "pnlReceiptTop"
        Me.pnlReceiptTop.Size = New System.Drawing.Size(420, 44)
        Me.pnlReceiptTop.TabIndex = 0
        Me.pnlReceiptTop.BackColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(78, Byte), Integer), CType(CType(121, Byte), Integer))
        Me.pnlReceiptTop.Dock = System.Windows.Forms.DockStyle.Top
        '
        'lblReceiptTitle
        '
        Me.lblReceiptTitle.Location = New System.Drawing.Point(12, 10)
        Me.lblReceiptTitle.Name = "lblReceiptTitle"
        Me.lblReceiptTitle.Text = "Receipt"
        Me.lblReceiptTitle.AutoSize = True
        Me.lblReceiptTitle.Font = New System.Drawing.Font("Segoe UI", 12!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblReceiptTitle.ForeColor = System.Drawing.Color.White
        '
        'pnlReceiptButtons
        '
        Me.pnlReceiptButtons.Controls.Add(Me.btnPrint)
        Me.pnlReceiptButtons.Controls.Add(Me.btnPrintPreview)
        Me.pnlReceiptButtons.Controls.Add(Me.btnCloseReceipt)
        Me.pnlReceiptButtons.Location = New System.Drawing.Point(0, 600)
        Me.pnlReceiptButtons.Name = "pnlReceiptButtons"
        Me.pnlReceiptButtons.Size = New System.Drawing.Size(420, 52)
        Me.pnlReceiptButtons.TabIndex = 1
        Me.pnlReceiptButtons.BackColor = System.Drawing.Color.FromArgb(CType(CType(238, Byte), Integer), CType(CType(242, Byte), Integer), CType(CType(247, Byte), Integer))
        Me.pnlReceiptButtons.Dock = System.Windows.Forms.DockStyle.Bottom
        '
        'btnPrint
        '
        Me.btnPrint.Location = New System.Drawing.Point(12, 10)
        Me.btnPrint.Name = "btnPrint"
        Me.btnPrint.Size = New System.Drawing.Size(130, 32)
        Me.btnPrint.TabIndex = 2
        Me.btnPrint.Text = "&Print..."
        Me.btnPrint.BackColor = System.Drawing.Color.FromArgb(CType(CType(46, Byte), Integer), CType(CType(117, Byte), Integer), CType(CType(182, Byte), Integer))
        Me.btnPrint.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(46, Byte), Integer), CType(CType(117, Byte), Integer), CType(CType(182, Byte), Integer))
        Me.btnPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPrint.Font = New System.Drawing.Font("Segoe UI", 9!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPrint.ForeColor = System.Drawing.Color.White
        Me.btnPrint.UseVisualStyleBackColor = False
        '
        'btnPrintPreview
        '
        Me.btnPrintPreview.Location = New System.Drawing.Point(148, 10)
        Me.btnPrintPreview.Name = "btnPrintPreview"
        Me.btnPrintPreview.Size = New System.Drawing.Size(130, 32)
        Me.btnPrintPreview.TabIndex = 3
        Me.btnPrintPreview.Text = "Print Pre&view"
        Me.btnPrintPreview.BackColor = System.Drawing.Color.White
        Me.btnPrintPreview.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(166, Byte), Integer), CType(CType(166, Byte), Integer), CType(CType(166, Byte), Integer))
        Me.btnPrintPreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnPrintPreview.Font = New System.Drawing.Font("Segoe UI", 9!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnPrintPreview.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        Me.btnPrintPreview.UseVisualStyleBackColor = False
        '
        'btnCloseReceipt
        '
        Me.btnCloseReceipt.Location = New System.Drawing.Point(300, 10)
        Me.btnCloseReceipt.Name = "btnCloseReceipt"
        Me.btnCloseReceipt.Size = New System.Drawing.Size(108, 32)
        Me.btnCloseReceipt.TabIndex = 4
        Me.btnCloseReceipt.Text = "&Close"
        Me.btnCloseReceipt.BackColor = System.Drawing.Color.White
        Me.btnCloseReceipt.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(CType(CType(166, Byte), Integer), CType(CType(166, Byte), Integer), CType(CType(166, Byte), Integer))
        Me.btnCloseReceipt.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnCloseReceipt.Font = New System.Drawing.Font("Segoe UI", 9!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCloseReceipt.ForeColor = System.Drawing.Color.FromArgb(CType(CType(31, Byte), Integer), CType(CType(41, Byte), Integer), CType(CType(55, Byte), Integer))
        Me.btnCloseReceipt.UseVisualStyleBackColor = False
        '
        'pnlPaperBack
        '
        Me.pnlPaperBack.Controls.Add(Me.pnlPaper)
        Me.pnlPaperBack.Location = New System.Drawing.Point(0, 44)
        Me.pnlPaperBack.Name = "pnlPaperBack"
        Me.pnlPaperBack.Size = New System.Drawing.Size(420, 556)
        Me.pnlPaperBack.TabIndex = 5
        Me.pnlPaperBack.BackColor = System.Drawing.Color.FromArgb(CType(CType(217, Byte), Integer), CType(CType(222, Byte), Integer), CType(CType(229, Byte), Integer))
        Me.pnlPaperBack.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlPaperBack.AutoScroll = True
        '
        'pnlPaper
        '
        Me.pnlPaper.Location = New System.Drawing.Point(40, 16)
        Me.pnlPaper.Name = "pnlPaper"
        Me.pnlPaper.Size = New System.Drawing.Size(340, 520)
        Me.pnlPaper.TabIndex = 6
        Me.pnlPaper.BackColor = System.Drawing.Color.White
        Me.pnlPaper.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        '
        'ReceiptForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(420, 652)
        Me.Controls.Add(Me.pnlPaperBack)
        Me.Controls.Add(Me.pnlReceiptTop)
        Me.Controls.Add(Me.pnlReceiptButtons)
        Me.Font = New System.Drawing.Font("Segoe UI", 9!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.AcceptButton = Me.btnPrint
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(217, Byte), Integer), CType(CType(222, Byte), Integer), CType(CType(229, Byte), Integer))
        Me.CancelButton = Me.btnCloseReceipt
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Receipt"
        Me.Name = "ReceiptForm"
        Me.pnlPaper.ResumeLayout(False)
        Me.pnlPaper.PerformLayout()
        Me.pnlPaperBack.ResumeLayout(False)
        Me.pnlPaperBack.PerformLayout()
        Me.pnlReceiptButtons.ResumeLayout(False)
        Me.pnlReceiptButtons.PerformLayout()
        Me.pnlReceiptTop.ResumeLayout(False)
        Me.pnlReceiptTop.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents pnlReceiptTop As System.Windows.Forms.Panel
    Friend WithEvents lblReceiptTitle As System.Windows.Forms.Label
    Friend WithEvents pnlReceiptButtons As System.Windows.Forms.Panel
    Friend WithEvents btnPrint As System.Windows.Forms.Button
    Friend WithEvents btnPrintPreview As System.Windows.Forms.Button
    Friend WithEvents btnCloseReceipt As System.Windows.Forms.Button
    Friend WithEvents pnlPaperBack As System.Windows.Forms.Panel
    Friend WithEvents pnlPaper As System.Windows.Forms.Panel
End Class
