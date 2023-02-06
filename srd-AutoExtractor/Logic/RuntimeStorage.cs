using srdAutoExtractor.Handlers;
using srdAutoExtractor.Models;

namespace srdAutoExtractor.Logic
{
    internal static class RuntimeStorage
    {
        internal static ConfigurationHandler<Configuration> ConfigurationHandler { get; set; }
    }
}
