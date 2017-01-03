using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace FWF.KeyExchange.Logging
{
    public class DefaultLogFactory : Startable, ILogFactory
    {
        public const string RepositoryName = "FWF";

        private static readonly IDictionary<string, WeakReference> _loggerCache = new Dictionary<string, WeakReference>();
        private static volatile object _lockObject = new object();

        private readonly List<ILogWriter> _writers = new List<ILogWriter>();

        public DefaultLogFactory(IComponentContext componentContext)
        {
            var logWriters = componentContext.ResolveAll<ILogWriter>();

            if (!ReferenceEquals(logWriters, null))
            {
                if (logWriters.Any())
                {
                    _writers.AddRange(logWriters);
                }
            }
        }

        protected override void OnStart()
        {
            foreach (var logWriter in _writers)
            {
                logWriter.Start();
            }
        }

        protected override void OnStop()
        {
            foreach (var logWriter in _writers)
            {
                logWriter.Stop();
            }
        }

        public ILog CreateForType(Type type)
        {
            string logName = CreateLogNameFromType(type);

            return GetInternal(logName);
        }

        public ILog CreateForType<T>()
        {
            string logName = CreateLogNameFromType(typeof(T));

            return GetInternal(logName);
        }

        public ILog CreateForType(object instance)
        {
            if (ReferenceEquals(instance, null))
            {
                throw new ArgumentNullException("instance");
            }

            string logName = CreateLogNameFromType(instance.GetType());

            return GetInternal(logName);
        }

        public ILog Create(string logFullName)
        {
            return GetInternal(logFullName);
        }

        private ILog GetInternal(string logName)
        {
            ILog log = null;
            WeakReference weakReference;

            if (_loggerCache.TryGetValue(logName, out weakReference))
            {
                log = weakReference.Target as ILog;
            }

            if (log == null)
            {
                lock (_lockObject)
                {
                    if (_loggerCache.TryGetValue(logName, out weakReference))
                    {
                        log = weakReference.Target as ILog;
                    }

                    if (log == null)
                    {
                        // TODO: Turn on switches for logging based upon filter setup
                        LogLevel minLogLevel = GetMinLogLevel();

                        log = new InternalLogger(logName, minLogLevel, Write);

                        _loggerCache[logName] = new WeakReference(log);
                    }
                }
            }

            return log;
        }

        private string CreateLogNameFromType(Type type)
        {
            // If the type is not generic, then the type name is fine
            if (!type.IsGenericType)
            {
                return type.FullName;
            }

            // NOTE: When dealing with a type with generic parameters, create the log string
            // by appending each generic parameter.
            // Type Name = BusinessEvent<T, U>
            // Log Name  = BusinessEvent.T.U

            // NOTE: Do not use FullName, since it will already contain 
            // the generic parameters with assembly qualified names
            string logName = type.Namespace + "." + type.Name;

            Type[] childTypes = type.GetGenericArguments();

            for (int i = 0; i < childTypes.Length; i++)
            {
                // Replace the ugly "`1" in the type name
                logName = logName.Replace("`" + (i + 1), null);

                logName += "." + childTypes[i].Name;
            }

            return logName;
        }

        private LogLevel GetMinLogLevel()
        {
            LogLevel minLogLevel = LogLevel.Off;

            foreach (var logWriter in _writers)
            {
                if (logWriter.Level < minLogLevel)
                {
                    minLogLevel = logWriter.Level;
                }
            }

            return minLogLevel;
        }

        protected virtual void Write(LogPayload logPayload)
        {
            foreach (var logWriter in _writers)
            {
                if (!logWriter.Enabled)
                {
                    continue;
                }

                if (logPayload.LogLevel < logWriter.Level.Value)
                {
                    continue;
                }

                if (
                    logWriter.Filters.Count == 0 ||
                    logWriter.Filters.Any(x => x.ShouldLog(logPayload.Name, logPayload.LogLevel))
                    )
                {
                    logWriter.Write(logPayload);
                }
            }
        }

    }
}
