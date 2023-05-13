using Microsoft.Extensions.DependencyInjection;

namespace Known.Razor.Templates;

class ComServiceBuilder
{
    private readonly IServiceProvider provider;
    private readonly IEnumerable<IServiceProvider> providers;

    public ComServiceBuilder(IServiceProvider provider, IEnumerable<IServiceProvider> providers = null)
    {
        this.provider = provider;
        this.providers = providers;
    }

    public IServiceProvider Build()
    {
        if (providers is IEnumerable<IServiceProvider> comProviders)
        {
            var scopes = comProviders.Select(sp => sp.CreateScope());
            return new ComServiceProvider(provider, scopes);
        }
        return provider;
    }
}