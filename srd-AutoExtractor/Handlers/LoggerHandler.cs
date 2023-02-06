using Serilog;
using Serilog.Events;
using srdAutoExtractor.Logger.Enricher;
using srdAutoExtractor.Logic;
using System.IO;

namespace srdAutoExtractor.Handlers
{
    internal static class LoggerHandler
    {
#if DEBUG
        private static readonly LogEventLevel level = LogEventLevel.Verbose;
#else
        private static readonly LogEventLevel level = LogEventLevel.Information;
#endif
        private const string logOutputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}][{Caller}] {Message}{NewLine}{Exception}";

        internal static void ConfigureLogger()
        {
            string logfilepath = Path.Combine(HelperFunctions.GetAssemblyDirectory(), "logs", "logfile.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .Enrich.WithCaller()
#if DEBUG
                .WriteTo.Debug(restrictedToMinimumLevel: level, outputTemplate: logOutputTemplate)
#endif
                .WriteTo.File(logfilepath, restrictedToMinimumLevel: level, rollOnFileSizeLimit: true, fileSizeLimitBytes: 1048576, outputTemplate: logOutputTemplate)
                .CreateLogger();

            Log.Debug("Logger initialized");
        }
    }
}
