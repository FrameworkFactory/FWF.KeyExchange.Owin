using System;
using System.Diagnostics;
using FWF.KeyExchange.Logging;
using Microsoft.Owin.Logging;

namespace FWF.KeyExchange.Sample.OwinApi
{
    internal class OwinLoggerFactory : ILoggerFactory
    {

        private readonly ILogFactory _logFactory;

        private class OwinLogger : ILogger
        {

            private readonly ILog _log;

            public OwinLogger(ILog log)
            {
                _log = log;
            }

            public bool WriteCore(TraceEventType eventType, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
            {
                string logMessage = formatter(state, exception);

                switch (eventType)
                {
                    case TraceEventType.Critical:
                        _log.Fatal(exception, logMessage);
                        return true;

                    case TraceEventType.Error:
                        _log.Error(exception, logMessage);
                        return true;

                    case TraceEventType.Information:
                        _log.Info(logMessage);
                        return true;

                    case TraceEventType.Warning:
                        _log.Warn(logMessage);
                        return true;

                    case TraceEventType.Verbose:
                        _log.Verbose(logMessage);
                        return true;

                    default:
                        return false;
                }
            }
        }

        public OwinLoggerFactory(
            ILogFactory logFactory
            )
        {
            _logFactory = logFactory;
        }

        public ILogger Create(string name)
        {
            ILog log = _logFactory.Create(name);

            return new OwinLogger(log);
        }

    }
}
