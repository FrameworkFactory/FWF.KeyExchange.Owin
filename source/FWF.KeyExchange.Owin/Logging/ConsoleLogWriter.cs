using System;
using System.Globalization;
using System.Text;

namespace FWF.KeyExchange.Owin.Logging
{
    internal class ConsoleLogWriter : ILogWriter
    {
        private const string DateTimeConsoleFormat = "HH:mm:ss.ffff";
        private static volatile object _lockObject = new object();

        public void Write(LogPayload logPayload)
        {
            lock (_lockObject)
            {
                ConsoleColor consoleColor = Console.ForegroundColor;

                var stringBuilder = new StringBuilder();
                stringBuilder.Append(DateTime.UtcNow.ToString(DateTimeConsoleFormat, CultureInfo.CurrentCulture));
                stringBuilder.Append(" ");
                stringBuilder.Append(("[" + logPayload.LogLevel.Name + "]").PadRight(8));
                stringBuilder.Append(logPayload.Name);

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Out.WriteLine(stringBuilder.ToString());

                Console.ForegroundColor = FetchColorByLevel(logPayload.LogLevel);

                if (logPayload.LogLevel == LogLevel.Traffic)
                {
                    var from = logPayload.Properties["from"];
                    var to = logPayload.Properties["to"];
                    var payload = logPayload.Properties["payload"];

                    Console.Out.WriteLine("{0} -> {1}", from, to);
                    Console.Out.WriteLine(payload);
                }
                else
                {
                    Console.Out.WriteLine(logPayload.Message);
                }

                if (logPayload.Callstack != null)
                {
                    Console.Out.WriteLine(logPayload.Callstack);
                }

                Console.ForegroundColor = consoleColor;
            }
        }

        private static ConsoleColor FetchColorByLevel(LogLevel logLevel)
        {
            if (logLevel >= LogLevel.Error)
            {
                return ConsoleColor.Red;
            }

            if (logLevel >= LogLevel.Warn)
            {
                return ConsoleColor.DarkYellow;
            }

            return ConsoleColor.White;
        }

    }
}
