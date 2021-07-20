
Imports SlimDX
Imports SlimDX.Direct3D9
Imports SharpNeatLib.Maths


Namespace smallDemo

    Public Class Forest

        Public forestTrees As New List(Of SpriteHandler.SpriteItem)

        'Useful objects we can use if we need/want to.
        Public x As Single = 0.0!
        Public y As Single = 0.0!
        Public z As Single = 0.0!

        Public screenWidth As Integer = 0
        Public screenHeight As Integer = 0

        Public slowRand As New Random()
        Public rand As New FastRandom()
        Public projection As SpriteEngine.PerspectiveProjection = Nothing
        Public spriteManager As SpriteHandler.SpriteManager = Nothing

        Public treeSprite As SpriteHandler.SpriteItem = Nothing
        Public treeTexture As SlimDX.Direct3D9.Texture = Nothing

        Public Sub New(ByVal spriteManager As SpriteHandler.SpriteManager, ByVal projection As SpriteEngine.PerspectiveProjection, ByVal screenWidth As Integer, ByVal screenHeight As Integer)
            Me.projection = projection
            Me.spriteManager = spriteManager
            Me.screenWidth = screenWidth
            Me.screenHeight = screenHeight
            initialise()
        End Sub

        Public Sub initialise()
            treeTexture = spriteManager.bitmapToTexture(My.Resources.treeSmall)

            'Position is middle of the display, and very far into the screen.
            x = CSng(screenWidth / 2)
            y = CSng(screenHeight / 2)
            z = 0.0!

            treeSprite = spriteManager.addSprite(treeTexture, x, y, z, Color.White)

            'Make it small as it's huge.
            treeSprite.width = 250.0!
            treeSprite.height = 300.0!

            forestTrees.Add(treeSprite)
        End Sub

        Public Sub update()

            'Move all the trees closer. (only one right now)
            For Each sprite As SpriteHandler.SpriteItem In forestTrees

                'Move tree a bit closer.
                sprite.z += 0.01!

                'If it's as close as possible, move it back to the distance again.
                If sprite.z > 1 Then sprite.z = 0.0!

            Next
        End Sub

    End Class

End Namespace