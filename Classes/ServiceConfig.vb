Imports Microsoft.Win32
Imports System.IO
Public Class ServiceConfig
    Public Shared writeHistoryFile As Boolean
    Public Shared writeLogFile As Boolean
    Public Shared operatingPath As String

    Public Shared Sub InitConfig()
        'Load Config File or create
        Dim keyName As String = Nothing
        Using r As RegistryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Services")
            keyName = r.GetSubKeyNames.Where(Function(x) If(x.ToString.Contains(System.Reflection.Assembly.GetExecutingAssembly.GetName.Name), True, False)).FirstOrDefault
        End Using

        Dim serviceDirectory As String = Nothing
        Dim backupR As Boolean = False
        If keyName Is Nothing Then
            Console.WriteLine("| Error 7000: Couldn't find myself in windows registry...")
            Console.WriteLine("| Trying other way...")
            backupR = True
        Else
            Using regK As RegistryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\CurrentControlSet\Services\" & keyName)
                serviceDirectory = Path.GetDirectoryName(regK.GetValue("ImagePath"))
            End Using
        End If

        If backupR Then
            If Not Directory.Exists(Path.Combine(Environment.CurrentDirectory)) Then
                Console.WriteLine("| Error 7001: Couldn't find config file... exiting.")
                Environment.Exit(-1)
            Else
                serviceDirectory = Environment.CurrentDirectory
            End If
        End If


        Dim content As String = "[config]" & vbLf &
                                "writeLogFile=0" & vbLf &
                                "writeHistoryFile=1" & vbLf &
                                "operatingPath="
        If Not File.Exists(Path.Combine(serviceDirectory, "config.nxn")) Then
            Using f As FileStream = File.Create(Path.Combine(serviceDirectory, "config.nxn"), 4096, FileOptions.SequentialScan)
                Dim contentB As Byte() = System.Text.Encoding.UTF8.GetBytes(content)
                f.Write(contentB, 0, contentB.Length)
                f.Close()
            End Using
        Else
            content = ""
            Using f As FileStream = File.Open(Path.Combine(serviceDirectory, "config.nxn"), FileMode.Open, FileAccess.ReadWrite)
                Using fS As StreamReader = New StreamReader(f)
                    While Not fS.EndOfStream
                        content &= fS.ReadLine & vbLf
                    End While
                    fS.Close()
                End Using
                f.Close()
            End Using
        End If

        Dim i As IniParser.Parser.IniDataParser = New IniParser.Parser.IniDataParser()
        Dim b As IniParser.Model.IniData = i.Parse(content)

        Debug.Print(b("config").GetKeyData("writeLogFile").Value)
        ServiceConfig.writeLogFile = Convert.ToBoolean(Convert.ToInt16(b("config").GetKeyData("writeLogFile").Value))
        ServiceConfig.writeHistoryFile = Convert.ToBoolean(Convert.ToInt16(b("config").GetKeyData("writeHistoryFile").Value))
        ServiceConfig.operatingPath = Convert.ToString(b("config").GetKeyData("operatingPath").Value)
        '# ### #
    End Sub

    Public Shared Sub LoadLogger()
        If writeLogFile Then
            Log.Logger = New LoggerConfiguration() _
                                .WriteTo.Debug(0) _
                                .WriteTo.File(Path.Combine(operatingPath, "logfile.log"), rollOnFileSizeLimit:=True, fileSizeLimitBytes:=1024000) _
                                .CreateLogger()
        Else
            Log.Logger = New LoggerConfiguration() _
                            .WriteTo.Debug(0) _
                            .CreateLogger()
        End If
    End Sub
End Class
