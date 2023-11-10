using System.Linq.Expressions;
using Known.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public class ComponentBuilder<T> where T : IComponent
{
    private readonly RenderTreeBuilder builder;
    internal readonly Dictionary<string, object> Parameters = new(StringComparer.Ordinal);

    public ComponentBuilder(RenderTreeBuilder builder)
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
        Parameters[property.Name] = value;
        return this;
    }

    public void Build()
    {
        builder.OpenComponent<T>(0);
        if (Parameters.Count > 0)
            builder.AddMultipleAttributes(1, Parameters);
        builder.CloseComponent();
    }

    public void Build(Action<T> action)
    {
        builder.OpenComponent<T>(0);
        if (Parameters.Count > 0)
            builder.AddMultipleAttributes(1, Parameters);
        builder.AddComponentReferenceCapture(2, value => action?.Invoke((T)value));
        builder.CloseComponent();
    }

    public ComponentBuilder<T> Id(string id)
    {
        Add(nameof(BaseComponent.Id), id);
        return this;
    }

    public ComponentBuilder<T> Name(string name)
    {
        Add(nameof(BaseComponent.Name), name);
        return this;
    }

    public ComponentBuilder<T> ReadOnly(bool readOnly)
    {
        Add(nameof(BaseComponent.ReadOnly), readOnly);
        return this;
    }

    public ComponentBuilder<T> Enabled(bool enabled)
    {
        Add(nameof(BaseComponent.Enabled), enabled);
        return this;
    }
    
    public ComponentBuilder<T> Visible(bool visible)
    {
        Add(nameof(BaseComponent.Visible), visible);
        return this;
    }
}