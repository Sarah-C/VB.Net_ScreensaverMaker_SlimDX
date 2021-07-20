Imports Microsoft.Win32

Public Class settingsForm

    Public screensaverSettings As New ScreensaverSettings.screensaverSettings()

#Region " Configure these as needed."

    Private Sub setupForm()
        Dim multiMonitorSetting As Boolean = screensaverSettings.getBool("MultiMonitor")
        Dim treeSetting As Boolean = screensaverSettings.getBool("Tree")

        radiobuttonMultiMonitorOn.Checked = multiMonitorSetting
        radiobuttonMultiMonitorOff.Checked = Not multiMonitorSetting
        pictureboxDisplayMode.Image = If(multiMonitorSetting, My.Resources.multiMonitorSettingsAll, My.Resources.multiMonitorSettingsOne)

        radiobuttonDisplayTree.Checked = treeSetting
        radiobuttonNoTree.Checked = Not treeSetting
    End Sub

    Private Sub saveForm()
        screensaverSettings.setBool("MultiMonitor", radiobuttonMultiMonitorOn.Checked)
        screensaverSettings.setBool("Tree", radiobuttonDisplayTree.Checked)
    End Sub

#End Region

#Region " Code that shouldn't need touching."

    Private Sub settingsForm_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        Dim name As System.Reflection.AssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName()
        Dim screensaverName As String = name.Name
        Dim registrySubKey As String = "ScreensaverMaker-" & screensaverName
        Me.Name = name.Name
        Me.Text = name.Name
        If screensaverSettings.makeNewSettings Then SettingsCreate.makeNewSettings(screensaverSettings)
        setupForm()
    End Sub

    Private Sub buttonOK_Click(sender As System.Object, e As System.EventArgs) Handles buttonOK.Click
        saveForm()
        closeEverything()
    End Sub

    Private Sub buttonCancel_Click(sender As System.Object, e As System.EventArgs) Handles buttonCancel.Click
        closeEverything()
    End Sub

    Private Sub closeEverything()
        screensaverSettings.mainKey.Close()
        Me.Close()
    End Sub

    Private Sub radiobuttonMultiMonitorOn_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles radiobuttonMultiMonitorOn.CheckedChanged
        pictureboxDisplayMode.Image = My.Resources.multiMonitorSettingsAll
    End Sub

    Private Sub radiobuttonMultiMonitorOff_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles radiobuttonMultiMonitorOff.CheckedChanged
        pictureboxDisplayMode.Image = My.Resources.multiMonitorSettingsOne
    End Sub

#End Region


End Class

