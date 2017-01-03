using System;
using System.Diagnostics;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using Autofac;
using FWF.KeyExchange.Logging;
using Module = Autofac.Module;

namespace FWF.KeyExchange.Sample.OwinApi
{
    public static class ServiceStarter
    {
        private static ILogFactory _logFactory;
        private static ILog _log;

        public static void Start<T>(
            params Module[] modules
            ) where T : class, IService
        {
            var args = Environment.GetCommandLineArgs();

            bool runInConsole = Debugger.IsAttached || Array.Exists(args, x => x.Equals("/console", StringComparison.OrdinalIgnoreCase));
            bool runDebugger = Array.Exists(args, x => x.Equals("/debug", StringComparison.OrdinalIgnoreCase));
            bool noLog = Array.Exists(args, x => x.Equals("/nolog", StringComparison.OrdinalIgnoreCase));

            // Add a handler for any unhandled exceptions.
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += AppDomain_UnhandledException;

            if (runDebugger)
            {
                Debugger.Launch();
            }

            //
            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

            T instance = default(T);

            try
            {
                var containerBuilder = new ContainerBuilder();

                if (modules != null)
                {
                    foreach (var kernelModule in modules)
                    {
                        containerBuilder.RegisterModule(kernelModule);
                    }
                }

                if (noLog)
                {
                    containerBuilder.RegisterType<NoOpLog>().As<ILog>().SingleInstance();
                }

                IContainer container = containerBuilder.Build();

                // Start logging
                _logFactory = container.Resolve<ILogFactory>();
                _logFactory.Start();

                try
                {
                    instance = container.Resolve<T>();
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("Unable to instantiate the service component - " + ex.Message, ex);
                }

                // 
                _log = _logFactory.CreateForType(instance);

                if (runInConsole)
                {
                    Console.Title = instance.GetType().Name;
                    instance.Start();
                    Console.ReadLine();
                    instance.Stop();
                }
                else
                {
                    var runnableServiceBase = new RunnableServiceBase(instance);
                    ServiceBase.Run(runnableServiceBase);
                }

                _logFactory.Stop();
                container.Dispose();

            }
            catch (Exception ex)
            {

                // TODO: If a fatal error occurs before logging is setup - need a way to log this...
                if (!ReferenceEquals(_log, null) && !(_log is NoOpLog))
                {
                    _log.Fatal(
                        ex,
                        "Fatal exception occurred"
                        );
                }
                else
                {
                    Debug.Fail(
                        ex.Message,
                        ex.RenderDetailString()
                        );

                    Trace.WriteLine(
                        string.Format(
                            "Fatal exception occurred: {0}",
                            ex.RenderDetailString()
                            ));

                    if (Environment.UserInteractive)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(
                            "Fatal exception occurred: {0}",
                            ex.RenderDetailString()
                            );
                        Console.ResetColor();
                    }
                }

                if (!ReferenceEquals(instance, null))
                {
                    if (instance.IsRunning)
                    {
                        instance.Stop();
                    }
                }
            }

            if (!ReferenceEquals(_logFactory, null))
            {
                if (_logFactory.IsRunning)
                {
                    _logFactory.Stop();
                }
            }

            // NOTE: ProcessExit.Exit() will allow subscribing background threads to continue, but
            // give just a smigen of time to allow for them to complete without the framework trying
            // to close them
            Thread.Sleep(TimeSpan.FromMilliseconds(100));
        }

        private static void AppDomain_UnhandledException(object exception, UnhandledExceptionEventArgs e)
        {
            _log.Fatal(
                (Exception)e.ExceptionObject,
                "Fatal exception occurred"
                );

            if (e.IsTerminating)
            {
                Environment.Exit(1);
            }
        }

    }
}

