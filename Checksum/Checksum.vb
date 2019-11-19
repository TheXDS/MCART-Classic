Public Class Info
    Inherits MCARuntime.AppServices.PluginIdentification
    Public Sub New()
        ReleaseType = MCARuntime.AppServices.AppInfo.ReleaseType.Stub
        ExtDescription = My.Resources.Description
        Unstable = False
        FileName = Reflection.Assembly.GetExecutingAssembly.Location
    End Sub
End Class
Public Class CRC32
    Inherits MCARuntime.Security.Checksum.ChecksumPlugin
    Private CRC32_Tab(255) As UInteger
    Private CRCCheck As UInteger
    Public Overrides Sub ParseArguments(ParamArray Args() As Object)
        Dim Seed As Long = 3988292384
        If Args.Count = 1 Then
            If TypeOf (Args(0)) Is UInteger Then
                Seed = Args(0)
            Else
                Throw New MCARuntime.Exceptions.InvalidArgumentException("Seed")
            End If
        ElseIf Args.Count <> 0 Then
            Throw New MCARuntime.Exceptions.TooManyArgumentsException()
        End If
        Dim CRC As UInteger
        For i As Short = 0 To 255
            CRC = i
            For j As Byte = 0 To 7
                If (CRC And &H1) = &H1 Then
                    CRC = (CRC >> 1) Xor Seed
                Else
                    CRC = CRC >> 1
                End If
            Next
            CRC32_Tab(i) = CRC
        Next
        Initialized = True
    End Sub
    Sub New()
        ParseArguments()
    End Sub
    Public Overrides Function Calculate(X As String) As String
        If Not Initialized Then Throw New MCARuntime.Exceptions.PluginNotInitializedException()
        Dim CRC As Long = 4294967295
        Dim b() As Byte = Text.Encoding.ASCII.GetBytes(X)
        For Each BT As Byte In b
            CRC = (CRC >> 8) Xor CRC32_Tab((CRC And &HFF) Xor Convert.ToUInt32(BT))
        Next
        CRC = Not (CRC)
        Return Chr(CRC >> 24) & Chr((CRC >> 16) And &HFF) & Chr((CRC >> 8) And &HFF) & Chr(CRC And &HFF)
    End Function
    Protected Overrides Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            Erase CRC32_Tab
        End If
        disposedValue = True
    End Sub
End Class
Public Class ChecksumXOR8
    Inherits MCARuntime.Security.Checksum.ChecksumPlugin
    Public Overrides Function Calculate(X As String) As String
        Dim CS As Byte = &H0
        Dim b() As Byte = Text.Encoding.ASCII.GetBytes(X)
        For Each BT As Byte In b
            CS = CS Xor BT
        Next
        Return Chr(CS)
    End Function
End Class