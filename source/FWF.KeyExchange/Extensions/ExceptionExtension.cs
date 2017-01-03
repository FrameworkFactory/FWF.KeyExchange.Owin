using System;
using System.Text;

namespace FWF.KeyExchange
{
    public static class ExceptionExtensions
    {
        public static string RenderDetailString(this Exception ex)
        {
            if (ex == null)
            {
                return string.Empty;
            }

            // Format Stack Trace from all wrapped exceptions
            var messageBuilder = new StringBuilder();
            messageBuilder.Append(ex.Message);

            var stackTraceBuilder = new StringBuilder(1024);
            stackTraceBuilder.Append(ex.StackTrace);

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;

                // Append exception message
                messageBuilder.Append(Environment.NewLine + "-->" + ex.Message);

                // Append Stack Trace
                stackTraceBuilder.Append(Environment.NewLine + "------- INNER STACK TRACE -------" + Environment.NewLine);
                stackTraceBuilder.Append(ex.StackTrace);
            }

            if (stackTraceBuilder.Length > 0)
            {
                return messageBuilder + Environment.NewLine + stackTraceBuilder;
            }

            return messageBuilder.ToString();
        }
    }
}
