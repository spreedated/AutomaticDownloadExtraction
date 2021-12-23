#pragma warning disable S1185

using nxn_AutoExtractor.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace nxn_AutoExtractor
{
    internal class ServiceEntry : ServiceBase
    {
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            Engine.Initialize();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        public void StartDebugging()
        {
            this.OnStart(null);
        }
    }
}
