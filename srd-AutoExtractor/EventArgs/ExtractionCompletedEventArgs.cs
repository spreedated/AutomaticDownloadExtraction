namespace System
{
    public class ExtractionCompletedEventArgs : EventArgs
    {
        public TimeSpan Duration { get; set; }
        public string Filepath { get; set; }

        #region Constructor
        public ExtractionCompletedEventArgs() : base()
        {
        }
        public ExtractionCompletedEventArgs(string filepath, TimeSpan duration) : base()
        {
            this.Filepath = filepath;
            this.Duration = duration;
        }
        #endregion
    }
}
