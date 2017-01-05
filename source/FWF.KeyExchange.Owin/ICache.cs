using System;

namespace FWF.KeyExchange.Owin
{
    public interface ICache
    {

        byte[] Fetch(string endpointId);

        void Push(string endpointId, byte[] keyData);
    }
}
