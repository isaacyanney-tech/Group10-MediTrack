' ============================================================================
'  ReceiptPrinter.vb
'  Draws a receipt like a till slip (about 80 mm wide). The SAME drawing code
'  is used for the receipt window on screen and for the printer, so what you
'  see is exactly what gets printed.
' ============================================================================
Imports System.Collections.Generic
Imports System.Drawing

Public Module ReceiptPrinter

    ''' <summary>Width of the receipt in 1/100 inch (80 mm paper = about 3.15 inches).</summary>
    Public Const ReceiptWidth As Integer = 300

    ''' <summary>
    ''' Draws the receipt for a sale starting at (left, top) and returns the
    ''' height it used. Pass render = False to only measure the height.
    ''' </summary>
    Public Function Draw(g As Graphics, s As Sale, left As Single, top As Single, width As Single,
                         Optional render As Boolean = True) As Single
        Dim y As Single = top
        Dim items As List(Of SaleItem) = DataStore.ItemsOfSale(s.SaleID)

        Using titleFont As New Font("Segoe UI", 12.0F, FontStyle.Bold),
              boldFont As New Font("Segoe UI", 8.25F, FontStyle.Bold),
              textFont As New Font("Segoe UI", 8.25F),
              smallFont As New Font("Segoe UI", 7.5F),
              totalFont As New Font("Segoe UI", 11.0F, FontStyle.Bold),
              ink As New SolidBrush(Color.FromArgb(25, 25, 25)),
              grey As New SolidBrush(Color.FromArgb(90, 90, 90)),
              dashPen As New Pen(Color.FromArgb(120, 120, 120)),
              centre As New StringFormat(), rightAlign As New StringFormat()

            centre.Alignment = StringAlignment.Center
            rightAlign.Alignment = StringAlignment.Far
            dashPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash

            ' ---- small helpers (lambdas) ----
            Dim lineOf = Function(f As Font) f.GetHeight(g) + 1
            Dim centred = Sub(text As String, f As Font, b As Brush)
                              If render Then g.DrawString(text, f, b, New RectangleF(left, y, width, lineOf(f)), centre)
                              y += lineOf(f)
                          End Sub
            Dim twoColumns = Sub(leftText As String, rightText As String, f As Font)
                                 If render Then
                                     g.DrawString(leftText, f, ink, left, y)
                                     g.DrawString(rightText, f, ink, New RectangleF(left, y, width, lineOf(f)), rightAlign)
                                 End If
                                 y += lineOf(f)
                             End Sub
            Dim dashes = Sub()
                             y += 3
                             If render Then g.DrawLine(dashPen, left, y, left + width, y)
                             y += 5
                         End Sub

            ' ---- Header ----
            ' The business's own logo (if it has one), centred and at most 56 units high
            Dim logo As Image = Branding.Logo()
            If logo IsNot Nothing Then
                Dim h As Single = Math.Min(56.0F, logo.Height)
                Dim w As Single = h * logo.Width / logo.Height
                If w > width Then
                    w = width
                    h = w * logo.Height / logo.Width
                End If
                If render Then g.DrawImage(logo, left + (width - w) / 2, y, w, h)
                y += h + 4
            End If
            centred(Branding.BusinessName(), titleFont, ink)
            Dim address As String = DataStore.GetSetting("PharmacyAddress")
            If address <> "" Then centred(address, textFont, grey)
            For Each phoneLine As String In Branding.PhoneLines(2)        ' two numbers per line
                centred(phoneLine, textFont, grey)
            Next
            Dim email As String = DataStore.GetSetting("PharmacyEmail")
            If email <> "" Then centred(email, textFont, grey)
            y += 4
            centred("SALES RECEIPT", boldFont, ink)
            If s.Voided Then
                y += 2
                centred("*** VOIDED - NOT VALID ***", totalFont, ink)
                If s.VoidReason <> "" Then centred(s.VoidReason, smallFont, grey)
            End If
            dashes()

            ' ---- Receipt details ----
            twoColumns("Receipt No:", s.SaleID.ToString(), textFont)
            twoColumns("Date:", s.SaleDate.ToString("dd/MM/yyyy  h:mm tt"), textFont)
            twoColumns("Customer:", s.Customer, textFont)
            twoColumns("Served by:", s.SoldBy, textFont)
            dashes()

            ' ---- Items (columns: item | qty | price | amount) ----
            Dim qtyX As Single = left + width * 0.52F
            Dim priceRight As Single = left + width * 0.78F
            If render Then
                g.DrawString("ITEM", boldFont, ink, left, y)
                g.DrawString("QTY", boldFont, ink, qtyX, y)
                g.DrawString("PRICE", boldFont, ink, New RectangleF(left, y, priceRight - left, lineOf(boldFont)), rightAlign)
                g.DrawString("AMOUNT", boldFont, ink, New RectangleF(left, y, width, lineOf(boldFont)), rightAlign)
            End If
            y += lineOf(boldFont) + 2

            Dim units As Integer = 0
            For Each item As SaleItem In items
                units += item.Quantity
                ' Drug name on its own line if it is long
                Dim nameWidth As Single = g.MeasureString(item.DrugName, textFont).Width
                If nameWidth > qtyX - left - 4 Then
                    If render Then g.DrawString(item.DrugName, textFont, ink, left, y)
                    y += lineOf(textFont)
                ElseIf render Then
                    g.DrawString(item.DrugName, textFont, ink, left, y)
                End If
                If render Then
                    g.DrawString(item.Quantity.ToString(), textFont, ink, qtyX, y)
                    g.DrawString(item.UnitPrice.ToString("N2"), textFont, ink, New RectangleF(left, y, priceRight - left, lineOf(textFont)), rightAlign)
                    g.DrawString(item.Total.ToString("N2"), textFont, ink, New RectangleF(left, y, width, lineOf(textFont)), rightAlign)
                End If
                y += lineOf(textFont)
                If render Then g.DrawString("Batch " & item.BatchNo, smallFont, grey, left + 8, y)
                y += lineOf(smallFont) + 2
            Next
            dashes()

            ' ---- Totals ----
            twoColumns("Items:", units.ToString(), textFont)
            If s.Discount > 0 Then
                twoColumns("Subtotal", s.Subtotal.ToString("N2"), textFont)
                twoColumns("Discount", "-" & s.Discount.ToString("N2"), textFont)
            End If
            twoColumns("TOTAL (GH₵)", s.Total.ToString("N2"), totalFont)
            twoColumns("Payment", If(s.PaymentMethod = "", "Cash", s.PaymentMethod), textFont)
            twoColumns("Amount paid", s.AmountPaid.ToString("N2"), textFont)
            twoColumns("Change", (s.AmountPaid - s.Total).ToString("N2"), boldFont)
            dashes()

            ' ---- Footer ----
            centred(DataStore.GetSetting("ReceiptFooter", "Thank you!"), boldFont, ink)
            centred("Goods sold are not returnable after use.", smallFont, grey)
            centred("Printed " & Date.Now.ToString("dd/MM/yyyy h:mm tt"), smallFont, grey)
            centred("Powered by MediTrack", smallFont, grey)
        End Using

        Return y - top
    End Function

End Module
