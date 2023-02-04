#pragma warning disable IDE0063

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace nxn_AutoExtractor.Classes
{
    internal class Configuration
    {
        public Models.Configuration Config { get; private set; }

        internal string confPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Constants.CONFIGURATION_FILENAME);

        #region Singleton
        private static Configuration instance;
        private Configuration()
        {
            this.Load();
        }
        public static Configuration GetInstance()
        {
            if (instance == null)
            {
                instance = new Configuration();
            }
            return instance;
        }
        #endregion

        private void Load()
        {
            string json = null;

            if (!File.Exists(this.confPath))
            {
                json = JsonConvert.SerializeObject(new Models.Configuration(), Formatting.Indented);

                using (FileStream f = File.Create(this.confPath))
                {
                    using (StreamWriter w = new(f))
                    {
                        w.Write(json);
                    }
                }
            }
            else
            {
                using (FileStream f = File.Open(this.confPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader r = new(f))
                    {
                        json = r.ReadToEnd();
                    }
                }
            }

            this.Config = JsonConvert.DeserializeObject<Models.Configuration>(json);
        }
    }
}
