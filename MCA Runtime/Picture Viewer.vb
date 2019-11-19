Public Class PictView
    Private ImageArray As New List(Of String), idx As Integer = -1
    Sub New()
        InitializeComponent()
        SeeOnly = False
    End Sub
    Property SeeOnly As Boolean
        Get
            Return Not BtnAdd.Visible
        End Get
        Set(ByVal value As Boolean)
            BtnAdd.Visible = Not value
            BtnDel.Visible = Not value
        End Set
    End Property
    ReadOnly Property Count As Integer
        Get
            Return ImageArray.Count
        End Get
    End Property
    Public Function Image(ByVal index As Integer) As String
        Try
            Return ImageArray(index)
        Catch ex As Exception
            Throw ex
        End Try
    End Function
    Public Function Images() As String()
        Return ImageArray.ToArray
    End Function
    Public Sub Clear()
        ImageArray.Clear()
        idx = -1
        CmbEscala.SelectedIndex = 0
        Loadimages()
    End Sub
    Public Sub AddImage(ByVal Path As String)
        ImageArray.Add(Path)
        idx = ImageArray.LastIndexOf(Path)
        Loadimages()
    End Sub
    Public Overloads Sub RemoveImage(ByVal Item As String)
        RemoveImage(ImageArray.LastIndexOf(Item))
    End Sub
    Public Overloads Sub RemoveImage(ByVal index As Integer)
        ImageArray.RemoveAt(index)
        If idx > 0 And Count > 0 Then idx -= 1

        Loadimages()
    End Sub
    Public Overloads Sub RemoveImage() Handles BtnDel.Click
        RemoveImage(idx)
    End Sub
    Private Sub BtnPrev_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnPrev.Click
        If idx = 0 Then Exit Sub
        idx -= 1
        LoadImage()
    End Sub
    Private Sub BtnNext_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnNext.Click
        If idx >= Count - 1 Then Exit Sub
        idx += 1
        LoadImage()
    End Sub
    Private Sub Loadimages()
        If Count = 0 Then
            LblStat.Text = "No hay imágenes para mostrar."
            PictureBox1.Image = Nothing
            BtnPrev.Enabled = False
            BtnNext.Enabled = False
            BtnDel.Enabled = False
            idx = -1
        Else
            LoadImage()
            BtnPrev.Enabled = True
            BtnNext.Enabled = True
            BtnDel.Enabled = True
        End If
    End Sub
    Private Sub Picture_Viewer_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Clear()
    End Sub
    Private Sub LoadImage()
        If idx < 0 Then Throw New IndexOutOfRangeException()
        PictureBox1.Load(ImageArray(idx))
        LblStat.Text = "Imagen " & idx + 1 & " de " & Count
    End Sub
    Private Sub ShowAddDlg(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnAdd.Click
        Using PictDialog As New System.Windows.Forms.OpenFileDialog
            With PictDialog
                .FileName = ""
                .Multiselect = True
                .Title = "Abrir imágenes"
                .Filter = "Todos los archivos de imagen|*.jpg; *.png; *.bmp" _
                & "|Imagen JPEG|*.jpg" _
                & "|Imagen PNG|*.png" _
                & "|Imagen BMP|*.bmp"
                .ReadOnlyChecked = True
                .CheckFileExists = True
                .AddExtension = True
                .DefaultExt = "jpg"
                .ValidateNames = True
                .ShowDialog()
                For Each j As String In .FileNames
                    If FileIO.FileSystem.FileExists(j) Then AddImage(j)
                Next
            End With
        End Using
    End Sub
    Private Sub Cambiarescala(sender As Object, e As EventArgs) Handles CmbEscala.SelectedIndexChanged
        With PictureBox1
            Select Case CmbEscala.SelectedIndex
                Case 0
                    .SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
                Case 1
                    .SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
                Case 2
                    .SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
            End Select
            .Refresh()
        End With
    End Sub
End Class