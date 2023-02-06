namespace srdAutoExtractor.Logic
{
    internal interface IService
    {
        public bool IsRunning { get; }
        public void Start();
        public void Stop();
    }
}
