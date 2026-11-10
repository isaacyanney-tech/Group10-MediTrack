' ============================================================================
'  ConnectDatabaseForm.vb  -  Database Settings (from the sign-in window)
'
'  Main / admin PC : "On this computer"  - the database is kept next to the program.
'  Counter PCs     : "Shared database"   - browse to the database on the main PC,
'                    e.g. \\ADMIN-PC\MediTrack\MediTrack.accdb
'  The choice is saved in database_location.txt next to the program.
' ============================================================================
Imports System.Windows.Forms

Public Class ConnectDatabaseForm

    Private Sub ConnectDatabaseForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim saved As String = Database.GetSavedLocation()
        If saved = "" Then
            rbLocalDb.Checked = True
        Else
            rbSharedDb.Checked = True
            txtDbPath.Text = saved
        End If
        UpdateBoxes()
    End Sub

    Private Sub Choice_CheckedChanged(sender As Object, e As EventArgs) Handles rbLocalDb.CheckedChanged, rbSharedDb.CheckedChanged
        UpdateBoxes()
    End Sub

    Private Sub UpdateBoxes()
        txtDbPath.Enabled = rbSharedDb.Checked
        btnBrowseDb.Enabled = rbSharedDb.Checked
        btnTestDb.Enabled = rbSharedDb.Checked
        lblDbTestResult.Text = ""
    End Sub

    Private Sub btnBrowseDb_Click(sender As Object, e As EventArgs) Handles btnBrowseDb.Click
        Using dlg As New OpenFileDialog()
            dlg.Title = "Find the MediTrack database on the main computer"
            dlg.Filter = "MediTrack database (*.accdb;*.mdb)|*.accdb;*.mdb"
            dlg.FileName = "MediTrack.accdb"
            If dlg.ShowDialog(Me) = DialogResult.OK Then
                txtDbPath.Text = dlg.FileName
                lblDbTestResult.Text = ""
            End If
        End Using
    End Sub

    Private Sub btnTestDb_Click(sender As Object, e As EventArgs) Handles btnTestDb.Click
        TestPath()
    End Sub

    ''' <summary>Tries to open the chosen file. Shows the result and returns True if it works.</summary>
    Private Function TestPath() As Boolean
        If txtDbPath.Text.Trim() = "" Then
            ShowResult("Please choose the database file first.", False)
            Return False
        End If
        Cursor = Cursors.WaitCursor
        Dim problem As String = Database.TestLocation(txtDbPath.Text.Trim())
        Cursor = Cursors.Default
        If problem = "" Then
            ShowResult("Connection works.", True)
            Return True
        End If
        ShowResult(problem, False)
        Return False
    End Function

    Private Sub ShowResult(message As String, ok As Boolean)
        lblDbTestResult.Text = message
        lblDbTestResult.ForeColor = If(ok, System.Drawing.Color.FromArgb(46, 125, 50), System.Drawing.Color.FromArgb(198, 40, 40))
    End Sub

    Private Sub btnDbSave_Click(sender As Object, e As EventArgs) Handles btnDbSave.Click
        Try
            If rbLocalDb.Checked Then
                Database.SaveLocation("")
            Else
                If Not TestPath() Then
                    MessageBox.Show(lblDbTestResult.Text, "Database Settings", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
                Database.SaveLocation(txtDbPath.Text.Trim())
            End If
        Catch ex As Exception
            MessageBox.Show("Could not save the setting." & vbCrLf & ex.Message, "Database Settings",
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnDbCancel_Click(sender As Object, e As EventArgs) Handles btnDbCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

End Class
