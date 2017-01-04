using System;
using FWF.KeyExchange.Bootstrap;
using FWF.KeyExchange.Sample.OwinApi.Bootstrap;

namespace FWF.KeyExchange.Sample.OwinApi
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ServiceStarter.Start<OwinApiService>(
                    new FWFKeyExchangeModule(), 
                    new FWFKeyExchangeSampleOwinApiModule()
                    );
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(
                    "Fatal exception:\r\n" + ex.RenderDetailString()
                    );
                Console.ResetColor();
            }

        }
    }
}
