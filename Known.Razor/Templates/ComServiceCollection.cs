using Microsoft.Extensions.DependencyInjection;

namespace Known.Razor.Templates;

class ComServiceCollection : ServiceCollection, ICollection<IServiceProvider>, IEnumerable<IServiceProvider>, IList<IServiceProvider>
{
    private readonly IList<IServiceProvider> providers = new List<IServiceProvider>();

    IServiceProvider IList<IServiceProvider>.this[int index]
    {
        get => providers[index];
        set => providers[index] = value;
    }

    public void Add(IServiceProvider item)
    {
        if (!Contains(item))
            providers.Add(item);
    }

    public new void Clear()
    {
        base.Clear();
        providers.Clear();
    }

    public bool Contains(IServiceProvider item)
    {
        return providers.Contains(item);
    }

    public void CopyTo(IServiceProvider[] array, int arrayIndex)
    {
        providers.CopyTo(array, arrayIndex);
    }

    public int IndexOf(IServiceProvider item)
    {
        return providers.IndexOf(item);
    }

    public void Insert(int index, IServiceProvider item)
    {
        providers.Insert(index, item);
    }

    public bool Remove(IServiceProvider item)
    {
        return providers.Remove(item);
    }

    IEnumerator<IServiceProvider> IEnumerable<IServiceProvider>.GetEnumerator()
    {
        return providers.GetEnumerator();
    }
}