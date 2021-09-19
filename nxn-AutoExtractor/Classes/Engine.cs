using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace nxn_AutoExtractor.Classes
{
    internal static class Engine
    {
        public static void Initialize()
        {
            //Init
            mainLoopTimer.Interval = 1000;
            mainLoopTimer.Elapsed += MainLoopTimerElapsed;
            mainLoopTimer.Start();
        }

        #region Main Loop
        internal static readonly System.Timers.Timer mainLoopTimer = new();
        internal static bool loopRunning = false;
        public static async void MainLoopTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (loopRunning)
            {
                return;
            }

            loopRunning = true;

            //Step 0
            Task<bool> t0 = S0();
            await t0;
            //# ### #

            loopRunning = false;
        }
        #endregion

        #region Step 0
        public static async Task<bool> S0()
        {
            Task<bool> t = Task<bool>.Factory.StartNew(() =>
            {
                Thread.Sleep(5000);

                return true;
            });
            await t;
            return t.Result;
        }
        #endregion
    }
}
