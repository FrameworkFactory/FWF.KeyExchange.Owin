using System;
using Autofac;
using FWF.KeyExchange.Logging;

namespace FWF.KeyExchange.Bootstrap
{
    public class FWFKeyExchangeModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Setup logging
            builder.RegisterType<DefaultLogFactory>().AsSelf().As<ILogFactory>().SingleInstance();
            builder.RegisterType<ConsoleOutLogWriter>().AsSelf().As<ILogWriter>().SingleInstance();

            // Register a crypto random implementation
            builder.RegisterType<RngRandom>().AsSelf().As<IRandom>().SingleInstance();

            // Register a basic symmetric encryption implementation
            builder.RegisterType<SymmetricEncryptionProvider>()
                .AsSelf()
                .As<ISymmetricEncryptionProvider>()
                .SingleInstance();

            // Register a no-op cache implementation
            builder.RegisterType<NoOpCache>().AsSelf().As<ICache>().SingleInstance();
            
            // 
            builder.RegisterType<KeyExchangeProvider>()
                .AsSelf()
                .As<IKeyExchangeProvider>()
                .SingleInstance();


        }
    }
}
