using System;
using Microsoft.Owin;

namespace FWF.KeyExchange.Owin
{
    public interface IOwinEndpointIdProvider
    {
        string DetermineEndpointId(IOwinContext owinContext);
    }
}
