Imports SharpNeatLib.Maths
Imports SlimDX
Imports SlimDX.Direct3D9
Imports System.Reflection

Namespace smallDemo

    Friend Class DemoScreensaver
        Inherits SpriteEngine.ScreensaverSlimDX

        Public screensaverSettings As ScreensaverSettings.screensaverSettings = Nothing

        Public snowStorm As SnowStorm = Nothing
        Public forest As Forest = Nothing

        Public showTree As Boolean = True

        Public Sub New(ByVal sss As ScreensaverSettings.screensaverSettings)
            screensaverSettings = sss
            If screensaverSettings.makeNewSettings Then SettingsCreate.makeNewSettings(screensaverSettings)
            showTree = screensaverSettings.getBool("Tree")
        End Sub

        <STAThread()>
        Shared Sub Main()
            Dim VSync As Boolean = True
            Dim screensaverSettings As ScreensaverSettings.screensaverSettings = New ScreensaverSettings.screensaverSettings()
            Dim displayMode As SpriteEngine.FullscreenMode = Nothing
            If screensaverSettings.getBool("MultiMonitor") Then
                displayMode = SpriteEngine.FullscreenMode.SingleWindow
            Else
                displayMode = SpriteEngine.FullscreenMode.MultipleWindows
            End If
            bootUp(displayMode, VSync)

            'Pass on our local settings object to the instantiation of the screensaver.
            Dim screensaverInstance As New DemoScreensaver(screensaverSettings)
            screensaverInstance.closeOnKeyboardInput = False
            screensaverInstance.windowedHeight = 500
            screensaverInstance.windowedWidth = 1024
            screensaverInstance.framerate = 1000 ' Can be lower than VSync refresh rate
            screensaverInstance.virtualScreenHeight = 500
            screensaverInstance.run()
        End Sub

        Protected Overrides Sub _start()
            snowStorm = New SnowStorm(spriteManager, projection)
            If showTree Then forest = New Forest(spriteManager, projection, screenWidth, screenHeight)
            AddHandler window0.KeyDown, AddressOf _keyPress
        End Sub

        Protected Overrides Sub _update()
            snowStorm.update()
            If showTree Then forest.update()
        End Sub

        Protected Overrides Sub _stop()
            snowStorm.snowFlakeTexture.Dispose()
            If showTree Then forest.treeTexture.Dispose()
        End Sub

        Public Sub _keyPress(ByVal sender As Object, ByVal e As KeyEventArgs)
            If e.KeyValue = 112 Then
                showStats = Not showStats ' F1 toggles stats display.
            Else
                window0.form.Close()
            End If
        End Sub

        Protected Overrides Sub showSettingsDialog()
            Dim f As New settingsForm()
            f.Show()
            While (f.Visible)
                Application.DoEvents()
            End While
        End Sub

    End Class

End Namespace