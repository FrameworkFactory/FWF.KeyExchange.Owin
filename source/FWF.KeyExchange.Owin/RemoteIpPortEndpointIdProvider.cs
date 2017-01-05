using System;
using Microsoft.Owin;

namespace FWF.KeyExchange.Owin
{
    internal class RemoteIpPortEndpointIdProvider : IEndpointIdProvider
    {

        public string DetermineEndpointId(Uri remoteHost)
        {
            var endpointId = string.Concat(remoteHost.Host, remoteHost.Port);

            return endpointId;
        }

        public string DetermineEndpointId(IOwinContext owinContext)
        {
            return owinContext.Request.RemoteIpAddress;
        }

    }
}
