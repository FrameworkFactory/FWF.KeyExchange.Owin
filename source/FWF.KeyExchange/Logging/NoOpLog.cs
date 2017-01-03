using System;
using System.Collections.Generic;

namespace FWF.KeyExchange.Logging
{
    public class NoOpLog : ILog
    {

        public string Name
        {
            get { return "NoOp"; }
        }

        public bool IsLevelEnabled(LogLevel level)
        {
            return false;
        }

        public void Log(LogPayload logPayload)
        {
            // Do nothing
        }

        public void Log(LogLevel level, string message)
        {
            // Do nothing
        }

        public void LogFormat(LogLevel level, string format, params object[] args)
        {
            // Do nothing
        }

        public void LogException(LogLevel level, Exception exception, string message)
        {
            // Do nothing
        }

        public void LogExceptionFormat(LogLevel level, Exception exception, string format, params object[] args)
        {
            // Do nothing
        }

        public void LogProperties(LogLevel level, IDictionary<string, string> propertiesDictionary)
        {
            // Do nothing
        }
    }
}
