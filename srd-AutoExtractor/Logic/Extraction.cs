using neXn.Lib.Files;
using SevenZipExtractor;
using SharpCompress.Readers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace srdAutoExtractor.Logic
{
    public class Extraction
    {
        public event EventHandler FolderAlreadyExists;
        public event EventHandler<ExtractionStartedEventArgs> ExtractionStarted;
        public event EventHandler<ExtractionCompletedEventArgs> ExtractionCompleted;

        public FileInfo Archivepath { get; private set; }
        public string ExtractionPath { get; private set; }
        public bool Processing { get; private set; }
        public uint ExtractedFilecount { get; private set; }

        #region Constructor
        public Extraction(string archivepath)
        {
            this.Archivepath = new(archivepath);
        }
        public Extraction(string archivepath, string extractionFolder) : this(archivepath)
        {
            this.ExtractionPath = extractionFolder;
        }
        #endregion

        public void Extract()
        {
            if (this.Processing || Core.IsFileLocked(this.Archivepath.FullName))
            {
                return;
            }

            this.Processing = true;
            this.ExtractedFilecount = 0;

            var sWatch = new Stopwatch();
            sWatch.Start();

            string newFolder = DetermineExtractionPath();

            if (Directory.Exists(newFolder))
            {
                this.FolderAlreadyExists?.Invoke(this, EventArgs.Empty);
                return;
            }

            Directory.CreateDirectory(newFolder);

            this.ExtractionStarted?.Invoke(this, new(this.Archivepath.Name));

            if (this.Archivepath.Name.EndsWith("7z"))
            {
                this.Extract7z(this.Archivepath.FullName, newFolder);
            }
            else
            {
                this.ExtractArchive(this.Archivepath.FullName, newFolder);
            }

            sWatch.Stop();

            this.ExtractionCompleted?.Invoke(this, new(this.Archivepath.Name, sWatch.Elapsed));
            this.Processing = false;
        }

        public Task ExtractAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                this.Extract();
            });
        }

        internal string DetermineExtractionPath()
        {
            if (this.ExtractionPath == null)
            {
                return Path.Combine(Path.GetDirectoryName(this.Archivepath.FullName), DetermineExtractionFolder(this.Archivepath.Name));
            }

            return Path.Combine(this.ExtractionPath, DetermineExtractionFolder(this.Archivepath.Name));
        }

        internal static string DetermineExtractionFolder(string filepath)
        {
            if (filepath.EndsWith("tar.gz"))
            {
                return Path.GetFileNameWithoutExtension(filepath).Substring(0, Path.GetFileNameWithoutExtension(filepath).Length - 4);
            }

            return Path.GetFileNameWithoutExtension(filepath);
        }

        private void ExtractArchive(string filepath, string extractionfolder)
        {
            using (FileStream fs = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (IReader r = ReaderFactory.Open(fs))
                {
                    while (r.MoveToNextEntry())
                    {
                        if (!r.Entry.IsDirectory)
                        {
                            this.ExtractedFilecount++;
                            r.WriteEntryToDirectory(extractionfolder, new()
                            {
                                ExtractFullPath = true,
                                Overwrite = true,
                                PreserveFileTime = true
                            });
                        }
                    }
                }
            }
        }

        private void Extract7z(string filepath, string extractionfolder)
        {
            using (FileStream fs = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (ArchiveFile a = new(fs))
                {
                    foreach (SevenZipExtractor.Entry e in a.Entries)
                    {
                        if (e.IsFolder)
                        {
                            Directory.CreateDirectory(Path.Combine(extractionfolder, e.FileName));
                            continue;
                        }
                        this.ExtractedFilecount++;
                        e.Extract(Path.Combine(extractionfolder, e.FileName));
                    }
                }
            }
        }
    }
}