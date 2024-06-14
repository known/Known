namespace Sample.Client;

public static class AppClient
{
    public static void AddSampleClient(this IServiceCollection services)
    {
        services.AddKnownAntDesign(option =>
        {
            //option.Footer = b => b.Component<Foot>().Build();
        });

        Config.AddModule(typeof(AppClient).Assembly);
    }
}