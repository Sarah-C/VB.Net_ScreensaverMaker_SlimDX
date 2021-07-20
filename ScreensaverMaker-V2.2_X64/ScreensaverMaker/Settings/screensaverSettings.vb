Imports Microsoft.Win32
Imports System.Reflection

Namespace ScreensaverSettings

    Public Class screensaverSettings

        Public name As AssemblyName = Nothing
        Public screensaverName As String = Nothing
        Public registrySubKey As String = Nothing
        Public mainKey As RegistryKey = Nothing
        Public makeNewSettings As Boolean = False

        Public Sub New()
            name = Assembly.GetExecutingAssembly().GetName()
            screensaverName = name.Name
            registrySubKey = "Software\ScreensaverMaker-" & screensaverName
            mainKey = My.Computer.Registry.CurrentUser.OpenSubKey(registrySubKey, True)
            If My.Computer.Registry.CurrentUser.OpenSubKey(registrySubKey) Is Nothing Then
                mainKey = My.Computer.Registry.CurrentUser.CreateSubKey(registrySubKey)
                makeNewSettings = True
            End If
        End Sub

        Public Sub setValue(ByVal key As String, ByVal value As String)
            mainKey.SetValue(key, value)
        End Sub

        Public Function getValue(ByVal key As String) As String
            Return CType(mainKey.GetValue(key), String)
        End Function

        Public Sub setBool(ByVal key As String, ByVal value As Boolean)
            mainKey.SetValue(key, onOff(value))
        End Sub

        Public Function getBool(ByVal key As String) As Boolean
            Return onOff(CType(mainKey.GetValue(key), String))
        End Function

        Public Function onOff(ByVal setting As Boolean) As String
            Return If(setting, "On", "Off")
        End Function

        Public Function onOff(ByVal setting As String) As Boolean
            Return setting = "On"
        End Function

    End Class

End Namespace