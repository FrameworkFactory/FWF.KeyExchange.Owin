using System;
using Microsoft.Owin;

namespace FWF.KeyExchange.Owin
{
    public interface IEndpointIdProvider
    {
        string DetermineEndpointId(Uri url);

        string DetermineEndpointId(IOwinContext owinContext);
    }
}
