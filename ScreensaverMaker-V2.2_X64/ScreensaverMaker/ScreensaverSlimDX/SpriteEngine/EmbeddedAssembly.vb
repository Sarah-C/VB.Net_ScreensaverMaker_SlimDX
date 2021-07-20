Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports System.Reflection
Imports System.Security.Cryptography
Imports System.Threading
Imports System.Runtime.InteropServices

Namespace SpriteEngine

    ''' <summary>
    ''' A class for loading Embedded Assembly
    ''' </summary>
    Public Class EmbeddedAssembly

        Public Shared trd As Thread = Nothing

        <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
        Public Shared Function LoadLibrary(<[In](), MarshalAs(UnmanagedType.LPStr)> ByVal lpFileName As String) As IntPtr
        End Function
        <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
        Public Shared Function GetModuleHandle(ByVal lpModuleName As String) As IntPtr
        End Function
        <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
        Public Shared Function FreeLibrary(<[In]()> ByVal hModule As IntPtr) As Boolean
        End Function

        Public Shared Sub unpack(ByVal embeddedResource As String, ByVal fileName As String)
            Dim ba() As Byte = Nothing
            Dim asm As System.Reflection.Assembly = Nothing
            Dim curAsm As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
            Dim unpackPath As String = Application.StartupPath() & "\"
            Using stm As Stream = curAsm.GetManifestResourceStream(embeddedResource)
                If stm Is Nothing Then
                    Throw New Exception(fileName & " is not found in embedded resources.")
                End If
                ba = New Byte(CInt(stm.Length) - 1) {}
                Try
                    stm.Read(ba, 0, CInt(stm.Length))
                    File.WriteAllBytes(unpackPath & fileName, ba)
                Catch e As Exception
                End Try
            End Using
        End Sub

        Public Shared Sub delete(ByVal fileName As String)
            trd = New Thread(Sub() deleteFileThread(fileName))
            trd.IsBackground = False
            trd.Start()
        End Sub

        Public Shared Sub deleteFileThread(ByVal fileNameData As Object)
            Thread.Sleep(500)
            Dim fileName As String = CType(fileNameData, String)
            Dim path As String = Application.StartupPath() & "\" & fileName
            Try
                Dim hAddress As Long = CLng(GetModuleHandle(fileName))
                While hAddress >= 1
                    FreeLibrary(CType(hAddress, IntPtr))
                    hAddress = CLng(GetModuleHandle(fileName))
                End While
                If HasAccess(path) Then My.Computer.FileSystem.DeleteFile(path)
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End Sub

        Public Shared Function HasAccess(ByVal ltFullPath As String) As Boolean
            Try
                Using inputstreamreader As New StreamReader(ltFullPath)
                    inputstreamreader.Close()
                End Using
                Using inputStream As FileStream = File.Open(ltFullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None)
                    inputStream.Close()
                    Return True
                End Using
            Catch ex As Exception
                Return False
            End Try
        End Function

        ' Version 1.3
        Private Shared dic As Dictionary(Of String, System.Reflection.Assembly) = Nothing

        ''' <summary>
        ''' Load Assembly, DLL from Embedded Resources into memory.
        ''' </summary>
        ''' <param name="embeddedResource">Embedded Resource string. Example: WindowsFormsApplication1.SomeTools.dll</param>
        ''' <param name="fileName">File Name. Example: SomeTools.dll</param>
        Public Shared Sub preLoad(ByVal embeddedResource As String, ByVal fileName As String)

            Dim verbose As Boolean = False

            If verbose Then MsgBox("Loading assembly from embedded resources: " & fileName)

            If dic Is Nothing Then
                dic = New Dictionary(Of String, System.Reflection.Assembly)()
            End If

            Dim ba() As Byte = Nothing
            Dim asm As System.Reflection.Assembly = Nothing
            Dim curAsm As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()


            'If verbose then MsgBox("Manifest: " & String.Join(", " & vbcrlf, curAsm.GetManifestResourceNames))

            Using stm As Stream = curAsm.GetManifestResourceStream(embeddedResource)
                ' Either the file does not exist or it is not mark as embedded resource
                If stm Is Nothing Then
                    Throw New Exception(fileName & " is not found in embedded resources.")
                End If

                ' Get byte[] from the file from embedded resource
                ba = New Byte(CInt(stm.Length) - 1) {}
                stm.Read(ba, 0, CInt(stm.Length))
                Try
                    asm = System.Reflection.Assembly.Load(ba)

                    ' Add the assembly/dll into dictionary
                    dic.Add(asm.FullName, asm)
                    If verbose Then MsgBox("Loaded direct from embedded resources OK: " & fileName)
                    Return
                Catch e As Exception
                    If verbose Then MsgBox(fileName & " - Error: " & e.Message)

                    ' Purposely do nothing
                    ' Unmanaged dll or assembly cannot be loaded directly from byte[]
                    ' Let the process fall through for next part
                End Try
            End Using

            Dim fileOk As Boolean = False
            Dim tempFile As String = ""

            Using sha1 As New SHA1CryptoServiceProvider()
                ' Get the hash value from embedded DLL/assembly
                Dim fileHash As String = BitConverter.ToString(sha1.ComputeHash(ba)).Replace("-", String.Empty)

                ' Define the temporary storage location of the DLL/assembly
                tempFile = Path.GetTempPath() & fileName

                ' Determines whether the DLL/assembly is existed or not
                If File.Exists(tempFile) Then
                    If verbose Then MsgBox("Attempting to load from " & tempFile)
                    ' Get the hash value of the existed file
                    Dim bb() As Byte = File.ReadAllBytes(tempFile)
                    Dim fileHash2 As String = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", String.Empty)

                    ' Compare the existed DLL/assembly with the Embedded DLL/assembly
                    If fileHash = fileHash2 Then
                        ' Same file
                        fileOk = True
                    Else
                        ' Not same
                        fileOk = False
                    End If
                Else
                    If verbose Then MsgBox("File does not exist: " & tempFile)
                    ' The DLL/assembly is not existed yet
                    fileOk = False
                End If
            End Using

            ' Create the file on disk
            If Not fileOk Then
                If verbose Then MsgBox("Creating file from embedded resources: " & tempFile)
                System.IO.File.WriteAllBytes(tempFile, ba)
            End If

            If verbose Then MsgBox("Loading file from: " & tempFile)
            ' Load it into memory
            asm = System.Reflection.Assembly.LoadFile(tempFile)
            If verbose Then MsgBox("Loaded from file OK: " & tempFile)

            ' Add the loaded DLL/assembly into dictionary
            dic.Add(asm.FullName, asm)
        End Sub

        ''' <summary>
        ''' Retrieve specific loaded DLL/assembly from memory
        ''' </summary>
        ''' <param name="assemblyFullName"></param>
        ''' <returns></returns>
        Public Shared Function [Get](ByVal assemblyFullName As String) As System.Reflection.Assembly
            If dic Is Nothing OrElse dic.Count = 0 Then
                Return Nothing
            End If

            If dic.ContainsKey(assemblyFullName) Then
                Return dic(assemblyFullName)
            End If

            Return Nothing

            ' Don't throw Exception if the dictionary does not contain the requested assembly.
            ' This is because the event of AssemblyResolve will be raised for every
            ' Embedded Resources (such as pictures) of the projects.
            ' Those resources wil not be loaded by this class and will not exist in dictionary.
        End Function

    End Class

End Namespace