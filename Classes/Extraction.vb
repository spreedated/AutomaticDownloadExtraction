Imports System.IO
Public Class Extraction
    ''' <summary>
    ''' Checks if the file we want to access is locked or not.
    ''' A lock means, another process might be still writing the file, e.g. the scanner
    ''' </summary>
    ''' <param name="filePath"></param>
    ''' <returns></returns>
    Public Shared Function IsFileLocked(ByVal filePath As String) As Boolean
        Dim stream As FileStream = Nothing
        Try
            stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None)
        Catch ex As Exception
            Return True
        Finally
            If Not stream Is Nothing Then
                stream.Close()
            End If
        End Try
        Return False
    End Function

    Public Shared Sub Extraction(ByRef x As CompressedFile)
        If Not x.Processing Then
            x.Processing = True
        Else
            Exit Sub
        End If

        'Check if file is locked
        If IsFileLocked(x.FileInfo.FullName) Then
            ExitExtraction(x)
            Exit Sub
        End If
        '# ### #

        Dim sWatch As Stopwatch = New Stopwatch
        sWatch.Start()

        Dim newFolder As String = Path.Combine(ServiceConfig.operatingPath, Path.GetFileNameWithoutExtension(x.FileInfo.Name))
        Try
            If Not Directory.Exists(newFolder) Then
                Directory.CreateDirectory(newFolder)
            Else
                Log.Warning("| [~] Folder already exists, extraction aborted")
                ExitExtraction(x)
                Exit Sub
            End If
        Catch ex As Exception
            Log.Error(ex, "| [" & SpecialChars.GetChar(SpecialChars.Chars.Crossmark) & "] Error 3000: ")
            Exit Sub
        End Try

        Log.Information("| [+] Extraction started for """ & x.FileInfo.Name & """")
        Try
            Using archiveFile As SevenZipExtractor.ArchiveFile = New SevenZipExtractor.ArchiveFile(x.FileInfo.FullName)
                'Debug.Print(archiveFile.Entries.Count)
                For Each entry In archiveFile.Entries
                    If entry.IsFolder Then
                        Directory.CreateDirectory(Path.Combine(newFolder, entry.FileName))
                        x.EntriesDirectory += 1
                    Else
                        entry.Extract(Path.Combine(newFolder, entry.FileName))
                        x.EntriesFile += 1
                    End If
                Next
            End Using

            'tar.gz Exception (Addtional Extraction of TAR)
            If x.FileInfo.Name.EndsWith(".tar.gz") Then
                Dim tarName As String = Directory.GetFiles(newFolder).FirstOrDefault
                Dim newerFolder As String = Path.Combine(ServiceConfig.operatingPath, x.FileInfo.Name.Replace(".tar.gz", ""))
                If Not Directory.Exists(newerFolder) Then
                    Directory.CreateDirectory(newerFolder)
                End If

                Using archiveFile As SevenZipExtractor.ArchiveFile = New SevenZipExtractor.ArchiveFile(tarName)
                    'Debug.Print(archiveFile.Entries.Count)
                    For Each entry In archiveFile.Entries
                        If entry.IsFolder Then
                            Directory.CreateDirectory(Path.Combine(newerFolder, entry.FileName))
                            x.EntriesDirectory += 1
                        Else
                            entry.Extract(Path.Combine(newerFolder, entry.FileName))
                            x.EntriesFile += 1
                        End If
                    Next
                End Using

                If Directory.Exists(newerFolder) Then
                    Directory.Delete(newerFolder & ".tar", True)
                End If
            End If
            '# ### #


            sWatch.Stop()

            Log.Information("| [" & SpecialChars.GetChar(SpecialChars.Chars.Checkmark) & "] Extraction successful for """ & x.FileInfo.Name & """ in " & sWatch.Elapsed.TotalSeconds.ToString("0.##") & " Seconds")
            x.OperationTime = sWatch.Elapsed.TotalSeconds
        Catch ex As Exception
            Log.Error(ex, "| [" & SpecialChars.GetChar(SpecialChars.Chars.Crossmark) & "] Error 3001: ")
            ExitExtraction(x)
            Exit Sub
        End Try

        ExitExtraction(x, True)
    End Sub

    Private Shared Sub ExitExtraction(ByRef x As CompressedFile, ByVal Optional success As Boolean = False)
        With x
            .Processing = False
            .WasSuccessful = success
        End With
    End Sub
End Class
