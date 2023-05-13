namespace Test.WinForm;

static class Extension
{
    internal static void AddAlone(this IServiceCollection services)
    {
        services.AddApp();
        services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(AppAlone.Host) });
    }
}