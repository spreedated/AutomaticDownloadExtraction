using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.IO;

namespace nxn_AutoExtractor.Logic
{
    internal class ConfigurationHandler
    {
        private readonly string configPath;

        #region Constructor
        public ConfigurationHandler(string configPath)
        {
            this.configPath = configPath;
        }
        #endregion

        public void Load()
        {
            if (!File.Exists(configPath))
            {
                Log.Information($"No config found, creating new with default values");
                Save();
                return;
            }

            string json = null;

            using (FileStream fs = File.Open(configPath, new FileStreamOptions() { BufferSize = 128, Share = FileShare.Read, Mode = FileMode.Open }))
            {
                using (StreamReader r = new(fs))
                {
                    json = r.ReadToEnd();
                }
            }

            if (!IsValidJson(json))
            {
                Log.Warning($"Invalid config file found");
                Save();
                return;
            }

            ObjectStorage.Config = JsonConvert.DeserializeObject<Models.Configuration>(json);

            Log.Information($"Config loaded");
        }

        public void Save()
        {
            if (!File.Exists(configPath))
            {
                TouchConfigFile();
            }

            using (FileStream fs = File.Open(configPath, new FileStreamOptions() { BufferSize = 128, Share = FileShare.ReadWrite, Mode = FileMode.Truncate, Access = FileAccess.ReadWrite }))
            {
                using (StreamWriter w = new(fs))
                {
                    if (ObjectStorage.Config == null)
                    {
                        ObjectStorage.Config = new();
                    }

                    w.Write(JsonConvert.SerializeObject(ObjectStorage.Config, Formatting.Indented));
                }
            }

            Log.Information($"Config saved");
        }

        internal void TouchConfigFile()
        {
            File.Create(this.configPath).Dispose();
        }

        internal static bool IsValidJson(string json)
        {
            try
            {
                JObject.Parse(json);
            }
            catch (System.Exception)
            {
                return false;
            }

            return true;
        }
    }
}
