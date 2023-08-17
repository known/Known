using Coravel;
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

    public static void UseApp(this IServiceProvider provider)
    {
        provider.UseScheduler(scheduler =>
        {
            scheduler.Schedule<ImportTaskJob>().EveryFiveSeconds();
        });
    }
}