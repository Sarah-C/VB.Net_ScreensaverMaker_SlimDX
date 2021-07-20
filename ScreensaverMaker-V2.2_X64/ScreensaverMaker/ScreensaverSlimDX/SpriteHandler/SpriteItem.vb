
Imports SlimDX
Imports SlimDX.Direct3D9
Imports SharpNeatLib.Maths

Namespace SpriteHandler

    Public Class SpriteItem

        Public spriteTexture As SlimDX.Direct3D9.Texture = Nothing
        Public hasAnimation As Boolean = False

        Private _animationEngine As AnimationEngine = Nothing
        Public Property animationEngine() As AnimationEngine
            Get
                Return _animationEngine
            End Get
            Set(ByVal animationEngine As AnimationEngine)
                _animationEngine = animationEngine
                hasAnimation = If(animationEngine IsNot Nothing, True, False)
            End Set
        End Property

        Public x As Single = 0.0!
        Public y As Single = 0.0!
        Public z As Single = 0.5!

        Public vx As Single = 0.0!
        Public vy As Single = 0.0!
        Public vz As Single = 0.0!

        Public width As Single = 0.0!
        Public height As Single = 0.0!
        Public originalWidth As Single = 0.0!
        Public originalHeight As Single = 0.0!

        Public angle As Single = 0.0!
        Public color As Integer = 0I

        Public centerX As Single = 0.0!
        Public centerY As Single = 0.0!

        Public centerV3 As New Vector3(0, 0, 0)
        Public positionV3 As New Vector3(0, 0, 0)
        Public matrix As Matrix = nothing

        Public sourceRectangle As Rectangle = Nothing

        Public Sub New(ByVal t As SlimDX.Direct3D9.Texture, ByVal x As Single, ByVal y As Single, ByVal c As Integer)
            spriteTexture = t
            Me.x = x
            Me.y = y
            width = spriteTexture.GetSurfaceLevel(0).Description.Width
            height = spriteTexture.GetSurfaceLevel(0).Description.Height
            originalWidth = width
            originalHeight = height
            centerX = width / 2.0!
            centerY = height / 2.0!
            color = c
        End Sub

        Public Sub New(ByVal t As SlimDX.Direct3D9.Texture, ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal c As Integer)
            spriteTexture = t
            Me.x = x
            Me.y = y
            Me.z = z
            width = CSng(spriteTexture.GetSurfaceLevel(0).Description.Width)
            height = CSng(spriteTexture.GetSurfaceLevel(0).Description.Height)
            originalWidth = width
            originalHeight = height
            centerX = width / 2.0!
            centerY = height / 2.0!
            color = c
        End Sub

        Public Sub New(ByVal t As SlimDX.Direct3D9.Texture, ByVal x As Single, ByVal y As Single, ByVal angle As Double, ByVal c As Integer)
            spriteTexture = t
            Me.x = x
            Me.y = y
            width = spriteTexture.GetSurfaceLevel(0).Description.Width
            height = spriteTexture.GetSurfaceLevel(0).Description.Height
            originalWidth = width
            originalHeight = height
            originalWidth = width
            originalHeight = height
            Me.angle = CSng(angle)
            color = c
        End Sub

        Public Sub New(ByVal t As SlimDX.Direct3D9.Texture, ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal angle As Double, ByVal c As Integer)
            spriteTexture = t
            Me.x = x
            Me.y = y
            width = spriteTexture.GetSurfaceLevel(0).Description.Width
            height = spriteTexture.GetSurfaceLevel(0).Description.Height
            originalWidth = width
            originalHeight = height
            centerX = width / 2.0!
            centerY = height / 2.0!
            Me.angle = CSng(angle)
            color = c
        End Sub

    End Class

End Namespace
