using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using srdAutoExtractor.Models;
using System;
using System.IO;
using System.Timers;

namespace srdAutoExtractor.Handlers
{
    internal class ConfigurationHandler<T>
    {
        private readonly CongfigurationHandlerOptions options;
        private Timer autoloadTimer;
        private bool isAutoloadTimerRunning;

        public event EventHandler AutoloadTriggered;
        public event EventHandler ConfigLoaded;
        public event EventHandler ConfigSaved;
        public event EventHandler ConfigInvalid;

        public T RuntimeConfiguration { get; private set; }

        #region Constructor
        public ConfigurationHandler(CongfigurationHandlerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentException("Cannot be null", nameof(options));
            }
            if (string.IsNullOrEmpty(options.ConfigPath))
            {
                throw new ArgumentException("The configpath cannot be unset", nameof(options));
            }

            this.RuntimeConfiguration = (T)Activator.CreateInstance(typeof(T));
            this.options = options;

            this.InterpretOptions();
        }
        private void InterpretOptions()
        {
            if (!Directory.Exists(Path.GetDirectoryName(this.options.ConfigPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(this.options.ConfigPath));
            }

            if (this.options.Autorefresh)
            {
                this.EnableAutoload();
            }
        }
        #endregion

        public void EnableAutoload()
        {
            if (isAutoloadTimerRunning)
            {
                return;
            }
            this.autoloadTimer?.Dispose();

            this.autoloadTimer = new()
            {
                Enabled = true,
                Interval = this.options.AutoloadInterval.TotalMilliseconds
            };

            this.autoloadTimer.Elapsed += this.Autloadtimer_Elapsed;
            this.autoloadTimer.Start();

            this.isAutoloadTimerRunning = true;
        }
        public void DisableAutoload()
        {
            if (!isAutoloadTimerRunning)
            {
                return;
            }
            this.autoloadTimer?.Dispose();
            this.isAutoloadTimerRunning = false;
        }
        private void Autloadtimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.AutoloadTriggered?.Invoke(this, EventArgs.Empty);
            this.Load();
        }

        public void Load()
        {
            if (!File.Exists(this.options.ConfigPath))
            {
                if (this.options.CreateOnNothing)
                {
                    Save();
                }
                return;
            }

            string json = null;

            using (FileStream fs = File.Open(this.options.ConfigPath, new FileStreamOptions() { BufferSize = this.options.Buffersize, Share = FileShare.Read, Mode = FileMode.Open }))
            {
                using (StreamReader r = new(fs))
                {
                    json = r.ReadToEnd();
                }
            }

            if (!IsValidJson(json))
            {
                this.ConfigInvalid?.Invoke(this, EventArgs.Empty);
                if (this.options.OverrideOnInvalid)
                {
                    Save();
                }
                return;
            }

            T c = JsonConvert.DeserializeObject<T>(json);

            if (this.RuntimeConfiguration != null && this.RuntimeConfiguration.Equals(c))
            {
                return;
            }

            this.RuntimeConfiguration = c;

            this.ConfigLoaded?.Invoke(this, EventArgs.Empty);
        }

        public void Save()
        {
            if (!File.Exists(this.options.ConfigPath))
            {
                TouchConfigFile();
            }

            using (FileStream fs = File.Open(this.options.ConfigPath, new FileStreamOptions() { BufferSize = this.options.Buffersize, Share = FileShare.ReadWrite, Mode = FileMode.Truncate, Access = FileAccess.ReadWrite }))
            {
                using (StreamWriter w = new(fs))
                {
                    if (this.RuntimeConfiguration == null)
                    {
                        this.RuntimeConfiguration = (T)Activator.CreateInstance(typeof(T));
                    }

                    w.Write(JsonConvert.SerializeObject(this.RuntimeConfiguration, Formatting.Indented));
                }
            }

            this.ConfigSaved?.Invoke(this, EventArgs.Empty);
        }

        internal void TouchConfigFile()
        {
            File.Create(this.options.ConfigPath).Dispose();
        }

        internal static bool IsValidJson(string json)
        {
            try
            {
                JObject.Parse(json);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
