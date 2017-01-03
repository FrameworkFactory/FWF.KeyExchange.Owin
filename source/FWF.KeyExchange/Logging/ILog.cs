using System;
using System.Collections.Generic;

namespace FWF.KeyExchange.Logging
{
    public interface ILog
    {
        string Name { get; }

        bool IsLevelEnabled(LogLevel level);

        void Log(LogPayload logPayload);

        void Log(LogLevel level, string message);

        void LogFormat(LogLevel level, string format, params object[] args);

        void LogException(LogLevel level, Exception exception, string message);
        
        void LogExceptionFormat(LogLevel level, Exception exception, string format, params object[] args);

        void LogProperties(LogLevel level, IDictionary<string, string> propertiesDictionary);
    }
}
