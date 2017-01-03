using System.Collections.Generic;

namespace FWF.KeyExchange.Logging
{
    public interface ILogFilter
    {
        void Configure(IDictionary<string, string> configs);

        bool ShouldLog(string logName, LogLevel level);
    }
}
