using System;
using Microsoft.Owin;

namespace FWF.KeyExchange.Owin
{
    public interface IOwinEndpointIdProvider : IEndpointIdProvider
    {
        string DetermineEndpointId(IOwinContext owinContext);
    }
}
