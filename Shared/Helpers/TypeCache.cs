namespace Known.Helpers;

class TypeCache
{
    private static readonly ConcurrentDictionary<Type, TypeModelInfo> _typeCache = new();
    //private static readonly ConcurrentDictionary<Type, Lazy<TypeModelInfo>> _typeCache = new();
    //private static readonly ConcurrentDictionary<Type, int> _inheritanceDepthCache = new();

    public static void PreloadTypes(IEnumerable<Type> types) => Parallel.ForEach(types, type => _typeCache.GetOrAdd(type, CreateLazy));
    public static TypeModelInfo Model(Type type) => _typeCache.GetOrAdd(type, CreateLazy);//.Value;
    public static List<TypeFieldInfo> Fields(Type type) => Model(type).Fields;
    public static TypeFieldInfo Field(Type type, string name) => Model(type).Dictionary.GetValueOrDefault(name);
    public static PropertyInfo[] Properties(Type type) => Model(type).Properties;
    public static PropertyInfo Property(Type type, string name) => Model(type).GetProperty(name);

    private static TypeModelInfo CreateLazy(Type type) => new(CreateSortedProperties(type));
    //private static Lazy<TypeModelInfo> CreateLazy(Type type) => new(() => new TypeModelInfo(CreateSortedProperties(type)), LazyThreadSafetyMode.ExecutionAndPublication);

    private static PropertyInfo[] CreateSortedProperties(Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        if (properties.Length == 0) return properties;

        //// 创建属性->深度的映射
        //var depthMap = new Dictionary<PropertyInfo, int>(properties.Length);
        //foreach (var property in properties)
        //{
        //    depthMap[property] = _inheritanceDepthCache.GetOrAdd(property.DeclaringType!, CalculateInheritanceDepth);
        //}
        //// 稳定排序
        //Array.Sort(properties, (x, y) => depthMap[x].CompareTo(depthMap[y]));
        return properties;
    }

    //private static int CalculateInheritanceDepth(Type type)
    //{
    //    int depth = 0;
    //    for (var t = type; t != null; t = t.BaseType) depth++;
    //    return depth;
    //}
}

class PropertyAccessor
{
    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object>>> _getterCache = new();
    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, Action<object, object>>> _setterCache = new();

    public static object GetPropertyValue(object model, string name)
    {
        if (model == null || string.IsNullOrWhiteSpace(name))
            return default;

        var type = model.GetType();
        var typeGetters = _getterCache.GetOrAdd(type, t => new ConcurrentDictionary<string, Func<object, object>>());

        if (!typeGetters.TryGetValue(name, out var getter))
        {
            var property = TypeCache.Property(type, name);
            if (property == null || !property.CanRead)
                return default;

            getter = CompileGetter(property);
            typeGetters[name] = getter;
        }

        return getter(model);
    }

    public static void SetPropertyValue(object model, string name, object value)
    {
        if (model == null || string.IsNullOrWhiteSpace(name))
            return;

        var type = model.GetType();
        var typeSetters = _setterCache.GetOrAdd(type, t => new ConcurrentDictionary<string, Action<object, object>>());

        if (!typeSetters.TryGetValue(name, out var setter))
        {
            var property = TypeCache.Property(type, name);
            if (property == null || !property.CanWrite)
                return;

            setter = CompileSetter(property);
            typeSetters[name] = setter;
        }

        setter(model, value);
    }

    // 编译Getter委托（高性能）
    private static Func<object, object> CompileGetter(PropertyInfo property)
    {
        var instance = Expression.Parameter(typeof(object), "instance");
        var instanceCast = Expression.Convert(instance, property.DeclaringType);
        var propertyAccess = Expression.Property(instanceCast, property);
        var castPropertyValue = Expression.Convert(propertyAccess, typeof(object));
        return Expression.Lambda<Func<object, object>>(castPropertyValue, instance).Compile();
    }

    // 编译Setter委托（高性能 + 内置类型转换）
    private static Action<object, object> CompileSetter(PropertyInfo property)
    {
        var instance = Expression.Parameter(typeof(object), "instance");
        var value = Expression.Parameter(typeof(object), "value");

        // 转换实例到声明类型
        var instanceCast = Expression.Convert(instance, property.DeclaringType);
        // 转换值到目标类型
        var valueCast = Expression.Convert(
            Expression.Call(
                typeof(PropertyAccessor).GetMethod(nameof(ConvertValue), BindingFlags.Static | BindingFlags.NonPublic),
                Expression.Constant(property.PropertyType),
                value
            ),
            property.PropertyType
        );
        // 属性赋值表达式
        var setterCall = Expression.Call(instanceCast, property.GetSetMethod(), valueCast);
        return Expression.Lambda<Action<object, object>>(setterCall, instance, value).Compile();
    }

    // 类型转换方法（复用原有逻辑）
    private static object ConvertValue(Type targetType, object value) => Utils.ConvertTo(targetType, value);
}