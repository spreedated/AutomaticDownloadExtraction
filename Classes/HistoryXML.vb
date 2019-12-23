Imports System.IO
Imports System.Globalization
Imports System.Reflection
Public Class HistoryXML
    Private Shared ReadOnly xmlDocPath As String = Path.Combine(Engine.DownloadFolderPath, "_extractionHistory.xml")
    Public Shared Sub Update(ByRef x As CompressedFile)
        Dim xmlDoc As System.Xml.XmlDocument = New Xml.XmlDocument

        If File.Exists(xmlDocPath) Then
            xmlDoc.Load(xmlDocPath)
        Else
            If Not Directory.Exists(Path.GetDirectoryName(xmlDocPath)) Then
                Directory.CreateDirectory(Path.GetDirectoryName(xmlDocPath))
            End If
            'Create XML
            Dim xmlSettings As Xml.XmlWriterSettings = New Xml.XmlWriterSettings With {
                .Indent = True
                }
            Using writer As Xml.XmlWriter = System.Xml.XmlWriter.Create(xmlDocPath, xmlSettings)
                'writer.Settings.Indent = True
                writer.WriteStartDocument()

                'Root
                writer.WriteStartElement("AutoExtractionService")
                writer.WriteEndElement()
                '# ### #

                writer.WriteEndDocument()
                writer.Close()
            End Using
            '# ### #
            xmlDoc.Load(xmlDocPath)
        End If

        'Create New Node for File
        Dim rootNode As Xml.XmlNode = xmlDoc.SelectSingleNode("AutoExtractionService")

        'New Parent Node
        Dim fileNode As Xml.XmlNode = xmlDoc.CreateNode(Xml.XmlNodeType.Element, "ExtractedFile", "")
        Dim arrayList As Object(,) = {
                                        {"Name", x.FileInfo.Name},
                                        {"Type", x.FileInfo.Extension.Replace(".", "")},
                                        {"Filesize", x.FileInfo.Length},
                                        {"Filepath", x.FileInfo.FullName},
                                        {"TotalDirectories", x.EntriesDirectory},
                                        {"TotalFiles", x.EntriesFile},
                                        {"ExtractionDuration", x.OperationTime},
                                        {"Successful", x.WasSuccessful},
                                        {"Date", x.Date.ToString("g", CultureInfo.CreateSpecificCulture("de-DE"))}
        }
        For i = 0 To arrayList.GetLength(0) - 1
            Dim b As Xml.XmlElement = xmlDoc.CreateElement(arrayList(i, 0))
            b.InnerText = arrayList(i, 1).ToString
            fileNode.AppendChild(b)
        Next
        rootNode.AppendChild(fileNode)
        '# ### #

        xmlDoc.Save(xmlDocPath)

        Log.Information("| [" & SpecialChars.GetChar(SpecialChars.Chars.Checkmark) & "] History Updated - """ & x.FileInfo.Name & """")
    End Sub
End Class
