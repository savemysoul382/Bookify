// Bookify.Api

using Bookify.Application.Abstractions.Data;
using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;

// не рекомендуется, но если что, вот так можно
// регистрируем так - builder.Services.AddHealthChecks().AddCheck<CustomSqlHealthCheck>("custom-sql");
namespace Bookify.Api;

public class CustomSqlHealthCheck : IHealthCheck
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public CustomSqlHealthCheck(ISqlConnectionFactory connectionFactory)
    {
        this._connectionFactory = connectionFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = this._connectionFactory.CreateConnection();
            await connection.ExecuteScalarAsync("SELECT 1;");
            return HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            return HealthCheckResult.Unhealthy(e.Message);
        }
    }
}