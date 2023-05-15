using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Template.Core.Extensions;

public static class Extension
{
    public static IServiceCollection AddApp(this IServiceCollection services)
    {
        return services;
    }

    public static void UseApp(this WebApplication app)
    {
    }
}