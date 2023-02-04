using Newtonsoft.Json;

namespace srd_AutoExtractor.Models
{
    internal class Configuration
    {
        [JsonProperty("osPath")]
        internal string OperatingPath { get; set; }
        [JsonProperty("writeLogFile")]
        internal bool WriteLogfile { get; set; }
        [JsonProperty("writeHistory")]
        internal bool WriteHistory { get; set; } = true;
    }
}