' ============================================================================
'  SetupForm.vb  -  First-Time Setup
'
'  Shown once, when the database has no user accounts yet (a brand-new
'  pharmacy). The owner enters the pharmacy details, creates the first
'  administrator account and chooses the admin access code. There are no
'  built-in demo passwords: every account is created by the pharmacy itself.
' ============================================================================
Imports System.Drawing
Imports System.Windows.Forms

Public Class SetupForm

    Private Const MinimumLength As Integer = 6
    Private logoText As String = ""                       ' chosen logo (Base64 PNG), "" = none
    Private themeColour As Color = Branding.DefaultTheme

    Private Sub SetupForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        txtSetupName.Text = "MediTrack Pharmacy"
        chkSampleData.Checked = False
        pnlSetupSwatch.BackColor = themeColour
    End Sub

    Private Sub chkSetupShow_CheckedChanged(sender As Object, e As EventArgs) Handles chkSetupShow.CheckedChanged
        For Each box As TextBox In New TextBox() {txtSetupPass, txtSetupPass2, txtSetupCode, txtSetupCode2}
            box.UseSystemPasswordChar = Not chkSetupShow.Checked
        Next
    End Sub

    Private Sub btnSetupLogo_Click(sender As Object, e As EventArgs) Handles btnSetupLogo.Click
        Dim picked As String = Branding.PickLogo(Me)
        If picked = "" Then Return
        logoText = picked
        picSetupLogo.Image = Branding.TextToImage(logoText)
    End Sub

    Private Sub btnSetupColour_Click(sender As Object, e As EventArgs) Handles btnSetupColour.Click
        Dim chosen As Color
        If Not Branding.PickColour(Me, themeColour, chosen) Then Return
        If chosen.GetBrightness() > 0.8F Then
            MessageBox.Show("That colour is too light for white text. Please choose a darker colour.", "Colour",
                            MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        themeColour = chosen
        pnlSetupSwatch.BackColor = chosen
        pnlSetupHeader.BackColor = chosen
    End Sub

    Private Sub SetupForm_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
        txtSetupName.SelectAll()
        txtSetupName.Focus()
    End Sub

    Private Sub btnSetupFinish_Click(sender As Object, e As EventArgs) Handles btnSetupFinish.Click
        lblSetupError.Text = ""
        Dim username As String = txtSetupUser.Text.Trim().ToLower()

        If txtSetupName.Text.Trim() = "" Then
            ShowProblem("Please enter the business name.", txtSetupName) : Return
        End If
        If txtSetupFullName.Text.Trim() = "" Then
            ShowProblem("Please enter the administrator's full name.", txtSetupFullName) : Return
        End If
        If username = "" OrElse username.Contains(" ") Then
            ShowProblem("Please enter a username with no spaces, e.g. manager", txtSetupUser) : Return
        End If
        If txtSetupPass.Text.Length < MinimumLength Then
            ShowProblem("The password must be at least " & MinimumLength & " characters.", txtSetupPass) : Return
        End If
        If txtSetupPass.Text <> txtSetupPass2.Text Then
            ShowProblem("The two passwords do not match.", txtSetupPass2) : Return
        End If
        If txtSetupCode.Text.Trim().Length < MinimumLength Then
            ShowProblem("The access code must be at least " & MinimumLength & " characters.", txtSetupCode) : Return
        End If
        If txtSetupCode.Text <> txtSetupCode2.Text Then
            ShowProblem("The two access codes do not match.", txtSetupCode2) : Return
        End If
        If txtSetupCode.Text.Trim() = txtSetupPass.Text Then
            ShowProblem("Use a different access code from the administrator password.", txtSetupCode) : Return
        End If

        Try
            Cursor = Cursors.WaitCursor
            DataStore.CompleteSetup(txtSetupName.Text.Trim(), txtSetupAddress.Text.Trim(), txtSetupPhone.Text.Trim(),
                                    txtSetupFullName.Text.Trim(), username, txtSetupPass.Text,
                                    txtSetupCode.Text.Trim(), chkSampleData.Checked, logoText,
                                    If(themeColour.ToArgb() = Branding.DefaultTheme.ToArgb(), "", Branding.ColorToText(themeColour)))
        Catch ex As Exception
            Cursor = Cursors.Default
            MessageBox.Show("Setup could not be saved." & vbCrLf & vbCrLf & ex.Message, "First-Time Setup",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try
        Cursor = Cursors.Default

        MessageBox.Show("Setup is complete." & vbCrLf & vbCrLf &
                        "Sign in with the username '" & username & "' and your password." & vbCrLf &
                        "Then open Users and Settings to add accounts for your counter staff.",
                        "MediTrack is Ready", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub ShowProblem(message As String, box As Control)
        lblSetupError.Text = message
        box.Focus()
        If TypeOf box Is TextBox Then DirectCast(box, TextBox).SelectAll()
    End Sub

    Private Sub btnSetupCancel_Click(sender As Object, e As EventArgs) Handles btnSetupCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

End Class
