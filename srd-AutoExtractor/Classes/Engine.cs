using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using neXn.Lib5.ASCIIArt;
using neXn.Lib5.SpecialCharacters;
using Serilog;

namespace srd_AutoExtractor.Classes
{
    internal static class Engine
    {
        public static void Initialize()
        {
            //Welcome
            Log.Information(new Frame("neXn-Systems", new string[] { "Auto Extraction Service" }) { PrecedingLineBreak = true }.Build());

            //Init
            mainLoopTimer.Interval = 1000;
            mainLoopTimer.Elapsed += MainLoopTimerElapsed;
            mainLoopTimer.Start();

            Log.Information($"| [{Chars.Checkmark}] Service fully initialized!");
            Log.Information("");
        }

        #region Main Loop
        private static readonly System.Timers.Timer mainLoopTimer = new();
        private static bool loopRunning = false;
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