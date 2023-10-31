namespace Known.Renders;

public class RenderFactory
{
    private static ConcurrentDictionary<Type, Type> renders = [];

    public static void AddRender(Assembly assembly)
    {
        renders.Clear();

        var types = assembly.GetTypes();
        foreach (var item in types)
        {
            var baseType = item.BaseType;
            if (baseType != null && baseType.Name.Contains("BaseRender"))
            {
                var type = baseType.GenericTypeArguments[0];
                renders[type] = item;
            }
        }
    }

    internal static BaseRender<T> Create<T>() where T : BaseComponent, new()
    {
        var type = typeof(T);
        if (!renders.TryGetValue(type, out var renderType))
            return default;

        return renderType.Assembly.CreateInstance(renderType.FullName) as BaseRender<T>;
    }
}