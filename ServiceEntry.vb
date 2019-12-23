Imports Serilog
Public Class ServiceEntry

    Protected Overrides Sub OnStart(ByVal args() As String)
        Log.Logger = New LoggerConfiguration() _
                            .WriteTo.Debug(0) _
                            .CreateLogger()

        Try
            Dim i As Engine = New Engine
            i.Initialize()
        Catch ex As Exception
            Log.Error(ex, "| Error 1000: ")
        End Try
    End Sub

    Protected Overrides Sub OnStop()
        Log.Information("+ ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ +")
        Log.Information("|          Service closed         |")
        Log.Information("+ ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ +")

    End Sub

End Class
