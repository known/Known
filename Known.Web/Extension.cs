using Coravel;
using Coravel.Invocable;
using Known.Helpers;

namespace Known.Web;

static class Extension
{
    public static void AddKnownWeb(this IServiceCollection services)
    {
        services.AddScheduler();
        services.AddTransient<ImportTaskJob>();
    }

    public static void UseKnownWeb(this IServiceProvider provider)
    {
        provider.UseScheduler(scheduler =>
        {
            scheduler.Schedule<ImportTaskJob>().EveryFiveSeconds();
        });
    }
}

class ImportTaskJob : IInvocable
{
    public Task Invoke() => ImportHelper.ExecuteAsync();
}