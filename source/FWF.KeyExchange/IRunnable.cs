namespace FWF.KeyExchange
{
    public interface IRunnable
    {
        bool IsRunning { get; }
        void Start();
        void Stop();
    }
}
