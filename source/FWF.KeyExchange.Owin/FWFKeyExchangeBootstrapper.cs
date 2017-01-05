using System;
using System.Collections.Generic;
using Autofac;
using FWF.KeyExchange.Owin.Bootstrap;

namespace FWF.KeyExchange.Owin
{
    public class FWFKeyExchangeBootstrapper
    {
        // NOTE: Not everyone uses AutoFac.  Create a basic
        // bootstrapper component that allows for the retrival
        // of implementations within FWF.KeyExchange

        private readonly IContainer _container;
        
        public FWFKeyExchangeBootstrapper()
        {
            // NOTE: The IoC container strategy could be (much) different than the
            // implementation that these components will be a part of.  Using this
            // Options component to build and contain the AutoFac IoC container
            // seems to be the best approach to abstract the complexities of IoC, but
            // still allow different implementations to be set at the property level.
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<FWFKeyExchangeOwinModule>();
            _container = containerBuilder.Build();
        }

        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return _container.ResolveAll<T>();
        }

    }
}
