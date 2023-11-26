using System.Linq.Expressions;
using Known.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class ComponentBuilder<T> where T : IComponent
{
    private readonly RenderTreeBuilder builder;
    private readonly Dictionary<string, object> Parameters = new(StringComparer.Ordinal);

    internal ComponentBuilder(RenderTreeBuilder builder)
    {
        this.builder = builder;
    }

    public ComponentBuilder<T> Add(string name, object value)
    {
        Parameters[name] = value;
        return this;
    }

    public ComponentBuilder<T> Set<TValue>(Expression<Func<T, TValue>> selector, TValue value)
    {
        var property = TypeHelper.Property(selector);
        return Add(property.Name, value);
    }

    public void Build(Action<T> action = null)
    {
        builder.OpenComponent<T>(0);
        if (Parameters.Count > 0)
            builder.AddMultipleAttributes(1, Parameters);
        if (action != null)
            builder.AddComponentReferenceCapture(2, value => action.Invoke((T)value));
        builder.CloseComponent();
    }
}