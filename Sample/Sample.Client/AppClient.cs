namespace Sample.Client;

class AppClient
{
    internal static void Initialize()
    {
        AppConfig.Initialize();
        AppRazor.Initialize();
    }
}