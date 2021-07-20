
Imports SlimDX
Imports SlimDX.Direct3D9

Namespace SpriteHandler

    Public Class sectionBlockProcessorSystem

        Public sprites As List(Of SpriteItem) = Nothing
        Public projectionRenderer As SpriteEngine.PerspectiveProjection = Nothing

        Public inTriplet As New SpriteEngine.PerspectiveProjection.tuple()
        Public outTriplet As New SpriteEngine.PerspectiveProjection.tuple()

        Public startIndex As Integer = 0
        Public endIndex As Integer = 0

        Public Sub New(ByVal projectionRenderer As SpriteEngine.PerspectiveProjection, ByVal sprites As List(Of SpriteItem))
            Me.projectionRenderer = projectionRenderer
            Me.sprites = sprites
        End Sub

        Public Sub updateSpriteBlock()
            For spriteIndex As Integer = startIndex To endIndex
                Dim sprite As SpriteItem = sprites(spriteIndex)
                If sprite.hasAnimation Then
                    If Not sprite.animationEngine.timeoutEnded() Then Continue For
                    sprite.animationEngine.updateSpriteFrame()
                End If

                Dim x As Single = 0.0!
                Dim y As Single = 0.0!
                Dim scaleX As Single = 0.0!
                Dim scaleY As Single = 0.0!

                If sprite.hasAnimation Then
                    inTriplet.x = sprite.x
                    inTriplet.y = sprite.y
                    inTriplet.z = sprite.z

                    inTriplet.scaleX = CSng(sprite.animationEngine.width / sprite.animationEngine.originalWidth)
                    inTriplet.scaleY = CSng(sprite.animationEngine.width / sprite.animationEngine.originalWidth)

                    projectionRenderer.transform(inTriplet, outTriplet)

                    sprite.matrix = Matrix.Translation(-outTriplet.x, -outTriplet.y, 0)
                    sprite.matrix *= Matrix.Scaling(outTriplet.scaleX, outTriplet.scaleY, 0)
                    sprite.matrix *= Matrix.RotationZ(sprite.angle)
                    sprite.matrix *= Matrix.Translation(outTriplet.x, outTriplet.y, sprite.z)

                    sprite.centerV3.X = sprite.animationEngine.centerX
                    sprite.centerV3.Y = sprite.animationEngine.centerY

                    sprite.positionV3.X = outTriplet.x
                    sprite.positionV3.Y = outTriplet.y
                    sprite.positionV3.Z = outTriplet.z
                Else
                    inTriplet.x = sprite.x
                    inTriplet.y = sprite.y
                    inTriplet.z = sprite.z

                    inTriplet.scaleX = CSng(sprite.width / sprite.originalWidth)
                    inTriplet.scaleY = CSng(sprite.height / sprite.originalHeight)

                    projectionRenderer.transform(inTriplet, outTriplet)

                    sprite.matrix = Matrix.Translation(-outTriplet.x, -outTriplet.y, 0)
                    sprite.matrix *= Matrix.Scaling(outTriplet.scaleX, outTriplet.scaleY, 0)
                    sprite.matrix *= Matrix.RotationZ(sprite.angle)
                    sprite.matrix *= Matrix.Translation(outTriplet.x, outTriplet.y, sprite.z)

                    sprite.centerV3.X = sprite.centerX
                    sprite.centerV3.Y = sprite.centerY

                    sprite.positionV3.X = outTriplet.x
                    sprite.positionV3.Y = outTriplet.y
                    sprite.positionV3.Z = outTriplet.z
                End If
            Next
        End Sub

    End Class

End Namespace