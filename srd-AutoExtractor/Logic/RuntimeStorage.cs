using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using srd_AutoExtractor.Models;
using srd_AutoExtractor.Handlers;

namespace srd_AutoExtractor.Logic
{
    internal static class RuntimeStorage
    {
        internal static Configuration Configuration { get; set; }
        internal static ConfigurationHandler<Configuration> ConfigurationHandler { get; set; }
    }
}
