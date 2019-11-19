Imports MCARuntime.AppServices.FileManager
Public Class Info
    Inherits MCARuntime.AppServices.PluginIdentification
    Public Sub New()
        ReleaseType = MCARuntime.AppServices.AppInfo.ReleaseType.Release
        ExtDescription = My.Resources.Description
        Unstable = False
        FileName = Reflection.Assembly.GetExecutingAssembly.Location
    End Sub
End Class

Public Class Deprecated
    Inherits MCARuntime.AppServices.GenericPlugin
    Sub New()
        Class2Exec = New Routines
    End Sub
    Private Class Routines
        Public Function ToBin(ByVal Str As String) As String
            Dim H As String = ""
            For j As UInteger = 0 To Str.Length - 1
                H &= UBin(Asc(Str.Chars(j)))
            Next
            Return H
        End Function
        ''' <summary>Convierte una cadena a su equivalente binario.</summary>
        ''' <param name="Str">La cadena a convertir.</param>
        ''' <returns>Una cadena de bytes, que equivalen a la codificación binaria de Str.</returns>
        ''' <remarks>La cadena devuelta tendrá una longitud de 8 veces la de entrada, siendo múltiplo de 8.</remarks>
        ''' <summary>Convierte un número sin signo a una cadena binaria.</summary>
        ''' <param name="X">Número a convertir, sin signo.</param>
        ''' <returns>Una cadena conformada por unos y ceros, que representan la versión binaria del número sin signo.</returns>
        Public Function UBin(ByVal X As UInteger) As String
            Dim z As String = ""
            Do Until X = 0
                z = (X Mod 2).ToString & z
                X \= 2
            Loop
            Return FixedBin(z)
        End Function
        ''' <summary>Añade dígitos a un número binario hasta que alcanza una longitud estándar.</summary>
        ''' <param name="X">Binario a convertir.</param>
        ''' <param name="Lenght">Longitud del binario de salida. </param>
        ''' <remarks>
        ''' Si Lenght se omite, o es 8, la longitud se optimiza al menor tamaño posible.
        ''' </remarks>
        Public Function FixedBin(ByVal X As String, Optional ByVal Lenght As BinSize = BinSize.Byte8, Optional ByVal sign As UShort = 0) As String
            If (Lenght Mod 8) <> 0 Then Lenght -= Lenght Mod 8
            If sign = 0 Then X = "0" & X
            Dim L As UShort = X.Length
            If L = Lenght Then Return X
            If L > Lenght And Lenght <> BinSize.Byte8 Then X = X.Substring(0, Lenght)
            If (L Mod 8) > 0 Or L <> Lenght Then
                Dim K As UShort = L Mod 8
                If K = 0 Then K = 8
                For j As UShort = 1 To (Lenght - K)
                    X = sign.ToString & X
                Next
            End If
            Return X
        End Function
        ''' <summary>Convierte una cadena binaria a un número sin signo.</summary>
        ''' <param name="B">Binario a convertir, sin signo.</param>
        ''' <returns>Un número sin signo equivalente al binario de entrada.</returns>
        Public Function UDec(ByVal B As String) As UInteger
            B = FixedBin(B, BinSize.Byte8)
            Dim L As UShort = B.Length, D As UInteger = 0
            For J As UShort = 1 To L
                D += (2 ^ (L - J)) * Val(B.Chars(J - 1))
            Next
            Return D
        End Function
        ''' <summary>Convierte un número a una cadena binaria.</summary>
        ''' <param name="X">Número a convertir, con signo.</param>
        ''' <returns>Una cadena conformada por unos y ceros, que representan la versión binaria del número con signo.</returns>
        Public Function Bin(ByVal X As Long, Optional ByVal size As BinSize = BinSize.Byte8) As String
            Dim sign As Short = 0
            Dim z As String = ""
            If X < 0 Then
                sign = 1
                X = Math.Abs(X) - 1
            End If
            If X = 0 Then Return FixedBin(sign, size, sign)
            Do Until X = 0
                z = (Math.Abs((X Mod 2) - sign)).ToString & z
                X \= 2
            Loop
            Return FixedBin(z, size, sign)
        End Function
        ''' <summary>Convierte una cadena binaria a un número con signo.</summary>
        ''' <param name="B">Binario a convertir, con signo.</param>
        ''' <returns>Un número con signo equivalente al binario de entrada.</returns>
        Public Function Dec(ByVal B As String, Optional ByVal size As BinSize = BinSize.Byte8) As Integer
            B = FixedBin(B, size)
            Dim sign As UShort = Val(B.Chars(0))
            Return UDec(B) - ((2 ^ B.Length) * sign)
        End Function
        ''' <summary>Obtiene el inverso de un binario.</summary>
        ''' <param name="X">Binario del cual obtener el inverso.</param>
        ''' <returns>Un binario que representa el inverso de X</returns>
        ''' <remarks></remarks>
        Public Function InverseBin(X As String) As String
            Return Bin(Not UDec(X))
        End Function
        Public Enum BinSize As UShort
            Byte8 = 8
            Word = 16
            Byte24 = 24
            DWord = 32
            Byte48 = 48
            Qword = 64
        End Enum

        ''' <summary>Contiene rutinas para controlar archivos DUF</summary>
        ''' <remarks>
        ''' Estructura de archivo DUF
        ''' ========== == ======= ===
        ''' DUF versión 1.1 tipo 0 Descripción:
        ''' Charset: DOS (ASCII)
        ''' Campo  | Bytes | Offset | Contenido
        ''' -------+-------+--------+----------
        ''' Header | 4     | 0x0000 | "DUF" + Chr(0)
        ''' IV     | 16    | 0x0004 | Clave IV
        ''' Key    | 32    | 0x0020 | Contraseña AES
        ''' Numus  | 1     | 0x0052 | cantidad de usuarios registrados
        ''' off1   | 2     | 0x0053 | Offset de usuario 1
        ''' TagDta | ---   | 0x0055 | Información Tag encriptada
        ''' off2   | 2     | 0x---- | Offset de usuario 2
        ''' usr1ln | 1     | 0x---2 | Longitud de nombre/clave = 4 bits c/u (8 * [x])
        ''' usr1   | ---   | 0x---n | Nombre encriptado de usuario 1
        ''' usr1pw | ---   | 0x---n | Contraseña encriptada de usuario 1
        ''' usr1dt | ---   | 0x---n | Datos adicionales (desde aquí, hasta encontrarse en el siguiente offset de usuario)
        ''' La cuenta de offset de usuario 1  ayuda a especificar la longitud de la información Tag.
        ''' En caso de no haber usuarios registrados ([0x0052] = 0), [0x0053] y [0x0054] deben ser = 0
        ''' Por el momento, el generador DUF permite establecer una firma Tag.
        ''' DUFParser usa Subrutinas en lugar de una clase por motivos de seguridad.
        '''</remarks>
        Public Class AESPluginCompatibilityLayer
            Implements IDisposable
            Private x As MCARuntime.Security.Encryption.CrypterPlugin
            Private UsablePlugin As Boolean = True
            Public Sub New()
                Try
                    x = MCARuntime.AppServices.LoadPlugIn("AESCrypter.dll", "AESCrypter")
                Catch ex As Exception
                    UsablePlugin = False
                End Try
            End Sub
            Sub New(Key As String, IV As Byte())
                Me.New()
                If UsablePlugin Then
                    x.ParseArguments(Key, IV)
                End If
            End Sub
            Public Function Encrypt(Data As String) As String
                If UsablePlugin Then
                    Return x.Encrypt(Data)
                Else
                    Return Data
                End If
            End Function
            Public Function Decrypt(Data As String) As String
                If UsablePlugin Then
                    Return x.Decrypt(Data)
                Else
                    Return Data
                End If
            End Function
            Private disposedValue As Boolean
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not disposedValue Then
                    x = Nothing
                End If
                disposedValue = True
            End Sub
            Protected Overrides Sub Finalize()
                Dispose(False)
                MyBase.Finalize()
            End Sub
            Public Sub Dispose() Implements IDisposable.Dispose
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub
        End Class
        Private Crpt As New AESPluginCompatibilityLayer("TrustN@12543156", Text.Encoding.ASCII.GetBytes("N@1kN0w5+h3P455!"))
        Public Sub CreateDUF(ByRef DUFName As String, Optional ByRef TagData As String = "", Optional ByRef DUFType As Byte = 0)
            If TagData = "" Then TagData = MCARuntime.RTInfo.Name
            Using y As New FileStreamEx(DUFName, IO.FileMode.Create)
                Select Case DUFType
                    Case 0 'DUF Versión 1.1, tipo 0 (clave almacenada en archivo)
                        Dim iv As Byte() = MCARuntime.Security.RndBytes(16)
                        Dim key As String = MCARuntime.Security.RndString(32)
                        Using tmp As New AESPluginCompatibilityLayer(key, iv)
                            y.WriteString("DUF" & Chr(0) & Text.Encoding.ASCII.GetString(iv) & key _
                                    & Chr(0) & Chr(0) & Chr(0) & tmp.Encrypt(TagData))
                        End Using
                    Case 1 'DUF Versión 1.1 tipo 1 (clave almacenada en procedimiento)
                        y.WriteString("DUF" & Chr(1) _
                                & Chr(0) & Chr(0) & Chr(0) & Crpt.Encrypt(TagData))
                    Case Else
                        Throw New MCARuntime.Exceptions.NotCompatibleDUFException(DUFType)
                End Select
            End Using
        End Sub
        Public Sub AddToDUF(ByRef DUFName As String, ByRef UserName As String, ByRef Password As String, Optional ByRef MoreData As String = "")
            With New FileStreamEx(DUFName, IO.FileMode.Open)
                Select Case .ReadString(4)
                    Case "DUF" & Chr(0) 'DUF Versión 1.1, tipo 0
                        Dim IV() As Byte, Key As String, UsrCount As Byte
                        IV = Text.Encoding.ASCII.GetBytes(.ReadString(16))
                        Key = .ReadString(32)
                        UsrCount = .ReadByte
                        For j As Byte = 1 To UsrCount
                            .Seek((.ReadByte * 256) + .ReadByte, IO.SeekOrigin.Current)
                        Next
                        .WriteByte(.Length \ 256)
                        .WriteByte(.Length Mod 256)
                        .Seek(0, IO.SeekOrigin.End)
                        Using tmp As New AESPluginCompatibilityLayer(Key, IV)
                            Dim EUsr As String = tmp.Encrypt(UserName)
                            Dim EPwd As String = tmp.Encrypt(Password)
                            .WriteString(Chr(0) & Chr(0) & Chr((EUsr.Length * 2) + (EPwd.Length \ 8)) & EUsr & EPwd)
                            If MoreData <> Nothing Then .WriteString(tmp.Encrypt(MoreData))
                        End Using
                        .Seek(52, IO.SeekOrigin.Begin)
                        .WriteByte(UsrCount + 1)
                        .Flush()
                        .Close()
                    Case "DUF" & Chr(1) 'DUF Versión 1.1, tipo 1
                        Dim UsrCount As Byte
                        UsrCount = .ReadByte
                        For j As Byte = 1 To UsrCount
                            .Seek((.ReadByte * 256) + .ReadByte, IO.SeekOrigin.Current)
                        Next
                        .WriteByte(.Length \ 256)
                        .WriteByte(.Length Mod 256)
                        .Seek(0, IO.SeekOrigin.End)
                        Dim EUsr As String = Crpt.Encrypt(UserName)
                        Dim EPwd As String = Crpt.Encrypt(Password)
                        .WriteString(Chr(0) & Chr(0) & Chr((EUsr.Length * 2) + (EPwd.Length \ 8)) & EUsr & EPwd)
                        If MoreData <> Nothing Then .WriteString(Crpt.Encrypt(MoreData))
                        .Seek(4, IO.SeekOrigin.Begin)
                        .WriteByte(UsrCount + 1)
                        .Flush()
                        .Close()
                    Case Else
                        Throw New MCARuntime.Exceptions.NotCompatibleDUFException(DUFName)
                End Select
            End With
        End Sub
        Public Function GetDUFTag(ByRef DUFName As String) As String
            With New FileStreamEx(DUFName, IO.FileMode.Open)
                Select Case .ReadString(4)
                    Case "DUF" & Chr(0) 'DUF Versión 1.1, tipo 0
                        Dim IV() As Byte = Text.Encoding.ASCII.GetBytes(.ReadString(16)), Key As String = .ReadString(32)
                        .Skip(1)
                        Dim OffSet As UShort = (.ReadByte * 256) + .ReadByte
                        Using x As New AESPluginCompatibilityLayer(Key, IV)
                            If OffSet = 0 Then
                                Return x.Decrypt(.ReadToEnd)
                            Else
                                Return x.Decrypt(.ReadTo(OffSet))
                            End If
                        End Using
                    Case "DUF" & Chr(1) 'DUF Versión 1.1, tipo 1
                        .Skip(1)
                        Dim OffSet As UShort = (.ReadByte * 256) + .ReadByte
                        If OffSet = 0 Then
                            Return Crpt.Decrypt(.ReadToEnd)
                        Else
                            Return Crpt.Decrypt(.ReadTo(OffSet))
                        End If
                    Case Else
                        Throw New MCARuntime.Exceptions.NotCompatibleDUFException(DUFName)
                End Select
            End With
        End Function
        Public Function CheckPassword(ByVal DUFName As String, ByVal username As String, ByVal password As String, Optional ByRef MoreData As String = Nothing) As PasswordChecking
            Using f As New FileStreamEx(DUFName, IO.FileMode.Open)
                Select Case f.ReadString(4)
                    Case "DUF" & Chr(0) 'DUF Versión 1.1, tipo 0
                        Dim IV As Byte() = Text.Encoding.ASCII.GetBytes(f.ReadString(16)), Key As String = f.ReadString(32)
                        Using tmp As New AESPluginCompatibilityLayer(Key, IV)
                            Return CheckPassword2(username, password, MoreData, tmp, f)
                        End Using
                    Case "DUF" & Chr(1) 'DUF Versión 1.1, tipo 1
                        Return CheckPassword2(username, password, MoreData, Crpt, f)
                    Case Else
                        Throw New MCARuntime.Exceptions.NotCompatibleDUFException(DUFName)
                End Select
            End Using
            Return PasswordChecking.NotExist
        End Function
        Private Function CheckPassword2(ByVal username As String, ByVal password As String, ByRef MoreData As String, ByRef Cr As AESPluginCompatibilityLayer, ByRef F As FileStreamEx) As PasswordChecking
            Dim Usr As Byte = F.ReadByte, NextIndex As UInteger = (F.ReadByte * 256) + F.ReadByte
            Try
                For J As Byte = 1 To Usr
                    F.Position = NextIndex
                    NextIndex = (F.ReadByte * 256) + F.ReadByte
                    Dim datalen As Byte = F.ReadByte
                    If username = Cr.Decrypt(F.ReadString((datalen And 15) >> 4)) Then
                        If password = Cr.Decrypt(F.ReadString(datalen And 15)) Then
                            MoreData = Cr.Decrypt(F.ReadTo(NextIndex))
                            Return PasswordChecking.Valid
                        Else
                            Return PasswordChecking.Invalid
                        End If
                    End If
                Next
            Catch ex As Exception
                Return PasswordChecking.Corrupt
            End Try
            Return PasswordChecking.NotExist
        End Function

        Public Sub ChangePwd(ByRef DUFName As String, ByRef UserName As String, ByRef Password As String)
            'Using y As New FileStreamEx(DUFName, IO.FileMode.Open)
            '    Select Case y.ReadString(4)
            '        Case "DUF" & Chr(0) 'DUF Versión 1.1, tipo 0
            '            Dim IV As Byte() = Nothing
            '            y.Read(IV, 0, 16)
            '            Dim Key As String = y.ReadString(32), Usr As Byte = y.ReadByte, NextIndex As UInteger = (y.ReadByte * 256) + y.ReadByte
            '            Dim datalen As Byte, placeholder As String, nxtidpos As UInteger
            '            Using tmp As New AESPluginCompatibilityLayer(Key, IV)
            '                Try
            '                    For J As Byte = 1 To Usr
            '                        y.Position = NextIndex
            '                        nxtidpos = NextIndex
            '                        NextIndex = (y.ReadByte * 256) + y.ReadByte
            '                        datalen = y.ReadByte
            '                        If UserName = tmp.Decrypt(y.ReadString((datalen \ 16) * 8)) Then
            '                            Using z As New FileStreamEx(DUFName & ".tmp", IO.FileMode.Create)
            '                                z.WriteString(y.ReadTo(0))
            '                                placeholder = tmp.Encrypt(Password)

            '                                z.PushPOS((y.Position - ((datalen \ 16) * 8)) - 1)
            '                                z.Write(Chr(((datalen \ 16) * 16) + (placeholder.Length \ 8)))
            '                                z.PopPOS()
            '                                y.Skip((datalen Mod 16) * 8)
            '                                z.Write(placeholder & y.ReadTo(NextIndex))
            '                                Do
            '                                    z.PushPOS(nxtidpos)
            '                                    z.Write(Chr(z.Length \ 256) & Chr(z.Length Mod 256))
            '                                    z.PopPOS()
            '                                    nxtidpos = z.Position
            '                                    NextIndex = (Asc(y.Read(1)) * 256) + Asc(y.Read(1))
            '                                    If NextIndex = 0 Then
            '                                        z.Write(Chr(0) & Chr(0) & y.ReadToEnd)
            '                                        Exit Do
            '                                    End If
            '                                    z.Write(Chr(0) & Chr(0) & y.ReadTo(NextIndex))
            '                                Loop
            '                                y.Delete(False)
            '                                z.Extension = "DUF"
            '                                Exit Sub
            '                            End Using
            '                        End If
            '                    Next
            '                    Throw New MCARuntime.Exceptions.UserNotFoundException(UserName)
            '                Catch ex As Exception
            '                    Throw New MCARuntime.Exceptions.FileDamagedException(y.FileName & ".DUF")
            '                End Try
            '            End Using
            '        Case "DUF" & Chr(1) 'DUF Versión 1.1, tipo 1
            '            Dim Usr As Byte = Asc(y.Read(1)), NextIndex As UInteger = (Asc(y.Read(1)) * 256) + Asc(y.Read(1))
            '            Dim datalen As Byte, placeholder As String, nxtidpos As UInteger
            '            Try
            '                For J As Byte = 1 To Usr
            '                    y.MoveTo(NextIndex)
            '                    nxtidpos = NextIndex
            '                    NextIndex = (Asc(y.Read(1)) * 256) + Asc(y.Read(1))
            '                    datalen = Asc(y.Read(1))
            '                    If UserName = Crpt.Decrypt(y.Read((datalen \ 16) * 8)) Then
            '                        Using z As New MCARuntime.AppServices.FileManager.File(DUFName & ".tmp")
            '                            z.Open(MCARuntime.AppServices.FileMode.Binary)
            '                            z.Write(y.Read(0, y.Position))
            '                            placeholder = Crpt.Encrypt(Password)
            '                            z.PushPOS((y.Position - ((datalen \ 16) * 8)) - 1)
            '                            z.Write(Chr(((datalen \ 16) * 16) + (placeholder.Length \ 8)))
            '                            z.PopPOS()
            '                            y.Skip((datalen Mod 16) * 8)
            '                            z.Write(placeholder & y.ReadTo(NextIndex))
            '                            Do
            '                                z.PushPOS(nxtidpos)
            '                                z.Write(Chr(z.Length \ 256) & Chr(z.Length Mod 256))
            '                                z.PopPOS()
            '                                nxtidpos = z.Position
            '                                NextIndex = (Asc(y.Read(1)) * 256) + Asc(y.Read(1))
            '                                If NextIndex = 0 Then
            '                                    z.Write(Chr(0) & Chr(0) & y.ReadToEnd)
            '                                    Exit Do
            '                                End If
            '                                z.Write(Chr(0) & Chr(0) & y.ReadTo(NextIndex))
            '                            Loop
            '                            y.Delete(False)
            '                            z.Extension = "DUF"
            '                            Exit Sub
            '                        End Using
            '                    End If
            '                Next
            '                Throw New MCARuntime.Exceptions.UserNotFoundException(UserName)
            '            Catch ex As Exception
            '                Throw New MCARuntime.Exceptions.FileDamagedException(y.FileName & ".DUF")
            '            End Try
            '        Case Else
            '            y.Close()
            '            Throw New MCARuntime.Exceptions.NotCompatibleDUFException(DUFName)
            '    End Select
            'End Using
        End Sub
        Public Sub ChangePwd2(ByRef UserName As String, ByRef Password As String, ByRef cr As AESPluginCompatibilityLayer, ByRef f As FileStreamEx)
            'Dim Usr As Byte = f.ReadByte, NextIndex As UInteger = (f.ReadByte * 256) + f.ReadByte
            'Dim datalen As Byte, placeholder As String, nxtidpos As UInteger
            'Try
            '    For J As Byte = 1 To Usr
            '        f.Position = NextIndex
            '        nxtidpos = NextIndex
            '        NextIndex = (f.ReadByte * 256) + f.ReadByte
            '        datalen = f.ReadByte
            '        If UserName = cr.Decrypt(f.ReadString((datalen And 15) >> 4)) Then
            '            Using z As New FileStreamEx(f.Name & ".tmp", IO.FileMode.Create)
            '                z.WriteString(f.ReadTo(0))
            '                placeholder = cr.Encrypt(Password)
            '                z.PushPOS((y.Position - ((datalen \ 16) * 8)) - 1)
            '                z.Write(Chr(((datalen \ 16) * 16) + (placeholder.Length \ 8)))
            '                z.PopPOS()
            '                y.Skip((datalen Mod 16) * 8)
            '                z.Write(placeholder & y.ReadTo(NextIndex))
            '                Do
            '                    z.PushPOS(nxtidpos)
            '                    z.Write(Chr(z.Length \ 256) & Chr(z.Length Mod 256))
            '                    z.PopPOS()
            '                    nxtidpos = z.Position
            '                    NextIndex = (Asc(y.Read(1)) * 256) + Asc(y.Read(1))
            '                    If NextIndex = 0 Then
            '                        z.Write(Chr(0) & Chr(0) & y.ReadToEnd)
            '                        Exit Do
            '                    End If
            '                    z.Write(Chr(0) & Chr(0) & y.ReadTo(NextIndex))
            '                Loop
            '                y.Delete(False)
            '                z.Extension = "DUF"
            '                Exit Sub
            '            End Using
            '        End If
            '    Next
            '    Throw New MCARuntime.Exceptions.UserNotFoundException(UserName)
            'Catch ex As Exception
            '    Throw New MCARuntime.Exceptions.FileDamagedException(y.FileName & ".DUF")
            'End Try
        End Sub

        Public Sub DeleteUsr(ByVal DUFName As String, Username As String)
            'Using y As New MCARuntime.AppServices.FileManager.File(DUFName)
            '    y.Open(MCARuntime.AppServices.FileMode.Binary)
            '    Select Case y.Read(4)
            '        Case "DUF" & Chr(0) 'DUF Versión 1.1, tipo 0
            '            Dim IV As Byte() = Text.Encoding.ASCII.GetBytes(y.Read(16)), Key As String = y.Read(32),
            '                Usr As Byte = Asc(y.Read(1)), NextIndex As UInteger = (Asc(y.Read(1)) * 256) + Asc(y.Read(1))
            '            Dim datalen As Byte, placeholder As UShort, nxtidpos As UShort
            '            Using tmp As New AESPluginCompatibilityLayer(Key, IV)
            '                Try
            '                    For J As Byte = 1 To Usr
            '                        placeholder = y.Position
            '                        y.MoveTo(NextIndex)
            '                        nxtidpos = NextIndex
            '                        NextIndex = (Asc(y.Read(1)) * 256) + Asc(y.Read(1))
            '                        datalen = Asc(y.Read(1))
            '                        If Username = tmp.Decrypt(y.Read((datalen \ 16) * 8)) Then
            '                            Using z As New MCARuntime.AppServices.FileManager.File(DUFName & ".tmp")
            '                                z.Open(MCARuntime.AppServices.FileMode.Binary)
            '                                z.Write(y.Read(0, nxtidpos))
            '                                y.MoveTo(NextIndex)
            '                                Do
            '                                    z.PushPOS(placeholder)
            '                                    z.Write(Chr(z.Length \ 256) & Chr(z.Length Mod 256))
            '                                    z.PopPOS()
            '                                    placeholder = z.Position
            '                                    NextIndex = (Asc(y.Read(1)) * 256) + Asc(y.Read(1))
            '                                    If NextIndex = 0 Then
            '                                        z.Write(Chr(0) & Chr(0) & y.ReadToEnd)
            '                                        Exit Do
            '                                    End If
            '                                    z.Write(Chr(0) & Chr(0) & y.ReadTo(NextIndex))
            '                                Loop
            '                                z.MoveTo(52)
            '                                z.Write(Chr(Usr - 1))
            '                                y.Delete(False)
            '                                z.Extension = "DUF"
            '                                Exit Sub
            '                            End Using
            '                        End If
            '                    Next
            '                    Throw New MCARuntime.Exceptions.UserNotFoundException(Username)
            '                Catch ex As Exception
            '                    Throw New MCARuntime.Exceptions.FileDamagedException(y.FileName & ".DUF")
            '                End Try
            '            End Using
            '        Case "DUF" & Chr(1) 'DUF Versión 1.1, tipo 1
            '            Dim Usr As Byte = Asc(y.Read(1)), NextIndex As UInteger = (Asc(y.Read(1)) * 256) + Asc(y.Read(1))
            '            Dim datalen As Byte, placeholder As UShort, nxtidpos As UShort
            '            Try
            '                For J As Byte = 1 To Usr
            '                    placeholder = y.Position
            '                    y.MoveTo(NextIndex)
            '                    nxtidpos = NextIndex
            '                    NextIndex = (Asc(y.Read(1)) * 256) + Asc(y.Read(1))
            '                    datalen = Asc(y.Read(1))
            '                    If Username = Crpt.Decrypt(y.Read((datalen \ 16) * 8)) Then
            '                        Using z As New MCARuntime.AppServices.FileManager.File(DUFName & ".tmp")
            '                            z.Open(MCARuntime.AppServices.FileMode.Binary)
            '                            z.Write(y.Read(0, nxtidpos))
            '                            y.MoveTo(NextIndex)
            '                            Do
            '                                z.PushPOS(placeholder)
            '                                z.Write(Chr(z.Length \ 256) & Chr(z.Length Mod 256))
            '                                z.PopPOS()
            '                                placeholder = z.Position
            '                                NextIndex = (Asc(y.Read(1)) * 256) + Asc(y.Read(1))
            '                                If NextIndex = 0 Then
            '                                    z.Write(Chr(0) & Chr(0) & y.ReadToEnd)
            '                                    Exit Do
            '                                End If
            '                                z.Write(Chr(0) & Chr(0) & y.ReadTo(NextIndex))
            '                            Loop
            '                            z.MoveTo(4)
            '                            z.Write(Chr(Usr - 1))
            '                            y.Delete(False)
            '                            z.Extension = "DUF"
            '                            Exit Sub
            '                        End Using
            '                    End If
            '                Next
            '                Throw New MCARuntime.Exceptions.UserNotFoundException(Username)
            '            Catch ex As Exception
            '                Throw New MCARuntime.Exceptions.FileDamagedException(y.FileName & ".DUF")
            '            End Try
            '        Case Else
            '            y.Close()
            '            Throw New MCARuntime.Exceptions.NotCompatibleDUFException(DUFName)
            '    End Select
            'End Using
        End Sub
        Public Enum PasswordChecking As Byte
            Valid = 0
            Invalid = 1
            NotExist = 2
            Corrupt = 3
        End Enum
    End Class
End Class

