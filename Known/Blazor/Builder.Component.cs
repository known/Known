namespace Known.Blazor;

/// <summary>
/// 组件建造者类。
/// </summary>
/// <typeparam name="T">组件类型。</typeparam>
public class ComponentBuilder<T> where T : Microsoft.AspNetCore.Components.IComponent
{
    private readonly RenderTreeBuilder builder;
    private readonly Dictionary<string, object> Parameters = new(StringComparer.Ordinal);

    internal ComponentBuilder(RenderTreeBuilder builder)
    {
        this.builder = builder;
    }

    /// <summary>
    /// 添加组件参数。
    /// </summary>
    /// <param name="name">参数名。</param>
    /// <param name="value">参数对象。</param>
    /// <returns>组件建造者。</returns>
    public ComponentBuilder<T> Add(string name, object value)
    {
        Parameters[name] = value;
        return this;
    }

    /// <summary>
    /// 设置组件参数。
    /// </summary>
    /// <typeparam name="TValue">组件参数对象类型。</typeparam>
    /// <param name="selector">组件参数属性选择表达式。</param>
    /// <param name="value">组件参数对象。</param>
    /// <returns>组件建造者。</returns>
    public ComponentBuilder<T> Set<TValue>(Expression<Func<T, TValue>> selector, TValue value)
    {
        var property = TypeHelper.Property(selector);
        if (property == null)
            return this;

        return Add(property.Name, value);
    }

    /// <summary>
    /// 建造组件。
    /// </summary>
    /// <param name="action">组件对象实例委托方法。</param>
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