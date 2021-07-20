
Namespace SpriteHandler

    Public Class AnimationEngine

        Public spriteItem As SpriteItem = Nothing

        Public displayFrame As Integer = 0

        Public height As Single = 0.0!
        Public width As Single = 0.0!

        Public originalHeight As Single = 0.0!
        Public originalWidth As Single = 0.0!

        Public centerX As Single = 0.0!
        Public centerY As Single = 0.0!

        Public blocksAcross As Integer = 0
        Public blocksDown As Integer = 0

        Public totalFrames As Integer = 0

        Public frameDelay As Integer = 0

        Public looping As Boolean = True
        Public Delegate Sub callbackLoopEndedType(ByVal sprite As AnimationEngine)
        Public callbackLoopEnded As callbackLoopEndedType = Nothing

        Public Delegate Sub callbackLoopUpdatedType(ByVal sprite As AnimationEngine)
        Public callbackLoopUpdated As callbackLoopUpdatedType = Nothing

        Public Delegate Function findFrameType(ByVal frame As Integer) As Rectangle
        Public getFrame As findFrameType = Nothing

        Public Delegate Function findCurrentFrameType() As Rectangle
        Public currentFrame As findCurrentFrameType = Nothing

        Public Sub updateSpriteFrame()
            displayFrame += 1
            If displayFrame = totalFrames Then
                If looping Then
                    displayFrame = 0
                Else
                    displayFrame = 0
                    If callbackLoopEnded IsNot Nothing Then callbackLoopEnded(Me) : Return
                End If
            End If
            spriteItem.sourceRectangle = currentFrame()
            If callbackLoopUpdated IsNot Nothing Then callbackLoopUpdated(Me)
        End Sub

        Public Sub updateTimeout()
            If frameDelay > 0 Then frameDelay -= 1
        End Sub

        Public Function timeoutEnded() As Boolean
            'If frameDelay > 0 Then frameDelay-=1 
            Return If(frameDelay <= 0, True, False)
        End Function

        Public Sub New(ByVal SI As SpriteItem, ByVal frameWidth As Integer, ByVal frameHeight As Integer, ByVal totalFrames As Integer)
            spriteItem = SI
            blocksAcross = CInt(CLng(spriteItem.originalWidth) \ frameWidth)
            blocksDown = CInt(CLng(spriteItem.originalHeight) \ frameHeight)
            Me.totalFrames = totalFrames 'blocksAcross * blocksDown
            Me.width = frameWidth
            Me.height = frameHeight
            centerX = frameWidth / 2.0!
            centerY = frameHeight / 2.0!
            originalHeight = frameHeight
            originalWidth = frameWidth
            If blocksDown > 1 Then
                getFrame = AddressOf getFrameFromBlock
                currentFrame = AddressOf getCurrentFrameFromBlock
            Else
                getFrame = AddressOf getFrameFromStrip
                currentFrame = AddressOf getCurrentFrameFromStrip
            End If
        End Sub

        Public Sub New(ByVal SI As SpriteItem, ByVal frameWidth As Integer, ByVal frameHeight As Integer)
            spriteItem = SI
            blocksAcross = CInt(CLng(spriteItem.originalWidth) \ frameWidth)
            blocksDown = CInt(CLng(spriteItem.originalHeight) \ frameHeight)
            Me.totalFrames = blocksAcross * blocksDown
            Me.width = frameWidth
            Me.height = frameHeight
            centerX = frameWidth / 2.0!
            centerY = frameHeight / 2.0!
            originalWidth = frameWidth
            originalHeight = frameHeight

            If blocksDown > 1 Then
                getFrame = AddressOf getFrameFromBlock
                currentFrame = AddressOf getCurrentFrameFromBlock
            Else
                getFrame = AddressOf getFrameFromStrip
                currentFrame = AddressOf getCurrentFrameFromStrip
            End If
        End Sub

        Public Function getFrameFromStrip(ByVal frame As Integer) As Rectangle
            Dim x As Integer = CInt(frame * originalWidth)
            Dim y As Integer = 0
            Return New Rectangle(x, y, CInt(originalWidth), CInt(originalHeight))
        End Function

        Public Function getFrameFromBlock(ByVal frame As Integer) As Rectangle
            Dim framesAcross As Integer = frame Mod blocksAcross
            Dim framesDown As Integer = frame \ blocksAcross
            Dim x As Integer = CInt(framesAcross * originalWidth)
            Dim y As Integer = CInt(framesDown * originalHeight)
            Return New Rectangle(x, y, CInt(originalWidth), CInt(originalHeight))
        End Function

        Public Function getCurrentFrameFromStrip() As Rectangle
            Dim x As Integer = CInt(displayFrame * originalWidth)
            Dim y As Integer = 0
            Return New Rectangle(x, y, CInt(originalWidth), CInt(originalHeight))
        End Function

        Public Function getCurrentFrameFromBlock() As Rectangle
            Dim framesAcross As Integer = displayFrame Mod blocksAcross
            Dim framesDown As Integer = displayFrame \ blocksAcross
            Dim x As Integer = CInt(framesAcross * originalWidth)
            Dim y As Integer = CInt(framesDown * originalHeight)
            Return New Rectangle(x, y, CInt(originalWidth), CInt(originalHeight))
        End Function
    End Class

End Namespace