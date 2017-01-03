using System;

namespace FWF.KeyExchange.Logging
{
    public class NoOpLogFactory : Startable, ILogFactory
    {

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        public ILog CreateForType(Type type)
        {
            return new NoOpLog();
        }

        public ILog CreateForType<T>()
        {
            return new NoOpLog();
        }

        public ILog CreateForType(object instance)
        {
            return new NoOpLog();
        }

        public ILog Create(string logFullName)
        {
            return new NoOpLog();
        }

    }
}
