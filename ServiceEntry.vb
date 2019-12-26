Public Class ServiceEntry

    Protected Overrides Sub OnStart(ByVal args() As String)
        'Load config first
        ServiceConfig.InitConfig()
        'Load Logger
        ServiceConfig.LoadLogger()

        Try
            Dim i As Engine = New Engine
            i.Initialize()
        Catch ex As Exception
            Log.Error(ex, "| Error 1000: ")
            Environment.Exit(-1)
        End Try
    End Sub

    Protected Overrides Sub OnStop()
        Log.Information("+ ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ +")
        Log.Information("|          Service closed         |")
        Log.Information("+ ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ +")
    End Sub

End Class
