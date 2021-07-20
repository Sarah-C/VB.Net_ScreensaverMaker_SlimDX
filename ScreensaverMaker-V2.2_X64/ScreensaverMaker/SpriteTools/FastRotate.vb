
Imports SharpNeatLib.Maths


Namespace SpriteTools

    Public Class fastRotate

        Public sin(36000) As Single
        Public cos(36000) As Single

        Public xx As Single = 0.0!
        Public xy As Single = 0.0!
        Public xz As Single = 0.0!

        Public yx As Single = 0.0!
        Public yy As Single = 0.0!
        Public yz As Single = 0.0!

        Public zx As Single = 0.0!
        Public zy As Single = 0.0!
        Public zz As Single = 0.0!

        Public Sub New()
            For i As Integer = 0 To 35999
                sin(i) = CSng(Math.Sin((i / 100) / 180.0! * Math.PI))
                cos(i) = CSng(Math.Cos((i / 100) / 180.0! * Math.PI))
            Next
        End Sub

        Public Function modAngle(ByVal angle As Single) As Integer
            Dim angleFixed As Integer = CInt(angle * 100.0!)
            Dim modOfAngle As Integer = angleFixed Mod 36000
            If modOfAngle < 0 Then modOfAngle += 36000
            Return modOfAngle
        End Function

        Public Sub newRotationMatrix(ByVal xxx As Single, ByVal yyy As Single, ByVal zzz As Single)
            Dim x As Integer = modAngle(xxx)
            Dim y As Integer = modAngle(yyy)
            Dim z As Integer = modAngle(zzz)
            xx = cos(z) * cos(y)
            xy = -sin(z) * cos(y)
            xz = -sin(y)
            yx = sin(z) * cos(x) - cos(z) * sin(y) * sin(x)
            yy = cos(z) * cos(x) + sin(z) * sin(y) * sin(x)
            yz = -cos(y) * sin(x)
            zx = sin(z) * sin(x) + cos(z) * sin(y) * cos(x)
            zy = cos(z) * sin(x) - sin(z) * sin(y) * cos(x)
            zz = cos(y) * cos(x)
        End Sub

        Public Sub rotatePoint()
            'Use these to rotate points...
            'Dim x2 As Single = x * xx + y * xy + z * xz
            'Dim y2 As Single = x * yx + y * yy + z * yz
            'Dim z2 As Single = x * zx + y * zy + z * zz
        End Sub


    End Class

End Namespace