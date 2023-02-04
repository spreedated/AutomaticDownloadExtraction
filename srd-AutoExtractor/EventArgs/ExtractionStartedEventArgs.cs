namespace System
{
    public class ExtractionStartedEventArgs : EventArgs
    {
        public string Filepath { get; set; }

        #region Constructor
        public ExtractionStartedEventArgs() : base()
        {
        }
        public ExtractionStartedEventArgs(string filepath) : base()
        {
            this.Filepath = filepath;
        }
        #endregion
    }
}
