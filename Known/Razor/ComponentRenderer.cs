namespace Known.Razor;

public class ComponentRenderer<T> where T : IComponent
{
    private const string ChildContent = nameof(ChildContent);
    private static readonly Type componentType = typeof(T);

    private readonly Dictionary<string, object> parameters = new(StringComparer.Ordinal);
    private readonly ComTemplater templater;

    public ComponentRenderer()
    {
        templater = new ComTemplater();
    }

    public ComponentRenderer<T> AddServiceProvider(IServiceProvider serviceProvider)
    {
        templater.AddServiceProvider(serviceProvider);
        return this;
    }

    public ComponentRenderer<T> Set<TValue>(Expression<Func<T, TValue>> parameterSelector, TValue value)
    {
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        parameters.Add(GetParameterName(parameterSelector), value);
        return this;
    }

    private static string GetParameterName<TValue>(Expression<Func<T, TValue>> parameterSelector)
    {
        if (parameterSelector is null)
            throw new ArgumentNullException(nameof(parameterSelector));

        if (parameterSelector.Body is not MemberExpression memberExpression ||
            memberExpression.Member is not PropertyInfo propInfoCandidate)
            throw new ArgumentException($"The parameter selector '{parameterSelector}' does not resolve to a public property on the component '{typeof(T)}'.", nameof(parameterSelector));

        var propertyInfo = propInfoCandidate.DeclaringType != componentType
            ? componentType.GetProperty(propInfoCandidate.Name, propInfoCandidate.PropertyType)
            : propInfoCandidate;

        var paramAttr = propertyInfo?.GetCustomAttribute<ParameterAttribute>(inherit: true);

        if (propertyInfo is null || paramAttr is null)
            throw new ArgumentException($"The parameter selector '{parameterSelector}' does not resolve to a public property on the component '{typeof(T)}' with a [Parameter] or [CascadingParameter] attribute.", nameof(parameterSelector));

        return propertyInfo.Name;
    }

    public string Render()
    {
        return templater.RenderComponent<T>(parameters);
    }
}