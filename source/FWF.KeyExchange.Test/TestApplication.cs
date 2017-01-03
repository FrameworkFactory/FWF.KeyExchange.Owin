using System;
using Autofac;
using FWF.KeyExchange.Bootstrap;
using FWF.KeyExchange.Test.Bootstrap;
using NUnit.Framework;

namespace FWF.KeyExchange.Test
{
    internal static class TestApplicationState
    {
        public static IContainer Container { get; set; }
    }

    [SetUpFixture]
    public class TestApplication
    {
        [OneTimeSetUp]
        public void Start()
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule<FWFKeyExchangeModule>();
            containerBuilder.RegisterModule<FWFKeyExchangeTestModule>();

            TestApplicationState.Container = containerBuilder.Build();
        }

        [OneTimeTearDown]
        public void Stop()
        {
            TestApplicationState.Container.Dispose();
        }

    }
}
