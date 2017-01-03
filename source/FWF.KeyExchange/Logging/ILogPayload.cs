using System;
using System.Collections.Generic;

namespace FWF.KeyExchange.Logging
{
    public interface ILogPayload
    {
        DateTime Timestamp { get; set; }

        string Name { get; set; }

        LogLevel LogLevel { get; set; }

        string EnvironmentName { get; set; }

        Guid? ApplicationId { get; set; }

        string ApplicationVersion { get; set; }

        int? ApplicationInstance { get; set; }

        string MachineName { get; set; }

        Guid? TenantId { get; set; }

        Guid? IdentityId { get; set; }

        Guid? SessionId { get; set; }

        Guid? DeviceId { get; set; }

        Guid? CorrelationId { get; set; }

        string Source { get; set; }

        string Message { get; set; }

        string Callstack { get; set; }

        IDictionary<string, string> Properties { get; set; }
    }
}