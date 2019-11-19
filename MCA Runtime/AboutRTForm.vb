Public Class AboutRTForm
    Private Sub About_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Button1.Text = MCARuntime.My.Resources.Strings.Close
        Me.Text = String.Format(MCARuntime.My.Resources.Strings.About, MCARuntime.RuntimeInfo.RTInfo.Name)
        Label1.Text = MCARuntime.My.Resources.Strings.Version & MCARuntime.RuntimeInfo.RTInfo.GetVerStr
        AsmInfo.Text = MCARuntime.My.Resources.Strings.AsmblyInfo
        Legal.Text = MCARuntime.My.Resources.Strings.LegalNote
        TbPChanges.Text = MCARuntime.My.Resources.Strings.changes
        If MCARuntime.RuntimeInfo.RTInfo.DebugBuild Then
            DbgInfo.Text = MCARuntime.My.Resources.Strings.Debugging
            Button2.PerformClick()
            Dim x As System.Collections.ObjectModel.ReadOnlyCollection(Of String) = MCARuntime.AppServices.PluginSupport.ListOfPlugins()
            For Each j As String In x
                ListBox1.Items.Add(MCARuntime.AppServices.FileManager.FileName(j))
            Next
        Else
            DbgInfo.Dispose()
            PluginInfo.Dispose()
        End If
        Label2.Text = MCARuntime.RuntimeInfo.VersionString(False)
        Credits.Text = My.Resources.Credits
        LegalNote.Text = My.Resources.LegalNote
        TxtChanges.Text = My.Resources.Changes
    End Sub
    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        Me.Close()
    End Sub
    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        Label4.Text = MCARuntime.RuntimeInfo.WorkingRAM & " bytes"
        Label6.Text = MCARuntime.My.Computer.Info.AvailablePhysicalMemory & " bytes"
        Label8.Text = MCARuntime.My.Computer.Info.OSFullName
        Label10.Text = "Not implemented yet."
    End Sub
    Private Sub ListBox1_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ListBox1.SelectedIndexChanged
        If ListBox1.SelectedIndex >= 0 Then
            Try
                Using x As MCARuntime.AppServices.PluginSupport.PluginInfo = MCARuntime.AppServices.PluginSupport.LoadPlugIn(ListBox1.SelectedItem, "Info")
                    TextBox1.Text = x.Info.VersionString
                    If x.Info.ExtDescription <> Nothing Then
                        TextBox3.Text = x.Info.ExtDescription
                    Else
                        TextBox3.Text = x.Info.Description
                    End If
                End Using
            Catch ex As Exception
                MsgBox(String.Format(MCARuntime.My.Resources.Strings.Err18, ListBox1.SelectedItem) & Chr(13) & ex.Message)
            End Try
        End If
    End Sub
    Private Sub Button3_Click(sender As System.Object, e As System.EventArgs) Handles Button3.Click
        If ListBox1.SelectedIndex >= 0 Then
            Try
                Using x As New PluginDetailsForm
                    x.ShowDetailsOf(MCARuntime.AppServices.PluginSupport.LoadPlugIn(ListBox1.SelectedItem, "Info"))
                    x.Dispose()
                End Using
            Catch ex As Exception
                MsgBox(String.Format(MCARuntime.My.Resources.Strings.Err18, ListBox1.SelectedItem) & Chr(13) & ex.Message)
            End Try
        End If
    End Sub
End Class