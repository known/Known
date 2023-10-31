using Microsoft.Extensions.DependencyInjection;

namespace KnownAntDesign;

public static class Extension
{
    public static void AddKnownAntDesign(this IServiceCollection services)
    {
        KButton.Render = (b, k) => b.Component<KaButton>().Set(c => c.Button, k).Build();
    }
}
