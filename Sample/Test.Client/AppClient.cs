namespace Test.Client;

class AppClient
{
    internal static void Initialize()
    {
        KRConfig.IsWeb = true;
        AppConfig.Initialize();
        AppRazor.Initialize();
    }
}