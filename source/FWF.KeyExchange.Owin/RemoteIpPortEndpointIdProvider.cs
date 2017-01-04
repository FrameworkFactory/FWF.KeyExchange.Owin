using System;
using Microsoft.Owin;

namespace FWF.KeyExchange.Owin
{
    internal class RemoteIpPortEndpointIdProvider : IOwinEndpointIdProvider
    {

        public string DetermineEndpointId(IOwinContext owinContext)
        {
            var ipAddress = owinContext.Request.RemoteIpAddress;
            var port = owinContext.Request.RemotePort.GetValueOrDefault(0);

            return string.Concat(ipAddress, "-", port);
        }

    }
}
