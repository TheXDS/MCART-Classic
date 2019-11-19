Public Class Info
    Inherits MCARuntime.AppServices.PluginIdentification
    Public Sub New()
        ReleaseType = MCARuntime.AppServices.AppInfo.ReleaseType.Release
        ExtDescription = My.Resources.Description
        Unstable = False
        FileName = Reflection.Assembly.GetExecutingAssembly.Location
    End Sub
End Class

''' <summary>Permite realizar operaciones de encriptado, utilizando el algoritmo AES</summary>
''' <remarks>
''' Al inicializar Crypt, no es posible cambiar la contraseña ni
''' el IV. Esto, por razones de seguridad.
''' </remarks>
Public Class AESCrypter
    Inherits MCARuntime.Security.Encryption.CrypterPlugin
    Private crypt As Security.Cryptography.AesManaged
    ''' <summary>
    ''' Decripta una cadena
    ''' </summary>
    ''' <param name="Str">Cadena a desencriptar.</param>
    ''' <returns>Objeto tipo String desencriptado de la cadena de entrada.</returns>
    ''' <remarks>
    ''' Si la cadena es nula, se devolverá Nothing.
    ''' </remarks>
    ''' <exception cref="MCARuntime.Exceptions.InvalidPasswordException">
    ''' Se devuelve esta excepción si la cadena no se puede desencriptar.
    ''' Esto ocurre si la contraseña, IV o la cadena de entrada son incorrectos.
    '''</exception>
    Public Overrides Function Decrypt(Str As String) As String
        If Not Initialized Then Throw New MCARuntime.Exceptions.PluginNotInitializedException()
        If Str = "" Then Return Nothing
        Dim decryptor As Security.Cryptography.ICryptoTransform = crypt.CreateDecryptor(crypt.Key, crypt.IV)
        Using msDecrypt As New IO.MemoryStream(Text.Encoding.ASCII.GetBytes(Str))
            Using csDecrypt As New Security.Cryptography.CryptoStream(msDecrypt, decryptor, Security.Cryptography.CryptoStreamMode.Read)
                Using srDecrypt As New IO.StreamReader(csDecrypt)
                    Try
                        Return srDecrypt.ReadToEnd()
                    Catch ex As Exception
                        Throw New MCARuntime.Exceptions.InvalidPasswordException
                    End Try
                End Using
            End Using
        End Using
    End Function
    Protected Overloads Overrides Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                If Not IsNothing(crypt) Then crypt.Dispose()
            End If
            Initialized = False
        End If
        disposedValue = True
    End Sub
    ''' <summary>
    ''' Encripta una cadena
    ''' </summary>
    ''' <param name="Str">Cadena de tipo String a encriptar</param>
    ''' <returns>Un objeto String que contiene la cadena de entrada encriptada.</returns>
    Public Overrides Function Encrypt(Str As String) As String
        If Not Initialized Then Throw New MCARuntime.Exceptions.PluginNotInitializedException()
        Dim encryptor As Security.Cryptography.ICryptoTransform = crypt.CreateEncryptor(crypt.Key, crypt.IV)
        Using msEncrypt As New IO.MemoryStream()
            Using csEncrypt As New Security.Cryptography.CryptoStream(msEncrypt, encryptor, Security.Cryptography.CryptoStreamMode.Write)
                Using swEncrypt As New IO.StreamWriter(csEncrypt)
                    swEncrypt.Write(Str)
                End Using
                Return Text.Encoding.ASCII.GetString(msEncrypt.ToArray())
            End Using
        End Using
    End Function
    ''' <summary>Inicializa una nueva instancia de AESCrypter</summary>
    ''' <param name="Args">
    ''' Argumentos.
    ''' "Key" -> Clave a utilizar
    ''' "IV" -> IV a utilizar
    ''' </param>
    ''' <remarks>
    ''' Se inicializará una nueva instancia de Crypter utilizando la
    ''' clave y el IV especificados. Éstos deben tener tamaños
    ''' válidos según el estándar AES, se recomiendan 32 bytes para
    ''' la clave, y 16 para el IV. 
    ''' Al inicializarse sin parámetros, la clave y el IV serán
    ''' generados automáticamente.
    ''' ADVERTENCIA: si no se desencripta la información al
    ''' inicializar la instancia sin parámetros, será
    ''' permanentemente perdida.
    ''' </remarks>
    Public Overrides Sub ParseArguments(ParamArray Args() As Object)
        Select Case Args.Count
            Case 0
                crypt = New Security.Cryptography.AesManaged
            Case 2
                Dim xkey As String = TryCast(Args(0), String)
                If xkey Is Nothing Then
                    Throw New ArgumentException("Key")
                End If
                Dim xiv As Byte() = Args(1)
                If xiv Is Nothing Then
                    Throw New ArgumentException("IV")
                End If
                crypt = New Security.Cryptography.AesManaged
                crypt.Key = Text.Encoding.ASCII.GetBytes(xkey.PadLeft(32, Chr(0)))
                crypt.IV = Args(1)
                Initialized = True
            Case Else
                Throw New MCARuntime.Exceptions.TooManyArgumentsException()
        End Select
    End Sub
End Class