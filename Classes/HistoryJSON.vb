Imports Newtonsoft.Json
Imports System.IO
Public Class HistoryJSON
    Private Shared ReadOnly jsonDocPath As String = Path.Combine(ServiceConfig.operatingPath, "_extractionHistory.nxn")
    Public Shared Sub Update(ByRef x As CompressedFile)
        Dim jsonDoc As String = ""

        Using r As StreamReader = New StreamReader(File.Open(jsonDocPath, FileMode.Open))
            While Not r.EndOfStream
                jsonDoc &= r.ReadLine
            End While
        End Using
    End Sub
End Class
