using System;
using System.IO;

namespace nxn_AutoExtractor.Models
{
    public class CompressedFile
    {
        public FileInfo FileInfo { get; set; }
        public int EntriesFile { get; set; }
        public int EntriesDirectory { get; set; }
        public bool WasSuccessful { get; set; }
        public double OperationTime { get; set; }
        public bool Processing { get; set; }
        public DateTime Date { get; set; }
    }
}