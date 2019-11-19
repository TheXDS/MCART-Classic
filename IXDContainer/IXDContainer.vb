Public Class Info
    Inherits MCARuntime.AppServices.PluginIdentification
    Public Sub New()
        ReleaseType = MCARuntime.AppServices.AppInfo.ReleaseType.Stub
        ExtDescription = My.Resources.Description
        Unstable = False
        FileName = Reflection.Assembly.GetExecutingAssembly.Location
    End Sub
End Class

''' <summary>Define el estandar IXD para contenedor de datos indexados.</summary>
''' <remarks>
''' IXDFormat está diseñado para almacenar de forma indexada cualquier tipo de
''' contenido. Además soporta una capacidad máxima de 4 GiB por archivo y hasta
''' 4294967295 registros. Por el momento, el generador IXD permite establecer una
''' firma Tag.
''' </remarks>
Public Class IXDContainer
    Inherits MCARuntime.FileFormats.ContainerPlugin
    Private IXDName As String, Typ As Types, TagData As String
    Private Signature As Byte() = {73, 88, 68} ' Número magico: 'IXD'
    Public Enum Types As Byte
        Null = 0

        IXD32 = 1
        ' Campo  | Bytes | Offset | Contenido
        ' -------+-------+--------+----------
        ' Header | 4     | 0x0000 | "IXD" + Chr(1)
        ' Nents  | 4     | 0x0004 | cantidad de entradas
        ' off1   | 4     | 0x0008 | Offset de elemento 1
        ' TagDta | ---   | 0x0012 | Información Tag
        ' off2   | 4     | 0x---- | Offset de elemento 2
        ' idxlen | 1     | 0x---2 | Longitud de indice
        ' data   | ---   | 0x---n | Datos asociados (desde aquí, hasta encontrarse en el siguiente offset)
        ' La cuenta de offset de elemento 1  ayuda a especificar la longitud de la información Tag.
        ' En caso de no haber data registrada ([0x0004], [0x0005], [0x0006] y [0x0007] = 0), los siguientes 4 bytes deben ser = 0

        'DataBlock = 2
        'SplitFiles = 3
        'FATLike = 4
    End Enum
    Public Overrides Function ParseAndCheck() As Boolean
        If Not Initialized Then Throw New MCARuntime.Exceptions.PluginNotInitializedException()
        Try
            Reader.BaseStream.Position = 0
            If Reader.ReadBytes(3) Is Signature Then
                'más checks a agregar...
                Return True
            End If
        Catch : End Try
        Return False

    End Function
    Public Overrides Function InternalVersion() As Byte
        If Not Initialized Then Throw New MCARuntime.Exceptions.PluginNotInitializedException()
        Reader.BaseStream.Position = 3
        Return Reader.ReadByte
    End Function
    Sub New()
        Initialized = False
    End Sub
    Public Overrides Sub CreateFile()
        If Not Initialized Then Throw New MCARuntime.Exceptions.PluginNotInitializedException()
        Writer = New IO.BinaryWriter(New IO.FileStream(IXDName, IO.FileMode.OpenOrCreate))
        Writer.Write(Signature)
        Writer.Write(Typ)
        Select Case Typ
            Case Types.IXD32
                Writer.Write(Chr(0) & Chr(0) & Chr(0) & Chr(0) & Chr(0) & Chr(0) & Chr(0) & Chr(0) & TagData)
            Case Else
                Writer.Close()
                Writer.Dispose()
                Writer = Nothing
                FileIO.FileSystem.DeleteFile(IXDName)
                Throw New MCARuntime.Exceptions.NotValidIXDException
        End Select
        Writer.Close()
        Writer.Dispose()
        Writer = Nothing
    End Sub
    Public Overrides Sub Add(ByRef Index As String, ByRef Data As String)
        If Not Initialized Then Throw New MCARuntime.Exceptions.PluginNotInitializedException()
        'ContainerFile.Open(MCARuntime.AppServices.FileManager.FileMode.Binary)
        'ContainerFile.Skip(4)
        'Select Case Typ
        '    Case Types.IXD32
        '        Dim DataCount As UInteger = (Asc(ContainerFile.Read(1)) * 16777216) + (Asc(ContainerFile.Read(1)) * 65536) + (Asc(ContainerFile.Read(1)) * 256) + Asc(ContainerFile.Read(1))
        '        For j As UInteger = 1 To DataCount
        '            ContainerFile.MoveTo((Asc(ContainerFile.Read(1)) * 16777216) + (Asc(ContainerFile.Read(1)) * 65536) + (Asc(ContainerFile.Read(1)) * 256) + Asc(ContainerFile.Read(1)))
        '        Next
        '        ContainerFile.ClearBuffer()
        '        ContainerFile.Write(Chr(ContainerFile.Length \ 16777216) & Chr((ContainerFile.Length Mod 16777216) \ 65536) &
        '                Chr(((ContainerFile.Length Mod 16777216) Mod 65536) \ 256) &
        '                Chr(((ContainerFile.Length Mod 16777216) Mod 65536) Mod 256))
        '        ContainerFile.MoveToEnd()
        '        ContainerFile.ClearBuffer()
        '        ContainerFile.Write(Chr(0) & Chr(0) & Chr(0) & Chr(0) & Chr(Index.Length))
        '        ContainerFile.Write(Data)
        '        ContainerFile.MoveTo(4)
        '        Dim c As UInteger = DataCount + 1
        '        ContainerFile.Write(Chr(c \ 16777216) & Chr((c Mod 16777216) \ 65536) &
        '                Chr(((c Mod 16777216) Mod 65536) \ 256) &
        '                Chr(((c Mod 16777216) Mod 65536) Mod 256))
        '        ContainerFile.Close()
        '    Case Else
        '        ContainerFile.Close()
        '        Throw New MCARuntime.Exceptions.NotValidIXDException(IXDName)
        'End Select
    End Sub
    Public Overrides Function GetData(ByRef Index As String) As String
        If Not Initialized Then Throw New MCARuntime.Exceptions.PluginNotInitializedException()
        'ContainerFile.Open(MCARuntime.AppServices.FileManager.FileMode.Binary)
        'ContainerFile.Skip(4)
        'Select Case Typ
        '    Case Types.IXD32
        '        Dim DataCount As UInteger = (Asc(ContainerFile.Read(1)) * 16777216) + (Asc(ContainerFile.Read(1)) * 65536) + (Asc(ContainerFile.Read(1)) * 256) + Asc(ContainerFile.Read(1))
        '        Dim NextIndex As UInteger = (Asc(ContainerFile.Read(1)) * 16777216) + (Asc(ContainerFile.Read(1)) * 65536) + (Asc(ContainerFile.Read(1)) * 256) + Asc(ContainerFile.Read(1))
        '        For J As UInteger = 1 To DataCount
        '            ContainerFile.MoveTo(NextIndex)
        '            NextIndex = (Asc(ContainerFile.Read(1)) * 16777216) + (Asc(ContainerFile.Read(1)) * 65536) + (Asc(ContainerFile.Read(1)) * 256) + Asc(ContainerFile.Read(1))
        '            Dim IndexLength As Byte = Asc(ContainerFile.Read(1))
        '            If Index = ContainerFile.Read(IndexLength) Then
        '                Dim r As String = ContainerFile.ReadTo(NextIndex)
        '                ContainerFile.Close()
        '                Return r
        '            End If
        '        Next
        '    Case Else
        '        ContainerFile.Close()
        '        Throw New MCARuntime.Exceptions.NotValidIXDException(IXDName)
        'End Select
        'ContainerFile.Close()
        Return Nothing
        'Throw New MCARuntime.Exceptions.DataNotFoundException()
    End Function
    Public Overrides Sub ChangeData(ByRef Index As String, ByRef Data As String)
        If Not Initialized Then Throw New MCARuntime.Exceptions.PluginNotInitializedException()
        'ContainerFile.Open(MCARuntime.AppServices.FileManager.FileMode.Binary)
        'ContainerFile.Skip(4)
        'Select Case Typ
        '    Case Types.IXD32
        '        Dim DataCount As UInteger = (Asc(ContainerFile.Read(1)) * 16777216) + (Asc(ContainerFile.Read(1)) * 65536) + (Asc(ContainerFile.Read(1)) * 256) + Asc(ContainerFile.Read(1))
        '        Dim NextIndex As UInteger = (Asc(ContainerFile.Read(1)) * 16777216) + (Asc(ContainerFile.Read(1)) * 65536) + (Asc(ContainerFile.Read(1)) * 256) + Asc(ContainerFile.Read(1))
        '        Dim datalen As Byte, nxtidpos As UInteger
        '        For J As UInteger = 1 To DataCount
        '            ContainerFile.MoveTo(NextIndex)
        '            nxtidpos = NextIndex
        '            NextIndex = (Asc(ContainerFile.Read(1)) * 16777216) + (Asc(ContainerFile.Read(1)) * 65536) + (Asc(ContainerFile.Read(1)) * 256) + Asc(ContainerFile.Read(1))
        '            datalen = Asc(ContainerFile.Read(1))
        '            If Index = ContainerFile.Read(datalen) Then
        '                Using z As New MCARuntime.AppServices.FileManager.File()
        '                    z.Open(MCARuntime.AppServices.FileManager.FileMode.Binary)
        '                    z.Write(ContainerFile.Read(0, ContainerFile.Position))
        '                    z.Write(Data & ContainerFile.ReadTo(NextIndex))
        '                    Do
        '                        z.PushPOS(nxtidpos)
        '                        z.Write(Chr(z.Length \ 16777216) & Chr((z.Length Mod 16777216) \ 65536) &
        '                                Chr(((z.Length Mod 16777216) Mod 65536) \ 256) &
        '                                Chr(((z.Length Mod 16777216) Mod 65536) Mod 256))
        '                        z.PopPOS()
        '                        nxtidpos = z.Position
        '                        NextIndex = (Asc(ContainerFile.Read(1)) * 16777216) + (Asc(ContainerFile.Read(1)) * 65536) + (Asc(ContainerFile.Read(1)) * 256) + Asc(ContainerFile.Read(1))
        '                        If NextIndex = 0 Then
        '                            z.Write(Chr(0) & Chr(0) & Chr(0) & Chr(0) & ContainerFile.ReadToEnd)
        '                            Exit Do
        '                        End If
        '                        z.Write(Chr(0) & Chr(0) & Chr(0) & Chr(0) & ContainerFile.ReadTo(NextIndex))
        '                    Loop
        '                    ContainerFile.Destroy()
        '                    z.MoveTo(0)
        '                    ContainerFile.Write(z.ReadToEnd)
        '                    ContainerFile.Close()
        '                    Exit Sub
        '                End Using
        '            End If
        '        Next
        '        ContainerFile.Close()
        '        Throw New MCARuntime.Exceptions.DataNotFoundException
        '    Case Else
        '        ContainerFile.Close()
        '        Throw New MCARuntime.Exceptions.NotValidIXDException(IXDName)
        'End Select
        'ContainerFile.Close()
    End Sub
    Public Overrides Sub DeleteData(ByRef Index As String)
        If Not Initialized Then Throw New MCARuntime.Exceptions.PluginNotInitializedException()
        'ContainerFile.Open(MCARuntime.AppServices.FileManager.FileMode.Binary)
        'ContainerFile.Skip(4)
        'Select Case Typ
        '    Case Types.IXD32
        '        Dim DataCount As UInteger = (Asc(ContainerFile.Read(1)) * 16777216) + (Asc(ContainerFile.Read(1)) * 65536) + (Asc(ContainerFile.Read(1)) * 256) + Asc(ContainerFile.Read(1))
        '        Dim NextIndex As UInteger = (Asc(ContainerFile.Read(1)) * 16777216) + (Asc(ContainerFile.Read(1)) * 65536) + (Asc(ContainerFile.Read(1)) * 256) + Asc(ContainerFile.Read(1))
        '        Dim datalen As Byte, nxtidpos As UInteger
        '        For J As UInteger = 1 To DataCount
        '            ContainerFile.MoveTo(NextIndex)
        '            nxtidpos = NextIndex
        '            NextIndex = (Asc(ContainerFile.Read(1)) * 16777216) + (Asc(ContainerFile.Read(1)) * 65536) + (Asc(ContainerFile.Read(1)) * 256) + Asc(ContainerFile.Read(1))
        '            datalen = Asc(ContainerFile.Read(1))
        '            If Index = ContainerFile.Read(datalen) Then
        '                Using z As New MCARuntime.AppServices.FileManager.File()
        '                    z.Open(MCARuntime.AppServices.FileManager.FileMode.Binary)
        '                    z.Write(ContainerFile.Read(0, nxtidpos))
        '                    ContainerFile.MoveTo(NextIndex)
        '                    Do
        '                        z.PushPOS(nxtidpos)
        '                        z.Write(Chr(z.Length \ 16777216) & Chr((z.Length Mod 16777216) \ 65536) &
        '                                Chr(((z.Length Mod 16777216) Mod 65536) \ 256) &
        '                                Chr(((z.Length Mod 16777216) Mod 65536) Mod 256))
        '                        z.PopPOS()
        '                        nxtidpos = z.Position
        '                        NextIndex = (Asc(ContainerFile.Read(1)) * 16777216) + (Asc(ContainerFile.Read(1)) * 65536) + (Asc(ContainerFile.Read(1)) * 256) + Asc(ContainerFile.Read(1))
        '                        If NextIndex = 0 Then
        '                            z.Write(Chr(0) & Chr(0) & Chr(0) & Chr(0) & ContainerFile.ReadToEnd)
        '                            Exit Do
        '                        End If
        '                        z.Write(Chr(0) & Chr(0) & Chr(0) & Chr(0) & ContainerFile.ReadTo(NextIndex))
        '                    Loop
        '                    z.MoveTo(4)
        '                    Dim c As UInteger = DataCount - 1
        '                    z.Write(Chr(c \ 16777216) & Chr((c Mod 16777216) \ 65536) &
        '                            Chr(((c Mod 16777216) Mod 65536) \ 256) &
        '                            Chr(((c Mod 16777216) Mod 65536) Mod 256))
        '                    ContainerFile.Destroy()
        '                    z.MoveTo(0)
        '                    ContainerFile.Write(z.ReadToEnd)
        '                    ContainerFile.Close()
        '                    Exit Sub
        '                End Using
        '            End If
        '        Next
        '        ContainerFile.Close()
        '        Throw New MCARuntime.Exceptions.DataNotFoundException
        '    Case Else
        '        ContainerFile.Close()
        '        Throw New MCARuntime.Exceptions.NotValidIXDException(IXDName)
        'End Select
        'ContainerFile.Close()
    End Sub
    Protected Overrides Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            IXDName = Nothing
            Typ = Types.Null
            TagData = Nothing

            Initialized = False
            disposedValue = True
        End If
    End Sub
    Public Overrides Function GetTag() As Object
        If Not Initialized Then Throw New MCARuntime.Exceptions.PluginNotInitializedException()

        Reader.BaseStream.Position = 3
        Return Reader.ReadByte

        Return TagData
    End Function
    Public Overrides Function RegCount() As UInteger
        If Not Initialized Then Throw New MCARuntime.Exceptions.PluginNotInitializedException()
        Reader.BaseStream.Position = 4
        Return (Reader.ReadByte() * 16777216) + (Reader.ReadByte() * 65536) + (Reader.ReadByte() * 256) + Reader.ReadByte()
    End Function
    Public Overrides Sub ChangeTag(ByRef Tag As String)
        If Not Initialized Then Throw New MCARuntime.Exceptions.PluginNotInitializedException()
        'ContainerFile.Open(MCARuntime.AppServices.FileManager.FileMode.Binary)
        'ContainerFile.Skip(4)
        'Select Case Typ
        '    Case Types.IXD32
        '        Using x As New MCARuntime.AppServices.FileManager.File()
        '            ContainerFile.Open()
        '            x.Open()
        '            x.Write(ContainerFile.Read(8))
        '            Dim NextIndex As UInteger
        '            Dim nxtidpos As UInteger = (Asc(ContainerFile.Read(1)) * 16777216) + (Asc(ContainerFile.Read(1)) * 65536) + (Asc(ContainerFile.Read(1)) * 256) + Asc(ContainerFile.Read(1))
        '            If nxtidpos = 0 Then
        '                x.Write(Tag)
        '            Else
        '                x.Write(ContainerFile.ReadTo(nxtidpos))
        '                Do
        '                    x.PushPOS(nxtidpos)
        '                    x.Write(Chr(x.Length \ 16777216) & Chr((x.Length Mod 16777216) \ 65536) &
        '                            Chr(((x.Length Mod 16777216) Mod 65536) \ 256) &
        '                            Chr(((x.Length Mod 16777216) Mod 65536) Mod 256))
        '                    x.PopPOS()
        '                    nxtidpos = x.Position
        '                    NextIndex = (Asc(ContainerFile.Read(1)) * 16777216) + (Asc(ContainerFile.Read(1)) * 65536) + (Asc(ContainerFile.Read(1)) * 256) + Asc(ContainerFile.Read(1))
        '                    If NextIndex = 0 Then
        '                        x.Write(Chr(0) & Chr(0) & Chr(0) & Chr(0) & ContainerFile.ReadToEnd)
        '                        Exit Do
        '                    End If
        '                    x.Write(Chr(0) & Chr(0) & Chr(0) & Chr(0) & ContainerFile.ReadTo(NextIndex))
        '                Loop
        '            End If
        '            ContainerFile.Destroy()
        '            x.MoveTo(0)
        '            ContainerFile.Write(x.ReadToEnd)
        '            ContainerFile.Close()
        '            TagData = Tag
        '        End Using
        '    Case Else
        '        ContainerFile.Close()
        '        Throw New MCARuntime.Exceptions.NotValidIXDException(IXDName)
        'End Select
    End Sub
    Public Overrides Sub ParseArguments(ParamArray Args() As Object)
        Select Case Args.Count
            Case 0
                Throw New MCARuntime.Exceptions.MissingArgumentException("IXDName")
            Case 1
                If Not (TypeOf Args(0) Is String) Then
                    Throw New MCARuntime.Exceptions.InvalidArgumentException("IXDName")
                End If
                IXDName = Args(0).ToString
                If FileIO.FileSystem.FileExists(IXDName) Then
                    Throw New MCARuntime.Exceptions.FileNotFoundException(IXDName)
                End If
                Initialized = ParseAndCheck()
                If Not Initialized Then Throw New MCARuntime.Exceptions.NotValidIXDException(IXDName)
            Case 3
                If Not (TypeOf Args(0) Is String) Then
                    Throw New MCARuntime.Exceptions.InvalidArgumentException("IXDName")
                End If
                If Not (TypeOf Args(1) Is String) Then
                    Throw New MCARuntime.Exceptions.InvalidArgumentException("TagData")
                End If
                If Not (TypeOf Args(2) Is Byte) Then
                    Throw New MCARuntime.Exceptions.InvalidArgumentException("InternalVersion")
                End If
                Try
                    IXDName = Args(0).ToString
                    TagData = Args(1).ToString
                    Typ = Args(2)
                    Initialized = True
                    CreateFile()
                Catch ex As Exception
                    Throw ex
                End Try
            Case Else
                Throw New MCARuntime.Exceptions.TooManyArgumentsException
        End Select
    End Sub
    Public Overrides Function FileName() As String
        Return IXDName
    End Function
End Class

Public Class IXDFSDriver
    Private Header As Byte() = {73, 88, 68, 70, 83} 'IXDFS
    Private fstr As MCARuntime.AppServices.FileStreamEx
    Sub New(ByRef fstream As IO.FileStream)
        fstr = fstream
    End Sub
    Sub New(ByVal Fname As String)
        fstr = New MCARuntime.AppServices.FileStreamEx(Fname, IO.FileMode.OpenOrCreate)
        If fstr.Length = 0 Then
            fstr.WriteSeveralBytes(Header)
            'write the header.
        End If
    End Sub
End Class