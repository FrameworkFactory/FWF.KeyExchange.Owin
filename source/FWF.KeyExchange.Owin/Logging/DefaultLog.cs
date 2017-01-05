using System;
using System.Collections.Generic;

namespace FWF.KeyExchange.Owin.Logging
{
    internal class DefaultLog : ILog 
    {

        private readonly string _name;
        private readonly LogWriterHandler _logWriterHandler;

        public DefaultLog(string name, LogWriterHandler logWriterHandler)
        {
            _name = name;
            _logWriterHandler = logWriterHandler;
        }

        public string Name
        {
            get { return _name; }
        }
        
        public void Log(LogPayload logPayload)
        {
            LogInternal(logPayload);
        }

        public void Log(LogLevel level, string message)
        {
            var logPayload = new LogPayload
            {
                Timestamp = DateTime.UtcNow,
                Name = _name,
                LogLevel = level,
                Message = message,
            };

            LogInternal(logPayload);
        }

        public void LogFormat(LogLevel level, string format, params object[] args)
        {
            var logPayload = new LogPayload
            {
                Timestamp = DateTime.UtcNow,
                Name = _name,
                LogLevel = level,
                Message = string.Format(format, args)
            };

            LogInternal(logPayload);
        }

        public void LogException(LogLevel level, Exception exception, string message)
        {
            var logPayload = new LogPayload
            {
                Timestamp = DateTime.UtcNow,
                Name = _name,
                LogLevel = level,
                Message = message,
                Callstack = exception.RenderDetailString()
            };

            LogInternal(logPayload);
        }

        public void LogExceptionFormat(LogLevel level, Exception exception, string format, params object[] args)
        {
            var logPayload = new LogPayload
            {
                Timestamp = DateTime.UtcNow,
                Name = _name,
                LogLevel = level,
                Message = string.Format(format, args),
                Callstack = exception.RenderDetailString()
            };

            LogInternal(logPayload);
        }

        public void LogProperties(LogLevel level, IDictionary<string, string> propertiesDictionary)
        {
            var logPayload = new LogPayload
            {
                Timestamp = DateTime.UtcNow,
                Name = _name,
                LogLevel = level,
                Properties = propertiesDictionary
            };

            LogInternal(logPayload);
        }


        private void LogInternal(LogPayload logPayload)
        {
            if (logPayload.Timestamp == DateTime.MinValue)
            {
                logPayload.Timestamp = DateTime.UtcNow;
            }

            // Update log entry with information from the currently running process
            logPayload.MachineName = Environment.MachineName;
            
            _logWriterHandler(logPayload);
        }


    }
}
