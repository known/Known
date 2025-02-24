using Coravel;
using Coravel.Invocable;

namespace Sample.Web;

static class AppJobs
{
    internal static void AddTaskJobs(this IServiceCollection services)
    {
        services.AddScheduler();
        services.AddTransient<ImportTaskJob>();
    }

    internal static void UseTaskJobs(this WebApplication app)
    {
        app.Services.UseScheduler(scheduler =>
        {
            scheduler.Schedule<ImportTaskJob>().EveryFiveSeconds();
        });
    }
}

class ImportTaskJob : IInvocable
{
    public Task Invoke() => ImportHelper.ExecuteAsync();
}