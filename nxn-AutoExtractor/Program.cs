using System;
using System.ServiceProcess;
using System.Threading;

namespace nxn_AutoExtractor
{
    internal static class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            ServiceEntry o = new();
            o.StartDebugging();
            Thread.Sleep(Timeout.Infinite);
#else
            ServiceBase.Run(new ServiceEntry());
#endif
        }
    }
}
