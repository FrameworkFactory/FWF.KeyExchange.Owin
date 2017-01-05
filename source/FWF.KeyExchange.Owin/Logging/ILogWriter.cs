using System;

namespace FWF.KeyExchange.Owin.Logging
{
    public interface ILogWriter
    {       
        void Write(LogPayload logPayload);
    }
}
