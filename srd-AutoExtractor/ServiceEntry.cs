#pragma warning disable S1185

using Serilog;
using srdAutoExtractor.Collections;
using srdAutoExtractor.Handlers;
using srdAutoExtractor.Logic;
using srdAutoExtractor.Services;
using System.IO;
using System.ServiceProcess;

namespace srdAutoExtractor
{
    internal class ServiceEntry : ServiceBase
    {
        private readonly ServiceList<IService> services = new();

        private static void LoadConfiguration()
        {
            RuntimeStorage.ConfigurationHandler = new ConfigurationHandler<Models.Configuration>(new()
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

            services.Add(new ExtractionService(RuntimeStorage.ConfigurationHandler.RuntimeConfiguration.WatchFolder));
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
