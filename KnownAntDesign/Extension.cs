using Microsoft.Extensions.DependencyInjection;

namespace KnownAntDesign;

public static class Extension
{
    public static void AddKnownAntDesign(this IServiceCollection services)
    {
        var assembly = typeof(Extension).Assembly;
        RenderFactory.AddRender(assembly);
    }
}