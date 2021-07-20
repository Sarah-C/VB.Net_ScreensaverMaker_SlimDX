
Imports SharpNeatLib.Maths
Imports SlimDX
Imports SlimDX.Direct3D9
Imports System.Reflection

Namespace SpriteEngine

    Friend Class ScreensaverSlimDX
        Inherits ScreensaverCore

        Friend device As SlimDX.Direct3D9.Device = Nothing
        Friend slimD3D As SlimDX.Direct3D9.Direct3D = Nothing

        Friend spriteManager As SpriteHandler.SpriteManager = Nothing
        Friend spriteCount As UInt64 = Nothing
        Friend FPSSpriteCount As UInt64 = Nothing

        Friend fontDraw As SlimDX.Direct3D9.Font = Nothing
        Friend systemFont As System.Drawing.Font = New System.Drawing.Font("Arial", 8.0F, System.Drawing.FontStyle.Regular)

        Friend updates As Integer = 0

        Friend rand As New FastRandom()
        Friend slowRand As New Random()
        Friend screenWidth As Integer = 0
        Friend screenHeight As Integer = 0

        Friend projection As PerspectiveProjection = Nothing

        Friend showStats As Boolean = False

        Public Sub New()
            MyBase.New()
            Try
                slimD3D = New Direct3D()
                SlimDX.Configuration.EnableObjectTracking = True
                AddHandler Initialize, AddressOf initializeScreensaver
                AddHandler Update, AddressOf updateScreensaver
                AddHandler [Exit], AddressOf exitScreensaver
            Catch e As Exception
                MsgBox(e.Message & vbCrLf & e.InnerException.Message)
                Application.Exit()
            End Try
        End Sub

        Private Sub initializeScreensaver(ByVal sender As Object, ByVal e As EventArgs)
            Dim pp As New PresentParameters()
            Dim primaryScreenName As String = Screen.PrimaryScreen.DeviceName
            Dim primaryScreenIndex As Integer = 0I
            Dim index As Integer = 0I
            For Each w As Window In windows
                If w.screen.DeviceName = primaryScreenName Then primaryScreenIndex = index
            Next
            If Me.mode <> ScreensaverMode.Normal Then
                pp.Windowed = True
                screenWidth = Me.window0.size.Width
                screenHeight = Me.window0.size.Height
            Else
                screenWidth = window0.form.Width 'Manager.Adapters(primaryScreenIndex).CurrentDisplayMode.Width
                screenHeight = window0.form.Height 'Manager.Adapters(primaryScreenIndex).CurrentDisplayMode.Height
                pp.Windowed = True
                pp.BackBufferCount = 1
                pp.BackBufferFormat = slimD3D.Adapters(primaryScreenIndex).CurrentDisplayMode.Format 'Format.A8R8G8B8'Manager.Adapters(primaryScreenIndex).CurrentDisplayMode.Format
            End If
            pp.BackBufferWidth = screenWidth
            pp.BackBufferHeight = screenHeight
            pp.SwapEffect = SwapEffect.Flip

            'Comment out to run VSynced.
            If VSync Then
                pp.PresentationInterval = PresentInterval.Default
            Else
                pp.PresentationInterval = PresentInterval.Immediate
            End If

            device = New Device(slimD3D, primaryScreenIndex, DeviceType.Hardware, windows(primaryScreenIndex).handle, CreateFlags.HardwareVertexProcessing, pp)
            device.SetRenderState(RenderState.AlphaBlendEnable, True)
            device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha)
            device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha)
            device.SetRenderState(RenderState.BlendOperation, BlendOperation.Add)
            device.SetRenderState(RenderState.ZEnable, False)

            projection = New PerspectiveProjection(virtualScreenHeight, screenHeight, screenWidth, screenWidth \ 2, screenHeight \ 2)

            spriteManager = New SpriteHandler.SpriteManager(projection)
            spriteManager.setDevice(device)

            window0.doubleBuffer = False
            fontDraw = New SlimDX.Direct3D9.Font(device, systemFont)
            rand.Reinitialise(Now.Millisecond)
            _start()
        End Sub

        Private Sub updateScreensaver(ByVal sender As Object, ByVal e As EventArgs)
            updates += 1
            spriteCount = CULng(spriteCount + spriteManager.sprites.Count)
            device.Clear(ClearFlags.Target, Color.FromArgb(255, 40, 40, 50).ToArgb, 0, 0)
            device.BeginScene()
            spriteManager.updateSprites()
            spriteManager.drawSprites()
            If showStats Then
                Dim writer As New System.IO.StringWriter()
                writer.WriteLine("Achieved framerate: " & Me.achievedFramerate)
                writer.WriteLine("Update count: " & updates)
                writer.WriteLine("Sprites Per Second: " & String.Format("{0:#,#}", FPSSpriteCount))
                fontDraw.DrawString(Nothing, writer.ToString(), 0, 0, Color.White)
            End If
            device.EndScene()
            device.Present()
            _update()
        End Sub

        Private Sub exitScreensaver(ByVal sender As Object, ByVal e As EventArgs)
            _stop()
            spriteManager.spriteRender.Dispose()
            slimD3D.Dispose()
            fontDraw.Dispose()
            device.Dispose()
            EmbeddedAssembly.delete("D3DX9_43.dll")
        End Sub

        Shared Sub bootUp(ByVal displayType As SpriteEngine.FullscreenMode, ByVal VSync As Boolean)
            ScreensaverCore.fullscreenMode = displayType
            ScreensaverCore.VSync = VSync
            EmbeddedAssembly.unpack("Screensaver.D3DX9_43.dll", "D3DX9_43.dll")
            EmbeddedAssembly.preLoad("Screensaver.FastRandomX64.dll", "FastRandomX64.dll")
            EmbeddedAssembly.preLoad("Screensaver.SlimDXX64.dll", "SlimDXX64.dll")
            AddHandler AppDomain.CurrentDomain.AssemblyResolve, AddressOf CurrentDomain_AssemblyResolve
        End Sub

        Private Shared Function CurrentDomain_AssemblyResolve(ByVal sender As Object, ByVal args As ResolveEventArgs) As System.Reflection.Assembly
            Return EmbeddedAssembly.Get(args.Name)
        End Function

        Public Sub spriteCountUpdate() Handles MyBase.OneSecondTick
            FPSSpriteCount = spriteCount
            spriteCount = 0
        End Sub


        Protected Overridable Sub _start()
        End Sub

        Protected Overridable Sub _update()
        End Sub

        Protected Overridable Sub _stop()
        End Sub

    End Class

End Namespace