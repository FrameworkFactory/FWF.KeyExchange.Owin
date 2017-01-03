using System.Collections.Generic;

namespace FWF.KeyExchange.Logging
{
    public abstract class BaseLogWriter : Startable, ILogWriter
    {
        private List<ILogFilter> _filters = new List<ILogFilter>();

        public bool Enabled { get; set; }
        public LogLevel Level { get; set; }

        public List<ILogFilter> Filters
        {
            get { return _filters; }
            set { _filters = value; }
        }

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        public abstract void Configure(IDictionary<string, string> configs);

        public abstract void Write(LogPayload logPayload);

    }
}
