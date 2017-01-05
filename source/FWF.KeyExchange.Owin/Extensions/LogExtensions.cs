using System;
using System.Collections.Generic;
using FWF.KeyExchange.Owin.Logging;

namespace FWF.KeyExchange.Owin
{
    public static class LogExtensions
    {

        public static void Info(this ILog logger, string message)
        {
            logger.Log(LogLevel.Info, message);
        }

        public static void InfoFormat(this ILog logger, string format, params object[] args)
        {
            logger.LogFormat(LogLevel.Info, format, args);
        }

        public static void InfoException(this ILog logger, Exception exception, string message)
        {
            logger.LogException(LogLevel.Info, exception, message);
        }

        public static void InfoExceptionFormat(this ILog logger, Exception exception, string format, params object[] args)
        {
            logger.LogExceptionFormat(LogLevel.Info, exception, format, args);
        }

        public static void Debug(this ILog logger, string message)
        {
            logger.Log(LogLevel.Debug, message);
        }

        public static void DebugFormat(this ILog logger, string format, params object[] args)
        {
            logger.LogFormat(LogLevel.Debug, format, args);
        }

        public static void DebugException(this ILog logger, Exception exception, string message)
        {
            logger.LogException(LogLevel.Debug, exception, message);
        }

        public static void DebugExceptionFormat(this ILog logger, Exception exception, string format, params object[] args)
        {
            logger.LogExceptionFormat(LogLevel.Debug, exception, format, args);
        }

        public static void Warn(this ILog logger, string message)
        {
            logger.Log(LogLevel.Warn, message);
        }

        public static void WarnFormat(this ILog logger, string format, params object[] args)
        {
            logger.LogFormat(LogLevel.Warn, format, args);
        }

        public static void WarnException(this ILog logger, Exception exception, string message)
        {
            logger.LogException(LogLevel.Warn, exception, message);
        }

        public static void WarnExceptionFormat(this ILog logger, Exception exception, string format, params object[] args)
        {
            logger.LogExceptionFormat(LogLevel.Warn, exception, format, args);
        }

        public static void Trace(this ILog logger, string message)
        {
            logger.Log(LogLevel.Trace, message);
        }

        public static void TraceFormat(this ILog logger, string format, params object[] args)
        {
            logger.LogFormat(LogLevel.Trace, format, args);
        }

        public static void TraceException(this ILog logger, Exception exception, string message)
        {
            logger.LogException(LogLevel.Trace, exception, message);
        }

        public static void TraceExceptionFormat(this ILog logger, Exception exception, string format, params object[] args)
        {
            logger.LogExceptionFormat(LogLevel.Trace, exception, format, args);
        }

        public static void Verbose(this ILog logger, string message)
        {
            logger.Log(LogLevel.Verbose, message);
        }

        public static void VerboseFormat(this ILog logger, string format, params object[] args)
        {
            logger.LogFormat(LogLevel.Verbose, format, args);
        }

        public static void VerboseException(this ILog logger, Exception exception, string message)
        {
            logger.LogException(LogLevel.Verbose, exception, message);
        }

        public static void VerboseExceptionFormat(this ILog logger, Exception exception, string format, params object[] args)
        {
            logger.LogExceptionFormat(LogLevel.Verbose, exception, format, args);
        }

        public static void Error(this ILog logger, Exception exception, string message)
        {
            logger.LogException(LogLevel.Error, exception, message);
        }

        public static void ErrorFormat(this ILog logger, Exception exception, string format, params object[] args)
        {
            logger.LogExceptionFormat(LogLevel.Error, exception, format, args);
        }

        public static void Fatal(this ILog logger, Exception exception, string message)
        {
            logger.LogException(LogLevel.Fatal, exception, message);
        }

        public static void FatalFormat(this ILog logger, Exception exception, string format, params object[] args)
        {
            logger.LogExceptionFormat(LogLevel.Fatal, exception, format, args);
        }

        public static void Traffic(this ILog logger, string from, string to, string payload)
        {
            var properties = new Dictionary<string, string>
            {
                {"from", @from}, 
                {"to", to}, 
                {"payload", payload}
            };

            logger.LogProperties(LogLevel.Traffic, properties);
        }

    }
}
