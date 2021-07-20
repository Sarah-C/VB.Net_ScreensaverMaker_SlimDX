
Imports SlimDX
Imports SlimDX.Direct3D9
Imports SharpNeatLib.Maths


Namespace smallDemo

    Public Class SnowStorm

        Public staticSnow As New List(Of SpriteHandler.SpriteItem)

        Public x As Single = 0.0!
        Public y As Single = 0.0!
        Public z As Single = 0.0!

        Public slowRand As New Random()
        Public rand As New FastRandom()
        Public projection As SpriteEngine.PerspectiveProjection = Nothing
        Public spriteManager As SpriteHandler.SpriteManager = Nothing

        Public snowFlakeTexture As SlimDX.Direct3D9.Texture = Nothing

        Public Sub New(ByVal spriteManager As SpriteHandler.SpriteManager, ByVal projection As SpriteEngine.PerspectiveProjection)
            Me.projection = projection
            Me.spriteManager = spriteManager
            initialise()
        End Sub

        Public Sub initialise()
            snowFlakeTexture = spriteManager.bitmapToTexture(My.Resources.Snowflake)
            For count As Integer = 1 To 1000
                z = rand.Next(1, 80) / 100.0!
                x = rand.Next(CInt(projection.perspectiveLeftMax(-rand.Next(1, 100), z)), CInt(projection.perspectiveRightMax(z)))
                y = rand.Next(CInt(projection.perspectiveTopMax(z)), CInt(projection.perspectiveBottomMax(z)))
                Dim newSprite As SpriteHandler.SpriteItem = spriteManager.addSprite(snowFlakeTexture, x, y, z, Color.White)
                newSprite.vy = CSng(z * 5.0!)
                newSprite.vx = CSng(rand.Next(5, 10) / 20.0!)
                Dim size As Integer = 8
                newSprite.width = size
                newSprite.height = size
                staticSnow.Add(newSprite)
            Next
        End Sub

        Public Sub update()
            For Each s As SpriteHandler.SpriteItem In staticSnow
                s.y += s.vy
                s.x += s.vx
                s.angle += 0.01!
                If s.y - (s.height * projection.universalScaleFactor) > projection.perspectiveBottomMax(s.z) Then
                    s.angle = 0.0!
                    s.z = rand.Next(1, 80) / 100.0!
                    s.y = CInt(projection.perspectiveTopMax(-40, s.z))
                    s.x = rand.Next(CInt(projection.perspectiveLeftMax(-rand.Next(100, 200), s.z)), CInt(projection.perspectiveRightMax(s.z)))
                    s.vy = CSng(s.z * 5.0!)
                    s.vx = CSng(rand.Next(5, 10) / 20.0!)
                    Dim size As Integer = 8
                    s.width = size
                    s.height = size
                End If
            Next
        End Sub

    End Class

End Namespace