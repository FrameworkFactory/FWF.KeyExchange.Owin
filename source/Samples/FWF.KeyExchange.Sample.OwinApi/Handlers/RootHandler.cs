using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace FWF.KeyExchange.Sample.OwinApi.Handlers
{
    internal class RootHandler
    {

        public Task Handle(IOwinContext context, Func<Task> next)
        {
            var requestPath = context.Request.Path.Value.ToLowerInvariant();
            var requestMethod = context.Request.Method;

            var isRootRequest = (
                requestPath == "/"
                || requestPath == "/index.htm"
                || requestPath == "/index.html"
                || requestPath == "/default.htm"
                || requestPath == "/default.html"
                );

            //
            if (isRootRequest && requestMethod == "GET")
            {
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.Headers.Set("Content-Type", "application/json");
                context.Response.ContentType = "application/json";
                context.Response.Write("{}");

                return Task.CompletedTask;
            }

            return next();
        }


    }
}
