Imports System.Windows.Forms

Public Class PluginDetailsForm
    Public x As MCARuntime.AppServices.PluginSupport.PluginInfo
    Public y As ObjectModel.ReadOnlyCollection(Of String)
    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.Close()
    End Sub
    Public Sub ShowDetailsOf(ByVal Plugin As MCARuntime.AppServices.PluginSupport.PluginInfo)
        x = Plugin
        Me.Text = "About " & x.AssemblyFileName.Substring(x.AssemblyFileName.LastIndexOf("\") + 1)
        TextBox1.Text = ""
        CheckBox2.Checked = x.Info.DebugBuild
        CheckBox1.Checked = x.Info.Unstable
        CheckBox3.Checked = x.IsMultiClass
        CheckBox4.Checked = Not x.Info.IsPlugin
        Dim z As New List(Of String)(x.ContainedClasses)
        For Each j As String In z
            If Not (j.Contains(".My.")) Then ListBox1.Items.Add(j)
        Next
        z.Clear()
        y = x.Interfaces
        Me.ShowDialog()
    End Sub
    Private Sub ListBox1_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles ListBox1.SelectedIndexChanged
        If ListBox1.SelectedIndex >= 0 Then TextBox1.Text = y(ListBox1.SelectedIndex)
    End Sub
End Class
