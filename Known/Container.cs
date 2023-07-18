namespace Known;

public sealed class Container
{
    private static readonly Dictionary<Type, Type> types = new();
    private static readonly Hashtable cached = new();

    private Container() { }

    internal static Dictionary<string, Type> RegTypes { get; } = new();

    public static void RegisterType<T, TImpl>() where TImpl : T => types[typeof(T)] = typeof(TImpl);

    public static void RegisterType<TBase>(Assembly assembly)
    {
        foreach (var item in assembly.GetTypes())
        {
            if (item.IsSubclassOf(typeof(TBase)) && !item.IsAbstract)
            {
                RegTypes[item.Name] = item;
            }
        }
    }

    public static T Create<T>(params object[] args)
    {
        var key = typeof(T);
        if (!types.ContainsKey(key))
            return default;

        var type = types[key];
        return (T)Activator.CreateInstance(type, args);
    }

    public static T Resolve<T>(T objDefault = default)
    {
        var type = typeof(T);
        if (cached.ContainsKey(type))
            return (T)cached[type];

        var name = type.Name;
        if (cached.ContainsKey(name))
            return (T)cached[name];

        if (type.IsInterface)
        {
            var names = type.FullName.Split('.');
            for (int i = 0; i < names.Length; i++)
            {
                if (i == names.Length - 1)
                {
                    names[i] = names[i].Substring(1);
                }
            }
            var implName = string.Join(".", names);
            var implType = type.Assembly.GetType(implName);
            if (implType != null)
            {
                Register(type, () => Activator.CreateInstance(implType));
                return (T)cached[type];
            }
        }

        if (objDefault != null)
        {
            Register(type, () => objDefault);
        }

        return objDefault;
    }

    public static void Register<T, TImpl>() where TImpl : T => Register(typeof(T), () => Activator.CreateInstance<TImpl>());

    public static void Register<TBase>(Assembly assembly)
    {
        foreach (var item in assembly.GetTypes())
        {
            if (item.IsSubclassOf(typeof(TBase)) && !item.IsAbstract)
            {
                Register(item.Name, () => Activator.CreateInstance(item));
            }
        }
    }

    private static void Register(object key, Func<object> func)
    {
        lock (cached.SyncRoot)
        {
            cached[key] = func();
        }
    }
}