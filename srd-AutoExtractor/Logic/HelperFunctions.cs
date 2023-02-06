using System;
using System.IO;
using System.Reflection;

namespace srdAutoExtractor.Logic
{
    internal static class HelperFunctions
    {
        internal static string GetAssemblyDirectory()
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;

            if (assemblyLocation == null)
            {
                assemblyLocation = Environment.ProcessPath;
            }

            return Path.GetDirectoryName(assemblyLocation);
        }
    }
}
