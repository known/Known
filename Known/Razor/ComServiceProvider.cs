using Microsoft.Extensions.DependencyInjection;

namespace Known.Razor;

class ComServiceProvider : IServiceProvider, IDisposable
{
    private readonly IServiceProvider provider;
    private readonly IEnumerable<IServiceScope> scopes;

    public ComServiceProvider(IServiceProvider provider, IEnumerable<IServiceScope> scopes)
    {
        this.provider = provider;
        this.scopes = new List<IServiceScope>(scopes);
        // Force enumeration so to not need to re-execute the enumerator each time later in the foreach of GetService
        // HINT: NOTE: do not use scopes.ToList() but use new List<IServiceScope>(scopes) instead because .ToList will not
        // work as the real implementation of "scope" is an object that implments both IEnumerable<IServiceDescriptor>
        // and IEnumerable<IServiceScope> the .ToList will default to the IEnumerable<IServiceScope>.GetEnumerator and
        // will result in copying nothing. Instead the new List<IServiceScope> will use the correct override of the
        // GetEnumerator to extract the elements
    }

    public object GetService(Type serviceType)
    {
        if (provider.GetService(serviceType) is object foundService)
            return foundService;

        foreach (var scope in scopes)
        {
            if (scope.ServiceProvider.GetService(serviceType) is object serviceFromScope)
                return serviceFromScope;
        }

        return null;
    }

    public void Dispose()
    {
        foreach (var scope in scopes) scope.Dispose();
    }
}