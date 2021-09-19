using System;
using System.ServiceProcess;
using System.Threading;

namespace nxn_AutoExtractor
{
    internal class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            ServiceEntry o = new ServiceEntry();
            o.StartDebugging();
            Thread.Sleep(Timeout.Infinite);
#else
            ServiceBase.Run(new ServiceEntry());
#endif
        }
    }
}
