namespace Known.Helpers;

class TypeCache
{
    private static readonly ConcurrentDictionary<Type, TypeModelInfo> _typeCache = new();
    //private static readonly ConcurrentDictionary<Type, Lazy<TypeModelInfo>> _typeCache = new();
    //private static readonly ConcurrentDictionary<Type, int> _inheritanceDepthCache = new();

    public static object ConvertTo(Type type, object value) => Utils.ConvertTo(type, value);
    public static void PreloadTypes(IEnumerable<Type> types) => Parallel.ForEach(types, type => _typeCache.GetOrAdd(type, CreateLazy));
    public static TypeModelInfo Model<T>() => Model(typeof(T));
    public static TypeModelInfo Model(Type type) => _typeCache.GetOrAdd(type, CreateLazy);//.Value;
    public static FrozenDictionary<string, TypeFieldInfo> Dictionary(Type type) => Model(type).Dictionary;
    public static List<TypeFieldInfo> Fields<T>() => Fields(typeof(T));
    public static List<TypeFieldInfo> Fields(Type type) => Model(type).Fields;
    public static TypeFieldInfo Field(Type type, string name) => Model(type).Dictionary.GetValueOrDefault(name);
    public static PropertyInfo[] Properties(Type type) => Model(type).Properties;
    public static PropertyInfo Property(Type type, string name) => Model(type).GetProperty(name);

    private static TypeModelInfo CreateLazy(Type type) => new(CreateSortedProperties(type));
    //private static Lazy<TypeModelInfo> CreateLazy(Type type) => new(() => new TypeModelInfo(CreateSortedProperties(type)), LazyThreadSafetyMode.ExecutionAndPublication);

    private static PropertyInfo[] CreateSortedProperties(Type type)
    {
        var properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetIndexParameters().Length == 0)
            .ToArray();
        if (properties.Length == 0) return properties;

        // 去重：同名属性只保留最后一个（最派生），避免子类 new 隐藏基类属性时重复
        var distinct = new Dictionary<string, PropertyInfo>(properties.Length, StringComparer.OrdinalIgnoreCase);
        foreach (var prop in properties)
        {
            distinct[prop.Name] = prop;
        }
        properties = [.. distinct.Values];
        return properties;
    }

    //private static int CalculateInheritanceDepth(Type type)
    //{
    //    int depth = 0;
    //    for (var t = type; t != null; t = t.BaseType) depth++;
    //    return depth;
    //}
}
