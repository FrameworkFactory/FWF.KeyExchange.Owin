using System;

namespace FWF.KeyExchange.Owin.Logging
{
    public interface ILogFactory
    {
        ILog CreateForType<T>();
        ILog CreateForType(Type type);
        ILog CreateForType(object instance);
        ILog Create(string logFullName);
    }
}
