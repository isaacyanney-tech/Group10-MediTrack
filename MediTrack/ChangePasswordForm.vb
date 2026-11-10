' ============================================================================
'  ChangePasswordForm.vb  -  File > Change My Password
'  Lets the signed-in user change their own password. The old password must
'  be correct, and the new one is saved as a SHA-256 hash.
' ============================================================================
Imports System.Windows.Forms

Public Class ChangePasswordForm

    Private Sub chkCpShow_CheckedChanged(sender As Object, e As EventArgs) Handles chkCpShow.CheckedChanged
        For Each box As TextBox In New TextBox() {txtCpOld, txtCpNew, txtCpConfirm}
            box.UseSystemPasswordChar = Not chkCpShow.Checked
        Next
    End Sub

    Private Sub btnCpSave_Click(sender As Object, e As EventArgs) Handles btnCpSave.Click
        lblCpError.Text = ""
        If txtCpOld.Text = "" Then
            ShowProblem("Please type your current password.", txtCpOld)
            Return
        End If
        If txtCpNew.Text.Length < DataStore.MinimumPasswordLength Then
            ShowProblem("The new password must be at least " & DataStore.MinimumPasswordLength.ToString() & " characters.", txtCpNew)
            Return
        End If
        If txtCpNew.Text <> txtCpConfirm.Text Then
            ShowProblem("The two new passwords do not match.", txtCpConfirm)
            Return
        End If
        Try
            If Not DataStore.ChangeOwnPassword(DataStore.CurrentUsername, txtCpOld.Text, txtCpNew.Text) Then
                ShowProblem("Your current password is wrong.", txtCpOld)
                Return
            End If
        Catch ex As Exception
            MessageBox.Show("Could not change the password." & vbCrLf & vbCrLf & ex.Message, "Change Password",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try
        MessageBox.Show("Your password was changed. Use the new one next time you sign in.", "Change Password",
                        MessageBoxButtons.OK, MessageBoxIcon.Information)
        Me.Close()
    End Sub

    Private Sub ShowProblem(message As String, box As TextBox)
        lblCpError.Text = message
        box.SelectAll()
        box.Focus()
    End Sub

    Private Sub btnCpCancel_Click(sender As Object, e As EventArgs) Handles btnCpCancel.Click
        Me.Close()
    End Sub

End Class
