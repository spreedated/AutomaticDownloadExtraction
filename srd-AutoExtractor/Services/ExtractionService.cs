using neXn.Lib.Files;
using neXn.Lib.Paths;
using Serilog;
using srdAutoExtractor.Logic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace srdAutoExtractor.Services
{
    internal class ExtractionService : IService, IDisposable
    {
        private readonly ConcurrentDictionary<string, bool> work = new();
        private FileSystemWatcher watcher = null;

        public string WatchFolder { get; private set; }
        public bool IsRunning { get; private set; }

        #region Constructor
        public ExtractionService(string watchfolder)
        {
            Paths p = new(watchfolder);
            if (!p.IsValid())
            {
                throw new ArgumentException("Path is not valid", nameof(watchfolder));
            }

            this.WatchFolder = watchfolder;
        }
        #endregion

        public void Start()
        {
            this.CheckWatchFolder();
            this.InitializeWatcher();
            this.watcher.Error += OnError;
            this.watcher.Created += OnCreation;
            
            this.IsRunning = true;
        }

        public void Stop()
        {
            if (this.watcher != null)
            {
                this.watcher.Dispose();
                this.watcher.Error -= this.OnError;
                this.watcher.Created -= this.OnCreation;
            }
            this.work.Clear();
            this.IsRunning = false;
        }

        public void ChangeWatchfolder(string watchfolder)
        {
            this.WatchFolder = watchfolder;
            if (this.IsRunning)
            {
                this.Stop();
            }
            this.Start();
        }

        private async void OnCreation(object sender, FileSystemEventArgs e)
        {
            if (Constants.SUPPORTED_FILE_EXTENSIONS.Contains(Path.GetExtension(e.FullPath).TrimStart('.')) && (!this.work.ContainsKey(e.FullPath) || !this.work[e.FullPath]))
            {
                this.work.TryAdd(e.FullPath, true);

                Extraction ex = new(e.FullPath);
                ex.ExtractionStarted += ExtractionStarted;
                ex.ExtractionCompleted += ExtractionCompleted;

                await Task.Delay(500);

                while (Core.IsFileLocked(e.FullPath))
                {
                    await Task.Delay(50);
                }

                await ex.ExtractAsync();

                this.work[e.FullPath] = false;
            }
            this.CleanupWorkDictionary();
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            Log.Verbose(e.GetException(), "Error:");
            this.watcher.Dispose();
            this.CheckWatchFolder();
            this.InitializeWatcher();
        }

        private void CleanupWorkDictionary()
        {
            foreach (KeyValuePair<string, bool> w in this.work.Where(x => !x.Value))
            {
                this.work.TryRemove(w);
            }
        }

        private void InitializeWatcher()
        {
            this.watcher = new(this.WatchFolder)
            {
                EnableRaisingEvents = true
            };
        }
        private void ExtractionStarted(object sender, ExtractionStartedEventArgs e)
        {
            Log.Information($"Extracting \"{e.Filepath}\"...");
        }

        private void ExtractionCompleted(object sender, ExtractionCompletedEventArgs e)
        {
            Log.Information($"Extraction completed for file \"{e.Filepath}\" extracted {e.Filecount} file{(e.Filecount == 1 ? "s" : "")} - process duration {e.Duration:hh\\:mm\\:ss\\:ffffff}");
        }

        private void CheckWatchFolder()
        {
            if (!Directory.Exists(this.WatchFolder))
            {
                Directory.CreateDirectory(this.WatchFolder);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.Stop();
        }
    }
}
