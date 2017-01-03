using System.Collections.Generic;

namespace FWF.KeyExchange.Logging
{
    public delegate void LogWriterHandler(LogPayload logPayload);

    public interface ILogWriter : IRunnable
    {
        bool Enabled { get; }

        LogLevel Level { get; }

        List<ILogFilter> Filters { get; }

        void Configure(IDictionary<string, string> configs);

        void Write(LogPayload logPayload);

    }
}
