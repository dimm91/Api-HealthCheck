using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheck.Sample.HealthChecks
{
    public class EmailServiceProviderHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            //Your logic to check if your email service provider is working as expected.
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
