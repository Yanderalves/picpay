using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Picpay.Routes;

public static class RoutesHealth
{
    public static void HealthRoutes(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var result = new
                {
                    status = report.Status.ToString(),
                    duration = report.TotalDuration,
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        description = e.Value.Description,
                        duration = e.Value.Duration
                    })
                };

                await context.Response.WriteAsJsonAsync(result);
            }
        });
    }
}
