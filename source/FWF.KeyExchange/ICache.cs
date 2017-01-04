using System;

namespace FWF.KeyExchange
{
    public interface ICache
    {

        byte[] Fetch(string endpointId);

        void Push(string endpointId, byte[] keyData);
    }
}
