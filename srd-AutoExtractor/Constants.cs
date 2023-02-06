using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace srd_AutoExtractor
{
    internal static class Constants
    {
        public const string CONFIGURATION_FILENAME = "config.json";
        public static readonly string[] SUPPORTED_FILE_EXTENSIONS = { "7z", "rar", "zip", "tar.gz" };
    }
}
