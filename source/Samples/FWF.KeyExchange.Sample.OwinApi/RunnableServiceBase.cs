using System.ServiceProcess;

namespace FWF.KeyExchange.Sample.OwinApi
{
    public class RunnableServiceBase : ServiceBase
    {
        private readonly IRunnable _instance;

        public RunnableServiceBase(IRunnable instance)
        {
            _instance = instance;
        }
        
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            _instance.Start();
        }

        protected override void OnStop()
        {
            _instance.Stop();
            base.OnStop();
        }

    }
}
