Imports System.IO
Public Class Engine
    Private Property LoopRunning As Boolean = False
    'Public Shared ReadOnly DownloadFolderPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "AutoExtract")
    Public Shared ReadOnly DownloadFolderPath As String = "C:\Users\SpReeD\Downloads\AutoExtract"
    Private ReadOnly AllowedFileExtensions As String() = {"zip", "rar", "7z", "gz"}

    Private ReadOnly DetectedFiles As List(Of CompressedFile) = New List(Of CompressedFile)

    Public Sub Initialize()
        Log.Information(ASCIIArt.serviceArt)


        Log.Information("| [+] Inititalizing startup sequence...")
        With LoopTimer
            .Enabled = True
#If DEBUG Then
            .Interval = New TimeSpan(0, 0, 1).TotalMilliseconds
#Else
            .Interval = New TimeSpan(0, 0, 5).TotalMilliseconds
#End If
            .Start()
        End With
        LoopTimer_Tick() 'Kickstart on first run

        Log.Information("| [" & SpecialChars.GetChar(SpecialChars.Chars.Checkmark) & "] Service fully initialized!")
        Log.Information("")
    End Sub

#Region "Main Loop"
    Private WithEvents LoopTimer As Timers.Timer = New Timers.Timer
    Public Sub LoopTimer_Tick() Handles LoopTimer.Elapsed
        'Check if already running active loop
        If LoopRunning Then
            Exit Sub
        End If
        '# ### #
        LoopRunning = True

        'Integrity Checks
        If Not IntegrityChecksPassed Then
            If Not S0() Then
                LoopRunning = False
                Exit Sub
            End If
        End If
        '# ### #

        DetectFiles()
        RunExtractionTasks()
        UpdateXML()
        Cleanup()

        LoopRunning = False
    End Sub
#End Region

#Region "Step 0 - Integrity Checks"
    Private IntegrityChecksPassed As Boolean = False
    Private Function S0()
        'Check if Folder exists
        If Not Directory.Exists(downloadFolderPath) Then
            Try
                Directory.CreateDirectory(downloadFolderPath)
            Catch ex As Exception
                Log.Error(ex, "| [" & SpecialChars.GetChar(SpecialChars.Chars.Crossmark) & "] Error 2000: ")
                Return False
            End Try
        End If
        '# ### #

        'Is Folder writable
        Try
            File.Create(Path.Combine(downloadFolderPath, "tmp"), 8, FileOptions.None).Close()
            File.Delete(Path.Combine(downloadFolderPath, "tmp"))
        Catch ex As Exception
            Log.Error(ex, "| [" & SpecialChars.GetChar(SpecialChars.Chars.Crossmark) & "] Error 2001: ")
            Return False
        End Try
        '# ### #

        Log.Information("| [" & SpecialChars.GetChar(SpecialChars.Chars.Checkmark) & "] Integrity checks passed!")
        IntegrityChecksPassed = True
        Return True
    End Function

#End Region

#Region "Step 1 - Detection"
    Private Sub DetectFiles()
        'Detect Files
        Directory.GetFiles(DownloadFolderPath).All(Function(x)
                                                       Dim isInArray As Boolean = False
                                                       If Array.IndexOf(AllowedFileExtensions, Path.GetExtension(x).Replace(".", "")) <= -1 Then
                                                           Return True
                                                       End If

                                                       For Each f As CompressedFile In DetectedFiles
                                                           If f.FileInfo.FullName = x Then
                                                               isInArray = True
                                                           End If
                                                       Next
                                                       If Not isInArray Then
                                                           Dim f As CompressedFile = New CompressedFile With {
                                                            .FileInfo = New FileInfo(x)
                                                           }
                                                           DetectedFiles.Add(f)
                                                           Log.Information(String.Format("| [+] New file detected ""{0}""", x))
                                                       End If

                                                       Return True
                                                   End Function)
    End Sub
#End Region

#Region "Step 2 - Run extraction Tasks"
    Private Sub RunExtractionTasks()
        DetectedFiles.Where(Function(x) If(Not x.Processing And Not x.WasSuccessful, True, False)).All(Function(x)
                                                                                                           Task.Run(Sub()
                                                                                                                        Threading.Thread.CurrentThread.Name = "Extraction process for - " & x.FileInfo.Name
                                                                                                                        x.Date = Date.Now
                                                                                                                        Extraction.Extraction(x)
                                                                                                                    End Sub)
                                                                                                           Return True
                                                                                                       End Function)
    End Sub
#End Region

#Region "Step 2.1 - Update History XML"
    Private Sub UpdateXML()
        DetectedFiles.Where(Function(x) If(Not x.Processing And x.WasSuccessful, True, False)).All(Function(x)
                                                                                                       HistoryXML.Update(x)
                                                                                                       Return True
                                                                                                   End Function)
    End Sub
#End Region

#Region "Step 3 - Cleanup"
    Private Sub Cleanup()
        'Delete Finished files
        DetectedFiles.Where(Function(x) If(x.WasSuccessful And Not x.Processing, True, False)).All(Function(x)
                                                                                                       Try
                                                                                                           File.Delete(x.FileInfo.FullName)
                                                                                                       Catch ex As Exception
                                                                                                           Log.Error(ex, "| [" & SpecialChars.GetChar(SpecialChars.Chars.Crossmark) & "] Error 9000: ")
                                                                                                       End Try

                                                                                                       Return True
                                                                                                   End Function)

        DetectedFiles.RemoveAll(Function(x) If(x.WasSuccessful And Not x.Processing, True, False))
    End Sub
#End Region
End Class
