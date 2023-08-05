using Coravel;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sample.Core.TaskJobs;

namespace Sample.Core.Extensions;

public static class Extension
{
    public static IServiceCollection AddApp(this IServiceCollection services)
    {
        services.AddScheduler();
        services.AddTransient<ImportTaskJob>();
        return services;
    }

    public static void UseApp(this WebApplication app)
    {
        app.Services.UseScheduler(scheduler =>
        {
            scheduler.Schedule<ImportTaskJob>().EveryFiveSeconds();
        });
    }
}