Public Class Info
    Inherits MCARuntime.AppServices.PluginIdentification
    Public Sub New()
        ReleaseType = MCARuntime.AppServices.AppInfo.ReleaseType.RC
        ExtDescription = My.Resources.Description
        Unstable = False
        FileName = Reflection.Assembly.GetExecutingAssembly.Location
    End Sub
End Class
Public Class BrainFuckVM
    Inherits MCARuntime.VMachines.VMPlugin
    Private Mem(30000) As Byte
    Private pointer As UShort = 0
    Public Overrides Sub HardReset()
        SoftReset()
        Program = ""
    End Sub
    Public Overrides Sub SoftReset()
        ConsoleOut.Cls()
        pointer = 0
        Array.Clear(Mem, 0, Mem.Length - 1)
    End Sub
    Public Overrides Sub RunVM()
        Dim calls As Short = 0
        Array.Clear(Mem, 0, Mem.Length - 1)
        For j As UInteger = 0 To Program.Length - 1
            Select Case Program.Chars(j)
                Case "+"
                    If Mem(pointer) < 255 Then
                        Mem(pointer) += 1
                    Else
                        Mem(pointer) = 0
                    End If
                Case "-"
                    If Mem(pointer) > 0 Then
                        Mem(pointer) -= 1
                    Else
                        Mem(pointer) = 255
                    End If
                Case ">"
                    If pointer < Mem.Length - 1 Then
                        pointer += 1
                    Else
                        pointer = 0
                    End If
                Case "<"
                    If pointer > 0 Then
                        pointer -= 1
                    Else
                        pointer = Mem.Length - 1
                    End If
                Case "."
                    ConsoleOut.Print(Chr(Mem(pointer)))
                Case ","
                    If IsKeyBufferEmpty() Then RaiseEvtWaitingInput(My.Resources.vmMsg1)
                    While IsKeyBufferEmpty()
                    End While
                    Mem(pointer) = Asc(GetLastKey)
                Case "["
                    If Mem(pointer) = 0 Then
                        For k As Integer = j + 1 To Program.Length - 1
                            If Program.Chars(k) = "[" Then calls += 1
                            If Program.Chars(k) = "]" Then
                                If calls = 0 Then
                                    j = k
                                    Exit For
                                Else
                                    calls -= 1
                                End If
                            End If
                        Next
                    End If
                    If j = 0 Or calls <> 0 Then
                        RaiseEvtVMException(New MCARuntime.Exceptions.VMExecutionException(My.Resources.vmErr1))
                        j = Program.Length
                    End If
                Case "]"
                    If Mem(pointer) <> 0 Then
                        For k As Integer = j - 1 To 0 Step -1
                            If Program.Chars(k) = "]" Then calls += 1
                            If Program.Chars(k) = "[" Then
                                If calls = 0 Then
                                    j = k
                                    Exit For
                                Else
                                    calls -= 1
                                End If
                            End If
                        Next
                    End If
                    If j = 0 Or calls <> 0 Then
                        RaiseEvtVMException(New MCARuntime.Exceptions.VMExecutionException(My.Resources.vmErr1))
                        j = Program.Length
                    End If
            End Select
        Next
        RaiseEvtVMStopped(Nothing)
    End Sub
    Public Overrides Sub ParseArguments(ParamArray Args() As Object)
        Select Case Args.Count
            Case 0
            Case 1
                If Not (TypeOf Args(0) Is UShort) Then
                    Throw New MCARuntime.Exceptions.InvalidArgumentException("MemorySize")
                End If
                ReDim Mem(Args(0))
            Case Else
                Throw New MCARuntime.Exceptions.TooManyArgumentsException()
        End Select
    End Sub
    Protected Overrides Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            HardReset()
            Erase Mem
        End If
        disposedValue = True
    End Sub
End Class