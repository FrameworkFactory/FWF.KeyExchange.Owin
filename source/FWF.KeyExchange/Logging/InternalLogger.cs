using System;
using System.Collections.Generic;

namespace FWF.KeyExchange.Logging
{
    internal class InternalLogger : ILog
    {
        private readonly string _name;
        private readonly LogLevel _logLevel;
        private readonly LogWriterHandler _logWriterHandler;

        public InternalLogger(
            string name,
            LogLevel logLevel,
            LogWriterHandler writer
            )
        {
            _name = name;
            _logLevel = logLevel;
            _logWriterHandler = writer;
        }

        public string Name
        {
            get { return _name; }
        }

        public bool IsLevelEnabled(LogLevel level)
        {
            return _logLevel.Value <= level;
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

            // TODO: Need more data..
            //logPayload.DeviceId;
            //logPayload.SessionId;
            //logPayload.ApplicationVersion = new Version().Get();
            //logPayload.ThreadName?

            _logWriterHandler(logPayload);
        }
        
    }
}
