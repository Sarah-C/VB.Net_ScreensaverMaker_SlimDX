
Namespace SpriteEngine

    Public Class PerspectiveProjection

        Public screenHeight As Single = 0.0!
        Public screenWidth As Single = 0.0!
        Public nativeScreenHeight As Single = 0.0!
        Public universalScaleFactor As Single = 0.0!

        Public vanishingPointX As Single = 0.0!
        Public vanishingPointY As Single = 0.0!

        Public leftLength As Single = 0.0!
        Public rightLength As Single = 0.0!

        Public leftMax As Single = 0.0!
        Public rightMax As Single = 0.0!
        Public scaledWidth As Single = 0.0!

        ' Public transformQuad As New tuple()

        Public Class tuple

            Public x As Single = 0.0!
            Public y As Single = 0.0!
            Public z As Single = 0.0!
            Public scaleX As Single = 0.0!
            Public scaleY As Single = 0.0!

            Public Sub New()
            End Sub

            Public Sub New(ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal sx As Single, ByVal sy As Single)
                Me.x = x
                Me.y = y
                Me.z = z
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

            leftLength = vanishingPointX
            rightLength = screenWidth - vanishingPointX

            leftMax = vanishingPointX - (leftLength / universalScaleFactor)
            rightMax = vanishingPointX + (rightLength / universalScaleFactor)

            scaledWidth = Math.Abs(leftMax) + rightMax
        End Sub

        Public Function screenSpaceScaled(ByVal v As Single, ByVal z As Single) As Single
            Return v * (((z - 0.5!) * 1.4! + 1.0!) * universalScaleFactor)
        End Function

        Public Function perspectiveLeftMax(ByVal z As Single) As Single
            Dim depthScale As Single = (z - 0.5!) * 1.4! + 1.0!
            depthScale *= universalScaleFactor
            Dim differenceFromZero As Single = (leftLength * depthScale) - leftLength
            Dim screenPosition As Single = differenceFromZero / depthScale
            Return CSng(screenPosition)
        End Function

        Public Function perspectiveRightMax(ByVal z As Single) As Single
            Dim depthScale As Single = (z - 0.5!) * 1.4! + 1.0!
            depthScale *= universalScaleFactor
            Dim differenceFromZero As Single = (rightLength * depthScale) - rightLength
            Dim screenPosition As Single = differenceFromZero / depthScale
            Return CSng(screenWidth - screenPosition)
        End Function

        Public Function perspectiveTopMax(ByVal z As Single) As Single
            Dim depthScale As Single = (z - 0.5!) * 1.4! + 1.0!
            depthScale *= universalScaleFactor
            Dim differenceFromZero As Single = (vanishingPointY * depthScale) - vanishingPointY
            Dim screenPosition As Single = differenceFromZero / depthScale
            Return CSng(screenPosition)
        End Function

        Public Function perspectiveBottomMax(ByVal z As Single) As Single
            Dim depthScale As Single = (z - 0.5!) * 1.4! + 1.0!
            depthScale *= universalScaleFactor
            Dim differenceFromZero As Single = ((screenHeight - vanishingPointY) * depthScale) - (screenHeight - vanishingPointY)
            Dim screenPosition As Single = differenceFromZero / depthScale
            Return CSng(screenHeight - screenPosition)
        End Function

        'With tweaks

        Public Function perspectiveLeftMax(ByVal v As Single, ByVal z As Single) As Single
            Dim depthScale As Single = (z - 0.5!) * 1.4! + 1.0!
            depthScale *= universalScaleFactor
            Dim differenceFromZero As Single = ((leftLength + v) * depthScale) - leftLength
            Dim screenPosition As Single = differenceFromZero / depthScale
            Return CSng(screenPosition)
        End Function

        Public Function perspectiveRightMax(ByVal v As Single, ByVal z As Single) As Single
            Dim depthScale As Single = (z - 0.5!) * 1.4! + 1.0!
            depthScale *= universalScaleFactor
            Dim differenceFromZero As Single = ((rightLength - v) * depthScale) - rightLength
            Dim screenPosition As Single = differenceFromZero / depthScale
            Return CSng(screenWidth - screenPosition)
        End Function

        Public Function perspectiveTopMax(ByVal v As Single, ByVal z As Single) As Single
            Dim depthScale As Single = (z - 0.5!) * 1.4! + 1.0!
            depthScale *= universalScaleFactor
            Dim differenceFromZero As Single = ((vanishingPointY + v) * depthScale) - vanishingPointY
            Dim screenPosition As Single = differenceFromZero / depthScale
            Return CSng(screenPosition)
        End Function

        Public Function perspectiveBottomMax(ByVal v As Single, ByVal z As Single) As Single
            Dim depthScale As Single = (z - 0.5!) * 1.4! + 1.0!
            depthScale *= universalScaleFactor
            Dim differenceFromZero As Single = ((screenHeight - vanishingPointY - v) * depthScale) - (screenHeight - vanishingPointY)
            Dim screenPosition As Single = differenceFromZero / depthScale
            Return CSng(screenHeight - screenPosition)
        End Function

        Public Sub transform(ByVal inTuple As tuple, ByRef outTuple As tuple)
            Dim depthScale As Single = (inTuple.z - 0.5!) * 1.4! + 1.0!

            outTuple.x = (inTuple.x - vanishingPointX) * universalScaleFactor * depthScale + vanishingPointX
            outTuple.y = (inTuple.y - vanishingPointY) * universalScaleFactor * depthScale + vanishingPointY

            outTuple.scaleX = inTuple.scaleX * universalScaleFactor * depthScale
            outTuple.scaleY = inTuple.scaleY * universalScaleFactor * depthScale
            outTuple.z = inTuple.z
        End Sub

    End Class

End Namespace