<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AboutRTForm
    Inherits System.Windows.Forms.Form

    'Form reemplaza a Dispose para limpiar la lista de componentes.
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

    'Requerido por el Diseñador de Windows Forms
    Private components As System.ComponentModel.IContainer

    'NOTA: el Diseñador de Windows Forms necesita el siguiente procedimiento
    'Se puede modificar usando el Diseñador de Windows Forms.  
    'No lo modifique con el editor de código.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.InfoTabs = New System.Windows.Forms.TabControl()
        Me.AsmInfo = New System.Windows.Forms.TabPage()
        Me.Credits = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Legal = New System.Windows.Forms.TabPage()
        Me.LegalNote = New System.Windows.Forms.TextBox()
        Me.TbPChanges = New System.Windows.Forms.TabPage()
        Me.TxtChanges = New System.Windows.Forms.TextBox()
        Me.DbgInfo = New System.Windows.Forms.TabPage()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.PluginInfo = New System.Windows.Forms.TabPage()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.ListBox1 = New System.Windows.Forms.ListBox()
        Me.Button1 = New System.Windows.Forms.Button()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.InfoTabs.SuspendLayout()
        Me.AsmInfo.SuspendLayout()
        Me.Legal.SuspendLayout()
        Me.TbPChanges.SuspendLayout()
        Me.DbgInfo.SuspendLayout()
        Me.PluginInfo.SuspendLayout()
        Me.SuspendLayout()
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = Global.MCARuntime.My.Resources.Resources.MCARuntime
        Me.PictureBox1.Location = New System.Drawing.Point(12, 12)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(260, 65)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(278, 31)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(100, 29)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Short Version String"
        Me.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'InfoTabs
        '
        Me.InfoTabs.Controls.Add(Me.AsmInfo)
        Me.InfoTabs.Controls.Add(Me.Legal)
        Me.InfoTabs.Controls.Add(Me.TbPChanges)
        Me.InfoTabs.Controls.Add(Me.DbgInfo)
        Me.InfoTabs.Controls.Add(Me.PluginInfo)
        Me.InfoTabs.Location = New System.Drawing.Point(12, 83)
        Me.InfoTabs.Name = "InfoTabs"
        Me.InfoTabs.SelectedIndex = 0
        Me.InfoTabs.Size = New System.Drawing.Size(366, 177)
        Me.InfoTabs.TabIndex = 2
        '
        'AsmInfo
        '
        Me.AsmInfo.Controls.Add(Me.Credits)
        Me.AsmInfo.Controls.Add(Me.Label2)
        Me.AsmInfo.Location = New System.Drawing.Point(4, 22)
        Me.AsmInfo.Name = "AsmInfo"
        Me.AsmInfo.Padding = New System.Windows.Forms.Padding(3)
        Me.AsmInfo.Size = New System.Drawing.Size(358, 151)
        Me.AsmInfo.TabIndex = 1
        Me.AsmInfo.Text = "AsmInfo"
        Me.AsmInfo.UseVisualStyleBackColor = True
        '
        'Credits
        '
        Me.Credits.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Credits.Location = New System.Drawing.Point(6, 32)
        Me.Credits.Multiline = True
        Me.Credits.Name = "Credits"
        Me.Credits.ReadOnly = True
        Me.Credits.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.Credits.Size = New System.Drawing.Size(346, 113)
        Me.Credits.TabIndex = 1
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(9, 3)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(346, 26)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Version String"
        '
        'Legal
        '
        Me.Legal.Controls.Add(Me.LegalNote)
        Me.Legal.Location = New System.Drawing.Point(4, 22)
        Me.Legal.Name = "Legal"
        Me.Legal.Padding = New System.Windows.Forms.Padding(3)
        Me.Legal.Size = New System.Drawing.Size(358, 151)
        Me.Legal.TabIndex = 0
        Me.Legal.Text = "Legal"
        Me.Legal.UseVisualStyleBackColor = True
        '
        'LegalNote
        '
        Me.LegalNote.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LegalNote.Location = New System.Drawing.Point(6, 6)
        Me.LegalNote.Multiline = True
        Me.LegalNote.Name = "LegalNote"
        Me.LegalNote.ReadOnly = True
        Me.LegalNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.LegalNote.Size = New System.Drawing.Size(346, 139)
        Me.LegalNote.TabIndex = 0
        '
        'TbPChanges
        '
        Me.TbPChanges.Controls.Add(Me.TxtChanges)
        Me.TbPChanges.Location = New System.Drawing.Point(4, 22)
        Me.TbPChanges.Name = "TbPChanges"
        Me.TbPChanges.Padding = New System.Windows.Forms.Padding(3)
        Me.TbPChanges.Size = New System.Drawing.Size(358, 151)
        Me.TbPChanges.TabIndex = 3
        Me.TbPChanges.Text = "Changes"
        Me.TbPChanges.UseVisualStyleBackColor = True
        '
        'TxtChanges
        '
        Me.TxtChanges.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TxtChanges.Location = New System.Drawing.Point(6, 6)
        Me.TxtChanges.Multiline = True
        Me.TxtChanges.Name = "TxtChanges"
        Me.TxtChanges.ReadOnly = True
        Me.TxtChanges.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TxtChanges.Size = New System.Drawing.Size(346, 139)
        Me.TxtChanges.TabIndex = 1
        '
        'DbgInfo
        '
        Me.DbgInfo.Controls.Add(Me.Label10)
        Me.DbgInfo.Controls.Add(Me.Label9)
        Me.DbgInfo.Controls.Add(Me.Label8)
        Me.DbgInfo.Controls.Add(Me.Label7)
        Me.DbgInfo.Controls.Add(Me.Label6)
        Me.DbgInfo.Controls.Add(Me.Label5)
        Me.DbgInfo.Controls.Add(Me.Label4)
        Me.DbgInfo.Controls.Add(Me.Button2)
        Me.DbgInfo.Controls.Add(Me.Label3)
        Me.DbgInfo.Location = New System.Drawing.Point(4, 22)
        Me.DbgInfo.Name = "DbgInfo"
        Me.DbgInfo.Padding = New System.Windows.Forms.Padding(3)
        Me.DbgInfo.Size = New System.Drawing.Size(358, 151)
        Me.DbgInfo.TabIndex = 2
        Me.DbgInfo.Text = "DbgInfo"
        Me.DbgInfo.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(90, 64)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(114, 13)
        Me.Label10.TabIndex = 8
        Me.Label10.Text = "Please, press 'Refresh'"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(36, 64)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(48, 13)
        Me.Label9.TabIndex = 7
        Me.Label9.Text = "Logging:"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(39, 3)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(114, 13)
        Me.Label8.TabIndex = 6
        Me.Label8.Text = "Please, press 'Refresh'"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(8, 3)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(25, 13)
        Me.Label7.TabIndex = 5
        Me.Label7.Text = "OS:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(90, 25)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(114, 13)
        Me.Label6.TabIndex = 4
        Me.Label6.Text = "Please, press 'Refresh'"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(8, 25)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(76, 13)
        Me.Label5.TabIndex = 3
        Me.Label5.Text = "Physical RAM:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(90, 38)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(114, 13)
        Me.Label4.TabIndex = 2
        Me.Label4.Text = "Please, press 'Refresh'"
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(292, 6)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(60, 23)
        Me.Button2.TabIndex = 1
        Me.Button2.Text = "Refresh"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(17, 38)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(67, 13)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "Working set:"
        '
        'PluginInfo
        '
        Me.PluginInfo.Controls.Add(Me.Button3)
        Me.PluginInfo.Controls.Add(Me.TextBox3)
        Me.PluginInfo.Controls.Add(Me.Label13)
        Me.PluginInfo.Controls.Add(Me.TextBox1)
        Me.PluginInfo.Controls.Add(Me.ListBox1)
        Me.PluginInfo.Location = New System.Drawing.Point(4, 22)
        Me.PluginInfo.Name = "PluginInfo"
        Me.PluginInfo.Padding = New System.Windows.Forms.Padding(3)
        Me.PluginInfo.Size = New System.Drawing.Size(358, 151)
        Me.PluginInfo.TabIndex = 4
        Me.PluginInfo.Text = "PluginInfo"
        Me.PluginInfo.UseVisualStyleBackColor = True
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(277, 122)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(75, 23)
        Me.Button3.TabIndex = 8
        Me.Button3.Text = "More..."
        Me.Button3.UseVisualStyleBackColor = True
        '
        'TextBox3
        '
        Me.TextBox3.Location = New System.Drawing.Point(103, 45)
        Me.TextBox3.Multiline = True
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.ReadOnly = True
        Me.TextBox3.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextBox3.Size = New System.Drawing.Size(249, 71)
        Me.TextBox3.TabIndex = 6
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(103, 29)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(72, 13)
        Me.Label13.TabIndex = 5
        Me.Label13.Text = "Plugin details:"
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(106, 6)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ReadOnly = True
        Me.TextBox1.Size = New System.Drawing.Size(246, 20)
        Me.TextBox1.TabIndex = 2
        Me.TextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'ListBox1
        '
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.Location = New System.Drawing.Point(6, 6)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(91, 134)
        Me.ListBox1.TabIndex = 0
        '
        'Button1
        '
        Me.Button1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button1.Location = New System.Drawing.Point(303, 266)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 3
        Me.Button1.Text = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'AboutRTForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(390, 301)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.InfoTabs)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.PictureBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "AboutRTForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Form1"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.InfoTabs.ResumeLayout(False)
        Me.AsmInfo.ResumeLayout(False)
        Me.AsmInfo.PerformLayout()
        Me.Legal.ResumeLayout(False)
        Me.Legal.PerformLayout()
        Me.TbPChanges.ResumeLayout(False)
        Me.TbPChanges.PerformLayout()
        Me.DbgInfo.ResumeLayout(False)
        Me.DbgInfo.PerformLayout()
        Me.PluginInfo.ResumeLayout(False)
        Me.PluginInfo.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents InfoTabs As System.Windows.Forms.TabControl
    Friend WithEvents Legal As System.Windows.Forms.TabPage
    Friend WithEvents AsmInfo As System.Windows.Forms.TabPage
    Friend WithEvents LegalNote As System.Windows.Forms.TextBox
    Friend WithEvents DbgInfo As System.Windows.Forms.TabPage
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Credits As System.Windows.Forms.TextBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents TbPChanges As System.Windows.Forms.TabPage
    Friend WithEvents TxtChanges As System.Windows.Forms.TextBox
    Friend WithEvents PluginInfo As System.Windows.Forms.TabPage
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox
    Friend WithEvents TextBox3 As System.Windows.Forms.TextBox
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Button3 As System.Windows.Forms.Button
End Class
