using Microsoft.Extensions.DependencyInjection;

namespace Known.Razor.Templates;

class ComServiceProviderFactory : IServiceProviderFactory<ComServiceBuilder>
{
    public ComServiceBuilder CreateBuilder(IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        if (services is IEnumerable<IServiceProvider> providers)
        {
            return new(provider, providers);
        }
        return new(provider);
    }

    public IServiceProvider CreateServiceProvider(ComServiceBuilder builder)
    {
        return builder.Build();
    }
}