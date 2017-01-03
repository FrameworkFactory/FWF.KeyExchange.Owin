using System;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using FWF.KeyExchange.Logging;
using FWF.KeyExchange.Sample.OwinApi.Handlers;
using Microsoft.Owin;
using Microsoft.Owin.Host.HttpListener;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Logging;
using Owin;

namespace FWF.KeyExchange.Sample.OwinApi
{
    internal class OwinApiService : Startable, IService
    {

        private readonly IKeyExchangeProvider _keyExchangeProvider;

        private readonly KeyExchangeHandler _keyExchangeHandler;
        private readonly MessageSendHandler _messageSendHandler;

        private IDisposable _webHost;
        private OwinHttpListener _webListener;
        private readonly ILoggerFactory _owinLogFactory;
        
        private readonly ILog _log;

        public OwinApiService(
            IKeyExchangeProvider keyExchangeProvider,
            KeyExchangeHandler keyExchangeHandler,
            MessageSendHandler messageSendHandler,
            ILogFactory logFactory
            )
        {
            _keyExchangeProvider = keyExchangeProvider;
            _keyExchangeHandler = keyExchangeHandler;
            _messageSendHandler = messageSendHandler;

            _log = logFactory.CreateForType(this);

            _owinLogFactory = new OwinLoggerFactory(logFactory);
        }


        protected override void OnStart()
        {
            //
            _keyExchangeProvider.Start();

            // 
            var hostPort = int.Parse(ConfigurationManager.AppSettings["HostPort"]);

            var startOptions = new StartOptions
            {
                Urls = {string.Format("https://+:{0}/", hostPort)},
                ServerFactory = "Microsoft.Owin.Host.HttpListener"
            };

            try
            {
                _webHost = WebApp.Start(startOptions, WebAppConfig);
                foreach (var url in startOptions.Urls)
                {
                    _log.Info(string.Format("Listening on uri: {0}", url));
                }
            }
            catch (Exception ex)
            {
                _log.ErrorFormat(ex, "Unable to start web listener - {0}", ex.Message);
                throw;
            }
        }

        protected override void OnStop()
        {
            if (!ReferenceEquals(_webHost, null))
            {
                _webHost.Dispose();
            }
        }

        private void WebAppConfig(IAppBuilder appBuilder)
        {
            _webListener = appBuilder.Properties["Microsoft.Owin.Host.HttpListener.OwinHttpListener"] as OwinHttpListener;

            if (ReferenceEquals(_webListener, null))
            {
                throw new Exception("Invalid service configuration - missing OWIN http listener");
            }

            _webListener.Listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

            appBuilder.SetLoggerFactory(_owinLogFactory);

            appBuilder.Use(HandleRoot);
            appBuilder.Use(_keyExchangeHandler.Handle);
            appBuilder.Use(_messageSendHandler.Handle);
        }

        private Task HandleRoot(IOwinContext context, Func<Task> next)
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
