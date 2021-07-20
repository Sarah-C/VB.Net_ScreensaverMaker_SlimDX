#Region "Usage notice"
'
' * Screensaver.cs
' * 
' * (c) Rei Miyasaka 2006
' * rei@thefraser.com
' * 
' * Last updated 2006.05.16
' * 
' * You may use this code for any purpose, in part or in whole, on two conditions:
' * 1. I cannot be held legally responsible for any damage or problems caused by this code.
' * 2. If you make something cool using this code, give me a shout and tell me about it.
' *
' 
#End Region

Imports System
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Collections
Imports System.Text
Imports System.Runtime.InteropServices

Namespace SpriteEngine

    ''' <summary>
    ''' Provides initialization, timing and windowing facilities for screensavers.
    ''' </summary>

    Public MustInherit Class ScreensaverCore
        Public windowedWidth As Integer = 800
        Public windowedHeight As Integer = 600
        Public virtualScreenHeight As Integer = 500
        Friend Shared VSync As Boolean = True
        ''' <summary>
        ''' Creates a new <see cref="Screensaver"/> with the given fullscreen mode.
        ''' </summary>
        ''' <param name="fullscreenMode">A value indicating the fullscreen windowing mode.</param>
        'Protected Sub New(ByVal fullscreenMode As FullscreenMode)
        '    Application.EnableVisualStyles()
        '    Application.SetCompatibleTextRenderingDefault(False)
        '    framerate = 60
        '    AddHandler framerateTimer.Elapsed, AddressOf framerateTimer_Elapsed
        '    framerateTimer.Start()
        'End Sub


        ''' <summary>
        ''' Creates a new <see cref="Screensaver"/> that runs one window per screen.
        ''' </summary>
        Protected Sub New()
            Application.EnableVisualStyles()
            Application.SetCompatibleTextRenderingDefault(False)
            framerate = 60
            AddHandler framerateTimer.Elapsed, AddressOf framerateTimer_Elapsed
            framerateTimer.Start()
        End Sub

        Private Sub framerateTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs)
            _achievedFramerate = updatesThisSec
            updatesThisSec = 0
            RaiseEvent OneSecondTick(Me, New EventArgs())
        End Sub

        ''' <summary>
        ''' Occurs before the screensaver windows close.
        ''' </summary>
        Public Event [Exit] As EventHandler

#Region "Multimedia Timer"

        <DllImport("winmm.dll")>
        Shared Function timeSetEvent(ByVal delay As Integer, ByVal resolution As Integer, ByVal callback As TimeCallback, ByVal user As Integer, ByVal mode As Integer) As Integer
        End Function

        <DllImport("winmm.dll")>
        Shared Function timeKillEvent(ByVal id As Integer) As Integer
        End Function

        <DllImport("winmm.dll")>
        Shared Function timeGetTime() As Integer
        End Function

        Public Delegate Sub TimeCallback(ByVal id As UInteger, ByVal msg As UInteger, ByVal user As IntPtr, ByVal param1 As IntPtr, ByVal param2 As IntPtr)
        Public timerCallbackHandle As TimeCallback
        Private timerId As Integer

        Private Sub startUpdating()
            timerCallbackHandle = New TimeCallback(AddressOf timerCallback)
            'TIME_KILL_SYNCHRONOUS = 0x0100
            'TIME_PERIODIC = 0x0001
            timerId = timeSetEvent(CInt(Math.Truncate(1000 / CDbl(_framerate))), 0, timerCallbackHandle, 0, &H101)
            Do While timerCallbackHandle IsNot Nothing
                updateEventManual.WaitOne()
                doUpdate()
                Application.DoEvents()
                updateEventManual.Reset()
            Loop
        End Sub

        Private Sub stopUpdating()
            timerCallbackHandle = Nothing
            timeKillEvent(timerId)
            updateEventManual.WaitOne()
        End Sub

        Private updateEventManual As New System.Threading.ManualResetEvent(False)

        Private Sub timerCallback(ByVal id As UInteger, ByVal msg As UInteger, ByVal user As IntPtr, ByVal param1 As IntPtr, ByVal param2 As IntPtr)
            updateEventManual.Set()
        End Sub

        Private framerateTimer As New System.Timers.Timer(1000)

        ''' <summary>
        ''' Occurs once each second on a thread separate from the window thread.
        ''' </summary>
        Public Event OneSecondTick As EventHandler

        Private _framerate As Integer

        ''' <summary>
        ''' Gets or sets the target framerate.
        ''' </summary>
        Public Property framerate() As Integer
            Get
                Return _framerate
            End Get
            Set(ByVal value As Integer)
                If value < 0 Then
                    Throw New ArgumentOutOfRangeException()
                End If
                If timerCallbackHandle IsNot Nothing Then
                    stopUpdating()
                    _framerate = value
                    startUpdating()
                Else
                    _framerate = value
                End If
            End Set
        End Property

#End Region

        <StructLayout(LayoutKind.Sequential)>
        Public Structure RECT
            Public left, top, right, bottom As Integer
        End Structure

        <DllImport("user32.dll")>
        Shared Function GetClientRect(ByVal handle As IntPtr, ByRef rect As RECT) As Boolean
        End Function
        <DllImport("user32.dll")>
        Shared Function IsWindowVisible(ByVal handle As IntPtr) As Boolean
        End Function

        Private Shared Function getClientRect(ByVal handle As IntPtr) As Rectangle
            Dim rect As RECT
            getClientRect(handle, rect)
            Return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom)
        End Function

        ''' <summary>
        ''' Occurs when the screensaver should process its logic and render.
        ''' </summary>
        Public Event Update As EventHandler

        Private Event PreUpdate As EventHandler
        Private Event PostUpdate As EventHandler

        Private _achievedFramerate As Integer
        Private updatesThisSec As Integer

        ''' <summary>
        ''' Actual framerate achieved. This value is updated once each second.
        ''' </summary>
        Public ReadOnly Property achievedFramerate() As Integer
            Get
                Return _achievedFramerate
            End Get
        End Property

        Private Sub doUpdate()
            If screensaverMode = screensaverMode.Preview AndAlso (Not IsWindowVisible(windowHandle)) Then
                stopUpdating()
                RaiseEvent [Exit](Me, New EventArgs())
                previewShutdownEvent.Set()
                Return
            End If
            RaiseEvent PreUpdate(Me, New EventArgs())
            RaiseEvent Update(Me, New EventArgs())
            RaiseEvent PostUpdate(Me, New EventArgs())
            updatesThisSec += 1
        End Sub

        Private Function processCommandLine() As ScreensaverMode
            Dim args() As String = Environment.GetCommandLineArgs()
            If args.Length = 1 AndAlso hasScreensaverFilenameExtension Then Return screensaverMode.Settings
            If args.Length < 2 Then Return screensaverMode.Windowed
            'Throw New FormatException()
            'End If
            If args(1).ToLower().StartsWith("/c") Then Return screensaverMode.Settings
            Select Case args(1).ToLower()
                Case "w"
                    Return screensaverMode.Windowed
                Case "/s"
                    Return screensaverMode.Normal
                Case "/p"
                    If args.Length < 3 Then Throw New FormatException()
                    Try
                        windowHandle = CType(UInteger.Parse(args(2)), IntPtr)
                        Return screensaverMode.Preview
                    Catch e1 As FormatException
                        Throw New FormatException()
                    End Try
                Case Else
                    Throw New FormatException()
            End Select
        End Function

        Private ReadOnly Property hasScreensaverFilenameExtension() As Boolean
            Get
                Return System.IO.Path.GetExtension(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Equals(".scr", StringComparison.InvariantCultureIgnoreCase)
            End Get
        End Property

        ''' <summary>
        ''' Start the screensaver in windowed mode if the file extension is not scr, unless a mode is specified in the command line.
        ''' Otherwise, if the file extension is scr, start the screensaver in config mode.
        ''' </summary>
        Public Sub run()
            run(screensaverMode.Windowed)
        End Sub

        ''' <summary>
        ''' Start the screensaver in the specified mode unless one is specified in the command line.
        ''' </summary>
        ''' <param name="mode">The mode in which to run the screensaver. This value cannot be <see cref="ScreensaverMode.Preview"/>.</param>
        Public Sub run(ByVal mode As ScreensaverMode)
            If mode = screensaverMode.Preview AndAlso windowHandle = IntPtr.Zero Then
                Throw New ArgumentException("Cannot explicity run in preview mode", "mode")
            End If

            If isEnded Then Throw New Exception("This screensaver has already finished running")
            Try
                Me.screensaverMode = processCommandLine()
            Catch e1 As FormatException
                Me.screensaverMode = mode
            End Try


            'screensaverMode=newSprites.ScreensaverMode.Normal

            Try
                Select Case screensaverMode
                    Case screensaverMode.Windowed
                        runWindowed()
                    Case screensaverMode.Settings
                        showSettingsDialog()
                    Case screensaverMode.Normal
                        If Not closeOnMouseMoveOverride Then
                            _closeOnMouseMove = True
                        End If
                        If Not closeOnClickOverride Then
                            _closeOnClick = True
                        End If
                        If Not closeOnKeyboardInputOverride Then
                            _closeOnKeyboardInput = True
                        End If
                        runNormal()
                    Case screensaverMode.Preview
                        runPreview()
                End Select
            Finally
                isEnded = True
            End Try

        End Sub

        Friend Shared fullscreenMode As FullscreenMode = fullscreenMode.SingleWindow
        Private screensaverMode As ScreensaverMode

        ''' <summary>
        ''' Gets the current running mode of the screensaver.
        ''' </summary>
        Public ReadOnly Property mode() As ScreensaverMode
            Get
                Return screensaverMode
            End Get
        End Property

        Private windowHandle As IntPtr = IntPtr.Zero

        ''' <summary>
        ''' Occurs after the windows are created, before the screensaver runs.
        ''' </summary>
        Public Event Initialize As EventHandler

        Private _closeOnMouseMove As Boolean
        Private closeOnMouseMoveOverride As Boolean

        ''' <summary>
        ''' Gets or sets a value indicating whether or not the screensaver should close when the user moves the mouse.
        ''' </summary>
        ''' <remarks>This value is <c>true</c> by default in all modes except <see cref="ScreensaverMode.Windowed"/>.</remarks>
        Public Property closeOnMouseMove() As Boolean
            Get
                Return _closeOnMouseMove
            End Get
            Set(ByVal value As Boolean)
                _closeOnMouseMove = value
                closeOnMouseMoveOverride = True
            End Set
        End Property

        Private _closeOnClick As Boolean
        Private closeOnClickOverride As Boolean

        ''' <summary>
        ''' Gets or sets a value indicating whether or not the screensaver should close when the user clicks the mouse.
        ''' </summary>
        ''' <remarks>This value is <c>true</c> by default in all modes except <see cref="ScreensaverMode.Windowed"/>.</remarks>
        Public Property closeOnClick() As Boolean
            Get
                Return _closeOnClick
            End Get
            Set(ByVal value As Boolean)
                _closeOnClick = value
                closeOnClickOverride = True
            End Set
        End Property

        Private _closeOnKeyboardInput As Boolean
        Private closeOnKeyboardInputOverride As Boolean

        ''' <summary>
        ''' Gets or sets a value indicating whether or not the screensaver should close when the user presses a key.
        ''' </summary>
        ''' <remarks>This value is <c>true</c> by default in all modes except <see cref="ScreensaverMode.Windowed"/>.</remarks>
        Public Property closeOnKeyboardInput() As Boolean
            Get
                Return _closeOnKeyboardInput
            End Get
            Set(ByVal value As Boolean)
                _closeOnKeyboardInput = value
                closeOnKeyboardInputOverride = True
            End Set
        End Property

        Private _windows As WindowCollection

        ''' <summary>
        ''' Gets a collection of all of the running screensaver windows.
        ''' </summary>
        Public ReadOnly Property windows() As WindowCollection
            Get
                Return _windows
            End Get
        End Property


        Private _window0 As Window

        ''' <summary>
        ''' Gets the primary screensaver window.
        ''' </summary>
        Public ReadOnly Property window0() As Window
            Get
                If _window0 IsNot Nothing Then
                    Return _window0
                End If

                If _windows Is Nothing OrElse _windows.count = 0 Then
                    Return Nothing
                End If

                _window0 = _windows(0)

                Return _window0
            End Get
        End Property

        Private _graphics0 As Graphics

        ''' <summary>
        ''' Gets the GDI graphics object for the primary window.
        ''' </summary>
        Public ReadOnly Property graphics0() As Graphics
            Get
                If _graphics0 IsNot Nothing Then
                    Return _graphics0
                End If

                If window0 Is Nothing Then
                    Return Nothing
                End If

                _graphics0 = window0.graphics
                Return _graphics0
            End Get
        End Property

        Private _settingsText As String

        ''' <summary>
        ''' Gets or sets text to be displayed in the default settings message box.
        ''' </summary>
        Public Property settingsText() As String
            Get
                Return _settingsText
            End Get
            Set(ByVal value As String)
                _settingsText = value
            End Set
        End Property

        ''' <summary>
        ''' Shows the settings dialog, or, by default, shows a message box indicating the assembly name, version and copyright information.
        ''' </summary>
        Protected Overridable Sub showSettingsDialog()
            Dim sw As New System.IO.StringWriter()
            Dim name As System.Reflection.AssemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName()
            sw.WriteLine(name.Name)
            sw.WriteLine("Version " & name.Version.ToString())

            Dim attribs() As Object = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(System.Reflection.AssemblyDescriptionAttribute), False)
            If attribs IsNot Nothing AndAlso attribs.Length <> 0 Then
                Dim desc As System.Reflection.AssemblyDescriptionAttribute = TryCast(attribs(0), System.Reflection.AssemblyDescriptionAttribute)
                If desc.Description <> String.Empty Then
                    sw.WriteLine(desc.Description)
                End If
            End If

            attribs = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(System.Reflection.AssemblyCopyrightAttribute), False)
            If attribs IsNot Nothing AndAlso attribs.Length <> 0 Then
                Dim copyright As System.Reflection.AssemblyCopyrightAttribute = TryCast(attribs(0), System.Reflection.AssemblyCopyrightAttribute)
                If copyright.Copyright <> String.Empty Then
                    sw.WriteLine()
                    sw.WriteLine(copyright.Copyright)
                End If
            End If

            If _settingsText IsNot Nothing AndAlso _settingsText <> String.Empty Then
                sw.WriteLine()
                sw.WriteLine(settingsText)
            End If

            MessageBox.Show(sw.ToString(), "Screensaver", MessageBoxButtons.OK)
        End Sub

        Private previewShutdownEvent As New System.Threading.AutoResetEvent(False)

        Private Sub runPreview()
            '#If DEBUG Then
            '			System.Diagnostics.Debugger.Launch()
            '#End If
            _windows = New WindowCollection(New Window() {New Window(Me, windowHandle)})
            initializeAndStart()
            previewShutdownEvent.WaitOne()
        End Sub

        Private Sub runNormal()
            Cursor.Hide()
            Select Case fullscreenMode
                Case fullscreenMode.SingleWindow
                    runNormalSingleWindow()
                Case fullscreenMode.MultipleWindows
                    runNormalMultipleWindows()
            End Select
        End Sub

        Private Sub runNormalMultipleWindows()
            'List<Window> windows = new List<Window>();
            Dim _windows As New ArrayList()

            Dim primary As New Form()
            primary.StartPosition = FormStartPosition.Manual
            primary.Location = Screen.PrimaryScreen.Bounds.Location
            primary.Size = Screen.PrimaryScreen.Bounds.Size
            primary.BackColor = Color.Black
#If Not Debug Then
            primary.TopMost = True
#End If
            primary.FormBorderStyle = FormBorderStyle.None
            primary.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name

            For Each screen As Screen In System.Windows.Forms.Screen.AllScreens
                If screen Is System.Windows.Forms.Screen.PrimaryScreen Then
                    Continue For
                End If

                Dim form As New Form()
                form.Owner = primary
                form.BackColor = Color.Black
#If Not Debug Then
                form.TopMost = True
#End If
                form.StartPosition = FormStartPosition.Manual
                form.Location = screen.Bounds.Location
                form.Size = screen.Bounds.Size
                form.FormBorderStyle = FormBorderStyle.None
                form.Text = primary.Text

                _windows.Add(New Window(Me, form))
            Next screen

            _windows.Insert(0, New Window(Me, primary))

            AddHandler primary.Load, Sub(sender As Object, e As EventArgs)
                                         For Each window As Window In Me._windows
                                             If window.form.Owner Is Nothing Then
                                                 Continue For
                                             End If
                                             window.form.Show()
                                         Next window
                                     End Sub

            Me._windows = New WindowCollection(TryCast(_windows.ToArray(GetType(Window)), Window()))

            primary.Show()
            initializeAndStart()
        End Sub

        Private Sub runNormalSingleWindow()
            Dim form As New Form()
            Dim rect As Rectangle = getVirtualScreenRect()
            form.Location = rect.Location
            form.Size = rect.Size
            form.BackColor = Color.Black
#If Not Debug Then
            form.TopMost = True
#End If
            form.FormBorderStyle = FormBorderStyle.None
            form.StartPosition = FormStartPosition.Manual
            form.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name

            _windows = New WindowCollection(New Window() {New Window(Me, form)})

            form.Show()
            initializeAndStart()
        End Sub

        Private Shared Function getVirtualScreenRect() As Rectangle
            Dim screens() As Screen = Screen.AllScreens
            Dim rect As Rectangle = Rectangle.Empty
            For Each screen As Screen In System.Windows.Forms.Screen.AllScreens
                rect = Rectangle.Union(rect, screen.Bounds)
            Next screen
            Return rect
        End Function

        Private Sub runWindowed()
            Dim form As New Form()
            form.FormBorderStyle = FormBorderStyle.FixedSingle
            form.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name
            form.StartPosition = FormStartPosition.CenterScreen
            form.BackColor = Color.Black
#If Not Debug Then
            form.TopMost = True
#End If
            form.MaximizeBox = False
            'form.ClientSize = New Size(CInt(Math.Truncate(Screen.PrimaryScreen.WorkingArea.Width * 0.9)), CInt(Math.Truncate(Screen.PrimaryScreen.WorkingArea.Height * 0.9)))
            form.ClientSize = New Size(windowedWidth, windowedHeight)
            'form.FormBorderStyle=FormBorderStyle.Sizable

            'form.ClientSize = New Size(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height)
            _windows = New WindowCollection(New Window() {New Window(Me, form)})

            form.Show()
            initializeAndStart()
        End Sub

        Private Sub initializeAndStart()
            RaiseEvent Initialize(Me, New EventArgs())
            If window0 IsNot Nothing AndAlso window0.form IsNot Nothing Then
                AddHandler window0.form.FormClosing, AddressOf form_FormClosing
            End If
            startUpdating()
        End Sub

        Private Sub form_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs)
            stopUpdating()
            RaiseEvent [Exit](Me, New EventArgs())
            e.Cancel = False
        End Sub

#Region "IDisposable Members"

        Private isEnded As Boolean = False

#End Region

        Private Sub onMouseMove()
            If _closeOnMouseMove Then
                If window0.form IsNot Nothing Then
                    window0.form.Close()
                Else
                    Application.Exit()
                End If
            End If
        End Sub

        Private Sub onKeyboardInput()
            If _closeOnMouseMove Then
                If window0.form IsNot Nothing Then
                    window0.form.Close()
                Else
                    Application.Exit()
                End If
            End If
        End Sub

        Private Sub onMouseClick()
            If _closeOnMouseMove Then
                If window0.form IsNot Nothing Then
                    window0.form.Close()
                Else
                    Application.Exit()
                End If
            End If
        End Sub

        ''' <summary>
        ''' Represents a screensaver window.
        ''' </summary>
        Public Class Window
            Friend Sub New(ByVal screensaver As ScreensaverCore, ByVal form As Form)
                Me._screensaver = screensaver
                Me._form = form
                Me._size = form.ClientSize
                Me._graphics = form.CreateGraphics()
                Me._handle = form.Handle

                AddHandler form.MouseMove, AddressOf form_MouseMove
                AddHandler form.MouseClick, AddressOf form_MouseClick
                AddHandler form.MouseDoubleClick, AddressOf form_MouseDoubleClick
                AddHandler form.MouseDown, AddressOf form_MouseDown
                AddHandler form.MouseUp, AddressOf form_MouseUp
                AddHandler form.MouseWheel, AddressOf form_MouseWheel

                AddHandler form.KeyDown, AddressOf form_KeyDown
                AddHandler form.KeyUp, AddressOf form_KeyUp
                AddHandler form.KeyPress, AddressOf form_KeyPress

                AddHandler screensaver.PreUpdate, AddressOf screensaver_PreUpdate
                AddHandler screensaver.PostUpdate, AddressOf screensaver_PostUpdate
            End Sub

            Friend Sub New(ByVal screensaver As ScreensaverCore, ByVal handle As IntPtr)
                Me._screensaver = screensaver
                Me._handle = handle
                Me._graphics = System.Drawing.Graphics.FromHwnd(handle)
                Me._size = GetClientRect(handle).Size

                AddHandler screensaver.PreUpdate, AddressOf screensaver_PreUpdate
                AddHandler screensaver.PostUpdate, AddressOf screensaver_PostUpdate
            End Sub

            Private _doubleBuffer As Boolean = False
            Private doubleBufferSet As Boolean = False

            ''' <summary>
            ''' Gets or sets a value indicating whether or not the Graphics object should be double buffered.
            ''' Set to <c>false</c> if the Graphics object will not be used.
            ''' </summary>
            Public Property doubleBuffer() As Boolean
                Get
                    If Not doubleBufferSet Then
                        doubleBuffer = True
                    End If
                    Return _doubleBuffer
                End Get
                Set(ByVal value As Boolean)
                    doubleBufferSet = True
                    If _doubleBuffer <> value Then
                        _doubleBuffer = value
                        If _doubleBuffer Then
                            setDoubleBuffer()
                        Else
                            unsetDoubleBuffer()
                        End If
                    Else
                        _doubleBuffer = value
                    End If
                End Set
            End Property

            Private Sub setDoubleBuffer()
                graphicsSwap = _graphics
                BufferedGraphicsManager.Current.MaximumBuffer = Me.size
                buffer = BufferedGraphicsManager.Current.Allocate(graphicsSwap, New Rectangle(0, 0, Me.size.Width, Me.size.Height))
                _graphics = buffer.Graphics
            End Sub

            Private Sub unsetDoubleBuffer()
                buffer.Dispose()
                _graphics = graphicsSwap
                buffer = Nothing
                graphicsSwap = Nothing
            End Sub

            Private buffer As BufferedGraphics
            Private graphicsSwap As Graphics

            Private Sub screensaver_PreUpdate(ByVal sender As Object, ByVal e As EventArgs)
            End Sub

            Private Sub screensaver_PostUpdate(ByVal sender As Object, ByVal e As EventArgs)
                If _doubleBuffer Then
                    buffer.Render(graphicsSwap)
                End If
            End Sub

#Region "Keyboard and Mouse Events"

            Private Sub form_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs)
                RaiseEvent KeyPress(Me, e)
                _screensaver.onKeyboardInput()
            End Sub

            Private Sub form_KeyUp(ByVal sender As Object, ByVal e As KeyEventArgs)
                RaiseEvent KeyUp(Me, e)
                _screensaver.onKeyboardInput()
            End Sub

            Private Sub form_KeyDown(ByVal sender As Object, ByVal e As KeyEventArgs)
                RaiseEvent KeyDown(Me, e)
                _screensaver.onKeyboardInput()
            End Sub

            Private Sub form_MouseWheel(ByVal sender As Object, ByVal e As MouseEventArgs)
                RaiseEvent MouseWheel(Me, e)
                _screensaver.onMouseClick()
            End Sub

            Private Sub form_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
                RaiseEvent MouseUp(Me, e)
                _screensaver.onMouseClick()
            End Sub

            Private Sub form_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
                RaiseEvent MouseDown(Me, e)
                _screensaver.onMouseClick()
            End Sub

            Private Sub form_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs)
                RaiseEvent MouseDoubleClick(Me, e)
                _screensaver.onMouseClick()
            End Sub

            Private Sub form_MouseClick(ByVal sender As Object, ByVal e As MouseEventArgs)
                RaiseEvent MouseClick(Me, e)
                _screensaver.onMouseClick()
            End Sub

            'Keep track of the initial mouse position since we want to ignore the MouseMove messages that are fired right when the form is created.
            Private mousePosition As Point = Point.Empty

            Private Sub form_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
                RaiseEvent MouseMove(Me, e)

                If mousePosition = Point.Empty Then
                    mousePosition = e.Location
                ElseIf mousePosition <> e.Location Then
                    _screensaver.onMouseMove()
                End If
            End Sub

            ''' <summary>
            ''' Occurs when the mouse is moved over this window.
            ''' </summary>
            Public Event MouseMove As MouseEventHandler
            ''' <summary>
            ''' Occurs when the mouse is clicked inside this window.
            ''' </summary>
            Public Event MouseClick As MouseEventHandler
            ''' <summary>
            ''' Occurs when the mouse is double clicked inside this window.
            ''' </summary>
            Public Event MouseDoubleClick As MouseEventHandler
            ''' <summary>
            ''' Occurs when the mouse wheel is moved inside this window.
            ''' </summary>
            Public Event MouseWheel As MouseEventHandler
            ''' <summary>
            ''' Occurs when a mouse button goes up inside this window.
            ''' </summary>
            Public Event MouseUp As MouseEventHandler
            ''' <summary>
            ''' Occurs when a mouse button goes down inside this window.
            ''' </summary>
            Public Event MouseDown As MouseEventHandler

            ''' <summary>
            ''' Occurs when a key goes down.
            ''' </summary>
            Public Event KeyDown As KeyEventHandler
            ''' <summary>
            ''' Occurs when a key is released.
            ''' </summary>
            Public Event KeyUp As KeyEventHandler
            ''' <summary>
            ''' Occurs when a key is pressed.
            ''' </summary>
            Public Event KeyPress As KeyPressEventHandler

#End Region

            Private _tag As Object

            ''' <summary>
            ''' Gets or sets a tag value.
            ''' </summary>
            Public Property tag() As Object
                Get
                    Return _tag
                End Get
                Set(ByVal value As Object)
                    _tag = value
                End Set
            End Property

            Private _screensaver As ScreensaverCore

            ''' <summary>
            ''' Gets the <see cref="Screensaver"/> for which this window was created.
            ''' </summary>
            Public ReadOnly Property screensaver() As ScreensaverCore
                Get
                    Return _screensaver
                End Get
            End Property

            Private _form As Form

            ''' <summary>
            ''' Gets the form encapsulating this window.
            ''' This property is <c>null</c> if the screensaver is running in preview mode.
            ''' </summary>
            Public ReadOnly Property form() As Form
                Get
                    Return _form
                End Get
            End Property

            Private _handle As IntPtr

            ''' <summary>
            ''' Gets the native handle of the window.
            ''' </summary>
            Public ReadOnly Property handle() As IntPtr
                Get
                    Return _handle
                End Get
            End Property

            Private _size As Size

            ''' <summary>
            ''' Gets the size of the window.
            ''' </summary>
            Public ReadOnly Property size() As Size
                Get
                    Return _size
                End Get
            End Property

            Private _graphics As Graphics

            ''' <summary>
            ''' Gets the GDI graphics object for this window.
            ''' </summary>
            Public ReadOnly Property graphics() As Graphics
                Get
                    'Only set double buffering if the Graphics object is being used.
                    If Not doubleBufferSet Then
                        doubleBuffer = True
                    End If
                    Return _graphics
                End Get
            End Property

            ''' <summary>
            ''' Gets the screen on which this window resides
            ''' </summary>
            Public ReadOnly Property screen() As Screen
                Get
                    Return screen.FromHandle(_handle)
                End Get
            End Property

            ''' <summary>
            ''' Gets the display device index to use with this window.
            ''' </summary>
            Public ReadOnly Property deviceIndex() As Integer
                Get
                    Dim thisScreen As Screen = screen
                    For i As Integer = 0 To screen.AllScreens.Length - 1
                        If screen.AllScreens(i).Equals(thisScreen) Then
                            Return i
                        End If
                    Next i
                    Throw New ApplicationException()
                End Get
            End Property
        End Class

        ''' <summary>
        ''' Represents a collection of screensaver windows.
        ''' </summary>
        Public Class WindowCollection
            Implements IEnumerable

            Friend Sub New(ByVal windows() As Window)
                Me.windows = windows
            End Sub

            Private windows() As Window

            ''' <summary>
            ''' Gets the window at the given index.
            ''' </summary>
            ''' <param name="index">The zero-based index of the screensaver window.</param>
            ''' <returns>The window at hte given index.</returns>
            Default Public ReadOnly Property item(ByVal index As Integer) As Window
                Get
                    Return windows(index)
                End Get
            End Property

            ''' <summary>
            ''' Gets the number of screensaver windows available.
            ''' </summary>
            Public ReadOnly Property count() As Integer
                Get
                    Return windows.Length
                End Get
            End Property

#Region "IEnumerable Members"

            Private Function IEnumerable_GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
                Return windows.GetEnumerator()
            End Function

#End Region
        End Class
    End Class

    ''' <summary>
    ''' Specifies the types of multiple monitor support to make available.
    ''' </summary>
    Public Enum FullscreenMode
        ''' <summary>
        ''' Single window covering all monitors.
        ''' </summary>
        SingleWindow
        ''' <summary>
        ''' Multiple windows, one for each monitor.
        ''' </summary>
        MultipleWindows
    End Enum

    ''' <summary>
    ''' Specifies the mode in which to run the screensaver.
    ''' </summary>
    Public Enum ScreensaverMode
        ''' <summary>
        ''' Show a the settings dialog.
        ''' </summary>
        Settings
        ''' <summary>
        ''' Render inside the preview box of the Windows Display Properties.
        ''' </summary>
        Preview
        ''' <summary>
        ''' Run the screensaver in full screen mode.
        ''' </summary>
        Normal
        ''' <summary>
        ''' Run the screensaver inside a fixed-sized window.
        ''' </summary>
        Windowed
    End Enum
End Namespace