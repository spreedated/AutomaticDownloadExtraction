#pragma warning disable S1185

using Serilog;
using srd_AutoExtractor.Classes;
using srd_AutoExtractor.Handlers;
using srd_AutoExtractor.Logic;
using System.IO;
using System.ServiceProcess;

namespace srd_AutoExtractor
{
    internal class ServiceEntry : ServiceBase
    {
        private static void LoadConfiguration()
        {
            RuntimeStorage.ConfigurationHandler = new(RuntimeStorage.Configuration, new()
            {
                ConfigPath = Path.Combine(Path.GetDirectoryName(typeof(ServiceEntry).Assembly.Location), Constants.CONFIGURATION_FILENAME)
            });

            RuntimeStorage.ConfigurationHandler.ConfigSaved += (o, e) => Log.Verbose($"Config saved");
            RuntimeStorage.ConfigurationHandler.ConfigInvalid += (o, e) => Log.Warning($"Invalid config file found");
            RuntimeStorage.ConfigurationHandler.ConfigLoaded += (o, e) => Log.Verbose($"Config loaded");
            RuntimeStorage.ConfigurationHandler.AutoloadTriggered += (o, e) => Log.Verbose($"Refreshing config");

            RuntimeStorage.ConfigurationHandler.Load();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            LoggerHandler.ConfigureLogger();
            LoadConfiguration();

            Engine.Initialize();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public void StartDebugging()
        {
            this.OnStart(null);
        }
    }
}
