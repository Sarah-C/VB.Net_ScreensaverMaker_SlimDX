
Namespace SpriteEngine

    Public Class OrthogonalProjection

        Public screenHeight As Single = 0.0!
        Public screenWidth As Single = 0.0!
        Public nativeScreenHeight As Single = 0.0!
        Public universalScaleFactor As Single = 0.0!

        Public vanishingPointX As Single = 0.0!
        Public vanishingPointY As Single = 0.0!

        Public leftMax As Single = 0.0!
        Public rightMax As Single = 0.0!
        Public scaledWidth As Single = 0.0!

        Public transformQuad As New tuple()

        Public Class tuple
            Public x As Single = 0.0!
            Public y As Single = 0.0!
            Public scaleX As Single = 0.0!
            Public scaleY As Single = 0.0!
            Public Sub New()
            End Sub
            Public Sub New(ByVal x As Single, ByVal y As Single, ByVal sx As Single, ByVal sy As Single)
                Me.x = x
                Me.y = y
                Me.scaleY = sy
                Me.scaleX = sx
            End Sub
        End Class

        Public Sub New()
        End Sub

        Public Sub New(ByVal nativeScreenHeight As Integer)
            Me.nativeScreenHeight = nativeScreenHeight
        End Sub

        Public Sub New(ByVal nativeScreenHeight As Integer, ByVal screenHeight As Integer)
            universalScaleFactor = (CSng(screenHeight) / CSng(nativeScreenHeight))
            Me.nativeScreenHeight = nativeScreenHeight
            Me.screenHeight = screenHeight
        End Sub

        Public Sub New(ByVal nativeScreenHeight As Integer, ByVal screenHeight As Integer, ByVal screenWidth As Integer, ByVal vanishingPointX As Integer, ByVal vanishingPointY As Integer)
            universalScaleFactor = (CSng(screenHeight) / CSng(nativeScreenHeight))
            Me.nativeScreenHeight = nativeScreenHeight
            Me.screenHeight = screenHeight
            Me.screenWidth = screenWidth
            Me.vanishingPointX = vanishingPointX
            Me.vanishingPointY = vanishingPointY

            Dim leftLength As Single = vanishingPointX
            Dim rightlength As Single = screenWidth - vanishingPointX

            leftMax = vanishingPointX - (leftLength / universalScaleFactor)
            rightMax = vanishingPointX + (rightlength / universalScaleFactor)

            scaledWidth = Math.Abs(leftMax) + rightMax
        End Sub

        Public Function transform(ByVal inTuple As tuple) As tuple
            transformQuad.x = (inTuple.x - vanishingPointX) * universalScaleFactor + vanishingPointX
            transformQuad.y = (inTuple.y - vanishingPointY) * universalScaleFactor + vanishingPointY
            transformQuad.scaleX = inTuple.scaleX * universalScaleFactor
            transformQuad.scaleY = inTuple.scaleY * universalScaleFactor
            Return transformQuad
        End Function

    End Class

End Namespace