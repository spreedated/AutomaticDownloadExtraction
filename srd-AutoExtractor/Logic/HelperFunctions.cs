using System;
using System.IO;
using System.Reflection;

namespace srd_AutoExtractor.Logic
{
    internal static class HelperFunctions
    {
        internal static string GetAssemblyDirectory()
        {
#pragma warning disable IL3000
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
#pragma warning restore IL3000

            if (assemblyLocation == null)
            {
                assemblyLocation = Environment.ProcessPath;
            }

            return Path.GetDirectoryName(assemblyLocation);
        }
    }
}
