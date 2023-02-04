using srd_AutoExtractor.Models;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SevenZipExtractor;
using neXn.Lib.Files;

namespace srd_AutoExtractor.Classes
{
    public class Extraction
    {
        internal CompressedFile compressedFile;
        internal string operatingPath;

        public event EventHandler FolderAlreadyExists;
        public event EventHandler<ExtractionStartedEventArgs> ExtractionStarted;
        public event EventHandler<ExtractionCompletedEventArgs> ExtractionCompleted;
        public event EventHandler ExtractionFailed;

        #region Constructor
        public Extraction(CompressedFile compressedFile, string operatingPath = null)
        {
            this.compressedFile = compressedFile;
            if (operatingPath != null)
            {
                this.operatingPath = operatingPath;
            }
            else
            {
                this.operatingPath = this.compressedFile.FileInfo.DirectoryName;
            }
        }
        #endregion

        public void Extract()
        {
            if (this.compressedFile.Processing)
            {
                return;
            }

            this.compressedFile.Processing = true;

            // Check if file is locked
            if (Core.IsFileLocked(this.compressedFile.FileInfo.FullName))
            {
                ExitExtraction();
                return;
            }
            // # ### #

            var sWatch = new Stopwatch();
            sWatch.Start();
            string newFolder = Path.Combine(this.operatingPath, Path.GetFileNameWithoutExtension(this.compressedFile.FileInfo.Name));

            if (Directory.Exists(newFolder))
            {
                this.FolderAlreadyExists?.Invoke(this, EventArgs.Empty);
                ExitExtraction();
                return;
            }

            Directory.CreateDirectory(newFolder);

            this.ExtractionStarted?.Invoke(this, new(this.compressedFile.FileInfo.Name));

            try
            {
                using (var archiveFile = new ArchiveFile(this.compressedFile.FileInfo.FullName))
                {
                    foreach (var entry in archiveFile.Entries)
                    {
                        if (entry.IsFolder)
                        {
                            Directory.CreateDirectory(Path.Combine(newFolder, entry.FileName));
                            this.compressedFile.EntriesDirectory++;
                        }
                        else
                        {
                            entry.Extract(Path.Combine(newFolder, entry.FileName));
                            this.compressedFile.EntriesFile++;
                        }
                    }
                }

                // tar.gz Exception (Addtional Extraction of TAR)
                if (this.compressedFile.FileInfo.Name.EndsWith(".tar.gz"))
                {
                    string tarName = Directory.GetFiles(newFolder).FirstOrDefault();
                    string newerFolder = Path.Combine(this.operatingPath, this.compressedFile.FileInfo.Name.Replace(".tar.gz", ""));
                    if (!Directory.Exists(newerFolder))
                    {
                        Directory.CreateDirectory(newerFolder);
                    }
                    
                    using (var archiveFile = new ArchiveFile(tarName))
                    {
                        foreach (var entry in archiveFile.Entries)
                        {
                            if (entry.IsFolder)
                            {
                                Directory.CreateDirectory(Path.Combine(newerFolder, entry.FileName));
                                this.compressedFile.EntriesDirectory++;
                            }
                            else
                            {
                                entry.Extract(Path.Combine(newerFolder, entry.FileName));
                                this.compressedFile.EntriesFile++;
                            }
                        }
                    }

                    if (Directory.Exists(newerFolder))
                    {
                        Directory.Delete(newerFolder + ".tar", true);
                    }
                }
                // # ### #


                sWatch.Stop();
                Log.Information($"Extraction successful for \"" + this.compressedFile.FileInfo.Name + "\" in " + sWatch.Elapsed.TotalSeconds.ToString("0.##") + " Seconds");
                this.compressedFile.OperationTime = sWatch.Elapsed.TotalSeconds;

                this.ExtractionCompleted?.Invoke(this, new(this.compressedFile.FileInfo.Name, sWatch.Elapsed));
            }
            catch (Exception)
            {
                this.ExtractionFailed?.Invoke(this, EventArgs.Empty);
                ExitExtraction();
                return;
            }

            ExitExtraction(true);
        }

        private void ExitExtraction(bool success = false)
        {
            this.compressedFile.Processing = false;
            this.compressedFile.WasSuccessful = success;
        }
    }
}