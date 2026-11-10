' ============================================================================
'  Branding.vb
'  MediTrack is sold to many businesses, so each one can show its OWN
'  name, logo, colour, phone numbers and email. They are kept in the
'  Settings table, so every computer that shares the database shows the
'  same branding:
'     PharmacyName, PharmacyAddress, PharmacyPhone (one or more, comma
'     separated), PharmacyEmail, ReceiptFooter, LogoImage (PNG as Base64
'     text) and ThemeColor (#RRGGBB).
' ============================================================================
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Windows.Forms

Public Module Branding

    ''' <summary>The MediTrack blue used when the business has not chosen a colour.</summary>
    Public ReadOnly DefaultTheme As Color = Color.FromArgb(15, 76, 129)

    Private cachedLogoText As String = Nothing
    Private cachedLogo As Image = Nothing

    ' ------------------------------------------------------------------ name and contacts
    Public Function BusinessName() As String
        Return DataStore.GetSetting("PharmacyName", "MediTrack Pharmacy")
    End Function

    ''' <summary>All phone numbers typed in Settings (separated by commas, semicolons or slashes).</summary>
    Public Function PhoneNumbers() As List(Of String)
        Dim result As New List(Of String)
        For Each part As String In DataStore.GetSetting("PharmacyPhone").Split(New Char() {","c, ";"c, "/"c, ControlChars.Lf})
            If part.Trim() <> "" Then result.Add(part.Trim())
        Next
        Return result
    End Function

    ''' <summary>Phone numbers as lines of text, at most 'perLine' numbers on each line (for receipts).</summary>
    Public Function PhoneLines(perLine As Integer) As List(Of String)
        Dim lines As New List(Of String)
        Dim numbers As List(Of String) = PhoneNumbers()
        For i As Integer = 0 To numbers.Count - 1 Step perLine
            Dim group As New List(Of String)
            For j As Integer = i To Math.Min(i + perLine, numbers.Count) - 1
                group.Add(numbers(j))
            Next
            lines.Add(If(i = 0, "Tel: ", "") & String.Join("  /  ", group.ToArray()))
        Next
        Return lines
    End Function

    ' ------------------------------------------------------------------ logo
    ''' <summary>The business logo, or Nothing if no logo was chosen.</summary>
    Public Function Logo() As Image
        Dim text As String = DataStore.GetSetting("LogoImage")
        If text = "" Then Return Nothing
        If text <> cachedLogoText Then
            cachedLogoText = text
            cachedLogo = Nothing
            Try
                Using ms As New MemoryStream(Convert.FromBase64String(text))
                    cachedLogo = New Bitmap(Image.FromStream(ms))      ' copy, so the stream can close
                End Using
            Catch
                cachedLogo = Nothing                                    ' damaged value: behave as "no logo"
            End Try
        End If
        Return cachedLogo
    End Function

    ''' <summary>
    ''' Reads a picture file, makes it at most 400 x 200 pixels (keeping its shape)
    ''' and returns it as Base64 text, ready to save in the Settings table.
    ''' </summary>
    Public Function LogoFileToText(fileName As String) As String
        Dim info As New FileInfo(fileName)
        If info.Length > 10 * 1024 * 1024 Then Throw New InvalidOperationException("The picture is too large (over 10 MB).")
        Using original As Image = Image.FromFile(fileName)
            Dim scale As Double = Math.Min(1.0, Math.Min(400.0 / original.Width, 200.0 / original.Height))
            Dim w As Integer = Math.Max(1, CInt(original.Width * scale))
            Dim h As Integer = Math.Max(1, CInt(original.Height * scale))
            Using small As New Bitmap(w, h, PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(small)
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic
                    g.SmoothingMode = SmoothingMode.HighQuality
                    g.Clear(Color.Transparent)
                    g.DrawImage(original, 0, 0, w, h)
                End Using
                Using ms As New MemoryStream()
                    small.Save(ms, ImageFormat.Png)
                    Return Convert.ToBase64String(ms.ToArray())
                End Using
            End Using
        End Using
    End Function

    ''' <summary>Turns Base64 text back into a picture (used for previews before saving).</summary>
    Public Function TextToImage(text As String) As Image
        If text = "" Then Return Nothing
        Using ms As New MemoryStream(Convert.FromBase64String(text))
            Return New Bitmap(Image.FromStream(ms))
        End Using
    End Function

    ''' <summary>Opens a file dialog to pick a logo. Returns the Base64 text, or "" if cancelled.</summary>
    Public Function PickLogo(owner As IWin32Window) As String
        Using dlg As New OpenFileDialog()
            dlg.Title = "Choose your business logo"
            dlg.Filter = "Pictures (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif"
            If dlg.ShowDialog(owner) <> DialogResult.OK Then Return ""
            Try
                Return LogoFileToText(dlg.FileName)
            Catch ex As Exception
                MessageBox.Show("That picture could not be used." & vbCrLf & ex.Message, "Logo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return ""
            End Try
        End Using
    End Function

    ' ------------------------------------------------------------------ colour
    ''' <summary>The business's theme colour (header, login panel, table headings, titles).</summary>
    Public Function ThemeColor() As Color
        Dim hex As String = DataStore.GetSetting("ThemeColor")
        If hex = "" Then Return DefaultTheme
        Try
            Return ColorTranslator.FromHtml(hex)
        Catch
            Return DefaultTheme
        End Try
    End Function

    Public Function ColorToText(c As Color) As String
        Return "#" & c.R.ToString("X2") & c.G.ToString("X2") & c.B.ToString("X2")
    End Function

    ''' <summary>Opens the Windows colour picker. Returns True and the colour if one was chosen.</summary>
    Public Function PickColour(owner As IWin32Window, current As Color, ByRef chosen As Color) As Boolean
        Using dlg As New ColorDialog()
            dlg.Color = current
            dlg.FullOpen = True
            dlg.CustomColors = New Integer() {
                ToOle(15, 76, 129), ToOle(46, 125, 50), ToOle(106, 27, 154), ToOle(183, 28, 28),
                ToOle(0, 121, 107), ToOle(55, 71, 79), ToOle(230, 81, 0), ToOle(21, 101, 192)}
            If dlg.ShowDialog(owner) <> DialogResult.OK Then Return False
            chosen = Color.FromArgb(dlg.Color.R, dlg.Color.G, dlg.Color.B)
            Return True
        End Using
    End Function

    Private Function ToOle(r As Integer, g As Integer, b As Integer) As Integer
        Return r Or (g << 8) Or (b << 16)
    End Function

    ''' <summary>
    ''' Finds every place in a window that uses the MediTrack blue (backgrounds,
    ''' text, table headings). Call it once, when the window opens; then
    ''' ApplyTheme can repaint exactly those places as often as needed.
    ''' </summary>
    Public Function FindThemed(root As Control) As List(Of Action(Of Color))
        Dim found As New List(Of Action(Of Color))
        Collect(root, DefaultTheme.ToArgb(), found)
        Return found
    End Function

    Private Sub Collect(c As Control, argb As Integer, found As List(Of Action(Of Color)))
        Dim ctl As Control = c
        If c.BackColor.ToArgb() = argb Then found.Add(Sub(col) ctl.BackColor = col)
        If c.ForeColor.ToArgb() = argb Then found.Add(Sub(col) ctl.ForeColor = col)
        If TypeOf c Is DataGridView Then
            Dim grid As DataGridView = DirectCast(c, DataGridView)
            If grid.ColumnHeadersDefaultCellStyle.BackColor.ToArgb() = argb Then
                found.Add(Sub(col)
                              grid.ColumnHeadersDefaultCellStyle.BackColor = col
                              grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = col
                          End Sub)
            End If
        End If
        For Each child As Control In c.Controls
            Collect(child, argb, found)
        Next
    End Sub

    ''' <summary>Paints every place found by FindThemed in the given colour.</summary>
    Public Sub ApplyTheme(targets As List(Of Action(Of Color)), col As Color)
        For Each paint As Action(Of Color) In targets
            paint(col)
        Next
    End Sub

    ''' <summary>Shows the logo in a PictureBox, or the "+" symbol label when there is no logo.</summary>
    Public Sub ShowLogo(box As PictureBox, fallback As Control)
        Dim img As Image = Logo()
        box.Image = img
        box.Visible = img IsNot Nothing
        If fallback IsNot Nothing Then fallback.Visible = img Is Nothing
    End Sub

End Module
