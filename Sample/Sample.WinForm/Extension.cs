namespace Sample.WinForm;

static class Extension
{
    internal static void AddAlone(this IServiceCollection services)
    {
        services.AddApp();
        services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(AppAlone.Host) });
    }

    internal static void UseAlone(this IServiceProvider provider)
    {
        provider.UseApp();
    }
}