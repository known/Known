using System.Collections.Frozen;

namespace Known.Helpers;

class TypeCache
{
    private static readonly ConcurrentDictionary<Type, Lazy<PropertyCache>> _typeCache = new();
    private static readonly ConcurrentDictionary<Type, int> _inheritanceDepthCache = new();

    private sealed class PropertyCache(PropertyInfo[] properties)
    {
        public PropertyInfo[] Array { get; } = properties;
        public FrozenDictionary<string, PropertyInfo> Dictionary { get; } = properties.ToFrozenDictionary(p => p.Name, StringComparer.Ordinal);
    }

    public static void PreloadTypes(IEnumerable<Type> types)
    {
        Parallel.ForEach(types, type => _typeCache.GetOrAdd(type, CreateLazy));
    }

    public static PropertyInfo[] Properties(Type type) => _typeCache.GetOrAdd(type, CreateLazy).Value.Array;

    public static PropertyInfo Property(Type type, string name) => _typeCache.GetOrAdd(type, CreateLazy).Value.Dictionary.GetValueOrDefault(name);

    private static Lazy<PropertyCache> CreateLazy(Type type)
    {
        return new Lazy<PropertyCache>(() => new PropertyCache(CreateSortedProperties(type)), LazyThreadSafetyMode.ExecutionAndPublication);
    }

    private static PropertyInfo[] CreateSortedProperties(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        if (properties.Length == 0) return properties;

        // 创建属性->深度的映射
        var depthMap = new Dictionary<PropertyInfo, int>(properties.Length);
        Parallel.For(0, properties.Length, i =>
        {
            var property = properties[i];
            depthMap[property] = _inheritanceDepthCache.GetOrAdd(property.DeclaringType!, CalculateInheritanceDepth);
        });
        // 稳定排序
        Array.Sort(properties, (x, y) => depthMap[x].CompareTo(depthMap[y]));
        return properties;
    }

    private static int CalculateInheritanceDepth(Type type)
    {
        int depth = 0;
        for (var t = type; t != null; t = t.BaseType) depth++;
        return depth;
    }
}