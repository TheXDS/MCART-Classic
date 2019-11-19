Imports System.IO
' -=<[ MCA RUNTIME ]>=-
'Biblioteca de procedimientos varios para programas .Net

#Region "Información y directivas de la Runtime"
''' <summary>
''' Contiene la información de compilación del runtime y directivas del compilador.
''' </summary>
''' <remarks>Modificar estas líneas conforme el proyecto avance.</remarks>
Public Module BuildInfo
#Const DisableUnusableCode = 0
#Const CompileBetaCode = 0
#Const BreakPoints = 0
#Const CopyrightProtect = 0
    Public Const ReleaseType As Byte = 6
    Public Const Unstable As Boolean = True
    Public Const CopyrightProtect As Boolean = True
End Module
''' <summary>
''' Obtiene la información de la rutina como objeto compatible de Plugin
''' </summary>
''' <remarks></remarks>
Public Class Info
    Inherits AppServices.PluginIdentification
    Public Sub New()
        ReleaseType = RTInfo.Release
        ExtDescription = My.Resources.Description
        Unstable = RTInfo.Unstable
        FileName = RTInfo.Path
        IsPlugin = False
        RestrictedSource = CopyrightProtect
    End Sub
End Class
''' <summary>
''' Interpreta la información de BuildInfo como objeto MCARuntime.AppServices.AppInfo
''' </summary>
''' <remarks>
''' La información de ensamblado se encuentra en la parte superior
''' de la rutina. Cambie estos valores ahí según sea necesario,
''' pero no modifique este módulo.
''' </remarks>
Public Module RuntimeInfo
    ''' <summary>
    ''' Convierte directamente la información de BuildInfo a AppInfo
    ''' </summary>
    ''' <returns>Un objeto Appinfo con la informaicón de la rutina.</returns>
    Public Function RTInfo() As AppServices.AppInfo
        RTInfo = New AppServices.AppInfo
        With FileVersionInfo.GetVersionInfo(RTInfo.Path)
            RTInfo.Name = .FileDescription
            RTInfo.Major = .FileMajorPart
            RTInfo.Minor = .FileMinorPart
            RTInfo.Rev = .FileBuildPart
            RTInfo.Build = .FilePrivatePart
            'RTInfo.DebugBuild = .IsDebug
            RTInfo.Unstable = Unstable
            RTInfo.Release = ReleaseType
        End With
        Return RTInfo
    End Function
    ''' <summary>
    ''' Devuelve información de la rutina como cadena.
    ''' </summary>
    ''' <returns>Una cadena que contiene el nombre de la rutina junto con la versión.</returns>
    Public Function VersionString(Optional ByVal shrt As Boolean = False) As String
        Return RTInfo.VersionString
    End Function
    ''' <summary>Devuelve la cantidad de RAM física asignada al proceso de la rutina.</summary>
    ''' <returns>Un valor de tipo Long que contiene el número de bytes de memoria física asignada al contexto del proceso.</returns>
    ''' <remarks>
    ''' Esta función siempre devolverá cero para Windows 98 y Windows Millennium Edition.
    '''</remarks>
    Public Function WorkingRAM() As Long
        Return My.Application.Info.WorkingSet
    End Function
End Module
#End Region

Namespace Beta
#If CompileBetaCode Then
        ''' <summary>
        ''' Representa un archivo del disco duro, junto con toda su funcionalidad.
        ''' </summary>
        Public Class File
            Implements IDisposable
            Public InmeddiateWrite As Boolean = True
            Public BufferSize As UInteger = 32768
            Public AutoCarReturn As Boolean = True
            Public IsTMP As Boolean = False
            Private FNumber As UShort = FreeFile(), M As FileMode = FileMode.Closed
            Private Buffer As String = "", disposedValue As Boolean, FPath As String = ""
            Private PosStack As New List(Of ULong), MdStack As List(Of FileMode)
            Public ReadOnly Property UsedBuffer As UInteger
                Get
                    Return Buffer.Length
                End Get
            End Property
            Public ReadOnly Property Length As UInteger
                Get
                    Return LOF(FNumber)
                End Get
            End Property
            Public Property Attributes As IO.FileAttributes
                Get
                    Return FileIO.FileSystem.GetFileInfo(FPath).Attributes
                End Get
                Set(value As IO.FileAttributes)
                    FileIO.FileSystem.GetFileInfo(FPath).Attributes = value
                End Set
            End Property
            Public ReadOnly Property AccessStackCount As UShort
                Get
                    Return MdStack.Count
                End Get
            End Property
            Public ReadOnly Property PositionStack As List(Of ULong)
                Get
                    Return PosStack
                End Get
            End Property
            Public ReadOnly Property ModeStack As List(Of FileMode)
                Get
                    Return MdStack
                End Get
            End Property
            Public Property Position As ULong
                Get
                    If Status <> FileMode.Closed Then
                        Return Seek(FNumber) - 1
                    Else
                        Throw New Exceptions.FileClosedException
                    End If
                End Get
                Set(value As ULong)
                    If Status <> FileMode.Closed Then
                        MoveTo(value)
                    Else
                        Throw New Exceptions.FileClosedException()
                    End If
                End Set
            End Property
            Public Property Status As FileMode
                Get
                    Return M
                End Get
                Set(value As FileMode)
                    Open(value)
                End Set
            End Property
            Public Property Extension As String
                Get
                    Return FileManager.Extension(FPath)
                End Get
                Set(value As String)
                    Rename(FileName & value)
                End Set
            End Property
            Public Property FileName As String
                Get
                    Return FileManager.FileName(FPath)
                End Get
                Set(value As String)
                    Rename(value)
                End Set
            End Property
            Public Property FilePath As String
                Get
                    Return FPath
                End Get
                Set(value As String)
                    Close()
                    FPath = value
                End Set
            End Property
            Public Property FileDir As String
                Get
                    Return FileManager.FileDir(FPath)
                End Get
                Set(value As String)
                    If FileIO.FileSystem.DirectoryExists(value) Then
                        If Exists() Then
                            If M = FileMode.Closed Then
                                FileIO.FileSystem.MoveFile(FPath, value)
                            Else
                                Throw New Exceptions.IncorrectFileModeException
                            End If
                        Else
                            Throw New Exceptions.FileNotFoundException
                        End If
                    Else
                        Throw New Exceptions.InvalidDirectoryException
                    End If
                End Set
            End Property
            ''' <summary>Inicializa la instancia de File</summary>
            ''' <param name="fname">Nombre de archivo a Crear/Administrar</param>
            ''' <remarks>
            ''' Si no se especifican parámetros, New() prepara la
            ''' instancia como un archivo temporal.
            ''' </remarks>
            Public Sub New(ByRef fname As String)
                FilePath = fname
            End Sub
            ''' <summary>Inicializa la instancia de File</summary>
            ''' <remarks>
            ''' Si no se especifican parámetros, New() prepara la
            ''' instancia como un archivo temporal.
            ''' </remarks>
            Public Sub New()
                FilePath = Security.RndFileName
                IsTMP = True
            End Sub

            Public Function Exists() As Boolean
                Return FileIO.FileSystem.FileExists(FPath)
            End Function
            Public Sub Open(Optional ByRef Mode As FileMode = FileMode.Binary)
                If M <> FileMode.Closed Then
                    PushState()
                End If
                M = Mode
                Select Case M
                    Case FileMode.Binary
                        FileOpen(FNumber, FPath, OpenMode.Binary)
                        AutoCarReturn = False
                    Case FileMode.Input
                        If Exists() Then
                            FileOpen(FNumber, FPath, OpenMode.Input)
                        Else
                            Throw New Exceptions.FileNotFoundException()
                        End If
                    Case FileMode.Output
                        FileOpen(FNumber, FPath, OpenMode.Append)
                        AutoCarReturn = True
                    Case FileMode.ReWrite
                        FileOpen(FNumber, FPath, OpenMode.Output)
                        AutoCarReturn = True
                    Case FileMode.CreateOnly
                        'Durante la creación del archivo, la propiedad Status devuelve 5.
                        'Esto puede ser usado para indicar al usuario de un dispositivo de almacenamiento lento.
                        FileOpen(FNumber, FPath, OpenMode.Binary)
                        FileClose(FNumber)
                        M = FileMode.Closed
                End Select
            End Sub
            Public Sub Close()
                If M <> FileMode.Closed Then
                    If InmeddiateWrite = False Then BufferFlush()
                    FileClose(FNumber)
                    M = FileMode.Closed
                End If
                If MdStack IsNot Nothing Then
                    If MdStack.Count > 0 Then PopState()
                End If
            End Sub
            Public Sub Rename(ByRef NewName As String)
                If Exists() Then
                    Dim oldstate As FileMode = M
                    If M <> FileMode.Closed Then PushPOS()
                    Close()
                    If NewName = FPath Then Throw New Exceptions.InvalidFileNameException()
                    FileIO.FileSystem.RenameFile(FPath, NewName)
                    If oldstate <> FileMode.Closed Then
                        Open(oldstate)
                        PopPOS()
                    End If
                Else
                    Throw New Exceptions.FileNotFoundException()
                End If
            End Sub
            Public Overloads Sub PushPOS()
                PosStack.Add(Position)
            End Sub
            Public Overloads Sub PushPOS(ByRef NewPOS As ULong)
                PushPOS()
                MoveTo(NewPOS)
            End Sub
            Public Sub PopPOS()
                Position = PosStack.Item(PosStack.Count - 1)
                PosStack.RemoveAt(PosStack.Count - 1)
            End Sub
            Public Sub MoveToEnd()
                MoveTo(Length)
            End Sub
            Private Sub PushState()
                MdStack.Add(M)
                PushPOS()
                Close()
            End Sub
            Private Sub PopState()
                Open(MdStack.Item(MdStack.Count - 1))
                PopPOS()
                MdStack.RemoveAt(MdStack.Count - 1)
            End Sub

            ''' <summary>Elimina el contenido del búffer.</summary>
            Public Sub ClearBuffer()
                Buffer = ""
            End Sub

            ''' <summary>Vacía el búffer, escribiendo la información que contenga.</summary>
            ''' <remarks>
            ''' En modo binario, Usar BufferFlush sólo durante la
            ''' escritura. Si se usa durante lectura, se escribirá la
            ''' información leída.
            ''' </remarks>
            Public Sub BufferFlush()
                Select Case M
                    Case FileMode.Binary
                        FilePut(FNumber, Buffer)
                    Case FileMode.Output, FileMode.ReWrite
                        Print(FNumber, Buffer)
                End Select
                ClearBuffer()
            End Sub

            ''' <summary>
            ''' Destruye el contenido de un archivo.
            ''' </summary>
            ''' <remarks>El archivo es restaurado al modo en que se encontraba luego de ser destruido.</remarks>
            Public Sub Destroy()
                'If Exists() Then
                Dim oldstate As FileMode = M
                If Exists() Then Delete()
                Open(FileMode.CreateOnly)
                Open(oldstate)
                MoveTo(0)
                'Else
                'Throw New MCARuntime.Exceptions.FileNotFoundException
                'End If
            End Sub

            ''' <summary>
            ''' Elimina un archivo.
            ''' </summary>
            ''' <param name="RecycleIt">True para enviar a la papelera de reciclaje. False para borrar permanentemente.</param>
            ''' <remarks>El archivo es cerrado automáticamente antes de proceder a eliminarlo.</remarks>
            Public Sub Delete(Optional ByRef RecycleIt As Boolean = False)
                Close()
                If Not Exists() Then Throw New Exceptions.FileNotFoundException
                Dim rm As Byte = 2
                If RecycleIt Then rm = 3
                FileIO.FileSystem.DeleteFile(FPath, rm, rm)
            End Sub

            ''' <summary>
            ''' Escribe datos en un archivo.
            ''' </summary>
            ''' <param name="Str">Datos a escribir</param>
            ''' <remarks>Si la escritura excede el tamaño del búffer, se escribe todo inmediatamente y el búffer queda vacío nuevamente.</remarks>
            Public Sub Write(ByRef Str As String)
                If M <> FileMode.Input Then
                    Buffer &= Str
                    If AutoCarReturn Then Buffer &= Chr(13) & Chr(10)
                    If InmeddiateWrite Then
                        BufferFlush()
                    Else
                        If Len(Buffer) >= BufferSize Then BufferFlush()
                    End If
                Else
                    Throw New Exceptions.IncorrectFileModeException()
                End If
            End Sub

            ''' <summary>Lee datos de un archivo</summary>
            ''' <param name="Len">Cantidad de bytes o líneas a leer</param>
            ''' <returns>Una cadena que contiene la lectura realizada</returns>
            ''' <remarks>
            ''' En modo binario, <para>Len</para> especifica la cantidad de Bytes a
            ''' leer. En modo de lectura, <para>Len</para> indica cuantas líneas
            ''' deben ser leídas.
            ''' Si no se especifica <para>Len</para> o es igual a cero, para modo
            ''' binario, se leerá todo el buffer, y en modo de lectura, se leerá
            ''' una línea.
            ''' En caso de que <para>Len</para> sea menor a cero, Se leerán
            ''' <para>Len</para> Bytes antes de la posicion actual, y se invalida
            ''' si el archivo se encuentra en modo de lectura.
            ''' </remarks>
            Public Function Read(Optional ByRef Len As Long = 0) As String
                Buffer = ""
                Select Case M
                    Case FileMode.Binary
                        If Len = 0 Then Len = BufferSize
                        If Len < 0 Then
                            If Position + Len >= 0 Then
                                MoveTo(Position + Len)
                                Len = 0 - Len
                            Else
                                Throw New ArgumentOutOfRangeException("Len")
                            End If
                        End If
                        If (Position + Len) > Length Then Len = (Length - Position)
                        If Position > Length Then Return ""
                        Buffer = Space(Len)
                        FileGet(FNumber, Buffer)
                    Case FileMode.Input
                        For j As Integer = 0 To Len
                            Input(FNumber, Buffer)
                            Buffer &= Chr(13) & Chr(10)
                        Next
                    Case FileMode.Closed
                        Throw New Exceptions.FileClosedException
                    Case Else
                        Throw New Exceptions.IncorrectFileModeException
                End Select
                Return Buffer
            End Function

            ''' <summary>Lee datos de un archivo</summary>
            ''' <param name="start">Punto de inicio de la lectura</param>
            ''' <param name="Len">Cantidad de bytes o líneas a leer</param>
            ''' <returns>Una cadena que contiene la lectura realizada</returns>
            ''' <remarks>
            ''' En modo binario, <para>Len</para> especifica la cantidad de Bytes a
            ''' leer. En modo de lectura, <para>Len</para> indica cuantas líneas
            ''' deben ser leídas.
            ''' Si no se especifica <para>Len</para> o es igual a cero, para modo
            ''' binario, se leerá todo el buffer, y en modo de lectura, se leerá
            ''' una línea.
            ''' En caso de que <para>Len</para> sea menor a cero, Se leerán
            ''' <para>Len</para> Bytes antes de la posicion especificada, y se invalida
            ''' si el archivo se encuentra en modo de lectura.
            ''' </remarks>
            Public Function Read(ByRef start As ULong, ByVal Len As Long) As String
                PushPOS(start)
                Read = Read(Len)
                PopPOS()
                Return Read
            End Function

            ''' <summary>Lee los datos de un archivo hasta el final</summary>
            ''' <returns>
            ''' Una cadena que contiene los bytes leídos desde la posición
            ''' actual hasta el final del archivo.
            ''' </returns>
            Public Function ReadToEnd() As String
                Return Read(Length - Position)
            End Function

            ''' <summary>Lee los datos de un archivo hasta la posición especificada</summary>
            ''' <param name="Pos">Posición de final de la lectura</param>
            ''' <returns>Una Cadena con los bytes leídos desde la posición actual hasta la especificada</returns>
            ''' <remarks>Si Pos es menor a la posición actual, se devolverá Nothing</remarks>
            Public Function ReadTo(ByRef Pos As ULong) As String
                If Pos > Position Then
                    Return Read(Pos - Position)
                Else
                    MoveTo(Pos)
                    Return ""
                End If
            End Function

            ''' <summary>
            ''' Mueve el cursor de lectura a la posición especificada.
            ''' </summary>
            ''' <param name="Position">Nueva posición del cursor de lectura.</param>
            ''' <remarks></remarks>
            Public Sub MoveTo(ByVal Position As UInteger)
                If Status <> FileMode.Closed Then
                    If Position < Length Then
                        Seek(FNumber, Position + 1)
                    Else
                        MoveToEnd()
                        'Throw New ArgumentOutOfRangeException("Position")
                    End If
                Else
                    Throw New Exceptions.FileClosedException()
                End If
            End Sub

            ''' <summary>
            ''' Mueve el cursor de lectura, ignorando los bytes especificados.
            ''' </summary>
            ''' <param name="X">Cantidad de bytes a ignorar.</param>
            ''' <remarks></remarks>
            Public Sub Skip(ByVal X As UInteger)
                MoveTo(Position + X)
            End Sub

            'IDisposable
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not disposedValue Then
                    If disposing Then
                        PosStack.Clear()
                        MdStack.Clear()
                        Close()
                        If IsTMP Then Delete()
                    End If
                    InmeddiateWrite = False
                    BufferSize = 0
                    FPath = ""
                    AutoCarReturn = False
                    FNumber = 0
                    M = 0
                    Buffer = ""
                End If
                disposedValue = True
            End Sub
            Protected Overrides Sub Finalize()
                Dispose(False)
                MyBase.Finalize()
            End Sub
            ''' <summary>
            ''' Libera todos los recursos utilizados por el objeto
            ''' </summary>
            Public Sub Dispose() Implements IDisposable.Dispose
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub
        End Class

    Public Module Data
        Public Interface DataMgr

        End Interface
        Public Class Table
            Implements IDisposable
            Private data As ArrayList ' Esto debería ser un arreglo de Reg
            Private nme As String ' Nombre interno de la tabla.
            Private headr() As String ' Campos de la tabla junto al Keyfield, q es índice 0
            Sub AddReg(ByRef Data As MCARuntime.Beta.Data.Reg)
                ' TODO: Añade un registro nuevo
            End Sub
            Sub EditReg(ByRef Data As MCARuntime.Beta.Data.Reg)
                ' TODO: Actualiza la información de un registro
            End Sub
            Sub RemoveReg(ByRef Key As String)
                ' TODO: Elimina el registro especificado
            End Sub
            Function GetReg(ByRef Key As String) As MCARuntime.Beta.Data.Reg
                ' TODO: Esta funcion buscaria el registro con el indice especificado, y devolverlo.
                Return Nothing
            End Function
            Sub New(ByRef Name As String, ByRef Headr() As String)
                ' Crea una nueva tabla vacia
            End Sub

            ReadOnly Property Name As String
                Get
                    Return nme
                End Get
            End Property
            ReadOnly Property KeyField As String
                Get
                    Return headr(0)
                End Get
            End Property

            Private disposedValue As Boolean ' Para detectar llamadas redundantes
            Protected Overridable Sub Dispose(ByVal disposing As Boolean)
                If Not Me.disposedValue Then
                    If disposing Then
                        ' TODO: eliminar estado administrado (objetos administrados).
                    End If

                    ' TODO: liberar recursos no administrados (objetos no administrados) e invalidar Finalize() below.
                    ' TODO: Establecer campos grandes como Null.
                End If
                Me.disposedValue = True
            End Sub
            ' TODO: invalidar Finalize() sólo si la instrucción Dispose(ByVal disposing As Boolean) anterior tiene código para liberar recursos no administrados.
            'Protected Overrides Sub Finalize()
            '    ' No cambie este código. Ponga el código de limpieza en la instrucción Dispose(ByVal disposing As Boolean) anterior.
            '    Dispose(False)
            '    MyBase.Finalize()
            'End Sub
            Public Sub Dispose() Implements IDisposable.Dispose
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub
        End Class
        Public Class Reg
            Property Key As String
            Property Data() As String
            ' ^^^^ Cambiar esto por la nueva versión de Varstack.
        End Class
    End Module

    ''' <summary>
    ''' Contiene mecanismos de control de base de datos.
    ''' </summary>
    ''' <remarks>
    ''' MCAData 'Codename Zonda' está en fase Beta
    ''' </remarks>
    Public Module MCAData
        Private DU As New System.Data.DataSet, Reg As System.Data.DataRow, Dsn As String = "dsn="
        Public Sub DoThis(ByVal DoT As String)
            'Ejecuta instrucciomes SQL sobre la base de datos.
            Using DoIt As New System.Data.Odbc.OdbcCommand(DoT)
                Using Cx As New System.Data.Odbc.OdbcConnection(Dsn)
                    DoIt.Connection = Cx
                    Cx.Open()
                    DoIt.ExecuteNonQuery()
                End Using
            End Using
        End Sub

        ''' <summary>Lee el contenido de una tabla en la base de datos y la almacena en la memoria.</summary>
        ''' <param name="Tabla">Tabla a cargar.</param>
        ''' <param name="fields">Campos a cargar</param>
        ''' <remarks>Si se omite fields, se cargarán todos los campos de la tabla.</remarks>
        Public Sub Connection(ByVal Tabla As String, Optional ByVal fields As String = "*")
            Using C As New System.Data.Odbc.OdbcConnection(Dsn)
                C.Open()
                Using AU As New System.Data.Odbc.OdbcDataAdapter("Select " & fields & " from " & Tabla & ";", C)
                    If DU.Tables.Contains(Tabla) Then DU.Tables.Remove(Tabla)
                    AU.Fill(DU, Tabla)
                End Using
                C.Close()
            End Using
        End Sub

        Public Sub SetDsn(ByVal X As String)
            Dsn = "dsn=" & X
        End Sub
        Public Function GetDsn()
            Return Dsn.Substring(4)
        End Function

        Enum SaveCheckState
            Cancel = 0
            Insert = 1
            Update = 2
            Delete = 3
        End Enum
        Enum ShowPromt
            All = 0
            DontAsk = 1
            HideKey = 2
        End Enum
        Enum CloseMode
            Normal = 0
            CloseAfterNew = 1 ' Cierra el formulario luego de guardar un nuevo registro.
            CloseAfterEdit = 2 ' Cierra el formulario luego de guardar un registro editado.
        End Enum

        ''' <summary>Describe un controlador de base de datos basado en ODBC.</summary>
        ''' <remarks>
        ''' Esta clase está en fase Beta, y su uso debe ser controlado.
        ''' </remarks>
        Public Class Controller
            Implements IDisposable
            Private DUlocal As New System.Data.DataSet ' Es un DataSet local, listo para búsquedas.
            Private UseDU As Boolean = True ' Bandera interna de control, que es False si el usuario ha realizado una búsqueda.
            Private cheapfix1 As String = "" 'Contiene instrucciones SQL a ejecutar.
            Private cheapfix2 As String = "" 'Contiene instrucciones SQL a ejecutar.
            Private Tabla As String = "" 'Nombre de la tabla asociada
            Private Pos As Integer = 0 ' Especifica el registro actualmente seleccionado.
            Public KeyField As String = "" ' Contiene el nombre del campo llave de la tabla asociada al objeto.

            ''' <summary>
            ''' Bandera externa de control, que es True si el usuario presiona el botón de nuevo registro.
            ''' </summary>
            ''' <remarks>
            ''' Es necesario establecer manualmente esta bandera, para que la rutina
            ''' funcione corectamente al registrar nuevas filas en la base de datos.
            ''' </remarks>
            Public NewReg As Boolean = False

            '''<summary> Contiene un registro cargado en memoria.</summary>
            Public Reg As System.Data.DataRow

            ''' <summary>Indica o establece el registro seleccionado.</summary>
            ''' <value>Índice de registro a seleccionar</value>
            ''' <returns>El registro actualmente seleccionado</returns>
            ''' <remarks></remarks>
            Property Position As Integer
                Get
                    If RegCount = 0 Then Return -1
                    Return Pos
                End Get
                Set(value As Integer)
                    If value > RegCount - 1 Or value < 0 Then Throw New ArgumentOutOfRangeException("value")
                    Pos = value
                End Set
            End Property

            ''' <summary>Obtiene o establece la tabla asociada a esta instancia.</summary>
            ''' <value>Nombre de tabla a cargar.</value>
            ''' <returns>Nombre de la tabla asociada.</returns>
            ''' <remarks></remarks>
            Property Table As String
                Get
                    Return Tabla
                End Get
                Set(value As String)
                    Tabla = value
                    Connection(Tabla)
                End Set
            End Property

            ''' <summary>Devuelve la cuenta de registros de una tabla.</summary>
            ''' <returns>Un entero de 32-bits sin signo que representa la cuenta de registros que contiene una tabla.</returns>
            ReadOnly Property RegCount() As UInteger
                Get
                    If UseDU Then Return DU.Tables(Tabla).Rows.Count Else Return DUlocal.Tables(Tabla).Rows.Count
                End Get
            End Property

            ''' <summary>Ir al primer registro.</summary>
            Public Sub GotoFirst()
                Pos = 0
            End Sub

            ''' <summary>Ir al registro anterior.</summary>
            Public Sub GotoPrev()
                If Pos > 0 Then Pos = Pos - 1
            End Sub

            ''' <summary>Ir al regsitro siguiente.</summary>
            Public Sub GotoNext()
                If Pos < RegCount() - 1 Then Pos += 1
            End Sub

            ''' <summary>Ir al último registro.</summary>
            Public Sub GotoLast()
                Pos = RegCount() - 1
            End Sub

            ''' <summary>Lee el registro seleccionado y lo carga en memoria.</summary>
            ''' <remarks>Si se ha realizado una búsqueda, el registro se leerá de ella.</remarks>
            Public Sub Read()
                If UseDU Then
                    Reg = DU.Tables(Tabla).Rows(Pos)
                Else
                    Reg = DUlocal.Tables(Tabla).Rows(Pos)
                End If
            End Sub

            ''' <summary>
            ''' Actualiza los registros de un conjunto de controles.
            ''' </summary>
            ''' <param name="X">Conjunto de controles a procesar.</param>
            ''' <param name="key">Campo llave a actualizar</param>
            ''' <remarks>
            ''' La función depende de que la propiedad Tag esté establecida a el nombre del campo en los objetos a actualizar.
            ''' </remarks>
            Public Sub UpdateReg(ByVal X As Object, ByVal key As String)
                For Each M As Object In X
                    If M.HasChildren Then
                        If M.GetType.ToString.Contains("System.Windows.Forms.") Then UpdateReg(M.Controls, key)
                    End If
                    If M.Tag <> "" Then
                        '' Speedhack ''
                        If Not CheckReg(M) Then DoThis("Update " & Tabla & " Set " & BuildSQL(M) & " where " & KeyField & "='" & key & "';")
                    End If
                Next
            End Sub

            Public Sub CreateReg(ByVal M As System.Windows.Forms.Control.ControlCollection)
                CreateReg2(M)
                DoThis("Insert into " & Tabla & " (" & cheapfix1.Substring(1) & ")values(" & cheapfix2.Substring(1) & ");")
                cheapfix1 = "" : cheapfix2 = ""
                Connection(Tabla)
            End Sub
            Private Sub CreateReg2(ByVal X As Object)
                For Each M As Object In X
                    If M.HasChildren Then
                        If M.GetType.ToString.Contains("System.Windows.Forms.") Then CreateReg2(M.Controls)
                    End If
                    If M.Tag <> "" Then
                        cheapfix1 &= "," & M.tag
                        cheapfix2 &= "," & BuildSQL(M).ToString.Substring(InStr(BuildSQL(M), "="))
                    End If
                Next
            End Sub

            ''' <summary>
            ''' Realiza búsquedas.
            ''' </summary>
            ''' <param name="Field">Campo de referencia.</param>
            ''' <param name="Query">Valor aproximado buscado.</param>
            ''' <remarks>Este procedimiento sin argumentos, cierra la búsqueda y limpia el DataSet local.</remarks>
            Public Overloads Sub Search(ByVal Field As String, ByVal Query As String)
                If Field = Nothing OrElse Field.Length <= 0 Then
                    Throw New ArgumentNullException("Field")
                End If
                If Query = Nothing OrElse Query.Length <= 0 Then
                    Throw New ArgumentNullException("Query")
                End If
                Search()
                Using X As New System.Data.Odbc.OdbcConnection
                    X.ConnectionString = Dsn
                    X.Open()
                    Using Y As New System.Data.Odbc.OdbcDataAdapter("Select * from " & Tabla & " where " & Field & " like '%" & Query & "%';", X)
                        Y.Fill(DUlocal, Tabla)
                    End Using
                    X.Close()
                End Using
                UseDU = False
            End Sub
            Public Overloads Sub Search()
                DUlocal = New System.Data.DataSet
                Pos = 0
                UseDU = True
            End Sub

            ''' <summary>
            ''' Deduce la acción de guardado a ejecutar.
            ''' </summary>
            ''' <param name="Key">Campo llave del registro a guardar.</param>
            ''' <returns>Un SaveCheckState que dicta al prorgrama la acción de guardado pertinente.</returns>
            ''' <remarks>
            ''' Si se omite <paramref>Key</paramref>, se asumirá que el campo llave es autogenerado, y se devolverá 
            ''' SaveCheckState.Insert siempre.
            ''' </remarks>
            Public Function SaveCheck(Optional ByRef Key As String = "") As SaveCheckState
                If NewReg Then
                    'Asumiendo Key autogenerado por el SQL...
                    If Key = "" Then Return SaveCheckState.Insert

                    Search(KeyField, Key)
                    If RegCount() = 0 Then
                        Search()
                        Return SaveCheckState.Insert
                    Else
                        Search()
                        If MsgBox(String.Format(MCARuntime.My.Resources.Strings.Ask1, Key), 52, MCARuntime.My.Resources.Strings.Save) = MsgBoxResult.No Then Return SaveCheckState.Cancel
                    End If
                End If
                Return SaveCheckState.Update
            End Function

            ''' <summary>
            ''' Borra un registro de la base de datos.
            ''' </summary>
            ''' <param name="key">Registro a borrar.</param>
            ''' <param name="Prmt">Define qué mensajes se muestran.</param>
            ''' <remarks></remarks>
            Public Sub DeleteReg(ByVal key As String, Optional ByVal Prmt As ShowPromt = ShowPromt.All)
                Dim x As MsgBoxResult = MsgBoxResult.Yes
                Select Case Prmt
                    Case ShowPromt.All
                        x = MsgBox(String.Format(MCARuntime.My.Resources.Strings.Ask2, key), 52, MCARuntime.My.Resources.Strings.Delete)
                    Case ShowPromt.HideKey
                        x = MsgBox(MCARuntime.My.Resources.Strings.Ask2a, 52, MCARuntime.My.Resources.Strings.Delete)
                End Select
                If x = MsgBoxResult.Yes Then
                    Search()
                    DoThis("Delete * from " & Tabla & " where " & KeyField & "='" & key & "';")
                    Connection(Tabla)
                End If
            End Sub

            ''' <summary>Muestra el contenido de Reg en sus controles correspondientes</summary>
            ''' <param name="X">Colección de controles a llenar.</param>
            ''' <remarks>
            ''' Esta Subrutina depende de la propiedad Tag, que contiene
            ''' el nombre del campo al cual hace referencia el control.
            ''' </remarks>
            Sub ShowReg(ByVal X As Object)
                For Each M As Object In X
                    If M.HasChildren Then
                        If TypeOf M Is System.Windows.Forms.Control Then ShowReg(M.Controls)
                    End If
                    If M.Tag <> "" Then
                        Fill(M)
                    End If
                Next
            End Sub

            ''' <summary>
            ''' Rellena el objeto con la información requerida.
            ''' </summary>
            ''' <param name="Obj">Objeto a rellenar</param>
            ''' <remarks>El objeto se rellenará basado en la propiedad Tag.</remarks>
            Public Sub Fill(ByVal Obj As System.Windows.Forms.ComboBox)
                If Reg(Obj.Tag).ToString <> "" Then Obj.Text = Reg(Obj.Tag) Else Obj.SelectedIndex = -1
            End Sub
            Public Sub Fill(ByVal Obj As System.Windows.Forms.DateTimePicker)
                If Reg(Obj.Tag).ToString <> "" Then Obj.Text = Reg(Obj.Tag).ToString Else Obj.Value = Nothing
            End Sub
            Public Sub Fill(ByVal Obj As System.Windows.Forms.ListBox)
                If Reg(Obj.Tag).ToString <> "" Then Obj.Text = Reg(Obj.Tag) Else Obj.SelectedIndex = -1
            End Sub
            Public Sub Fill(ByVal Obj As System.Windows.Forms.MaskedTextBox)
                Obj.Text = Reg(Obj.Tag).ToString
            End Sub
            Public Sub Fill(ByVal Obj As System.Windows.Forms.NumericUpDown)
                Obj.Value = Val(Reg(Obj.Tag).ToString)
            End Sub
            Public Sub Fill(ByVal Obj As System.Windows.Forms.PictureBox)
                If IO.File.Exists(Reg(Obj.Tag).ToString) Then
                    Obj.Load(Reg(Obj.Tag))
                Else
                    Obj.Image = Nothing
                    Obj.ImageLocation = ""
                End If
            End Sub
            Public Sub Fill(ByVal Obj As System.Windows.Forms.ProgressBar)
                Obj.Value = Val(Reg(Obj.Tag).ToString)
            End Sub
            Public Sub Fill(ByVal Obj As System.Windows.Forms.TextBox)
                Obj.Text = Reg(Obj.Tag).ToString
            End Sub
            Public Sub Fill(ByVal Obj As System.Windows.Forms.TrackBar)
                Obj.Value = Val(Reg(Obj.Tag).ToString)
            End Sub

            ''' <summary>
            ''' Comprueba si el campo del registro cargado es igual al valor de su objeto.
            ''' </summary>
            ''' <param name="Obj">Objeto a comprobar.</param>
            ''' <returns>Un valor booleano que indica si el registro es igual al valor actual del objeto.</returns>
            ''' <remarks>El objeto debe contener información de su campo en la propiedad Tag.</remarks>
            Public Overloads Function CheckReg(ByVal Obj As System.Windows.Forms.ComboBox) As Boolean
                Return (Reg(Obj.Tag).ToString = Obj.Text)
            End Function
            Public Overloads Function CheckReg(ByVal Obj As System.Windows.Forms.DateTimePicker) As Boolean
                Return (Reg(Obj.Tag).ToString = Obj.Text)
            End Function
            Public Overloads Function CheckReg(ByVal Obj As System.Windows.Forms.ListBox) As Boolean
                Return (Reg(Obj.Tag).ToString = Obj.Text)
            End Function
            Public Overloads Function CheckReg(ByVal Obj As System.Windows.Forms.MaskedTextBox) As Boolean
                Return (Reg(Obj.Tag).ToString = Obj.Text.Replace("-", ""))
            End Function
            Public Overloads Function CheckReg(ByVal Obj As System.Windows.Forms.NumericUpDown) As Boolean
                Return (Reg(Obj.Tag).ToString = Obj.Value)
            End Function
            Public Overloads Function CheckReg(ByVal Obj As System.Windows.Forms.PictureBox) As Boolean
                Return (Reg(Obj.Tag).ToString = Obj.ImageLocation)
            End Function
            Public Overloads Function CheckReg(ByVal Obj As System.Windows.Forms.ProgressBar) As Boolean
                Return (Reg(Obj.Tag).ToString = Obj.Value)
            End Function
            Public Overloads Function CheckReg(ByVal Obj As System.Windows.Forms.TextBox) As Boolean
                Return (Reg(Obj.Tag).ToString = Obj.Text)
            End Function
            Public Overloads Function CheckReg(ByVal Obj As System.Windows.Forms.TrackBar) As Boolean
                Return (Reg(Obj.Tag).ToString = Obj.Value)
            End Function

            ''' <summary>
            ''' Costruye una porción de una instrucción SQL.
            ''' </summary>
            ''' <param name="Obj">Objeto relacionado con la instrucción.</param>
            ''' <returns>Parte de un comando SQL para establecer campos.</returns>
            ''' <remarks>El objeto debe contener información de su campo en la propiedad Tag.</remarks>
            Public Overloads Function BuildSQL(ByVal Obj As System.Windows.Forms.ComboBox) As String
                Return Obj.Tag & "='" & Obj.Text & "'"
            End Function
            Public Overloads Function BuildSQL(ByVal Obj As System.Windows.Forms.DateTimePicker) As String
                Return Obj.Tag & "='" & Obj.Text & "'"
            End Function
            Public Overloads Function BuildSQL(ByVal Obj As System.Windows.Forms.ListBox) As String
                Return Obj.Tag & "='" & Obj.Text & "'"
            End Function
            Public Overloads Function BuildSQL(ByVal Obj As System.Windows.Forms.MaskedTextBox) As String
                Return Obj.Tag & "='" & Obj.Text & "'"
            End Function
            Public Overloads Function BuildSQL(ByVal Obj As System.Windows.Forms.NumericUpDown) As String
                Return Obj.Tag & "=" & Obj.Value
            End Function
            Public Overloads Function BuildSQL(ByVal Obj As System.Windows.Forms.PictureBox) As String
                Return Obj.Tag & "='" & Obj.ImageLocation & "'"
            End Function
            Public Overloads Function BuildSQL(ByVal Obj As System.Windows.Forms.ProgressBar) As String
                Return Obj.Tag & "=" & Obj.Value
            End Function
            Public Overloads Function BuildSQL(ByVal Obj As System.Windows.Forms.TextBox) As String
                Return Obj.Tag & "='" & Obj.Text & "'"
            End Function
            Public Overloads Function BuildSQL(ByVal Obj As System.Windows.Forms.TrackBar) As String
                Return Obj.Tag & "=" & Obj.Value
            End Function

            ''' <summary>
            ''' Crea una nueva instancia del controlador MCAData
            ''' </summary>
            ''' <param name="Tbl">Tabla a cargar</param>
            ''' <param name="KF">Campo llave de la tabla</param>
            ''' <remarks>Antes de crear este objeto, es necesario haber establecido la cadena DSN.</remarks>
            Public Sub New(ByVal Tbl As String, ByVal KF As String)
                If Dsn = "dsn=" Then Throw New MCARuntime.Exceptions.UnSetODBCException
                If Tbl = "" Then Throw New MCARuntime.Exceptions.InvalidTableException
                If KF = "" Then Throw New MCARuntime.Exceptions.InvalidKeyFieldException
                Tabla = Tbl : KeyField = KF
                Connection(Tabla)
            End Sub
#Region "IDisposable Support"
            Private disposedValue As Boolean ' Para detectar llamadas redundantes
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not Me.disposedValue Then
                    If disposing Then
                        DUlocal.Dispose()
                    End If
                    KeyField = Nothing
                    NewReg = Nothing
                    Pos = Nothing
                    UseDU = Nothing
                    cheapfix1 = Nothing
                    cheapfix2 = Nothing
                    Tabla = Nothing
                    Reg = Nothing
                End If
                Me.disposedValue = True
            End Sub
            Protected Overrides Sub Finalize()
                Dispose(False)
                MyBase.Finalize()
            End Sub
            Public Sub Dispose() Implements IDisposable.Dispose
                Dispose(True)
                GC.SuppressFinalize(Me)
            End Sub
#End Region
        End Class
    End Module
    Namespace Formats
        Namespace DUFFormat
            Public Enum DUFType As Byte
                Null = 0
                InternalKey = 1
                FileKey = 2
                UnCypher = 3
            End Enum
            ''' <summary>DUF 2.0</summary>
            ''' <remarks>
            ''' Ésta clase contiene el sistema de
            ''' administración de usuarios DUF en su
            ''' versión 2.0, permitiendo utilizar
            ''' cualquier contenedor especificado 
            ''' mediante la clase FileFormats.Containers.Container
            ''' </remarks>
            Public Class DUF
                Implements IDisposable
                Private Const Sign = "DUF2.0"
                Private File As MCARuntime.FileFormats.Containers.ContainerPluginInt
                Private Crpt As MCARuntime.Security.Encryption.CrypterPluginInt
                Private tmp As MCARuntime.Security.Encryption.CrypterPluginInt 'Recordar igualarlo a Nothing al terminar de usarlo...
                Private Function Tag() As String
                    Return File.GetTag()
                End Function
                Private Function DTipe() As Byte
                    Return Asc(Tag().Substring(Sign.Length, 1))
                End Function
                Sub New(ByRef Container As MCARuntime.FileFormats.Containers.ContainerInt, Optional ByVal DType As DUFType = DUFType.InternalKey)
                    Me.New(Container.FileName, DType)
                End Sub
                Sub New(ByVal Name As String, Optional ByVal DType As DUFType = DUFType.InternalKey)
                    If MCARuntime.AppServices.FileManager.Exists(Name) Then
                        Try
                            File = MCARuntime.AppServices.PluginSupport.LoadPlugIn("IXDContainer.dll", "IXDContainer")
                            File.ParseArguments(Name)
                        Catch ex As Exception
                            Throw ex
                        End Try
                    Else
                        Select Case DType
                            Case DUFType.InternalKey
                                Try
                                    File = MCARuntime.AppServices.PluginSupport.LoadPlugIn("IXDContainer.dll", "IXDContainer")
                                    File.ParseArguments(Name, Sign & Chr(DUFType.InternalKey), 1)
                                Catch ex As Exception
                                    Throw ex
                                End Try
                        End Select
                    End If
                    Try
                        Crpt = MCARuntime.AppServices.PluginSupport.LoadPlugIn("AESCrypter.dll", "AESCrypter")
                        Crpt.ParseArguments("TrustN@125431560", System.Text.Encoding.ASCII.GetBytes("N@1kN0w5+h3P455!"))
                    Catch ex As Exception
                        Throw ex
                    End Try
                End Sub
                Public Sub CreateDUF(Optional ByRef DUFTpe As DUFFormat.DUFType = DUFType.InternalKey)
                    Select Case DUFTpe
                        Case DUFFormat.DUFType.FileKey
                            Dim iv As String = MCARuntime.Security.Randomizations.RndString(16)
                            Dim key As String = MCARuntime.Security.Randomizations.RndString(32)
                            File.CreateFile()
                            File.ChangeTag(Sign & Chr(DUFTpe) & Crpt.Encrypt(key & iv))
                        Case DUFFormat.DUFType.InternalKey, DUFType.UnCypher, DUFType.Null
                            File.CreateFile()
                            File.ChangeTag(Sign & Chr(DUFTpe))
                        Case Else
                            Throw New MCARuntime.Exceptions.NotCompatibleDUFException(DUFTpe)
                    End Select
                End Sub
                Public Sub AddToDUF(ByRef UserName As String, ByRef Password As String, Optional ByRef MoreData As String = "")
                    Select Case DTipe()
                        Case DUFType.FileKey
                            Dim IV As String, Key As String, tpe As String
                            tpe = Crpt.Decrypt(Tag().Substring(Sign.Length + 1))
                            Key = tpe.Substring(0, 32)
                            IV = tpe.Substring(32, 16)
                            tmp = MCARuntime.AppServices.PluginSupport.LoadPlugIn("AESCrypter.dll", "AESCrypter")
                            tmp.ParseArguments(Key, System.Text.Encoding.ASCII.GetBytes(IV))
                            Dim EUsr As String = tmp.Encrypt(UserName)
                            Dim EPwd As String = tmp.Encrypt(Password)
                            Dim EMDt As String = ""
                            If MoreData <> "" Then EMDt = tmp.Encrypt(MoreData)
                            tmp = Nothing
                            File.Add(EUsr, Chr((EPwd.Length)) & EPwd & EMDt)
                        Case DUFType.InternalKey
                            Dim EUsr As String = Crpt.Encrypt(UserName)
                            Dim EPwd As String = Crpt.Encrypt(Password)
                            Dim EMDt As String = ""
                            If MoreData <> "" Then EMDt = Crpt.Encrypt(MoreData)
                            File.Add(EUsr, Chr((EPwd.Length)) & EPwd & EMDt)
                        Case Else
                            Throw New MCARuntime.Exceptions.NotCompatibleDUFException(File.FileName)
                    End Select
                End Sub
                Public Function GetDUFTag() As String
                    Return Tag().Substring(0, Sign.Length + 1)
                End Function
                Public Function CheckPassword(ByVal username As String, ByVal password As String, Optional ByRef MoreData As String = "") As MCARuntime.FileFormats.Formats.DUFFormat.DUF.PasswordChecking
                    Select Case DTipe()
                        Case DUFType.FileKey
                            Dim IV As String, Key As String, tpe As String
                            tpe = Crpt.Decrypt(Tag().Substring(Sign.Length + 1))
                            Key = tpe.Substring(0, 32)
                            IV = tpe.Substring(32, 16)
                            Try
                                tmp = MCARuntime.AppServices.PluginSupport.LoadPlugIn("AESCrypter.dll", "AESCrypter")
                                tmp.ParseArguments(Key, System.Text.Encoding.ASCII.GetBytes(IV))
                                Dim dt As String = File.GetData(tmp.Encrypt(username))
                                If dt = Nothing Then Return PasswordChecking.NotExist
                                If password = tmp.Decrypt(dt.Substring(1, Asc(dt.Substring(0, 1)))) Then
                                    MoreData = tmp.Decrypt(dt.Substring(password.Length))
                                    Return PasswordChecking.Valid
                                Else
                                    Return PasswordChecking.Invalid
                                End If
                                tmp = Nothing
                            Catch ex As Exception
                                Return PasswordChecking.Corrupt
                            End Try
                        Case DUFType.InternalKey
                            Try
                                Dim dt As String = File.GetData(Crpt.Encrypt(username))
                                If dt = Nothing Then Return PasswordChecking.NotExist
                                If password = Crpt.Decrypt(dt.Substring(1, Asc(dt.Substring(0, 1)))) Then
                                    MoreData = Crpt.Decrypt(dt.Substring(password.Length))
                                    Return PasswordChecking.Valid
                                Else
                                    Return PasswordChecking.Invalid
                                End If
                            Catch ex As Exception
                                Return PasswordChecking.Corrupt
                            End Try
                        Case Else
                            Throw New MCARuntime.Exceptions.NotCompatibleDUFException(File.FileName)
                    End Select
                    Return PasswordChecking.NotExist
                End Function
                Public Sub ChangePwd(ByVal UserName As String, ByVal OldPw As String, ByVal NewPw As String)
                    Dim EMDt As String = ""
                    If CheckPassword(UserName, OldPw, EMDt) = MCARuntime.FileFormats.Formats.DUFFormat.DUF.PasswordChecking.Valid Then
                        Select Case DTipe()
                            Case DUFType.FileKey
                                Dim IV As String, Key As String, tpe As String
                                tpe = Crpt.Decrypt(Tag().Substring(Sign.Length + 1))
                                Key = tpe.Substring(0, 32)
                                IV = tpe.Substring(32, 16)
                                tmp = MCARuntime.AppServices.PluginSupport.LoadPlugIn("AESCrypter.dll", "AESCrypter")
                                tmp.ParseArguments(Key, System.Text.Encoding.ASCII.GetBytes(IV))
                                Dim EUsr As String = tmp.Encrypt(UserName)
                                Dim EPwd As String = tmp.Encrypt(NewPw)
                                tmp = Nothing
                                File.Add(EUsr, Chr((EPwd.Length)) & EPwd & EMDt)
                            Case DUFType.InternalKey
                                Dim EUsr As String = Crpt.Encrypt(UserName)
                                Dim EPwd As String = Crpt.Encrypt(NewPw)
                                File.ChangeData(EUsr, Chr((EPwd.Length)) & EPwd & EMDt)
                            Case Else
                                Throw New MCARuntime.Exceptions.NotCompatibleDUFException(File.FileName)
                        End Select
                    End If
                End Sub
                Public Sub DeleteUsr(Username As String)
                    Select Case DTipe()
                        Case DUFType.FileKey
                            Dim IV As String, Key As String, tpe As String
                            tpe = Crpt.Decrypt(Tag().Substring(Sign.Length + 1))
                            Key = tpe.Substring(0, 32)
                            IV = tpe.Substring(32, 16)
                            tmp = MCARuntime.AppServices.PluginSupport.LoadPlugIn("AESCrypter.dll", "AESCrypter")
                            tmp.ParseArguments(Key, System.Text.Encoding.ASCII.GetBytes(IV))
                            Dim EUsr As String = tmp.Encrypt(Username)
                            tmp = Nothing
                            File.DeleteData(EUsr)
                        Case DUFType.InternalKey
                            Dim EUsr As String = Crpt.Encrypt(Username)
                            File.DeleteData(EUsr)
                        Case Else
                            Throw New MCARuntime.Exceptions.NotCompatibleDUFException(File.FileName)
                    End Select
                End Sub
                Public Enum PasswordChecking As Byte
                    Valid = 0
                    Invalid = 1
                    NotExist = 2
                    Corrupt = 3
                End Enum
                Private disposedValue As Boolean
                Protected Overridable Sub Dispose(disposing As Boolean)
                    If Not Me.disposedValue Then
                        File = Nothing
                        Crpt = Nothing
                    End If
                    Me.disposedValue = True
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
        End Namespace
    End Namespace
#End If
End Namespace

Namespace Security
    Namespace Encryption
        ''' <summary>Describe los métodos básicos a implementar por un mecanismo de encriptación.</summary>
        Public Interface Crypter
            ''' <summary>Encripta una cadena</summary>
            ''' <param name="Str">Cadena a encriptar</param>
            ''' <returns>Una cadena encriptada utilizando el algoritmo de esta instancia.</returns>
            Function Encrypt(ByVal Str As String) As String
            ''' <summary>Desencripta una cadena</summary>
            ''' <param name="Str">Cadena a desencriptar</param>
            ''' <returns>Una cadena desencriptada utilizando el algoritmo de esta instancia.</returns>
            Function Decrypt(ByVal Str As String) As String
        End Interface
        ''' <summary>Relaciona las interfaces necesarias para crear plugins de encriptación.</summary>
        Public Interface CrypterPluginInt
            Inherits AppServices.PluginInt
            Inherits Crypter
        End Interface
        ''' <summary>Implementa algunas funciones del plugin, y especifica cuales deben ser implementadas en las clases derivadas.</summary>
        ''' <remarks>Se debe heredar ésta clase, por lo que no se puede usar directamente.</remarks>
        Public MustInherit Class CrypterPlugin
            Inherits AppServices.Plugin
            Implements CrypterPluginInt
            ''' <summary>Devuelve el nombre de la interfaz implementada por ésta clase.</summary>
            Public Overrides ReadOnly Property MainInterface As String
                Get
                    Return "MCARuntime.Security.Encryption.CrypterPluginInt"
                End Get
            End Property
            Public MustOverride Function Decrypt(Str As String) As String Implements Crypter.Decrypt
            Public MustOverride Function Encrypt(Str As String) As String Implements Crypter.Encrypt
        End Class
    End Namespace
    ''' <summary>Contiene funciones que generan valores aleatorios de diferente tipo.</summary>
    Public Module Randomizations
        ''' <summary>
        ''' Crea un motor aleatorio personalizado.
        ''' </summary>
        Public Interface RandomEngineInt
            ''' <summary>
            ''' Obtiene un número aleatorio entre 0 y 1
            ''' </summary>
            ''' <returns>un Double que es un número aleatorio generado entre 0 y 1</returns>
            Function getRnd() As Double
            ''' <summary>
            ''' Obtiene un número aleatorio entre el rango especificado
            ''' </summary>
            ''' <param name="Top">Número más alto posible (inclusive). Si no se especifica, se supone 100</param>
            ''' <param name="Bottom">Número más bajo posible (inclusive). Si no se especifica, se supone 0</param>
            ''' <returns>Un Long que es un número aleatorio en el rango especificado</returns>
            ''' <remarks></remarks>
            Function Random(Optional Top As Long = 100, Optional Bottom As Long = 0) As Long
            ''' <summary>
            ''' Establece la semilla del motor aleatorio
            ''' </summary>
            ''' <param name="seed">Semilla a utilizar</param>
            ''' <exception cref="ArgumentOutOfRangeException"> Ocurre cuando <paramref name="seed"></paramref>
            ''' está fuera del rango permitido por el motor aleatorio</exception>
            Sub setSeed(ByVal seed As Long)
        End Interface
        ''' <summary>Relaciona las interfaces necesarias para crear plugins de motor aleatorio.</summary>
        Public Interface RandomEnginePluginInt
            Inherits AppServices.PluginInt
            Inherits RandomEngineInt
        End Interface
        ''' <summary>Implementa algunas funciones del plugin, y especifica cuales deben ser implementadas en las clases derivadas.</summary>
        ''' <remarks>Se debe heredar ésta clase, por lo que no se puede usar directamente.</remarks>
        Public MustInherit Class RandomEnginePlugin
            Inherits AppServices.Plugin
            Implements RandomEnginePluginInt
            ''' <summary>Devuelve el nombre de la interfaz implementada por ésta clase.</summary>
            Public Overrides ReadOnly Property MainInterface As String
                Get
                    Return "MCARuntime.Security.Randomizations.RandomEnginePluginInt"
                End Get
            End Property
            ''' <summary>
            ''' Obtiene un número aleatorio entre 0 y 1
            ''' </summary>
            ''' <returns>un Double que es un número aleatorio generado entre 0 y 1</returns>
            Public MustOverride Function getRnd() As Double Implements RandomEngineInt.getRnd
            ''' <summary>
            ''' Obtiene un número aleatorio entre el rango especificado
            ''' </summary>
            ''' <param name="Top">Número más alto posible (inclusive). Si no se especifica, se supone 100</param>
            ''' <param name="Bottom">Número más bajo posible (inclusive). Si no se especifica, se supone 0</param>
            ''' <returns>Un Long que es un número aleatorio en el rango especificado</returns>
            ''' <remarks></remarks>
            Public MustOverride Function Random(Optional Top As Long = 100, Optional Bottom As Long = 0) As Long Implements RandomEngineInt.Random
            ''' <summary>
            ''' Establece la semilla del motor aleatorio
            ''' </summary>
            ''' <param name="seed">Semilla a utilizar</param>
            ''' <exception cref="ArgumentOutOfRangeException"> Ocurre cuando <paramref name="seed"></paramref>
            ''' está fuera del rango permitido por el motor aleatorio</exception>
            Public MustOverride Sub setSeed(seed As Long) Implements RandomEngineInt.setSeed
        End Class
        ''' <summary>Genera un nombre de archivo aleatorio con extensión temporal no utilizado en el directorio.</summary>
        ''' <param name="Lenght">Parámetro opcional. Especifica la longitud del nombre de archivo a generar.</param>
        ''' <returns>Un nombre de archivo temporal libre en el directorio actual.</returns>
        ''' <exception cref="ArgumentOutOfRangeException">Se genera cuando <paramref name="Lenght"></paramref> es igual a cero.</exception>
        Public Function RndFileName(Optional ByVal Lenght As Byte = 8) As String
            If Lenght = 0 Then Throw New ArgumentOutOfRangeException("Lenght")
            Dim x As String
            Do
                x = RndText(Lenght) & ".tmp"
            Loop While FileIO.FileSystem.FileExists(x)
            Return x
        End Function
        ''' <summary>
        ''' Crea una cadena de caracteres completamente aleatorios.
        ''' </summary>
        ''' <param name="length">Longitud de la cadena deseada. Si es igual a cero, se devolverá Nothing.</param>
        Public Function RndString(ByRef length As UInteger) As String
            Dim x As String = Nothing
            For j As UInteger = 1 To length
                x &= Chr(RndByte())
            Next
            Return x
        End Function
        ''' <summary>
        ''' Crea una cadena de caracteres aleatorios dentro de los códigos imprimibles de texto de la tabla ASCII sin incluir espacios.
        ''' </summary>
        ''' <param name="length">Longitud de la cadena deseada. Si es igual a cero, se devolverá Nothing.</param>
        Public Function RndText(ByRef length As UInteger) As String
            Dim x As String = Nothing
            For j As UInteger = 1 To length
                Select Case Rnd(2)
                    Case 0 : x &= Chr(Rnd(57, 48))
                    Case 1 : x &= Chr(Rnd(90, 65))
                    Case 2 : x &= Chr(Rnd(122, 97))
                End Select
            Next
            Return x
        End Function
        ''' <summary>
        ''' Devuelve un byte escogido aleatoriamente de la tabla ASCII
        ''' </summary>
        Public Function RndByte() As Byte
            Return Rnd(255)
        End Function
        ''' <summary>
        ''' Crea un arreglo de bytes aleatorios de la tabla ASCII
        ''' </summary>
        ''' <param name="size">Cantidad de elementos deseados</param>
        Public Function RndBytes(ByRef size As UInteger) As Byte()
            If size <= 1 Then Throw New ArgumentOutOfRangeException("size")
            Dim x(size - 1) As Byte
            For j As UInteger = 0 To size - 1
                x(j) = RndByte()
            Next
            Return x
        End Function
        ''' <summary>
        ''' Facilita la aplicación de la función aleatoria de Visual Basic al proveerla de parámetros importantes.
        ''' </summary>
        ''' <param name="Value">Valor máximo a escoger.</param>
        ''' <param name="Floor">Valor mínimo a escoger.</param>
        Public Function Rnd(ByRef Value As UInteger, Optional ByRef Floor As UInteger = 0) As UInteger
            Randomize(My.Computer.Clock.TickCount)
            Return (Value - Floor) * VBMath.Rnd() + Floor
        End Function
        ''' <summary>Desordena un arreglo.</summary>
        ''' <param name="A">Arreglo a desordenar</param>
        ''' <param name="Deepness">Profundidad del desorden</param>
        ''' <remarks><paramref name="A"></paramref>puede ser un
        ''' arreglo o una String, en cuyo caso, será convertida en
        ''' un arreglo de Char.</remarks>
        Public Sub Shuffle(ByRef A As IList, Optional ByVal Deepness As UInteger = 1)
            If A Is Nothing Then Throw New ArgumentNullException("A")
            If A.Count <= 1 Then Throw New ArgumentException("A")
            If Deepness < 1 Or Deepness > A.Count Then Throw New ArgumentOutOfRangeException("Deepness")
            Try
                For j As ULong = 0 To A.Count - 1 Step Deepness
                    DataHandling.Swap(A(j), A(Rnd(A.Count - 1)))
                Next
            Catch
                Throw New Exceptions.InvalidArgumentException("A")
            End Try
        End Sub
        ''' <summary>Desordena un arreglo.</summary>
        ''' <param name="A">Arreglo a desordenar</param>
        ''' <param name="Deepness">Profundidad del desorden</param>
        ''' <remarks><paramref name="A"></paramref>puede ser un
        ''' arreglo o una String, en cuyo caso, será convertida en
        ''' un arreglo de Char.</remarks>
        Public Sub Shuffle(ByRef A As String, Optional ByVal Deepness As UInteger = 1)
            If A Is Nothing Then Throw New ArgumentNullException("A")
            If Deepness < 1 Or Deepness > A.Length Then Throw New ArgumentOutOfRangeException("Deepness")
            Dim xx As New List(Of Char)
            For Each j As Char In A.ToCharArray
                xx.Add(j)
            Next
            Shuffle(xx, Deepness)
            A = Nothing
            For Each j As Char In xx
                A &= j
            Next
            xx.Clear()
            xx = Nothing
        End Sub
        ''' <summary>
        ''' Devuelve un arreglo desordenado.
        ''' </summary>
        ''' <param name="A">Arreglo a desordenar</param>
        ''' <param name="Deepness">Profundidad del desorden</param>
        ''' <remarks><paramref name="A"></paramref>puede ser un
        ''' arreglo o una String, en cuyo caso, será convertida en
        ''' un arreglo de Char.</remarks>
        Public Function Shuffled(ByVal A As Object, Optional ByVal Deepness As UInteger = 1) As Object
            If TypeOf A Is IList Or TypeOf A Is String Then
                Shuffle(A)
                Return A
            End If
            Throw New ArgumentException()
        End Function
        ''' <summary>
        ''' Genera un listado de números desordenados.
        ''' </summary>
        ''' <param name="Top">Número más alto a incluir en el listado.</param>
        ''' <param name="Floor">Número base para el listado.</param>
        ''' <param name="Stepping">Paso generador de números.</param>
        ''' <param name="Deepness">Factor de aleatoreidad. Sus valores van desde 1 hasta el tamaño total del listado.</param>
        ''' <returns>Un arreglo del tipo System.Integer con una secuencia de números desordenada.</returns>
        Public Function RndSequence(ByVal Top As UInteger, Optional ByVal Floor As Integer = 0, Optional ByVal Stepping As UShort = 1, Optional ByVal Deepness As UInteger = 1) As Integer()
            Return Shuffled(DataHandling.Sequencer(Top, Floor, Stepping), Deepness)
        End Function
    End Module
    Namespace Checksum
        ''' <summary>Describe los métodos básicos a implementar por un mecanismo de cálculo de checksum.</summary>
        Public Interface ChecksumMethod
            ''' <summary>
            ''' Calcula el checksum de una cadena.
            ''' </summary>
            ''' <param name="X">Cadena a procesar.</param>
            ''' <returns>El checksum de la cadena en cuestión.</returns>
            Function Calculate(ByVal X As String) As String
            ''' <summary>Verifica si el checksum de la cadena es váildo.</summary>
            ''' <param name="X">Cadena a comprobar.</param>
            ''' <param name="Chksum">Checksum a verificar.</param>
            ''' <returns>True si el checksum calculado de <paramref name="X"></paramref> coincide con <paramref name="Chksum"></paramref>; en caso contrario, False.</returns>
            Function IsValid(ByVal X As String, Chksum As String) As Boolean
        End Interface
        ''' <summary>Relaciona las interfaces necesarias para crear plugins de Checksum.</summary>
        Public Interface ChecksumPluginInt
            Inherits ChecksumMethod
            Inherits AppServices.PluginInt
        End Interface
        ''' <summary>Implementa algunas funciones del plugin, y especifica cuales deben ser implementadas en las clases derivadas.</summary>
        ''' <remarks>Se debe heredar ésta clase, por lo que no se puede usar directamente.</remarks>
        Public MustInherit Class ChecksumPlugin
            Inherits AppServices.Plugin
            Implements ChecksumPluginInt
            ''' <summary>
            ''' Calcula el checksum de una cadena.
            ''' </summary>
            ''' <param name="X">Cadena a procesar.</param>
            ''' <returns>El checksum de la cadena en cuestión.</returns>
            Public MustOverride Function Calculate(X As String) As String Implements ChecksumMethod.Calculate
            ''' <summary>Verifica si el checksum de la cadena es váildo.</summary>
            ''' <param name="X">Cadena a comprobar.</param>
            ''' <param name="Chksum">Checksum a verificar.</param>
            ''' <returns>True si el checksum calculado de <paramref name="X"></paramref> coincide con <paramref name="Chksum"></paramref>; en caso contrario, False.</returns>
            Public Overridable Function IsValid(X As String, Chksum As String) As Boolean Implements ChecksumMethod.IsValid
                Return Chksum = Calculate(X)
            End Function
            ''' <summary>Devuelve el nombre de la interfaz implementada por ésta clase.</summary>
            Public Overrides ReadOnly Property MainInterface As String
                Get
                    Return "MCARuntime.Security.Checksum.ChecksumPluginInt"
                End Get
            End Property
        End Class
    End Namespace
End Namespace

Namespace DataHandling
    ''' <summary>Funciones genéricas para el manejo de valores y objetos, conversión de valores y comparaciones.</summary>
    Public Module Operations
        ''' <summary>Intercambia el valor de los objetos especificados</summary>
        ''' <param name="a">Objeto a</param>
        ''' <param name="b">Objeto b</param>
        Public Sub Swap(ByRef a As Object, ByRef b As Object)
            Push(a)
            a = b
            Pop(b)
        End Sub
        ''' <summary>Devuelve al mayor de los números indicados.</summary>
        ''' <param name="x">Listado de números a comprobar. Puede ser un arreglo, o números separados por coma.</param>
        Public Function TheHighestOf(ParamArray x() As Object) As Object
            Dim a As Object = Nothing
            For Each j As Object In x
                If IsNumeric(j) Then
                    If Not IsNumeric(a) OrElse j < a Then a = j
                End If
            Next
            Return a
        End Function
        ''' <summary>Devuelve el índice del mayor de los números del arreglo.</summary>
        ''' <param name="x">Listado de números a comprobar. Puede ser un arreglo, o números separados por coma.</param>
        Public Function TheHigestIdxOf(ParamArray x() As Object) As Integer
            Return Array.IndexOf(x, TheHighestOf(x))
        End Function
        ''' <summary>Devuelve al menor de los números indicados.</summary>
        ''' <param name="x">Listado de números a comprobar. Puede ser un arreglo, o números separados por coma.</param>
        Public Function TheLowestOf(ParamArray x() As Object) As Object
            Dim a As Object = Nothing
            For Each j As Object In x
                If IsNumeric(j) Then
                    If Not IsNumeric(a) OrElse j < a Then a = j
                End If
            Next
            Return a
        End Function
        ''' <summary>Devuelve el índice del menor de los números del arreglo.</summary>
        ''' <param name="x">Listado de números a comprobar. Puede ser un arreglo, o números separados por coma.</param>
        Public Function TheLowestIdxOf(ParamArray x() As Object) As Integer
            Return Array.IndexOf(x, TheLowestOf(x))
        End Function
        ''' <summary>Devuelve la posición de la N-ésima ocurrecia de una cadena en otra.</summary>
        ''' <param name="String1">Expresión de tipo String en la que se va a buscar.</param>
        ''' <param name="String2">Expresión de tipo String que se va a buscar.</param>
        ''' <param name="count">Número de ocurrencia a buscar.</param>
        ''' <param name="start">Expresión numérica que establece la posición inicial de cada búsqueda.Si se omite, la búsqueda comienza en la posición del primer carácter.El índice de inicio es de base 1.</param>
        ''' <param name="Compare">Especifica el tipo de comparación de cadenas.Si se omite Compare, la configuración de Option Compare determina el tipo de comparación.</param>
        Public Function InStrN(String1 As String, String2 As String, Optional Count As UInteger = 1, Optional Start As Integer = 0, Optional ByVal Compare As CompareMethod = CompareMethod.Binary) As Integer
            Dim pos As Integer = Start
            For j As UInteger = 1 To Count
                pos = InStr(pos + 1, String1, String2, Compare)
            Next
            Return pos
        End Function
        ''' <summary>Devuelve la posición de la N-ésima ocurrecia de una cadena en otra en orden inverso.</summary>
        ''' <param name="String1">Expresión de tipo String en la que se va a buscar.</param>
        ''' <param name="String2">Expresión de tipo String que se va a buscar.</param>
        ''' <param name="count">Número de ocurrencia a buscar.</param>
        ''' <param name="start">Expresión numérica que establece la posición inicial de base 1 para cada búsqueda, empezando por el lado izquierdo de la cadena.Si se omite Start, se utiliza -1, lo que significa que la búsqueda comienza en la posición del último carácter.A continuación, la búsqueda se realiza de derecha a izquierda.</param>
        ''' <param name="Compare">Especifica el tipo de comparación de cadenas.Si se omite Compare, la configuración de Option Compare determina el tipo de comparación.</param>
        Public Function InStrNRev(String1 As String, String2 As String, Optional count As UInteger = 1, Optional start As Integer = 0, Optional ByVal Compare As CompareMethod = CompareMethod.Binary) As Integer
            Dim pos As Integer = start
            For j As UInteger = 1 To count
                pos = InStrRev(String1, String2, pos + 1, Compare)
            Next
            Return pos
        End Function
        ''' <summary>
        ''' Genera una secuencia de números en el rango especificado
        ''' </summary>
        ''' <param name="Top">Valor más alto</param>
        ''' <param name="Floor">Valor más bajo</param>
        ''' <param name="Stepping">Saltos del secuenciador</param>
        ''' <returns>un arreglo de enteros con la secuencia generada</returns>
        ''' <exception cref="ArgumentOutOfRangeException">Se genera si
        ''' <paramref name="Stepping"></paramref>sobrepasa a la cantidad de
        ''' iteraciones totales, o si es cero.</exception>
        Public Function Sequencer(ByVal Top As Integer, Optional ByVal Floor As Integer = 0, Optional ByVal Stepping As Integer = 1) As Integer()
            If Floor > Top Then
                Swap(Floor, Top)
                Stepping = -Stepping
            End If
            If Stepping = 0 Or Math.Abs(Stepping) > Math.Abs(Top - Floor) Then Throw New ArgumentOutOfRangeException("Stepping")
            Dim a(Math.Abs(Top - Floor)) As Integer, c As UInteger = 0
            For b As Integer = Floor To Top Step Stepping
                a(c) = b
                c += 1
            Next
            Array.Resize(a, c)
            Return a
        End Function
        ''' <summary>Desplaza los elementos de un arreglo, lista o colección.</summary>
        ''' <param name="a">Arreglo a desplazar</param>
        ''' <param name="steps">Dirección y unidades de desplazamiento</param>
        ''' <remarks>
        ''' Si steps es un valor negativo, el desplazamiento borrará los primeros
        ''' elementos, si es positivo se eliminarán los últimos. Los espacios dejados
        ''' por los elementos desplazados, son rellenados con Nothing.
        ''' </remarks>
        Public Sub ShiftArray(ByRef a As IList, ByVal steps As Integer)
            If a.Count = 0 Then Throw New NullReferenceException("a")
            RotateArray(a, steps)
            If steps < 0 Then
                For j = a.Count - 1 To a.Count + steps Step -1
                    If j >= 0 Then a(j) = Nothing Else Exit For
                Next
            Else
                For j = 0 To steps - 1 Step 1
                    If j < a.Count Then a(j) = Nothing Else Exit For
                Next
            End If
        End Sub
        ''' <summary>Rota los elementos de un areglo, lista o colección.</summary>
        ''' <param name="a">Arreglo a rotar</param>
        ''' <param name="steps">Dirección y unidades de rotación.</param>
        ''' <remarks>
        ''' Si el valor de steps es positivo, la rotación ocurre de forma
        ''' ascendente; en caso contrario, descendente.
        ''' </remarks>
        Public Sub RotateArray(ByRef a As IList, ByVal steps As Integer)
            If a.Count = 0 Then Throw New NullReferenceException("a")
            If steps < 0 Then
                RotateArray(a, a.Count - 1)
            ElseIf steps > 0 Then
                For j = 0 To a.Count - 1 Step 1
                    Swap(a(j), a((j + steps) Mod a.Count))
                Next
            End If
        End Sub
        ''' <summary>Desempaca un Byte en 8 valores booleanos.</summary>
        ''' <param name="Packet">Byte a desempacar</param>
        Public Function ReadBoolPacket(ByVal Packet As Byte) As Boolean()
            Return ReadBoolPacket2(Packet, 7)
        End Function
        ''' <summary>Desempaca un SByte en 8 valores booleanos.</summary>
        ''' <param name="Packet">SByte a desempacar</param>
        Public Function ReadBoolPacket(ByVal Packet As SByte) As Boolean()
            Return ReadBoolPacket2(Packet, 7)
        End Function
        ''' <summary>Desempaca un UShort en 16 valores booleanos.</summary>
        ''' <param name="Packet">UShort a desempacar</param>
        Public Function ReadBoolPacket(ByVal Packet As UShort) As Boolean()
            Return ReadBoolPacket2(Packet, 15)
        End Function
        ''' <summary>Desempaca un Short en 16 valores booleanos.</summary>
        ''' <param name="Packet">Short a desempacar</param>
        Public Function ReadBoolPacket(ByVal Packet As Short) As Boolean()
            Return ReadBoolPacket2(Packet, 15)
        End Function
        ''' <summary>Desempaca un UInteger en 32 valores booleanos.</summary>
        ''' <param name="Packet">UInteger a desempacar</param>
        Public Function ReadBoolPacket(ByVal Packet As UInteger) As Boolean()
            Return ReadBoolPacket2(Packet, 31)
        End Function
        ''' <summary>Desempaca un Integer en 32 valores booleanos.</summary>
        ''' <param name="Packet">Integer a desempacar</param>
        Public Function ReadBoolPacket(ByVal Packet As Integer) As Boolean()
            Return ReadBoolPacket2(Packet, 31)
        End Function
        ''' <summary>Desempaca un ULong en 64 valores booleanos.</summary>
        ''' <param name="Packet">ULong a desempacar</param>
        Public Function ReadBoolPacket(ByVal Packet As ULong) As Boolean()
            Return ReadBoolPacket2(Packet, 63)
        End Function
        ''' <summary>Desempaca un Long en 64 valores booleanos.</summary>
        ''' <param name="Packet">Long a desempacar</param>
        Public Function ReadBoolPacket(ByVal Packet As Long) As Boolean()
            Return ReadBoolPacket2(Packet, 63)
        End Function
        ''' <summary>Desempaca una String en valores booleanos.</summary>
        ''' <param name="Packet">String a desempacar</param>
        Public Function ReadBoolPacket(ByVal Packet As String) As Boolean()()
            Dim lst As New List(Of Boolean())
            For Each j As Char In Packet
                lst.Add(ReadBoolPacket(j))
            Next
            Return lst.ToArray
        End Function
        ''' <summary>Desempaca un Char en 8 valores booleanos.</summary>
        ''' <param name="Packet">Char a desempacar</param>
        Public Function ReadBoolPacket(ByVal Packet As Char, Optional ByVal AsUnicode As Boolean = False) As Boolean()
            Return ReadBoolPacket(CByte(Asc(Packet) Mod 256))
        End Function
        Private Function ReadBoolPacket2(ByVal Packet As Object, lenght As SByte) As Boolean()
            Dim Output(lenght) As Boolean
            For j As SByte = 0 To lenght Step 1
                Output(j) = (Packet And 2 ^ j) <> 0
            Next
            Return Output
        End Function
        ''' <summary>Empaca un arreglo bidimensional de Boolean en un arreglo de Bytes.</summary>
        ''' <param name="Values">Parámetro necesario. Especifica la información a empacar.</param>
        ''' <returns>Un arreglo de Bytes empacando la información original.</returns>
        ''' <remarks></remarks>
        Public Function BoolPacketByte(ParamArray Values()() As Boolean) As Byte()
            Dim lst As New List(Of Byte), tmp As Byte = 0
            For Each j As Boolean() In Values
                If j.Length <> 8 Then Throw New ArgumentException(String.Format(My.Resources.Strings.Err32, 8), "Values")
                For k As Byte = 0 To 7
                    tmp += 2 ^ k And j(k)
                Next
                lst.Add(tmp)
                tmp = 0
            Next
            Return lst.ToArray
        End Function
        ''' <summary>
        ''' Calcula la parte decimal de un número Double
        ''' </summary>
        ''' <param name="Number">Necesario. Número del cual se desea obtener la parte decimal.</param>
        ''' <returns>Un Double que representa la parte decimal del número.</returns>
        Public Function GetDecimal(ByVal Number As Double) As Double
            Return Number - Math.Truncate(Number)
        End Function
        ''' <summary>
        ''' Calcula la parte entera de un número Double
        ''' </summary>
        ''' <param name="Number">Necesario. Número del cual se desea obtener la parte entera.</param>
        ''' <returns>Un Double que representa la parte entera del número.</returns>
        Public Function GetWhole(ByVal number As Double) As Double
            Return Math.Truncate(number)
        End Function
        ''' <summary>
        ''' Rellena una cadena con un caracter hasta que alcanza la longitud determinada
        ''' </summary>
        ''' <param name="s">Necesario. Es la cadena que se desea rellenar</param>
        ''' <param name="c">Necesario. Longitud deseada de la cadena</param>
        ''' <param name="f">Opcional. Carácter de relleno. Si se omite, se rellenará con espacios.</param>
        ''' <remarks></remarks>
        Public Sub FillString(ByRef s As String, ByRef c As UInteger, Optional ByRef f As Char = ChrW(32))
            Do Until Len(s) >= c
                s &= f
            Loop
        End Sub
        ''' <summary>
        ''' Devuelve una cadena rellenada con un caracter hasta que alcanza la longitud determinada
        ''' </summary>
        ''' <param name="s">Necesario. Es la cadena que se desea rellenar</param>
        ''' <param name="c">Necesario. Longitud deseada de la cadena</param>
        ''' <param name="f">Opcional. Carácter de relleno. Si se omite, se rellenará con espacios.</param>
        ''' <returns>Una cadena de por lo menos una longitud <paramref name="c">c</paramref> Rellena con el carácter <paramref name="f">f</paramref></returns>
        ''' <remarks></remarks>
        Public Function FilledString(ByVal s As String, ByRef c As UInteger, Optional ByRef f As Char = ChrW(32)) As String
            FillString(s, c, f)
            Return s
        End Function
        ''' <summary>
        ''' Calcula la potencia de dos más cercana menor o igual al número
        ''' </summary>
        ''' <param name="x">Número de entrada. Se buscará una potencia de dos menor o igual a este valor.</param>
        ''' <returns>Un valor ULong que es resultado de la operación.</returns>
        Public Function Nearest2Pow(x As UInteger) As ULong
            Dim c As ULong = 1
            Do Until c >= x
                c *= 2
            Loop
            Return c
        End Function
    End Module
    ''' <summary>
    ''' Añade al programa la funcionalidad de pila mediante PUSH y POP
    ''' </summary>
    Public Module PushPopSupport
        Private data As List(Of Object)
        ''' <summary>Agrega el valor del objeto a la pila.</summary>
        ''' <param name="a">Objeto del cual se desea guardar el valor</param>
        Public Sub Push(ByVal a As Object)
            data.Add(a)
        End Sub
        ''' <summary>Recupera el valor del último objeto de la pila.</summary>
        ''' <param name="a">Variable que almacenará al último objeto de la pila</param>
        Public Sub Pop(ByRef a As Object)
            If SP() = -1 Then Throw New Exceptions.StackUnderflowException
            a = data(SP())
            data.Remove(SP())
        End Sub
        ''' <summary>Devuelve la pila actual</summary>
        ''' <returns>Un List(Of Object) con los objetos almacenados en la pila</returns>
        Public Function Stack() As List(Of Object)
            Return data
        End Function
        ''' <summary>Obtiene el puntero de la pila.</summary>
        ''' <returns>La cantidad de objetos en la pila. Si la pila está vacía, se devuelve -1</returns>
        Public Function SP() As Integer
            Return data.Count - 1
        End Function
        ''' <summary>Elimina el contenido de la pila</summary>
        Public Sub ClearStack()
            data.Clear()
        End Sub
    End Module
End Namespace

Namespace DataTypes
    Namespace BinTools2
        Public Enum Size As Byte
            Null = 0
            Byte8 = 8
            Word = 16
            Byte24 = 24
            DWord = 32
            Byte40 = 40
            Byte48 = 48
            Byte56 = 56
            Qword = 64
        End Enum
        ''' <summary>MCA Binary Tools 2.0</summary>
        ''' <remarks>
        ''' Provee servicios binarios a los programas.
        ''' </remarks>
        Public Class Binary
            Private v(1) As Boolean
            Private sze As Size = Size.Null
            Property Bits As Size
                Get
                    Return sze
                End Get
                Set(value As Size)
                    Resize(value)
                End Set
            End Property
            Property Bit(index As Byte) As Boolean
                Get
                    If index > sze - 1 Then Throw New IndexOutOfRangeException
                    Return v(index)
                End Get
                Set(value As Boolean)
                    If index > sze - 1 Then Throw New IndexOutOfRangeException
                    v(index) = value
                End Set
            End Property
            Public Sub New(ByVal Size As Size, Optional ByVal n As ULong = 0)
                FromNumber(n, Size)
            End Sub
            Public Sub New(ByVal n As ValueType)
                Dim x As Type = n.GetType



            End Sub
            Public Sub Resize(ByVal Size As Size)
                If ((Size Mod 8) <> 0) Or Size > Size.Qword Then Throw New ArgumentOutOfRangeException("Size")
                sze = Size
                If sze = Size.Null Then ReDim v(1) Else ReDim Preserve v(Size - 1)
            End Sub
            ''' <summary>Establece el valor del binario desde un número.</summary>
            ''' <param name="n">Valor a establecer.</param>
            ''' <param name="size">Tamaño del binario.</param>
            ''' <remarks>
            ''' De forma predeterminada, el tamaño se establece en QWord, para albergar números
            ''' de 64 bits.
            ''' </remarks>
            ''' <exception cref="ArgumentOutOfRangeException"> Si se intenta establecer un valor mayor al que puede contener de acuerdo a size</exception>
            Public Sub FromNumber(ByVal n As ULong, Optional ByVal size As Size = Size.Qword)
                Resize(size)
                Dim j As Byte = 0
                Dim w(size) As Boolean
                Do Until n = 0
                    If j > size Then Throw New ArgumentOutOfRangeException("size")
                    v(j) = ((n Mod 2) = 1)
                    n \= 2
                    j += 1
                Loop
            End Sub
            ''' <summary>Reduce el tamaño de un binario a la menor longitud posible.</summary>
            ''' <remarks>
            ''' Si el valor del binario es cero, no se cambiará el tamaño.
            ''' </remarks>
            Public Sub Reduce()
                For j As SByte = sze - 1 To 0 Step -1
                    If v(j) Then
                        Resize(j + (8 - (j Mod 8)))
                        Exit Sub
                    End If
                Next
            End Sub
            ''' <summary>Desplaza los bits hacia la izquierda</summary>
            Public Sub ShiftLeft()
                Dim tmp As Boolean = v(sze - 1)
                For j As SByte = sze - 1 To 1 Step -1
                    v(j) = v(j - 1)
                Next
                v(0) = tmp
            End Sub
            ''' <summary>Desplaza los bits hacia la derecha</summary>
            Public Sub ShiftRight()
                Dim tmp As Boolean = v(0)
                For j As SByte = 0 To sze - 2
                    v(j) = v(j + 1)
                Next
                v(sze - 1) = tmp
            End Sub
            ''' <summary>Desplaza los bits X posiciones determinadas</summary>
            ''' <param name="X">
            ''' Cantidad de posiciones a desplazar. Si es mayor a cero, el
            ''' movimiento será hacia la derecha, si es menor a cero, será
            ''' hacia la izquierda.
            ''' </param>
            ''' <remarks>Si X es cero, no se realizará ninguna operación.</remarks>
            Public Sub Shift(ByVal X As SByte)
                Select Case X
                    Case Is < 0
                        For j As SByte = 0 To X Step -1
                            ShiftLeft()
                        Next
                    Case Is > 0
                        For j As SByte = 0 To X
                            ShiftRight()
                        Next
                End Select
            End Sub
            ''' <summary>Convierte el valor de esta instancia en la representación numérica equivalente.</summary>
            ''' <returns>Un número sin signo del tipo correspondiente con el valor del binario</returns>
            Public Function ToNumber() As Object
                Select Case sze
                    Case Size.Byte8
                        Dim D As Byte = 0
                        For J As SByte = 7 To 0 Step -1
                            If v(J) Then D += (2 ^ J)
                        Next
                        Return D
                    Case Size.Word
                        Dim D As UShort = 0
                        For J As SByte = 15 To 0 Step -1
                            If v(J) Then D += (2 ^ J)
                        Next
                        Return D
                    Case Size.Byte24, Size.DWord
                        Dim D As UInteger = 0
                        For J As SByte = sze - 1 To 0 Step -1
                            If v(J) Then D += (2 ^ J)
                        Next
                        Return D
                    Case Is >= Size.Byte40
                        Dim D As ULong = 0
                        For J As SByte = sze - 1 To 0 Step -1
                            If v(J) Then D += (2 ^ J)
                        Next
                        Return D
                End Select
                Return Nothing
            End Function
            ''' <summary>Convierte el valor de esta instancia en la representación de cadena equivalente.</summary>
            ''' <returns>Una cadena compuesta por la conversión a bytes del binario</returns>
            Public Overrides Function ToString() As String
                Dim chars As String = Nothing
                For j As Integer = 0 To (sze / 8) - 1
                    Dim D As Byte = 0
                    For k As Integer = 0 To 7
                        If v((j * 8) + k) Then D += (2 ^ (k))
                    Next
                    chars &= Chr(D)
                Next
                Return chars
            End Function
            ''' <summary>Convierte el binario a un formato compatible con Bintools 1.0</summary>
            ''' <returns>Una cadena de strings compuesta por unos y ceros compatibles con Bintools 1.0</returns>
            Public Function ToBintools1() As String
                Dim x As String = Nothing
                For J As SByte = 0 To sze - 1
                    If v(sze - 1 - J) Then x &= "1" Else x &= "0"
                Next
                Return x
            End Function
            Shared Operator +(ByVal a As Binary, ByVal b As Binary) As Binary
                Dim carry As Boolean
                Dim maxsize As Byte = a.Bits
                If b.Bits > a.Bits Then
                    maxsize = b.Bits
                    a.Resize(b.Bits)
                ElseIf a.Bits > b.Bits Then
                    b.Resize(a.Bits)
                End If
                Dim result As Binary = New Binary(maxsize)
                For j As SByte = 0 To maxsize - 1
                    result.Bit(j) = (a.Bit(j) Xor b.Bit(j)) Xor carry
                    carry = a.Bit(j) And b.Bit(j) Or ((carry And a.Bit(j)) Xor (carry And b.Bit(j)))
                Next
                If carry Then
                    If maxsize / 8 > 7 Then Throw New OverflowException
                    result.Resize(maxsize + 8)
                End If
                result.Reduce()
                Return result
            End Operator
            Shared Operator +(ByVal a As Binary, ByVal b As ULong) As Binary
                Dim x As New Binary(Size.Qword, b)
                x.Reduce()
                Return a + x
            End Operator
            Shared Operator -(ByVal a As Binary) As Binary
                Return (Not a) + 1
            End Operator
            Shared Operator -(ByVal a As Binary, ByVal b As ULong) As Binary
                Dim x As New Binary(Size.Qword, b)
                x.Reduce()
                Return a - x
            End Operator
            Shared Operator -(ByVal a As Binary, ByVal b As Binary) As Binary
                'Return a + MCARuntime.DataTypes.BinTools2.TwosComplement(b)

                'Cheapfix
                '========
                Dim x As ULong = a.ToNumber, y As ULong = b.ToNumber, maxsize As Byte = a.Bits
                If b.Bits > a.Bits Then
                    maxsize = b.Bits
                End If
                If x < y Then
                    x = Not x
                End If
                Return New Binary(maxsize, (x - y))
            End Operator
            Shared Operator And(ByVal a As Binary, ByVal b As Binary) As Binary
                Dim maxsize As Byte = a.Bits
                If b.Bits > a.Bits Then
                    maxsize = b.Bits
                    a.Resize(b.Bits)
                ElseIf a.Bits > b.Bits Then
                    b.Resize(a.Bits)
                End If
                Dim result As Binary = New Binary(maxsize)
                For j As SByte = 0 To maxsize - 1
                    result.Bit(j) = a.Bit(j) And b.Bit(j)
                Next
                Return result
            End Operator
            Shared Operator Or(ByVal a As Binary, ByVal b As Binary) As Binary
                Dim maxsize As Byte = a.Bits
                If b.Bits > a.Bits Then
                    maxsize = b.Bits
                    a.Resize(b.Bits)
                ElseIf a.Bits > b.Bits Then
                    b.Resize(a.Bits)
                End If
                Dim result As Binary = New Binary(maxsize)
                For j As SByte = 0 To maxsize - 1
                    result.Bit(j) = a.Bit(j) Or b.Bit(j)
                Next
                Return result
            End Operator
            Shared Operator Xor(ByVal a As Binary, ByVal b As Binary) As Binary
                Dim maxsize As Byte = a.Bits
                If b.Bits > a.Bits Then
                    maxsize = b.Bits
                    a.Resize(b.Bits)
                ElseIf a.Bits > b.Bits Then
                    b.Resize(a.Bits)
                End If
                Dim result As Binary = New Binary(maxsize)
                For j As SByte = 0 To maxsize - 1
                    result.Bit(j) = a.Bit(j) Xor b.Bit(j)
                Next
                Return result
            End Operator
            Shared Operator Not(ByVal a As Binary) As Binary
                Dim maxsize As Byte = a.Bits
                Dim result As Binary = New Binary(maxsize)
                For j As SByte = 0 To maxsize - 1
                    result.Bit(j) = Not a.Bit(j)
                Next
                Return result
            End Operator
            Shared Operator =(ByVal a As Binary, ByVal b As Binary) As Boolean
                Dim maxsize As Byte = a.Bits
                If b.Bits > a.Bits Then
                    maxsize = b.Bits
                    a.Resize(b.Bits)
                ElseIf a.Bits > b.Bits Then
                    b.Resize(a.Bits)
                End If
                For j As SByte = 0 To maxsize - 1
                    If a.Bit(j) <> b.Bit(j) Then Return False
                Next
                Return True
            End Operator
            Shared Operator <>(ByVal a As Binary, ByVal b As Binary) As Boolean
                Return Not (a = b)
            End Operator
        End Class
    End Namespace
End Namespace

Namespace AppServices
    ''' <summary>
    ''' Contiene rutinas de servicio básicas para los programas.
    ''' </summary>
    Public Module CommonServices
        ''' <summary>Contiene información del ensamblado del programa.</summary>
        ''' <remarks>
        ''' Esta clase provee información personalizada con datos que
        ''' se pueden usar dentro de MCA Runtime, y es una alternativa
        ''' a My.Application.Info.Version. Si no necesita usar estas
        ''' subrutinas con información de ensamblado, utilice la clase
        ''' My.Application.Info.Version de la manera habitual.
        ''' </remarks>
        Public Class AppInfo
            Implements IDisposable
            ''' <summary>
            ''' Describe el estado de completación del programa.
            ''' </summary>
            ''' <remarks>Entre más alta la numeración, menos completo es el código.</remarks>
            Public Enum ReleaseType As Byte
                Release = 0
                RC = 1
                Beta = 2
                TechBeta = 3
                Preview = 4
                Alpha = 5
                Milestone = 6
                Concept = 7
                Stub = 8
                Idea = 9
                CompilationTest = 10
                HelloWorld = 11
            End Enum

            'Los siguientes son los valores predeterminados de la instancia.
            Public Name As String = My.Application.Info.Title
            Public Release As ReleaseType = ReleaseType.Release
            Public Unstable As Boolean = False
#If DEBUG = -1 Then
            Public DebugBuild As Boolean = True
#Else
            Public DebugBuild As Boolean = False
#End If
            Public IsPlugin As Boolean = False
            Public Major As UShort = My.Application.Info.Version.Major
            Public Minor As UShort = My.Application.Info.Version.Minor
            Public Rev As UShort = My.Application.Info.Version.Revision
            Public Build As UShort = My.Application.Info.Version.Build
            Public Description As String = Nothing
            Public ExtDescription As String = Nothing
            Public Copyright As String = My.Application.Info.Copyright
#If CopyrightProtect Then
            Public RestrictedSource As Boolean = True
#Else
            Public RestrictedSource As Boolean = False
#End If
            Private disposedValue As Boolean
            Public Sub New(ByRef x As ApplicationServices.AssemblyInfo)
                SetVer(x.Version)
                Name = x.Title
            End Sub
            Public Sub New()
            End Sub
            Public Sub SetVer(ByVal X As Version)
                Major = X.Major
                Minor = X.Minor
                Rev = X.Revision
                Build = X.Build
            End Sub
            Public Function GetVerStr() As String
                Return Major & "." & Minor & "." & Rev & "." & Build
            End Function
            Public Function GetVer() As Version
                Return New Version(GetVerStr())
            End Function
            Public Function VersionString() As String
                Dim str As String = String.Format(My.Resources.Strings.Build, Name, Major, Minor, Rev, Build)
                If Release <> 0 Then str &= Space(1) & Release.ToString
                If Unstable = True Then
                    str &= My.Resources.Strings.Unstable
                End If
                If DebugBuild = True Then
                    str &= My.Resources.Strings.Debug
                End If
                Return str
            End Function
            Public Overrides Function ToString() As String
                Return GetVerStr()
            End Function
            Public Function Path() As String
                Return Reflection.Assembly.GetExecutingAssembly.Location
            End Function
            Public Sub Dispose() Implements IDisposable.Dispose
                If Not disposedValue Then
                    Name = Nothing
                    Release = 0
                    Unstable = False
                    DebugBuild = False
                    Major = 0
                    Minor = 0
                    Rev = 0
                    Build = 0
                End If
                disposedValue = True
                GC.SuppressFinalize(Me)
            End Sub
            Shared Operator +(ByVal a As AppInfo, ByVal b As AppInfo) As AppInfo
                Dim x As New AppInfo
                With x
                    .Name = a.Name & ", " & b.Name
                    .Release = DataHandling.TheLowestOf(a.Release, b.Release)
                    .Major = a.Major + b.Major
                    .Minor = a.Minor + b.Minor
                    .Rev = a.Rev + b.Rev
                    .Build = a.Build + b.Build
                    .Description = a.Description
                    If .Description <> Nothing AndAlso b.Description <> Nothing Then .Description &= ", "
                    .Description &= b.Description
                    .ExtDescription = a.ExtDescription
                    If .ExtDescription <> Nothing And b.ExtDescription <> Nothing Then .ExtDescription &= Chr(13) & Chr(10)
                    .ExtDescription &= b.ExtDescription
                    .DebugBuild = a.DebugBuild Or b.DebugBuild
                    .Unstable = a.Unstable Or b.Unstable
                    .IsPlugin = a.IsPlugin Or b.IsPlugin
                End With
                Return x
            End Operator
            Shared Operator -(ByVal a As AppInfo, ByVal b As AppInfo) As AppInfo
                Dim x As New AppInfo
                With x
                    .Name = b.Name
                    .Release = DataHandling.TheHighestOf(a.Release, b.Release)
                    .Major = Math.Abs(a.Major - b.Major)
                    .Minor = Math.Abs(a.Minor - b.Minor)
                    .Rev = Math.Abs(a.Rev - b.Rev)
                    .Build = Math.Abs(a.Build - b.Build)
                    .Description = b.Description
                    .ExtDescription = b.ExtDescription
                    .DebugBuild = a.DebugBuild Xor b.DebugBuild
                    .Unstable = a.Unstable Xor b.Unstable
                    .IsPlugin = a.IsPlugin Xor b.IsPlugin
                End With
                Return x
            End Operator
            Shared Operator =(ByVal a As AppInfo, b As AppInfo) As Boolean
                Return a.Major = b.Major _
                    AndAlso a.Minor = b.Minor _
                    AndAlso a.Rev = b.Rev _
                    AndAlso a.Build = b.Build _
                    AndAlso a.Release = b.Release
            End Operator
            Shared Operator <(ByVal a As AppInfo, b As AppInfo) As Boolean
                Select Case a.Major
                    Case b.Major
                        Select Case a.Minor
                            Case b.Minor
                                Select Case a.Rev
                                    Case b.Rev
                                        Select Case a.Build
                                            Case Is > b.Build
                                                Return False
                                            Case Is < b.Build
                                                Return True
                                        End Select
                                    Case Is > b.Rev
                                        Return False
                                    Case Is < b.Rev
                                        Return True
                                End Select
                            Case Is > b.Minor
                                Return False
                            Case Is < b.Minor
                                Return True
                        End Select
                    Case Is > b.Major
                        Return False
                    Case Is < b.Major
                        Return True
                End Select
                Return False
            End Operator
            Shared Operator >(ByVal a As AppInfo, b As AppInfo) As Boolean
                Select Case a.Major
                    Case b.Major
                        Select Case a.Minor
                            Case b.Minor
                                Select Case a.Rev
                                    Case b.Rev
                                        Select Case a.Build
                                            Case Is > b.Build
                                                Return True
                                            Case Is < b.Build
                                                Return False
                                        End Select
                                    Case Is > b.Rev
                                        Return True
                                    Case Is < b.Rev
                                        Return False
                                End Select
                            Case Is > b.Minor
                                Return True
                            Case Is < b.Minor
                                Return False
                        End Select
                    Case Is > b.Major
                        Return True
                    Case Is < b.Major
                        Return False
                End Select
                Return False
            End Operator
            Shared Operator <=(ByVal a As AppInfo, b As AppInfo) As Boolean
                Return Not (a > b)
            End Operator
            Shared Operator >=(ByVal a As AppInfo, b As AppInfo) As Boolean
                Return Not (a < b)
            End Operator
            Shared Operator <>(ByVal a As AppInfo, b As AppInfo) As Boolean
                Return Not (a = b)
            End Operator
        End Class
        ''' <summary>Permite abrir imágenes con un cuadro de diálogo</summary>
        ''' <param name="Into">Cargar la imagen en ésta variable. Debe ser del tipo String</param>
        ''' <param name="DefaultImage">Opcional. Ruta predeterminada de la imagen. Debe ser string.</param>
        ''' <returns>
        ''' Un entero que representa si el usuario abrió o canceló el proceso
        ''' </returns>
        ''' <remarks>
        ''' La imagen cargada será devuelta a través del argumento <paramref name="Into"></paramref>
        ''' </remarks>
        Public Function LoadPict(ByRef Into As String, Optional ByVal DefaultImage As String = Nothing) As Microsoft.VisualBasic.MsgBoxResult
            With New Windows.Forms.OpenFileDialog
                .FileName = DefaultImage
                .Title = My.Resources.Strings.SelectImage
                .Filter = My.Resources.Strings.OFDFilter
                .ReadOnlyChecked = True
                .CheckFileExists = True
                .AddExtension = True
                .DefaultExt = "jpg"
                .Multiselect = False
                .ValidateNames = True
                If .ShowDialog() = MsgBoxResult.Ok Then
                    Into = .FileName
                    Return MsgBoxResult.Ok
                Else
                    Return MsgBoxResult.Cancel
                End If
            End With
        End Function
    End Module
    Public Module Logging
        Public Sub Logit(ByVal msg As String, Optional ByVal LogFile As String = Nothing)
            If LogFile = Nothing Then LogFile = My.Resources.Strings.DefaultLogFile
            Try
                My.Computer.FileSystem.WriteAllText(LogFile, String.Format(My.Resources.Strings.LogStr, Date.Now, msg & Chr(13) & Chr(10)), True)
            Catch ex As Exception
                Throw ex
            End Try
        End Sub
    End Module
    ''' <summary>
    ''' Contiene interfaces, subrutinas y funciones para
    ''' implementar Plugins de MCARuntime.
    ''' </summary>
    Public Module PluginSupport
        Private Const PluginExt As String = "*.dll"
        ''' <summary>
        ''' Especifica subrutinas, funciones y propiedades para
        ''' crear Plugins de MCARuntime.
        ''' </summary>
        ''' <remarks>
        ''' Esta interfaz debe ser heredada por otras interfaces
        ''' excepto la clase genérica PluginInfo, quien puede
        ''' heredarla directamente.
        ''' </remarks>
        Public Interface PluginInt
            Inherits IDisposable
            ''' <summary>
            ''' Envía argumentos a un Plugin de MCARuntime
            ''' </summary>
            ''' <param name="Args">Lista de argumentos a pasar</param>
            ''' <remarks>
            ''' Es posible que algunos Plugins no requieran parámetros.
            ''' En ese caso, deje este procedimiento vacío.
            ''' </remarks>
            Sub ParseArguments(ByVal ParamArray Args() As Object)
            ''' <summary>Devuelve el nombre de la Interfaz utilizada por la clase cargada.</summary>
            ''' <returns>Una cadena con el nombre de una Interfaz definida en MCARuntime</returns>
            ReadOnly Property MainInterface As String
        End Interface
        ''' <summary>Define un plugin genérico de MCARuntime</summary>
        ''' <remarks>
        ''' Los plugins genéricos están formados por llamadas a procedimientos
        ''' mediante strings, por lo que pueden contener diversas funciones no
        ''' especificadas dentro de una interfaz, pero al usar dicho plugin,
        ''' debe poseer una copia de todos los métodos contenidos en éste,
        ''' debidamente descritos.
        ''' </remarks>
        Public Interface GenericPluginInt
            Inherits PluginInt
            Sub CallSub(ByVal Instruction As String, ParamArray Args() As Object)
            Function CallFunction(ByVal Instruction As String, ParamArray Args() As Object) As Object
            Sub SetProperty(ByVal PropertyName As String, Value As Object)
            Function GetProperty(ByVal PropertyName As String) As Object
            Event Something(ByVal sender As Object, ByVal e As Object)
            Sub RaiseEvtSomething(sender As Object, e As Object)
        End Interface
        Public MustInherit Class GenericPlugin
            Inherits Plugin
            Implements GenericPluginInt
            Protected Class2Exec As Object
            Public Overrides ReadOnly Property MainInterface As String
                Get
                    Return "MCARuntime.AppServices.PluginSupport.GenericPluginInt"
                End Get
            End Property
            Public Function CallFunction(Instruction As String, ParamArray Args() As Object) As Object Implements GenericPluginInt.CallFunction
                Return CallByName(Class2Exec, Instruction, CallType.Method, Args)
            End Function
            Public Sub CallSub(Instruction As String, ParamArray Args() As Object) Implements GenericPluginInt.CallSub
                CallByName(Class2Exec, Instruction, CallType.Method, Args)
            End Sub
            Public Function GetProperty(PropertyName As String) As Object Implements GenericPluginInt.GetProperty
                Return CallByName(Class2Exec, PropertyName, CallType.Get)
            End Function
            Public Sub SetProperty(PropertyName As String, Value As Object) Implements GenericPluginInt.SetProperty
                CallByName(Class2Exec, PropertyName, CallType.Set, Value)
            End Sub
            Public Event Something(sender As Object, e As Object) Implements GenericPluginInt.Something
            Public Sub RaiseEvtSomething(sender As Object, e As Object) Implements GenericPluginInt.RaiseEvtSomething
                RaiseEvent Something(sender, e)
            End Sub
        End Class
        ''' <summary>Define la interfaz de información de plugin de MCARuntime</summary>
        ''' <remarks>
        ''' Todos los plugins de MCARuntime contienen esta clase, con la cual es
        ''' posible obtener infomación del plugin.
        ''' </remarks>
        Public Interface PluginInfo
            Inherits PluginInt
            ''' <summary>
            ''' Devuelve True si el plugin contiene más de una clase cargable además de la clase Info.
            ''' </summary>
            ReadOnly Property IsMultiClass As Boolean
            ''' <summary>Devuelve una lista de clases contenidas en el plugin.</summary>
            ''' <returns>Un arreglo de sólo lectura de String con el nombre de las clases públicas contenidas en el ensamblado.</returns>
            Function ContainedClasses() As ObjectModel.ReadOnlyCollection(Of String)
            ''' <summary>Devuelve la información AppInfo del Plugin.</summary>
            ''' <returns>Un objeto MCARuntime.AppServices.AppInfo con la
            ''' información de identificación y versión del Plugin.</returns>
            ReadOnly Property Info As AppInfo
            ''' <summary>Devuelve una lista de las interfaces implementadas en un plugin.</summary>
            Function Interfaces() As ObjectModel.ReadOnlyCollection(Of String)
            ''' <summary>Devuelve el nombre del ensamblado de un plugin.</summary>
            ReadOnly Property AssemblyFileName As String
            ''' <summary>Devuelve el índice de la clase predeterminada contenida en el plugin.</summary>
            Function DefaultClassIndex() As Integer
            ''' <summary>Devuelve la clase predeterminada contenida en el plugin.</summary>
            Function DefaultClass() As String
            ''' <summary>Devuelve el índice de la clase deseada, para poder realizar búsquedas en otros listados relacionados.</summary>
            ''' <param name="ClassName">Nombre de clase a buscar.</param>
            Function IndexOfClass(ClassName As String) As Integer
            ''' <summary>Devuelve una lista de las clases públicas existentes en un plugin.</summary>
            Function ContClasses() As ObjectModel.ReadOnlyCollection(Of String)
        End Interface
        ''' <summary>Define una clase abstracta para que PluginIdentification la herede.</summary>
        ''' <remarks>
        ''' Esta clase debe ser heredada por la clase PluginIdentification contenida dentro del
        ''' ensamblado de un plugin de MCARuntime. Una vez heredada, es posible
        ''' personalizar la información de la instancia, como descripción y versión.
        ''' </remarks>
        Public MustInherit Class PluginIdentification
            Implements PluginInfo
            Protected Unstable As Boolean = False
            Protected ReleaseType As AppInfo.ReleaseType = AppInfo.ReleaseType.Release
            Protected ExtDescription As String
            Protected FileName As String = Reflection.Assembly.GetExecutingAssembly.Location
            Protected IsPlugin As Boolean = True
#If CopyrightProtect Then
            Protected RestrictedSource As Boolean = True
#Else
            Protected RestrictedSource As Boolean = False
#End If
            Private disposedValue As Boolean
            ''' <summary>Devuelve la información AppInfo del Plugin.</summary>
            ''' <returns>Un objeto MCARuntime.AppServices.AppInfo con la
            ''' información de identificación y versión del Plugin.</returns>
            Public ReadOnly Property Info As AppInfo Implements PluginInfo.Info
                Get
                    Dim RTInfo As AppInfo = New AppInfo
                    With FileVersionInfo.GetVersionInfo(AssemblyFileName)
                        RTInfo.Name = .FileDescription
                        RTInfo.Major = .FileMajorPart
                        RTInfo.Minor = .FileMinorPart
                        RTInfo.Rev = .FileBuildPart
                        RTInfo.Build = .FilePrivatePart
                        RTInfo.DebugBuild = .IsDebug
                        RTInfo.Unstable = Unstable
                        RTInfo.Release = ReleaseType
                        RTInfo.Description = .Comments
                        RTInfo.ExtDescription = ExtDescription
                        RTInfo.IsPlugin = IsPlugin
                        RTInfo.RestrictedSource = RestrictedSource
                    End With
                    Return RTInfo
                End Get
            End Property
            Public ReadOnly Property MainInterface As String Implements PluginInt.MainInterface
                Get
                    Return "MCARuntime.AppServices.PluginSupport.PluginInfo"
                End Get
            End Property
            Public Sub ParseArguments(ParamArray Args() As Object) Implements PluginInt.ParseArguments
            End Sub
            Protected Overridable Sub Dispose(disposing As Boolean)
                If Not disposedValue Then
                    If disposing Then
                    End If
                    ExtDescription = Nothing
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
            ''' <summary>Devuelve una lista de clases contenidas en el plugin.</summary>
            ''' <returns>Un arreglo de sólo lectura de String con el nombre de las clases públicas contenidas en el ensamblado.</returns>
            Public Function ContainedClasses() As ObjectModel.ReadOnlyCollection(Of String) Implements PluginInfo.ContainedClasses
                Dim X As New List(Of String)
                If RestrictedSource Then
                    Return New ObjectModel.ReadOnlyCollection(Of String)({Info.Copyright})
                End If
                Return ContClasses()
            End Function
            Friend Function ContClasses() As ObjectModel.ReadOnlyCollection(Of String) Implements PluginInfo.ContClasses
                Dim X As New List(Of String)
                Dim Y As Reflection.Assembly = Reflection.Assembly.LoadFrom(AssemblyFileName)
                For Each t As Type In Y.GetTypes()
                    If t.IsPublic Then X.Add(t.FullName)
                Next
                Return New ObjectModel.ReadOnlyCollection(Of String)(X)
            End Function
            Public Function Interfaces() As ObjectModel.ReadOnlyCollection(Of String) Implements PluginInfo.Interfaces
                Dim X As New List(Of String)
#If CopyrightProtect Then
                X.Add(Info.Copyright)
#Else
                Dim y As Integer
                If Not Info.RestrictedSource Then
                    For Each w As String In ContClasses()
                        y = w.IndexOf(".") + 1
                        Try
                            X.Add(LoadPlugIn(FileName, w.Substring(y)).MainInterface)
                        Catch
                            X.Add(String.Format(My.Resources.Strings.Err29, w.Substring(y)))
                        End Try
                    Next
                Else
                    X.Add(Info.Copyright)
                End If
#End If
                Return New ObjectModel.ReadOnlyCollection(Of String)(X)
            End Function
            ''' <summary>
            ''' Devuelve True si el plugin contiene más de una clase cargable además de la clase Info.
            ''' </summary>
            Public ReadOnly Property IsMultiClass As Boolean Implements PluginInfo.IsMultiClass
                Get
                    Dim y As Integer, z As Integer = 0
                    For Each x As String In ContainedClasses()
                        y = x.IndexOf(".") + 1
                        If Not (x.Substring(y).Contains(".") Or x.Substring(y).Contains("+")) Then
                            z += 1
                            If z > 2 Then Return True
                        End If
                    Next
                    Return False
                End Get
            End Property
            Public ReadOnly Property AssemblyFileName As String Implements PluginInfo.AssemblyFileName
                Get
                    Return FileName
                End Get
            End Property
            Public Overridable Function DefaultClassIndex() As Integer Implements PluginInfo.DefaultClassIndex
                For Each J As String In ContClasses()
                    If Not J.EndsWith(".Info") Then Return IndexOfClass(J.Substring(J.IndexOf(".") + 1))
                Next
                Return IndexOfClass("Info")
            End Function
            Public Overridable Function DefaultClass() As String Implements PluginInfo.DefaultClass
                For Each J As String In ContClasses()
                    If Not J.EndsWith(".Info") Then Return J.Substring(J.IndexOf(".") + 1)
                Next
                Return "Info"
            End Function
            Public Function IndexOfClass(ByVal ClassName As String) As Integer Implements PluginInfo.IndexOfClass
                Dim y As ObjectModel.ReadOnlyCollection(Of String) = ContClasses()
                Dim x As Integer = y.Count - 1
                For j As Integer = 0 To x Step 1
                    If y(j).Substring(y(j).IndexOf(".") + 1) = ClassName Then Return j
                Next
                Return -1
            End Function
        End Class
        Public MustInherit Class Plugin
            Implements PluginInt
            Protected Initialized As Boolean = False, disposedValue As Boolean
            Public MustOverride ReadOnly Property MainInterface As String Implements PluginInt.MainInterface
            Public Overridable Sub ParseArguments(ParamArray Args() As Object) Implements PluginInt.ParseArguments
                Initialized = True
            End Sub
            Protected Overridable Sub Dispose(disposing As Boolean)
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
        ''' <summary>Carga un plugin de MCARuntime</summary>
        ''' <param name="LoadPath">Ruta de archivo DLL a cargar.</param>
        ''' <param name="ClassToLoad">Clase a cargar.</param>
        ''' <returns>
        ''' Un objeto Plugin con la clase especificada.
        ''' </returns>
        ''' <remarks>
        ''' Si <paramref name="ClassToLoad"></paramref> no se especifica,
        ''' se cargará la primer clase encontrada. También puede cargar
        ''' explícitamente la clase Info para obtener información
        ''' específica del Plugin.
        ''' </remarks>
        ''' <exception cref="MCARuntime.Exceptions.PluginNeededException">
        ''' Ocurre cuando no es posible encontrar el archivo DLL, o no se
        ''' especifica un archivo válido.
        ''' </exception>
        ''' <exception cref="MCARuntime.Exceptions.PluginNotImplementedClassException">
        ''' Ocurre cuando <paramref name="ClassToLoad"></paramref> no
        ''' existe en el ensamblado.
        ''' </exception>
        ''' <exception cref="MCARuntime.Exceptions.PluginException">
        ''' Se produce si no es posible cargar un Plugin de MCARuntime
        ''' por alguna otra razón. La excepción obedece a diferentes
        ''' causas. Por ejemplo, un Plugin que posee un solo constructor
        ''' con parámetros opcionales constituye un error, debido a que
        ''' el sistema no puede cargar una clase de un ensamblado con
        ''' estas características.
        ''' </exception>
        Public Function LoadPlugIn(ByVal LoadPath As String, Optional ByVal ClassToLoad As String = "") As PluginInt
            If Not FileIO.FileSystem.FileExists(LoadPath) Then
                Throw New Exceptions.PluginNeededException(LoadPath)
            End If
            Try
                If ClassToLoad = "" Then
                    Dim x As PluginInfo = LoadPlugIn(LoadPath, "Info")
                    ClassToLoad = x.DefaultClass
                    x = Nothing
                End If
                Dim asm As Reflection.Assembly = Reflection.Assembly.LoadFrom(LoadPath)
                Dim myType As Type = asm.GetType(asm.GetName.Name + "." + ClassToLoad, False, True)
                If GetType(PluginInt).IsAssignableFrom(myType) Then
                    Dim plugin As PluginInt = CType(Activator.CreateInstance(myType), PluginInt)
                    Return plugin
                Else
                    Throw New Exceptions.InvalidPluginClassException(ClassToLoad)
                End If
            Catch ex As Exception
                Throw New Exceptions.PluginException(LoadPath + ", " + ClassToLoad)
            End Try
            Throw New Exceptions.PluginNotImplementedClassException(ClassToLoad)
        End Function
        ''' <summary>Carga cualquier plugin utilizable.</summary>
        ''' <param name="InterfaceToLoad">Interfaz a cargar.</param>
        ''' <param name="PreferredDLL">Plugin preferido. Si se omite, se devolverá el primer plugin utilizable encontrado.</param>
        ''' <returns>El primer plugin utilizable por la interfaz <paramref name="InterfaceToLoad"></paramref> especificada.</returns>
        ''' <exception cref="MCARuntime.Exceptions.PluginNeededException">
        ''' Ocurre si no es posible encontrar un plugin utilizable por la interfaz <paramref>ClassToLoad</paramref>.
        ''' </exception>
        Public Function LoadAnyPlugin(ByVal InterfaceToLoad As String, Optional ByVal PreferredDLL As String = "") As PluginInt
            If IsValidPlugin(PreferredDLL, InterfaceToLoad) Then Return LoadPlugIn(PreferredDLL, InterfaceToLoad)
            Dim x As ObjectModel.ReadOnlyCollection(Of String) = ListOfPlugins(InterfaceToLoad)
            For Each j As String In x
                If IsValidPlugin(j, InterfaceToLoad) Then Return LoadPlugIn(j, InterfaceToLoad)
            Next
            Throw New Exceptions.PluginNeededException(InterfaceToLoad)
        End Function
        ''' <summary>Devuelve una lista de plugins de MCARuntime.</summary>
        ''' <param name="Interf">
        ''' Interfaz a comprobar. Si no se especifica Interf, sólo se
        ''' comprobará que el Plugin sea de MCARuntime.
        ''' </param>
        ''' <param name="path">
        ''' Ruta de directorio a comprobar. Si no se especifica Path,
        ''' se buscarán plugins en el directorio principal.</param>
        ''' <returns>Un arreglo de String con las rutas completas de
        ''' archivo hacia los Plugins encontrados.</returns>
        Public Function ListOfPlugins(Optional ByVal Interf As String = "", Optional ByVal path As String = ".") As ObjectModel.ReadOnlyCollection(Of String)
            Dim Lst As ObjectModel.ReadOnlyCollection(Of String) = FileIO.FileSystem.GetFiles(path, 2, PluginExt)
            Dim loaded As PluginInt
            Dim rlst As New List(Of String)
            For Each PluginDLL As String In Lst
                Try
                    loaded = LoadPlugIn(PluginDLL)
                    If loaded IsNot Nothing Then
                        If Interf = "" Or loaded.MainInterface = Interf Then
                            rlst.Add(PluginDLL)
                        End If
                    End If
                Catch : End Try
            Next
            Return New ObjectModel.ReadOnlyCollection(Of String)(rlst)
        End Function
        ''' <summary>
        ''' Verifica si el Plugin especificado es válido.
        ''' </summary>
        ''' <param name="DLLPath">Ruta de archivo DLL a cargar.</param>
        ''' <param name="InterfaceToLoad">Clase a cargar.</param>
        ''' <returns>
        ''' True si el archivo es un plugin válido y si contiene la
        ''' clase especificada, en caso contrario, False.
        ''' </returns>
        Public Function IsValidPlugin(ByVal DLLPath As String, ByVal InterfaceToLoad As String) As Boolean
            Try
                Dim x As PluginInfo = LoadPlugIn(DLLPath, "Info")
                For Each y As String In x.Interfaces
                    If y = InterfaceToLoad Then Return True
                Next
            Catch : End Try
            Return False
        End Function
        ''' <summary>
        ''' Verifica si el Plugin especificado es válido.
        ''' </summary>
        ''' <param name="DLLPath">Ruta de archivo DLL a cargar.</param>
        ''' <returns>
        ''' True si el archivo es un plugin válido, en caso contrario,
        ''' False.
        ''' </returns>
        Public Function IsValidPlugin(ByVal DLLPath As String) As Boolean
            Try
                If LoadPlugIn(DLLPath) IsNot Nothing Then Return True
            Catch : End Try
            Return False
        End Function
    End Module

    ''' <summary>
    ''' Administra el acceso a archivos.
    ''' </summary>
    Public Module FileManager
        ''' <summary>Devuelve el nombre de archivo sin extensión de la ruta especificada.</summary>
        ''' <param name="FilePath">Ruta/archivo a comprobar</param>
        ''' <returns>Una String con el nombre de archivo encontrado, sin extensión.</returns>
        ''' <remarks>Se devolverá una cadena vacía si se especifica una ruta de carpeta</remarks>
        Public Function FileNameNoExt(ByVal FilePath As String) As String
            Dim x As String = FileName(FilePath)
            Return x.Substring(0, x.LastIndexOf("."))
        End Function

        ''' <summary>Devuelve el nombre de archivo de la ruta especificada.</summary>
        ''' <param name="FilePath">Ruta/archivo a comprobar</param>
        ''' <returns>Una String con el nombre de archivo encontrado, incluida su extensión.</returns>
        ''' <remarks>Se devolverá una cadena vacía si se especifica una ruta de carpeta</remarks>
        Public Function FileName(ByVal FilePath As String) As String
            Return FilePath.Substring(FilePath.LastIndexOf("\") + 1)
        End Function

        ''' <summary>Devuelve la extensión de archivo de la ruta especificada.</summary>
        ''' <param name="FilePath">Ruta/archivo a comprobar</param>
        ''' <returns>Una String con la extensión de archivo encontrada.</returns>
        ''' <remarks>Se devolverá una cadena vacía si se especifica una ruta de carpeta,
        ''' o si no es posible encontrar una extensión valida.</remarks>
        Public Function Extension(ByVal FilePath As String) As String
            Dim x As String = FileName(FilePath)
            Dim y As Short = x.LastIndexOf(".")
            If y < 1 Or y = x.Length - 1 Then Return Nothing
            Return x.Substring(y)
        End Function

        ''' <summary>Devuelve la ruta de archivo de la cadena especificada.</summary>
        ''' <param name="FilePath">Ruta a archivo a comprobar</param>
        ''' <returns>Una String con la ruta de archivo encontrada, sin el nombre de archivo.</returns>
        ''' <remarks>Se devolverá una cadena vacía si <paramref name="FilePath">
        ''' </paramref> no contiene rutas.</remarks>
        Public Function FileDir(ByVal FilePath As String) As String
            Return FilePath.Substring(0, FilePath.LastIndexOf("\") + 1)
        End Function

        ''' <summary>
        ''' Crea un directorio
        ''' </summary>
        ''' <param name="path">Ruta a crear. Debido a restricciones es los sistemas operativos Windows, debe ser menor a 255 caracteres.</param>
        Public Sub MakeDir(ByVal path As String)
            If path.Length > 255 Then Throw New Exceptions.InvalidArgumentException("path")
            Dim x As String = Nothing
            Try
                For Each j As String In Split(path, "\")
                    x &= j & "\"
                    If Not FileIO.FileSystem.DirectoryExists(x) Then FileIO.FileSystem.CreateDirectory(x)
                Next
            Catch ex As Exception : Throw ex
            End Try
        End Sub
        ''' <summary>
        ''' Extensión de la clase System.IO.FileStream
        ''' </summary>
        Public Class FileStreamEx
            Inherits FileStream
            Public Sub New(path As String, mode As FileMode)
                MyBase.New(path, mode)
            End Sub
            Public Function ReadString(qty As Integer) As String
                Dim buff As String = Nothing
                Try
                    For j As Integer = 1 To qty
                        buff &= Chr(ReadByte)
                    Next
                Catch ex As Exception
                    Throw ex
                End Try
                Return buff
            End Function
            Public Function ReadToEnd() As String
                Try
                    Return ReadTo(Length)
                Catch ex As Exception
                    Throw ex
                End Try
            End Function
            Public Function ReadTo(ByVal Pos As Long) As String
                Dim buff As String = Nothing
                Try
                    If Pos < Position Then
                        DataHandling.Swap(Pos, Position)
                    End If
                    For j As Integer = 1 To Pos - Position Step 1
                        buff &= Chr(ReadByte)
                    Next
                Catch ex As Exception
                    Throw ex
                End Try
                Return buff
            End Function
            Public Sub WriteString(StrToWrite As String)
                Try
                    For Each j As Char In StrToWrite.ToCharArray
                        WriteByte(Asc(j))
                    Next
                Catch ex As Exception
                    Throw ex
                End Try
            End Sub
            Public Sub WriteBytes(ParamArray Bytes As Byte())
                Try
                    Write(Bytes, Position, Bytes.Length)
                Catch ex As Exception
                    Throw ex
                End Try
            End Sub
            Public Sub WriteSeveralBytes(ParamArray Arrays()() As Byte)
                Try
                    For Each bytes As Byte() In Arrays
                        Write(bytes, Position, bytes.Length)
                    Next
                Catch ex As Exception
                    Throw ex
                End Try
            End Sub
            Public Sub Skip(BytesToSkip As Integer)
                Try
                    Seek(BytesToSkip, 1)
                Catch ex As Exception
                    Throw ex
                End Try
            End Sub
            Public Sub Destroy()
                Try
                    SetLength(0)
                Catch ex As Exception
                    Throw ex
                End Try
            End Sub
        End Class

        Public Function ReadString(ByRef File As FileStream, qty As Integer) As String
            Dim buff As String = Nothing
            Try
                For j As Integer = 1 To qty
                    buff &= Chr(File.ReadByte)
                Next
            Catch : End Try
            Return buff
        End Function
        Public Sub Writestring(ByRef File As FileStream, StrToWrite As String)
            Try
                For Each j As Char In StrToWrite.ToCharArray
                    File.WriteByte(Asc(j))
                Next
            Catch : End Try
        End Sub
    End Module
End Namespace

Namespace FileFormats
    Public Module Containers
        Public Interface ContainerInt
            Sub Add(ByRef Index As String, ByRef Data As String)
            Sub ChangeData(ByRef Index As String, ByRef Data As String)
            Sub CreateFile()
            Sub DeleteData(ByRef Index As String)
            Sub ChangeTag(ByRef Tag As String)
            Function GetData(ByRef Index As String) As String
            Function GetTag()
            Function FileName() As String
            Function RegCount() As UInteger
            Function InternalVersion() As Byte
            Function ParseAndCheck() As Boolean
        End Interface
        Public Interface ContainerPluginInt
            Inherits AppServices.PluginInt
            Inherits ContainerInt
        End Interface
        Public MustInherit Class ContainerPlugin
            Inherits AppServices.Plugin
            Implements ContainerPluginInt
            Protected Writer As BinaryWriter
            Protected Reader As BinaryReader
            'Protected FStream As System.IO.FileStream
            Public Overrides ReadOnly Property MainInterface As String
                Get
                    Return "MCARuntime.FileFormats.Containers.ContainerPluginInt"
                End Get
            End Property
            Public MustOverride Sub Add(ByRef Index As String, ByRef Data As String) Implements ContainerInt.Add
            Public MustOverride Sub ChangeData(ByRef Index As String, ByRef Data As String) Implements ContainerInt.ChangeData
            Public MustOverride Sub ChangeTag(ByRef Tag As String) Implements ContainerInt.ChangeTag
            Public MustOverride Sub CreateFile() Implements ContainerInt.CreateFile
            Public MustOverride Sub DeleteData(ByRef Index As String) Implements ContainerInt.DeleteData
            Public MustOverride Function GetData(ByRef Index As String) As String Implements ContainerInt.GetData
            Public Overridable Function GetTag() As Object Implements ContainerInt.GetTag
                Throw New Exceptions.FeatureNotAvailableException
            End Function
            Public MustOverride Function InternalVersion() As Byte Implements ContainerInt.InternalVersion
            Public MustOverride Function RegCount() As UInteger Implements ContainerInt.RegCount
            Public MustOverride Function ParseAndCheck() As Boolean Implements ContainerInt.ParseAndCheck
            Public MustOverride Function FileName() As String Implements ContainerInt.FileName
        End Class
    End Module
End Namespace

Namespace VMachines
    Public Class Console
        Private buffer As String()
        Property Buffers As Byte
            Get
                Return buffer.Length - 1
            End Get
            Set(value As Byte)
                ReDim buffer(value + 1)
            End Set
        End Property
        Public Sub Cls()
            For Each j As String In buffer
                j = Nothing
            Next
        End Sub
        Public Sub PrintLn(ParamArray ExpressionList() As String)
            For Each j As String In ExpressionList
                Print(j)
                DataHandling.ShiftArray(buffer, -1)
            Next
        End Sub
        Public Sub Print(Expression As String)
            For Each j As Char In Expression.ToCharArray
                Select Case j
                    Case Chr(13)
                        DataHandling.ShiftArray(buffer, -1)
                    Case Chr(11)
                        DataHandling.ShiftArray(buffer, -8)
                    Case Chr(10)
                        buffer(Buffers) = Nothing
                    Case Chr(9)
                        buffer(Buffers) = Space(8)
                    Case Chr(8)
                        buffer(Buffers) = buffer(Buffers).Substring(0, buffer(Buffers).Length - 1)
                    Case Else
                        If buffer(Buffers).Length = 80 Then
                            DataHandling.ShiftArray(buffer, -1)
                        End If
                        buffer(Buffers) &= j
                End Select
            Next
        End Sub
        Public ReadOnly Property Output As String()
            Get
                Return buffer
            End Get
        End Property
        Sub New(Optional ByVal InitialBuffers As Byte = 25)
            Buffers = InitialBuffers
        End Sub
        Public Function ToBitmap() As Drawing.Bitmap
            Dim x As New VirtualScreen
            For Each j As String In buffer
                x.PrintLn(j)
            Next
            ToBitmap = x.Screen
            x = Nothing
        End Function
    End Class
    Public Class MouseStatus
        Private mx As Short, moy As Short
        Private btns() As Boolean
        ReadOnly Property X As Short
            Get
                Return mx
            End Get
        End Property
        ReadOnly Property Y As Short
            Get
                Return moy
            End Get
        End Property
        ReadOnly Property Buttons As Boolean()
            Get
                Return btns
            End Get
        End Property
        Sub New(ByVal x As Short, ByVal y As Short, ParamArray buttons() As Boolean)
            mx = x
            moy = y
            btns = buttons
        End Sub
    End Class
    Public Interface VM
        Sub RunVM()
        ReadOnly Property ConsoleOutput As Console
        Sub SendMouseInput(ByRef x As Integer, y As Integer, ByVal ParamArray buttons() As Boolean)
        ReadOnly Property GraphicsOutput As Drawing.Bitmap
        Sub HardReset()
        Sub SoftReset()
        Sub SetProgram(ByRef x As String)
        Event VMStopped(Output As String)
        Event VMException(Err As Exception)
        Event WaitingInput(Msg As String)
        Event KeystrokeOccurred(key As String)
        Event KeyRode(key As String)
        Event MouseEvent(ByRef Mstat As MouseStatus)
        Sub SendKeyPress(e As Windows.Forms.KeyPressEventArgs)
        Function IsKeyBufferEmpty() As Boolean
        Sub ClearKeyBuffer()
        Function GetLastKey() As Char
        Sub RaiseEvtVMStopped(Output As String)
        Sub RaiseEvtVMException(Err As Exception)
        Sub RaiseEvtWaitingInput(Msg As String)
        Sub RaiseEvtKeyStrokeOccurred(Key As String)
        Sub RaiseEvtKeyRode(Key As String)
        Sub RaiseEvtMouseEvent(ByRef Mstat As MouseStatus)
        Sub ClearConsole()
        Property ConsoleBuffers As Byte
    End Interface
    Public Interface VMPluginInt
        Inherits VM
        Inherits AppServices.PluginInt
    End Interface
    Public MustInherit Class VMPlugin
        Inherits AppServices.Plugin
        Implements VMPluginInt

        ''' <summary>Permite acceso al programador para escribir sobre el búffer de pantalla de texto.</summary>
        ''' <remarks>La propiedad púbica que expone la salida de consola es ConsoleOutput</remarks>
        Protected ConsoleOut As New Console

        ''' <summary>Permite acceso al programador para dibujar sobre una superficie gráfica.</summary>
        ''' <remarks>La propiedad púbica que expone la salida de consola es GraphicsOutput.
        ''' Además, el tamaño predeterminado del canvas es de 720x400 píxeles</remarks>
        Protected GraphicsOut As New Drawing.Bitmap(720, 400)

        ''' <summary>Almacena las teclas pulsadas aún no procesadas.</summary>
        Private KeyBuffer As New List(Of Char)

        ''' <summary>Devuelve True si el búffer de teclado está vacío.</summary>
        ''' <returns>Un valor booleano que es igual a True si el búffer de teclado está vacío, en caso contrario, False</returns>
        Protected Function IsKeyBufferEmpty() As Boolean Implements VM.IsKeyBufferEmpty
            Return (KeyBuffer.Count = 0)
        End Function

        ''' <summary>
        ''' Devuelve la última tecla presionada sin procesar.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Function GetLastKey() As Char Implements VM.GetLastKey
            If Not IsKeyBufferEmpty() Then
                GetLastKey = KeyBuffer(KeyBuffer.Count - 1)
                KeyBuffer.RemoveAt(KeyBuffer.Count - 1)
            Else : Return Nothing
            End If
        End Function

        ''' <summary>Añade una pulsación al búffer de teclado.</summary>
        ''' <param name="e">Carácter presionado.</param>
        ''' <remarks>
        ''' Puede utilizar ésta subrutina dentro de un controlador de
        ''' eventos, para que las pulsaciones sean enviadas de forma
        ''' automática al búffer.
        ''' </remarks>
        Public Sub SendKeyPress(e As Windows.Forms.KeyPressEventArgs) Implements VM.SendKeyPress
            KeyBuffer.Add(e.KeyChar)
            RaiseEvent KeystrokeOccurred(e.KeyChar.ToString)
        End Sub

        ''' <summary>Elimina el contenido del buffer de teclado.</summary>
        Public Sub ClearKeyBuffer() Implements VM.ClearKeyBuffer
            KeyBuffer.Clear()
        End Sub

        ''' <summary>Especifica el programa a ejecutar.</summary>
        ''' <remarks>
        ''' Está al programador la disposición de tratar ésta variable como
        ''' código binario o ruta de archivo con código.
        ''' </remarks>
        Protected Program As String

        ''' <summary>Describe un estado de mouse para ser usado por una máquina virtual.</summary>
        ''' <remarks>
        ''' Éste estado no está anclado al mouse directamente, es responsabilidad del programador
        ''' que el objeto contenga información actualizada.
        ''' </remarks>
        Protected Mouse As MouseStatus

        ''' <summary>Devuelve la consola como un objeto Console</summary>
        Public ReadOnly Property ConsoleOutput As Console Implements VM.ConsoleOutput
            Get
                Return ConsoleOut
            End Get
        End Property

        ''' <summary>
        ''' Devuelve la salida gráfica de la máquina virtual
        ''' </summary>
        ''' <returns>Un objeto System.Drawing.Bitmap con la salida de gráficos actual.</returns>
        ''' <remarks></remarks>
        Public ReadOnly Property GraphicsOutput As Drawing.Bitmap Implements VM.GraphicsOutput
            Get
                Return GraphicsOut
            End Get
        End Property
        Public MustOverride Sub HardReset() Implements VM.HardReset
        Public MustOverride Sub SoftReset() Implements VM.SoftReset
        Public MustOverride Sub RunVM() Implements VM.RunVM

        Public Sub SendMouseInput(ByRef x As Integer, y As Integer, ParamArray buttons() As Boolean) Implements VM.SendMouseInput
            Mouse = New MouseStatus(x, y, buttons)
            RaiseEvent MouseEvent(Mouse)
        End Sub
        Public Sub SetProgram(ByRef x As String) Implements VM.SetProgram
            Program = x
        End Sub
        Public Overrides ReadOnly Property MainInterface As String
            Get
                Return "MCARuntime.VMachines.VMPluginInt"
            End Get
        End Property

        Public Event VMStopped(Output As String) Implements VM.VMStopped
        Public Event WaitingInput(Msg As String) Implements VM.WaitingInput
        Public Event KeystrokeOccurred(key As String) Implements VM.KeystrokeOccurred
        Public Event KeyRode(key As String) Implements VM.KeyRode
        Public Event MouseEvent(ByRef Mstat As MouseStatus) Implements VM.MouseEvent
        Public Event VMException(Err As Exception) Implements VM.VMException
        Protected Sub RaiseEvtKeyStrokeOccurred(Key As String) Implements VM.RaiseEvtKeyStrokeOccurred
            RaiseEvent KeystrokeOccurred(Key)
        End Sub
        Protected Sub RaiseEvtKeyRode(Key As String) Implements VM.RaiseEvtKeyRode
            RaiseEvent KeystrokeOccurred(Key)
        End Sub
        Protected Sub RaiseEvtMouseEvent(ByRef Mstat As MouseStatus) Implements VM.RaiseEvtMouseEvent
            RaiseEvent MouseEvent(Mstat)
        End Sub
        Protected Sub RaiseEvtVMException(Err As Exception) Implements VM.RaiseEvtVMException
            RaiseEvent VMException(Err)
        End Sub
        Protected Sub RaiseEvtVMStopped(Output As String) Implements VM.RaiseEvtVMStopped
            RaiseEvent VMStopped(Output)
        End Sub
        Protected Sub RaiseEvtWaitingInput(Msg As String) Implements VM.RaiseEvtWaitingInput
            RaiseEvent WaitingInput(Msg)
        End Sub

        Public Sub ClearConsole() Implements VM.ClearConsole
            ConsoleOut.Cls()
        End Sub
        Public Property ConsoleBuffers As Byte Implements VM.ConsoleBuffers
            Get
                Return ConsoleOut.Buffers
            End Get
            Set(value As Byte)
                ConsoleOut.Buffers = value
            End Set
        End Property
        Private Sub Console2Graphics()
            GraphicsOut = ConsoleOut.ToBitmap
        End Sub
    End Class
    Public Enum VMStatus
        Undefined = 0
        Stopped = 1
        Running = 2
        Paused = 3
    End Enum
    Public Class VMRunner
        Implements IDisposable
        Private disposedValue As Boolean
        Private t As Threading.Thread = Nothing
        Private WithEvents v As VM
        Public Event VMStopped(sender As Object, Message As String)
        Public Event VMException(sender As Object, e As Exception)
        Public Event VMEvent(sender As Object, Message As String)
        Public Sub RunVM()
            t = New Threading.Thread(AddressOf v.RunVM)
            t.Start()
        End Sub
        Public Sub SetProgram(ByRef x As String)
            v.SetProgram(x)
        End Sub
        Public Overloads Sub SendKBInput(ByRef e As Windows.Forms.KeyPressEventArgs)
            v.SendKeyPress(e)
        End Sub
        Public Overloads Sub SendKBInput(ByRef str As String)
            For Each j As Char In str.ToCharArray
                v.SendKeyPress(New Windows.Forms.KeyPressEventArgs(j))
            Next
        End Sub
        Public Sub StopVM(Optional ByRef Msg As String = "")
            t.Abort()
            t = Nothing
            RaiseEvent VMStopped(Me, Msg)
        End Sub
        Public Sub ResetVM()
            StopVM()
            v.HardReset()
        End Sub
        Public Sub RestartVM()
            StopVM()
            v.SoftReset()
            RunVM()
        End Sub
        Public Function IsRunning() As Boolean
            If t Is Nothing Then Return False
            Return t.IsAlive
        End Function
        Sub New(Machine As VM)
            disposedValue = False
            t = Nothing
            v = Machine
        End Sub
        Sub New(ByVal DLLPath As String, Optional ByVal Machine As String = "")
            disposedValue = False
            t = Nothing
            If AppServices.IsValidPlugin(DLLPath, "MCARuntime.VMachines.VM") Then
                v = AppServices.LoadPlugIn(DLLPath, Machine)
            Else
                Throw New Exceptions.PluginException(DLLPath)
            End If
        End Sub
        Public Function ConsoleOutput() As String
            Return v.ConsoleOutput.Output(0)
        End Function
        Sub SendMouseInput(ByRef x As Integer, y As Integer, ByVal ParamArray buttons() As Boolean)
            v.SendMouseInput(x, y, buttons)
        End Sub
        Public Function GraphicsOutput() As Drawing.Bitmap
            Return v.GraphicsOutput
        End Function
        Private Sub VMStop(x As String) Handles v.VMStopped
            StopVM(x)
        End Sub
        Private Sub VMError(x As Exception) Handles v.VMException
            t = Nothing
            RaiseEvent VMException(Me, x)
        End Sub
        Private Sub VMWaiting(x As String) Handles v.WaitingInput 'añadir mas eventos aqui...
            RaiseEvent VMEvent(Me, x)
        End Sub
        Protected Sub Dispose() Implements IDisposable.Dispose
            If Not disposedValue Then
                ResetVM()
                t = Nothing
                v = Nothing
                disposedValue = True
            End If
            MyBase.Finalize()
        End Sub
    End Class
    Public Class VirtualScreen
        Public Enum ScreenModes As Byte
            Text = 0
            Mode12 = 12
            Mode13 = 13
        End Enum
        Public Property ScreenMode As ScreenModes
            Get
                Return ScrMode
            End Get
            Set(value As ScreenModes)
                Select Case value
                    Case ScreenModes.Text
                        W = 720
                        H = 400
                        D = 16
                    Case ScreenModes.Mode12
                        W = 640
                        H = 480
                        D = 16
                    Case ScreenModes.Mode13
                        W = 320
                        H = 240
                        D = 256
                    Case Else


                End Select
                ScrMode = value
                ScrBuffer = New Drawing.Bitmap(W, H)
                Drawing.Graphics.FromImage(ScrBuffer).Clear(Drawing.Color.Black)
            End Set
        End Property
        Private ScrMode As ScreenModes = ScreenModes.Text
        Private W As UShort = 720 'Ancho de pantalla
        Private H As UShort = 400 'Alto de pantalla
        Private D As UShort = 16 'Profundidad de color
        Private R As Byte = 1 'Línea de cursor
        Private C As Byte = 1 'Columna de cursor
        Private CursorVisible As Boolean = True ' cursor visible?
        Private ScrBuffer As New Drawing.Bitmap(W, H)
        Public Foreground As Drawing.Color = Drawing.Color.LightGray
        Public Backgound As Drawing.Color = Drawing.Color.Black
        Public ReadOnly Property Screen As Drawing.Bitmap
            Get
                Return ScrBuffer
            End Get
        End Property
        Public Sub Cls()
            Drawing.Graphics.FromImage(ScrBuffer).Clear(Backgound)
        End Sub
        Public Sub Print(ByVal Txt As String, Optional ByVal Fore As Drawing.Color = Nothing, Optional ByVal Backg As Drawing.Color = Nothing)
            'If Fore is Nothing then fore
            Using fb As New Drawing.SolidBrush(Fore), bb As New Drawing.SolidBrush(Backg)
                If Txt <> "" Then
                    With Drawing.Graphics.FromImage(ScrBuffer)
                        .DrawString(Space(Txt.Length), New Drawing.Font("Perfect DOS VGA 437", 8), bb, C * 8, R * 8)
                        .DrawString(Txt, New Drawing.Font("Perfect DOS VGA 437", 8), fb, C * 8, R * 8)
                    End With
                End If
            End Using
        End Sub
        Public Sub PrintLn(Optional ByVal Txt As String = "")
            Print(Txt & Chr(13))
        End Sub
        Public Sub Locate(Row As Byte, Column As Byte)
            R = Row
            C = Column
        End Sub
    End Class
End Namespace

Namespace Exceptions
    Public Class FeatureNotAvailableException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err36
            End Get
        End Property
    End Class
    Public Class FileClosedException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err7
            End Get
        End Property
    End Class
    Public Class FileDamagedException
        Inherits Exception
        Protected text As String
        Public Sub New()
        End Sub
        Public Sub New(name As String)
            text = name
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return String.Format(My.Resources.Strings.Err12, text)
            End Get
        End Property
    End Class
    Public Class FileNotFoundException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err5
            End Get
        End Property
    End Class
    Public Class IncorrectFileModeException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err8
            End Get
        End Property
    End Class
    Public Class InsufficientDataException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err30
            End Get
        End Property
    End Class
    Public Class InvalidDirectoryException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err24
            End Get
        End Property
    End Class
    Public Class InvalidFileNameException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err10
            End Get
        End Property
    End Class
    Public Class NotCompatibleDUFException
        Inherits Exception
        Protected mode As String
        Public Sub New()
        End Sub
        Public Sub New(File As String)
            mode = File
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Sub New(type As Byte)
            mode = "DUF " & type.ToString
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return String.Format(My.Resources.Strings.Err6, mode)
            End Get
        End Property
    End Class
    Public Class NotValidIXDException
        Inherits Exception
        Protected text As String
        Public Sub New()
        End Sub
        Public Sub New(File As String)
            text = File
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return String.Format(My.Resources.Strings.Err15, text)
            End Get
        End Property
    End Class
    Public Class UserNotFoundException
        Inherits Exception
        Protected text As String
        Public Sub New()
        End Sub
        Public Sub New(name As String)
            text = name
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return String.Format(My.Resources.Strings.Err13, text)
            End Get
        End Property
    End Class
    Public Class UnSetODBCException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err1
            End Get
        End Property
    End Class
    Public Class DataNotFoundException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err16
            End Get
        End Property
    End Class
    Public Class InvalidTableException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err2
            End Get
        End Property
    End Class
    Public Class InvalidKeyFieldException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err3
            End Get
        End Property
    End Class
    Public Class UnknownTypeException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err14
            End Get
        End Property
    End Class
    Public Class InvalidPasswordException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err4
            End Get
        End Property
    End Class
    Public Class VMExecutionException
        Inherits Exception
        Protected text As String
        Public Sub New()
        End Sub
        Public Sub New(VMErr As String)
            text = VMErr
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return String.Format(My.Resources.Strings.Err17 & ": {0}", text)
            End Get
        End Property
    End Class
    Public Class PluginException
        Inherits Exception
        Protected text As String
        Public Sub New()
            text = My.Resources.Strings.Err0
        End Sub
        Public Sub New(File As String)
            text = File
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return String.Format(My.Resources.Strings.Err18, text)
            End Get
        End Property
    End Class
    Public Class PluginInitializationException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err19
            End Get
        End Property
    End Class
    Public Class PluginNotInitializedException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err21
            End Get
        End Property
    End Class
    Public Class PluginNeededException
        Inherits Exception
        Protected text As String
        Public Sub New()
            text = My.Resources.Strings.Unknown
        End Sub
        Public Sub New(File As String)
            text = File
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return String.Format(My.Resources.Strings.Err22, text)
            End Get
        End Property
    End Class
    Public Class PluginNotImplementedClassException
        Inherits Exception
        Protected text As String
        Public Sub New()
            text = My.Resources.Strings.Unknown
        End Sub
        Public Sub New(File As String)
            text = File
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return String.Format(My.Resources.Strings.Err23, text)
            End Get
        End Property
    End Class
    Public Class InvalidArgumentException
        Inherits Exception
        Protected Arg As String
        Public Sub New()
            Arg = My.Resources.Strings.Specified
        End Sub
        Public Sub New(ArgumentName As String)
            Arg = ArgumentName
        End Sub
        Public Sub New(ArgumentName As String, message As String, inner As Exception)
            MyBase.New(message, inner)
            Arg = ArgumentName
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return String.Format(My.Resources.Strings.Err26, Arg)
            End Get
        End Property
    End Class
    Public Class MissingArgumentException
        Inherits Exception
        Protected Arg As String
        Public Sub New()
            Arg = My.Resources.Strings.Needed
        End Sub
        Public Sub New(ArgumentName As String)
            Arg = ArgumentName
        End Sub
        Public Sub New(ArgumentName As String, message As String, inner As Exception)
            MyBase.New(message, inner)
            Arg = ArgumentName
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return String.Format(My.Resources.Strings.Err27, Arg)
            End Get
        End Property
    End Class
    Public Class TooManyArgumentsException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err28
            End Get
        End Property
    End Class
    Public Class TooFewArgumentsException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err31
            End Get
        End Property
    End Class
    Public Class InvalidPluginClassException
        Inherits Exception
        Protected text As String
        Public Sub New()
            text = My.Resources.Strings.Err0
        End Sub
        Public Sub New(File As String)
            text = File
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return String.Format(My.Resources.Strings.Err29, text)
            End Get
        End Property
    End Class
    Public Class StackUnderflowException
        Inherits Exception
        Public Sub New()
        End Sub
        Public Sub New(message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(message As String, inner As Exception)
            MyBase.New(message, inner)
        End Sub
        Public Overrides ReadOnly Property Message As String
            Get
                Return My.Resources.Strings.Err34
            End Get
        End Property
    End Class
End Namespace