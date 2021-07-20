
Imports SlimDX
Imports SlimDX.Direct3D9

Namespace SpriteHandler

    Public Class SpriteManager

        Public spriteRender As SlimDX.Direct3D9.Sprite = Nothing
        Public device As SlimDX.Direct3D9.Device = Nothing
        Public sprites As List(Of SpriteItem)
        Public sectionBlockProcessor As List(Of sectionBlockProcessorSystem) = Nothing

        Public batchSize As Integer = 0
        Public coresWithExtraItem As Integer = 0
        Public cores As Integer = Environment.ProcessorCount

        Public Sub New(ByVal projectionRenderer As SpriteEngine.PerspectiveProjection)
            sprites = New List(Of SpriteItem)
            sectionBlockProcessor = New List(Of sectionBlockProcessorSystem)
            For index As Integer = 0 To cores - 1
                sectionBlockProcessor.Add(New sectionBlockProcessorSystem(projectionRenderer, sprites))
            Next
        End Sub

        Public Sub setDevice(ByVal device As SlimDX.Direct3D9.Device)
            spriteRender = New SlimDX.Direct3D9.Sprite(device)
            Me.device = device
        End Sub

         Public Sub updateSprites()
            If sprites.Count < cores Then cores = sprites.Count
            batchSize = sprites.Count \ cores
            coresWithExtraItem = sprites.Count - batchSize * cores
            System.Threading.Tasks.Parallel.[For](0, cores, New Action(Of Integer)(AddressOf runSpriteUpdateBlockThread))
        End Sub

        Public Sub runSpriteUpdateBlockThread(ByVal core As Integer)
            If core < coresWithExtraItem Then
                sectionBlockProcessor(core).startIndex = (batchSize + 1) * core
                sectionBlockProcessor(core).endIndex = sectionBlockProcessor(core).startIndex + batchSize
            Else
                sectionBlockProcessor(core).startIndex = ((batchSize + 1) * coresWithExtraItem) + (((core - coresWithExtraItem)) * batchSize)
                sectionBlockProcessor(core).endIndex = sectionBlockProcessor(core).startIndex + batchSize - 1
            End If
            sectionBlockProcessor(core).updateSpriteBlock()
        End Sub

        Public Sub drawSprites()
            If sprites.Count = 0 Then Return
            spriteRender.Begin(SlimDX.Direct3D9.SpriteFlags.AlphaBlend Or SlimDX.Direct3D9.SpriteFlags.SortDepthBackToFront) ' Or SpriteFlags.SortTexture)
            For Each sprite As SpriteItem In sprites
                If sprite.hasAnimation Then
                    If Not sprite.animationEngine.timeoutEnded() Then sprite.animationEngine.updateTimeout() : Continue For
                End If
                spriteRender.Transform = sprite.matrix
                If Not sprite.sourceRectangle.IsEmpty Then
                    spriteRender.Draw(sprite.spriteTexture, sprite.sourceRectangle, sprite.centerV3, sprite.positionV3, CType(sprite.color, Color4))
                Else
                    spriteRender.Draw(sprite.spriteTexture, sprite.centerV3, sprite.positionV3, CType(sprite.color, Color4))
                End If
            Next
            spriteRender.End()
        End Sub

        Public Function bitmapToTexture(ByVal bitmap As Bitmap) As SlimDX.Direct3D9.Texture
            Dim stream As New System.IO.MemoryStream()
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp)
            stream.Position = 0
            Return SlimDX.Direct3D9.Texture.FromStream(device, stream)
        End Function

        Public Function resourceToTexture(ByVal device As SlimDX.Direct3D9.Device, ByVal name As String) As SlimDX.Direct3D9.Texture
            Return SlimDX.Direct3D9.Texture.FromStream(device, Me.GetType().Assembly.GetManifestResourceStream(name))
        End Function

        Public Function addSprite(ByVal bitmap As Bitmap, ByVal x As Single, ByVal y As Single, ByVal c As Integer) As SpriteItem
            Dim SD As New SpriteItem(bitmapToTexture(bitmap), x, y, c)
            'SD.originalWidth = bitmap.Width
            'SD.originalHeight = bitmap.Height
            sprites.Add(SD)
            Return SD
        End Function

        Public Function addSprite(ByVal bitmap As Bitmap, ByVal x As Single, ByVal y As Single, ByVal angle As Double, ByVal c As Integer) As SpriteItem
            Dim SD As New SpriteItem(bitmapToTexture(bitmap), x, y, angle, c)
            'SD.originalWidth = bitmap.Width
            'SD.originalHeight = bitmap.Height
            sprites.Add(SD)
            Return SD
        End Function

        Public Function addSprite(ByVal bitmap As Bitmap, ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal c As Integer) As SpriteItem
            Dim SD As New SpriteItem(bitmapToTexture(bitmap), x, y, z, c)
            'SD.originalWidth = bitmap.Width
            'SD.originalHeight = bitmap.Height
            sprites.Add(SD)
            Return SD
        End Function

        Public Function addSprite(ByVal bitmap As Bitmap, ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal angle As Double, ByVal c As Integer) As SpriteItem
            Dim SD As New SpriteItem(bitmapToTexture(bitmap), x, y, z, angle, c)
            'SD.originalWidth = bitmap.Width
            'SD.originalHeight = bitmap.Height
            sprites.Add(SD)
            Return SD
        End Function

        '###########################

        Public Function addSprite(ByVal texture As SlimDX.Direct3D9.Texture, ByVal x As Single, ByVal y As Single, ByVal c As Integer) As SpriteItem
            Dim SD As New SpriteItem(texture, x, y, c)
            sprites.Add(SD)
            Return SD
        End Function

        Public Function addSprite(ByVal texture As SlimDX.Direct3D9.Texture, ByVal x As Single, ByVal y As Single, ByVal angle As Double, ByVal c As Integer) As SpriteItem
            Dim SD As New SpriteItem(texture, x, y, angle, c)
            sprites.Add(SD)
            Return SD
        End Function

        Public Function addSprite(ByVal texture As SlimDX.Direct3D9.Texture, ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal c As Integer) As SpriteItem
            Dim SD As New SpriteItem(texture, x, y, z, c)
            sprites.Add(SD)
            Return SD
        End Function

        Public Function addSprite(ByVal texture As SlimDX.Direct3D9.Texture, ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal angle As Double, ByVal c As Integer) As SpriteItem
            Dim SD As New SpriteItem(texture, x, y, z, angle, c)
            sprites.Add(SD)
            Return SD
        End Function

        '###########################

        Public Function addSprite(ByVal bitmap As Bitmap, ByVal x As Single, ByVal y As Single, ByVal c As Color) As SpriteItem
            Dim SD As New SpriteItem(bitmapToTexture(bitmap), x, y, c.ToArgb())
            'SD.originalWidth = bitmap.Width
            'SD.originalHeight = bitmap.Height
            sprites.Add(SD)
            Return SD
        End Function

        Public Function addSprite(ByVal bitmap As Bitmap, ByVal x As Single, ByVal y As Single, ByVal angle As Double, ByVal c As Color) As SpriteItem
            Dim SD As New SpriteItem(bitmapToTexture(bitmap), x, y, angle, c.ToArgb())
            'SD.originalWidth = bitmap.Width
            'SD.originalHeight = bitmap.Height
            sprites.Add(SD)
            Return SD
        End Function

        Public Function addSprite(ByVal bitmap As Bitmap, ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal c As Color) As SpriteItem
            Dim SD As New SpriteItem(bitmapToTexture(bitmap), x, y, z, c.ToArgb())
            'SD.originalWidth = bitmap.Width
            'SD.originalHeight = bitmap.Height
            sprites.Add(SD)
            Return SD
        End Function

        Public Function addSprite(ByVal bitmap As Bitmap, ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal angle As Double, ByVal c As Color) As SpriteItem
            Dim SD As New SpriteItem(bitmapToTexture(bitmap), x, y, z, angle, c.ToArgb())
            'SD.originalWidth = bitmap.Width
            'SD.originalHeight = bitmap.Height
            sprites.Add(SD)
            Return SD
        End Function

        '###########################

        Public Function addSprite(ByVal texture As SlimDX.Direct3D9.Texture, ByVal x As Single, ByVal y As Single, ByVal c As Color) As SpriteItem
            Dim SD As New SpriteItem(texture, x, y, c.ToArgb())
            sprites.Add(SD)
            Return SD
        End Function

        Public Function addSprite(ByVal texture As SlimDX.Direct3D9.Texture, ByVal x As Single, ByVal y As Single, ByVal angle As Double, ByVal c As Color) As SpriteItem
            Dim SD As New SpriteItem(texture, x, y, angle, c.ToArgb())
            sprites.Add(SD)
            Return SD
        End Function

        Public Function addSprite(ByVal texture As SlimDX.Direct3D9.Texture, ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal c As Color) As SpriteItem
            Dim SD As New SpriteItem(texture, x, y, z, c.ToArgb())
            sprites.Add(SD)
            Return SD
        End Function

        Public Function addSprite(ByVal texture As SlimDX.Direct3D9.Texture, ByVal x As Single, ByVal y As Single, ByVal z As Single, ByVal angle As Double, ByVal c As Color) As SpriteItem
            Dim SD As New SpriteItem(texture, x, y, z, angle, c.ToArgb())
            sprites.Add(SD)
            Return SD
        End Function


    End Class

End Namespace