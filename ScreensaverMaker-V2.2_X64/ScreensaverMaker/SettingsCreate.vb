Public Class SettingsCreate

    Public Shared Sub makeNewSettings(ByVal screensaverSettings As ScreensaverSettings.screensaverSettings)

        screensaverSettings.setValue("MultiMonitor", "On")
        screensaverSettings.setValue("Tree", "On")

    End Sub

End Class
