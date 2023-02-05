using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using neXn.Lib.Files;
using Serilog;
using srd_AutoExtractor.Logic;

namespace srd_AutoExtractor.Classes
{
    internal static class Engine
    {
        public static void Initialize()
        {
            //Init
            mainLoopTimer.Interval = 1000;
            mainLoopTimer.Elapsed += MainLoopTimerElapsed;
            mainLoopTimer.Start();

            Log.Information($"Service fully initialized!");
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
        private static bool systemwatcheralreadyrunning = false;
        private static ConcurrentDictionary<string, bool> work = new();
        private static FileSystemWatcher watcher = null;
        public static async Task<bool> S0()
        {
            Task<bool> t = Task<bool>.Factory.StartNew(() =>
            {
                if (systemwatcheralreadyrunning)
                {
                    return true;
                }
                systemwatcheralreadyrunning = true;

                CheckWatchFolder();
                InitializeWatcher();

                watcher.Error += (o,e) =>
                {
                    watcher.Dispose();
                    systemwatcheralreadyrunning = false;
                    CheckWatchFolder();
                    InitializeWatcher();
                };

                watcher.Created += async (o, e) =>
                {
                    if ((e.Name.EndsWith(".zip") || e.Name.EndsWith(".7z") || e.Name.EndsWith(".rar")) && (!work.ContainsKey(e.FullPath) || !work[e.FullPath]))
                    {
                        work.TryAdd(e.FullPath, true);

                        Extraction ex = new(e.FullPath);
                        ex.ExtractionStarted += ExtractionStarted;
                        ex.ExtractionCompleted += ExtractionCompleted;

                        await Task.Delay(500);

                        while (Core.IsFileLocked(e.FullPath))
                        {
                            await Task.Delay(50);
                        }

                        await ex.ExtractAsync();

                        work[e.FullPath] = false;
                    }
                };

                return true;
            });
            await t;
            return t.Result;
        }
        #endregion

        private static void InitializeWatcher()
        {
            watcher = new(RuntimeStorage.ConfigurationHandler.RuntimeConfiguration.WatchFolder)
            {
                EnableRaisingEvents = true,
                
            };
        }

        private static void CheckWatchFolder()
        {
            if (!Directory.Exists(RuntimeStorage.ConfigurationHandler.RuntimeConfiguration.WatchFolder))
            {
                Directory.CreateDirectory(RuntimeStorage.ConfigurationHandler.RuntimeConfiguration.WatchFolder);
            }
        }

        private static void ExtractionStarted(object sender, ExtractionStartedEventArgs e)
        {
            Log.Information($"Extracting \"{e.Filepath}\"...");
        }

        private static void ExtractionCompleted(object sender, ExtractionCompletedEventArgs e)
        {
            Log.Information($"Extraction completed for file \"{e.Filepath}\" - process duration {e.Duration:hh\\:mm\\:ss\\:ffffff}");
        }
    }
}