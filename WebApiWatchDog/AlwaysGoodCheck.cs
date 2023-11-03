using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApiWatchDog;

public class AlwaysGoodCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}
