
# OWIN Middleware for Diffie-Hellman Key Exchange

[Article](http://www.frameworkfactory.com/owin-middleware-for-the-diffie-hellman-key-exchange/)

## Objectives

- Provide seamless OWIN middleware component to support run-time key exchanges.
- Provide symmetric encryption capabilities for data between two endpoints.
- Remove the need for custom key management or configuration.

## Review

The Diffie-Hellman key exchange algorithm, commonly used in SSH, secures the transport of data between two endpoints by generating a run-time key 
without sending the key over the wire.  This simple method provides a secure way to encrypt information beyond the known TLS/SSL methods and its 
vulnerabilities.  This library provides a simple way to extend any OWIN web application and provide a secure channel 
of communication without configuration overhead or manual key sharing.

## Installation

1. Install via NuGet 
    * Add the  [FWF.KeyExchange.Owin](https://www.nuget.org/packages/FWF.KeyExchange.Owin/) nuget package to your existing solution.

2.  Add the UseKeyExchange method to your IAppBuilder setup

```cs
// Use KeyExchange middleware to handle the key exchange
var bootstrapper = new FWFKeyExchangeBootstrapper();
appBuilder.UseKeyExchange(bootstrapper);
```

3.  Use the KeyExchangeHttpClient component to make HTTP(s) calls using the shared encryption key 

```cs
// Send the message to remote endpoint once the key exchange
// has taken place and the message is encrypted over the wire
using (var http = new KeyExchangeHttpClient(bootstrapper))
{
    http.SendPayload(rootUrl, plainTextMessage);
}
```


