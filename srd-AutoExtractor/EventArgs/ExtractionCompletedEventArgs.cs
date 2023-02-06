namespace System
{
    public class ExtractionCompletedEventArgs : EventArgs
    {
        public TimeSpan Duration { get; set; }
        public string Filepath { get; set; }
        public uint Filecount { get; set; }

        #region Constructor
        public ExtractionCompletedEventArgs() : base()
        {
        }
        public ExtractionCompletedEventArgs(string filepath, TimeSpan duration, uint filecount) : base()
        {
            this.Filepath = filepath;
            this.Duration = duration;
            this.Filecount = filecount;
        }
        #endregion
    }
}
