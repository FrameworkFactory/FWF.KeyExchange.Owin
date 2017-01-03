namespace FWF.KeyExchange
{
    public abstract class Startable : IRunnable
    {
        private volatile bool _isRunning;

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        protected abstract void OnStart();
        protected abstract void OnStop();

        public virtual void Start()
        {
            if (_isRunning)
            {
                return;
            }
            _isRunning = true;

            try
            {
                OnStart();
            }
            catch
            {
                _isRunning = false;
                throw;
            }
        }

        public virtual void Stop()
        {
            if (!_isRunning)
            {
                return;
            }
            _isRunning = false;

            try
            {
                OnStop();
            }
            catch
            {
                throw;
            }
        }
    }
}
