using System;

namespace srdAutoExtractor.Models
{
    internal class CongfigurationHandlerOptions
    {
        /// <summary>
        /// The absolute path for the configuration JSON
        /// </summary>
        public string ConfigPath { get; set; }
        /// <summary>
        /// Determines if the config should auto refresh
        /// </summary>
        public bool Autorefresh { get; set; } = true;
        /// <summary>
        /// The interval for the autorefresh feature<br/>
        /// default is one minute
        /// </summary>
        public TimeSpan AutoloadInterval { get; set; } = new TimeSpan(0, 1, 0);
        /// <summary>
        /// Determines if a corrupted JSON should be overwritten
        /// </summary>
        public bool OverrideOnInvalid { get; set; } = true;
        /// <summary>
        /// Determines if a config file should be created when none has been found
        /// </summary>
        public bool CreateOnNothing { get; set; } = true;
        /// <summary>
        /// Filestream buffersize, default is 128
        /// </summary>
        public int Buffersize { get; set; } = 128;
    }
}
