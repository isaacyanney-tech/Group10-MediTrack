' ============================================================================
'  ReceiptForm.vb  -  shows a receipt on screen and prints it
'
'  Opened after every sale (Counter tab) and from Sales Records ("Print Receipt").
'  Print...       : choose any printer (or "Microsoft Print to PDF" to save a PDF)
'  Print Preview  : see the printed page first
' ============================================================================
Imports System.Drawing
Imports System.Drawing.Printing
Imports System.Windows.Forms

Public Class ReceiptForm

    Private ReadOnly receiptSale As Sale
    Private WithEvents printDoc As New PrintDocument()

    ''' <summary>Needed by the Visual Studio designer.</summary>
    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(s As Sale)
        InitializeComponent()
        receiptSale = s
    End Sub

    Private Sub ReceiptForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If receiptSale Is Nothing Then Return
        Me.Text = "Receipt #" & receiptSale.SaleID.ToString()
        lblReceiptTitle.Text = "Receipt #" & receiptSale.SaleID.ToString() & "  -  " & "GH₵ " & receiptSale.Total.ToString("N2")

        ' Make the white "paper" exactly as tall as the receipt
        Using g As Graphics = pnlPaper.CreateGraphics()
            Dim h As Single = ReceiptPrinter.Draw(g, receiptSale, 16, 16, pnlPaper.Width - 34, False)
            pnlPaper.Height = CInt(h) + 36
        End Using
        Dim wanted As Integer = pnlPaper.Height + 32 + pnlReceiptTop.Height + pnlReceiptButtons.Height
        Me.ClientSize = New Size(Me.ClientSize.Width, Math.Min(wanted, Screen.FromControl(Me).WorkingArea.Height - 60))

        printDoc.DocumentName = "Receipt " & receiptSale.SaleID.ToString()
    End Sub

    ''' <summary>Draws the receipt on the white panel.</summary>
    Private Sub pnlPaper_Paint(sender As Object, e As PaintEventArgs) Handles pnlPaper.Paint
        If receiptSale Is Nothing Then Return
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit
        ReceiptPrinter.Draw(e.Graphics, receiptSale, 16, 16, pnlPaper.Width - 34)
    End Sub

    ' ---------------- Printing ----------------
    Private Sub btnPrint_Click(sender As Object, e As EventArgs) Handles btnPrint.Click
        Try
            Using dlg As New PrintDialog()
                dlg.Document = printDoc
                dlg.UseEXDialog = True
                If dlg.ShowDialog(Me) = DialogResult.OK Then
                    printDoc.Print()
                    MessageBox.Show("The receipt was sent to the printer.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Could not print. Is a printer installed?" & vbCrLf & vbCrLf & ex.Message, "Print",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnPrintPreview_Click(sender As Object, e As EventArgs) Handles btnPrintPreview.Click
        Try
            Using preview As New PrintPreviewDialog()
                preview.Document = printDoc
                preview.Text = Me.Text
                preview.Width = 600
                preview.Height = 750
                preview.StartPosition = FormStartPosition.CenterParent
                preview.ShowDialog(Me)
            End Using
        Catch ex As Exception
            MessageBox.Show("Could not show the preview. Is a printer installed?" & vbCrLf & vbCrLf & ex.Message, "Print Preview",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>Draws the receipt on the printed page (top-left, receipt width).</summary>
    Private Sub printDoc_PrintPage(sender As Object, e As PrintPageEventArgs) Handles printDoc.PrintPage
        Dim left As Single = Math.Max(e.MarginBounds.Left, 20)
        Dim top As Single = Math.Max(e.MarginBounds.Top, 20)
        Dim width As Single = Math.Min(ReceiptPrinter.ReceiptWidth, e.MarginBounds.Width)
        ReceiptPrinter.Draw(e.Graphics, receiptSale, left, top, width)
        e.HasMorePages = False
    End Sub

    Private Sub btnCloseReceipt_Click(sender As Object, e As EventArgs) Handles btnCloseReceipt.Click
        Me.Close()
    End Sub

End Class
