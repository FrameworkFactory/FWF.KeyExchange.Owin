using System;

namespace FWF.KeyExchange.Owin
{
    internal class NoOpCache : ICache
    {
        public byte[] Fetch(string endpointId)
        {
            return null;
        }

        public void Push(string endpointId, byte[] keyData)
        {
            // Do nothing
        }

    }
}
