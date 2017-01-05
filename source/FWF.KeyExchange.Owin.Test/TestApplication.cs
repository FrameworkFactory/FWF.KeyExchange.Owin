using System;
using Autofac;
using FWF.KeyExchange.Owin.Bootstrap;
using FWF.KeyExchange.Test.Bootstrap;
using NUnit.Framework;

namespace FWF.KeyExchange.Owin.Test
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
