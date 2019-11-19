Public Class Info
    Inherits MCARuntime.AppServices.PluginSupport.PluginIdentification
    Public Sub New()
        ReleaseType = MCARuntime.AppServices.AppInfo.ReleaseType.Beta
        ExtDescription = My.Resources.Description
        Unstable = False
        FileName = System.Reflection.Assembly.GetExecutingAssembly.Location
        RestrictedSource = True ' es muy peligroso tener un motor aleatorio decompilable.
    End Sub
End Class
Public Class RandomEng1
    Inherits MCARuntime.Security.Randomizations.RandomEnginePlugin
    Const m As Long = 44497
    Const a As Long = (2 ^ 29) - 1
    Const d As Long = (2 ^ 40) - 1
    Const x As Long = &H59C78719671E1C65
    Private s As Long = (2 ^ 13) - 1
    Public Overrides Function getRnd() As Double
        s = (((s * m) + a) Mod d) Xor x
        Return CDbl(s) / CDbl(d)
    End Function
    Public Overrides Function Random(Optional Top As Long = 100, Optional Bottom As Long = 0) As Long
        Return (getRnd() * (Top - Bottom)) + Bottom
    End Function
    Public Overrides Sub setSeed(seed As Long)
        If seed > -1 Then Throw New ArgumentOutOfRangeException()
        'hacer el primer paso de carga
        getRnd()
    End Sub
    Public Overrides Sub ParseArguments(ParamArray Args() As Object)
        If Args.Count = 1 AndAlso TypeOf Args(0) Is Long Then
            Try
                setSeed(Args(0))
            Catch ex As Exception
                Throw ex
            End Try
        Else
            Throw New MCARuntime.Exceptions.TooManyArgumentsException()
        End If
    End Sub
End Class
