﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.0
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    'This class was auto-generated by the StronglyTypedResourceBuilder
    'class via a tool like ResGen or Visual Studio.
    'To add or remove a member, edit your .ResX file then rerun ResGen
    'with the /str option, or rebuild your VS project.
    '''<summary>
    '''  A strongly-typed resource class, for looking up localized strings, etc.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Class Strings
        
        Private Shared resourceMan As Global.System.Resources.ResourceManager
        
        Private Shared resourceCulture As Global.System.Globalization.CultureInfo
        
        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>  _
        Friend Sub New()
            MyBase.New
        End Sub
        
        '''<summary>
        '''  Returns the cached ResourceManager instance used by this class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Shared ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("MCARuntime.Strings", GetType(Strings).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Overrides the current thread's CurrentUICulture property for all
        '''  resource lookups using this strongly typed resource class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend Shared Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Acerca de {0}.
        '''</summary>
        Friend Shared ReadOnly Property About() As String
            Get
                Return ResourceManager.GetString("About", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Aplicación.
        '''</summary>
        Friend Shared ReadOnly Property Application() As String
            Get
                Return ResourceManager.GetString("Application", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to El registro &apos;{0}&apos; ya existe. ¿Desea modificarlo?.
        '''</summary>
        Friend Shared ReadOnly Property Ask1() As String
            Get
                Return ResourceManager.GetString("Ask1", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to ¿Desea eliminar el registro &apos;{0}&apos;?.
        '''</summary>
        Friend Shared ReadOnly Property Ask2() As String
            Get
                Return ResourceManager.GetString("Ask2", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to ¿Desea eliminar este registro?.
        '''</summary>
        Friend Shared ReadOnly Property Ask2a() As String
            Get
                Return ResourceManager.GetString("Ask2a", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Info. Runtime.
        '''</summary>
        Friend Shared ReadOnly Property AsmblyInfo() As String
            Get
                Return ResourceManager.GetString("AsmblyInfo", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to {0} v{1}.{2}.{3} (Compilación {4}).
        '''</summary>
        Friend Shared ReadOnly Property Build() As String
            Get
                Return ResourceManager.GetString("Build", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Cambios.
        '''</summary>
        Friend Shared ReadOnly Property changes() As String
            Get
                Return ResourceManager.GetString("changes", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Cerrar.
        '''</summary>
        Friend Shared ReadOnly Property Close() As String
            Get
                Return ResourceManager.GetString("Close", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to  - Versión de depuración.
        '''</summary>
        Friend Shared ReadOnly Property Debug() As String
            Get
                Return ResourceManager.GetString("Debug", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Depuración.
        '''</summary>
        Friend Shared ReadOnly Property Debugging() As String
            Get
                Return ResourceManager.GetString("Debugging", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Log.txt.
        '''</summary>
        Friend Shared ReadOnly Property DefaultLogFile() As String
            Get
                Return ResourceManager.GetString("DefaultLogFile", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Eliminar.
        '''</summary>
        Friend Shared ReadOnly Property Delete() As String
            Get
                Return ResourceManager.GetString("Delete", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Error.
        '''</summary>
        Friend Shared ReadOnly Property Err() As String
            Get
                Return ResourceManager.GetString("Err", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Error desonocido.
        '''</summary>
        Friend Shared ReadOnly Property Err0() As String
            Get
                Return ResourceManager.GetString("Err0", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Se debe especificar una conexión ODBC antes de crear objetos controladores.
        '''</summary>
        Friend Shared ReadOnly Property Err1() As String
            Get
                Return ResourceManager.GetString("Err1", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Nombre de archivo inválido.
        '''</summary>
        Friend Shared ReadOnly Property Err10() As String
            Get
                Return ResourceManager.GetString("Err10", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Acceso denegado.
        '''</summary>
        Friend Shared ReadOnly Property Err11() As String
            Get
                Return ResourceManager.GetString("Err11", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Archivo dañado: {0}.
        '''</summary>
        Friend Shared ReadOnly Property Err12() As String
            Get
                Return ResourceManager.GetString("Err12", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Usuario no encontrado: {0}.
        '''</summary>
        Friend Shared ReadOnly Property Err13() As String
            Get
                Return ResourceManager.GetString("Err13", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Tipo de datos no básico.
        '''</summary>
        Friend Shared ReadOnly Property Err14() As String
            Get
                Return ResourceManager.GetString("Err14", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Archivo IXD inválido {0}.
        '''</summary>
        Friend Shared ReadOnly Property Err15() As String
            Get
                Return ResourceManager.GetString("Err15", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to No se encontraron datos.
        '''</summary>
        Friend Shared ReadOnly Property Err16() As String
            Get
                Return ResourceManager.GetString("Err16", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Error de ejecución en la máquina virtual.
        '''</summary>
        Friend Shared ReadOnly Property Err17() As String
            Get
                Return ResourceManager.GetString("Err17", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Imposible cargar el plugin: {0}.
        '''</summary>
        Friend Shared ReadOnly Property Err18() As String
            Get
                Return ResourceManager.GetString("Err18", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to El Plugin no se puede inicializar.
        '''</summary>
        Friend Shared ReadOnly Property Err19() As String
            Get
                Return ResourceManager.GetString("Err19", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Nombre de tabla inválido.
        '''</summary>
        Friend Shared ReadOnly Property Err2() As String
            Get
                Return ResourceManager.GetString("Err2", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to No se ha cargado el Plugin.
        '''</summary>
        Friend Shared ReadOnly Property Err20() As String
            Get
                Return ResourceManager.GetString("Err20", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to El Plugin no está inicializado.
        '''</summary>
        Friend Shared ReadOnly Property Err21() As String
            Get
                Return ResourceManager.GetString("Err21", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Se necesita el Plugin {0}.
        '''</summary>
        Friend Shared ReadOnly Property Err22() As String
            Get
                Return ResourceManager.GetString("Err22", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to El plugin no implementa la clase {0}.
        '''</summary>
        Friend Shared ReadOnly Property Err23() As String
            Get
                Return ResourceManager.GetString("Err23", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Directorio inválido.
        '''</summary>
        Friend Shared ReadOnly Property Err24() As String
            Get
                Return ResourceManager.GetString("Err24", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Argumentos inválidos.
        '''</summary>
        Friend Shared ReadOnly Property Err25() As String
            Get
                Return ResourceManager.GetString("Err25", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to El argumento {0} está fuera del rango de valores, o contiene un valor inválido.
        '''</summary>
        Friend Shared ReadOnly Property Err26() As String
            Get
                Return ResourceManager.GetString("Err26", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Falta el argumento {0}.
        '''</summary>
        Friend Shared ReadOnly Property Err27() As String
            Get
                Return ResourceManager.GetString("Err27", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Demasiados argumentos.
        '''</summary>
        Friend Shared ReadOnly Property Err28() As String
            Get
                Return ResourceManager.GetString("Err28", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to La clase {0} no es un Plugin válido.
        '''</summary>
        Friend Shared ReadOnly Property Err29() As String
            Get
                Return ResourceManager.GetString("Err29", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Campo llave inválido.
        '''</summary>
        Friend Shared ReadOnly Property Err3() As String
            Get
                Return ResourceManager.GetString("Err3", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Datos insuficientes.
        '''</summary>
        Friend Shared ReadOnly Property Err30() As String
            Get
                Return ResourceManager.GetString("Err30", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Muy pocos argumentos.
        '''</summary>
        Friend Shared ReadOnly Property Err31() As String
            Get
                Return ResourceManager.GetString("Err31", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Tamaño de arreglo inválido, se esperaba un tamaño de {0} elementos.
        '''</summary>
        Friend Shared ReadOnly Property Err32() As String
            Get
                Return ResourceManager.GetString("Err32", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to El argumento {0} es inválido.
        '''</summary>
        Friend Shared ReadOnly Property Err33() As String
            Get
                Return ResourceManager.GetString("Err33", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to La pila está vacía.
        '''</summary>
        Friend Shared ReadOnly Property Err34() As String
            Get
                Return ResourceManager.GetString("Err34", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to El plugin no requiere argumentos..
        '''</summary>
        Friend Shared ReadOnly Property Err35() As String
            Get
                Return ResourceManager.GetString("Err35", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Característica no disponible..
        '''</summary>
        Friend Shared ReadOnly Property Err36() As String
            Get
                Return ResourceManager.GetString("Err36", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Contraseña incorrecta.
        '''</summary>
        Friend Shared ReadOnly Property Err4() As String
            Get
                Return ResourceManager.GetString("Err4", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Archivo no encontrado.
        '''</summary>
        Friend Shared ReadOnly Property Err5() As String
            Get
                Return ResourceManager.GetString("Err5", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to DUF no compatible: {0}.
        '''</summary>
        Friend Shared ReadOnly Property Err6() As String
            Get
                Return ResourceManager.GetString("Err6", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to El archivo está cerrado.
        '''</summary>
        Friend Shared ReadOnly Property Err7() As String
            Get
                Return ResourceManager.GetString("Err7", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Modo de archivo incorrecto.
        '''</summary>
        Friend Shared ReadOnly Property Err8() As String
            Get
                Return ResourceManager.GetString("Err8", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to No existen usuarios registrados.
        '''</summary>
        Friend Shared ReadOnly Property Err9() As String
            Get
                Return ResourceManager.GetString("Err9", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Error en la inicialización.
        '''</summary>
        Friend Shared ReadOnly Property ErrAtInit() As String
            Get
                Return ResourceManager.GetString("ErrAtInit", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Notas legales.
        '''</summary>
        Friend Shared ReadOnly Property LegalNote() As String
            Get
                Return ResourceManager.GetString("LegalNote", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to {0} -&gt; {1}.
        '''</summary>
        Friend Shared ReadOnly Property LogStr() As String
            Get
                Return ResourceManager.GetString("LogStr", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to necesario.
        '''</summary>
        Friend Shared ReadOnly Property Needed() As String
            Get
                Return ResourceManager.GetString("Needed", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Todos los archivos de imagen|*.jpg; *.png; *.bmp|Imagen JPEG|*.jpg|Imagen PNG|*.png|Imagen BMP|*.bmp|Todos los archivos|*.*.
        '''</summary>
        Friend Shared ReadOnly Property OFDFilter() As String
            Get
                Return ResourceManager.GetString("OFDFilter", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Guardar.
        '''</summary>
        Friend Shared ReadOnly Property Save() As String
            Get
                Return ResourceManager.GetString("Save", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Seleccionar imagen.
        '''</summary>
        Friend Shared ReadOnly Property SelectImage() As String
            Get
                Return ResourceManager.GetString("SelectImage", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to especificado.
        '''</summary>
        Friend Shared ReadOnly Property Specified() As String
            Get
                Return ResourceManager.GetString("Specified", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to desconocido.
        '''</summary>
        Friend Shared ReadOnly Property Unknown() As String
            Get
                Return ResourceManager.GetString("Unknown", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to  - Compilación inestable.
        '''</summary>
        Friend Shared ReadOnly Property Unstable() As String
            Get
                Return ResourceManager.GetString("Unstable", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Versión: .
        '''</summary>
        Friend Shared ReadOnly Property Version() As String
            Get
                Return ResourceManager.GetString("Version", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Programa inválido..
        '''</summary>
        Friend Shared ReadOnly Property vmErr1() As String
            Get
                Return ResourceManager.GetString("vmErr1", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to La máquina virtual solicita información..
        '''</summary>
        Friend Shared ReadOnly Property vmMsg1() As String
            Get
                Return ResourceManager.GetString("vmMsg1", resourceCulture)
            End Get
        End Property
    End Class
End Namespace