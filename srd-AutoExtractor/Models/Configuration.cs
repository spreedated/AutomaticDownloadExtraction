using Newtonsoft.Json;
using System;
using System.IO;

namespace srd_AutoExtractor.Models
{
    internal class Configuration
    {
        [JsonProperty("writeLogFile")]
        internal bool WriteLogfile { get; set; }
        [JsonProperty("writeHistory")]
        internal bool WriteHistory { get; set; } = true;
        [JsonProperty("watchfolder")]
        internal string WatchFolder { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "AutoExtract");

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Configuration))
            {
                return false;
            }
            Configuration other = obj as Configuration;

            return this.WriteHistory == other.WriteHistory &&
                this.WriteLogfile == other.WriteLogfile &&
                this.WatchFolder == other.WatchFolder;
        }

        public override int GetHashCode()
        {
            return this.WriteLogfile.GetHashCode() ^
                this.WatchFolder.GetHashCode() ^
                this.WriteHistory.GetHashCode();
        }
    }
}