using System.Reflection.Emit;

namespace Known.Helpers;

/// <summary>
/// 类型帮助者类。
/// </summary>
public sealed class TypeHelper
{
    private TypeHelper() { }

    internal static async Task ActionAsync(object obj, UIContext context, BaseLayout app, ActionInfo info, object[] parameters)
    {
        var type = obj.GetType();
        var paramTypes = parameters?.Select(p => p.GetType()).ToArray();
        var method = paramTypes == null
                   ? type.GetMethod(info.Id)
                   : type.GetMethod(info.Id, paramTypes);
        if (method == null)
        {
            var message = context.Language["Tip.NoMethod"].Replace("{method}", $"{info.Name}[{type.Name}.{info.Id}]");
            context.UI.Error(message);
            return;
        }

        try
        {
            method.Invoke(obj, parameters);
        }
        catch (Exception ex)
        {
            await app?.OnError(ex);
        }
    }

    internal static List<CodeInfo> GetEnumCodes(Type type)
    {
        var category = type.Name;
        var codes = new List<CodeInfo>();
        var values = Enum.GetValues(type);
        foreach (Enum item in values)
        {
            var code = Enum.GetName(type, item);
            var name = item.GetDescription();
            if (string.IsNullOrWhiteSpace(name))
                name = code;
            codes.Add(new CodeInfo(category, code, name, null));
        }
        return codes;
    }

    internal static List<FieldInfo> GetFields(Type entityType, Language language)
    {
        var fields = new List<FieldInfo>();
        var properties = Properties(entityType);
        if (properties == null || properties.Length == 0)
            return fields;

        foreach (var item in properties)
        {
            if (item.Name == nameof(EntityBase.Id) ||
                item.Name == nameof(EntityBase.Version) ||
                item.Name == nameof(EntityBase.Extension) ||
                item.Name == nameof(EntityBase.AppId) ||
                item.Name == nameof(EntityBase.CompNo))
                continue;

            if (item.CanRead && item.CanWrite && !item.GetMethod.IsVirtual)
            {
                var name = item.DisplayName();
                var type = item.GetFieldType();
                fields.Add(new FieldInfo
                {
                    Id = item.Name,
                    Name = language.GetText("", item.Name, name),
                    Type = type
                });
            }
        }
        return fields;
    }

    /// <summary>
    /// 获取数据对象属性值。
    /// </summary>
    /// <param name="model">数据对象。</param>
    /// <param name="name">属性名称。</param>
    /// <returns>属性值。</returns>
    public static object GetPropertyValue(object model, string name)
    {
        if (model == null || string.IsNullOrWhiteSpace(name))
            return default;

        var property = Property(model.GetType(), name);
        if (property == null || !property.CanRead)
            return default;

        return property.GetValue(model);
    }

    /// <summary>
    /// 获取数据对象属性泛型值。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="model">数据对象。</param>
    /// <param name="name">属性名称。</param>
    /// <returns>属性泛型值。</returns>
    public static T GetPropertyValue<T>(object model, string name)
    {
        if (model == null || string.IsNullOrWhiteSpace(name))
            return default;

        var property = Property(model.GetType(), name);
        if (property == null || !property.CanRead)
            return default;

        var value = property.GetValue(model);
        return Utils.ConvertTo<T>(value);
    }

    /// <summary>
    /// 设置数据对象属性值。
    /// </summary>
    /// <param name="model">数据对象。</param>
    /// <param name="name">属性名称。</param>
    /// <param name="value">属性值。</param>
    public static void SetPropertyValue(object model, string name, object value)
    {
        if (model == null || string.IsNullOrWhiteSpace(name))
            return;

        var property = Property(model.GetType(), name);
        if (property != null && property.CanWrite)
        {
            var value1 = Utils.ConvertTo(property.PropertyType, value);
            property.SetValue(model, value1, null);
        }
    }

    /// <summary>
    /// 设置泛型对象属性值。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="model">泛型对象。</param>
    /// <param name="name">属性名称。</param>
    /// <param name="value">属性值。</param>
    public static void SetPropertyValue<T>(T model, string name, object value)
    {
        if (model == null || string.IsNullOrWhiteSpace(name))
            return;

        var property = Property(typeof(T), name);
        if (property != null && property.CanWrite)
        {
            var value1 = Utils.ConvertTo(property.PropertyType, value);
            property.SetValue(model, value1, null);
        }
    }

    private static readonly ConcurrentDictionary<string, PropertyInfo[]> typeProperties = new();
    /// <summary>
    /// 获取内存缓存的类型属性集合。
    /// </summary>
    /// <param name="type">类型。</param>
    /// <returns>属性集合。</returns>
    public static PropertyInfo[] Properties(Type type)
    {
        return typeProperties.GetOrAdd(type.FullName, type.GetProperties());
    }

    private static readonly ConcurrentDictionary<string, PropertyInfo> properties = new();
    /// <summary>
    /// 获取内存缓存的类型属性。
    /// </summary>
    /// <param name="type">类型。</param>
    /// <param name="name">属性名。</param>
    /// <returns>属性信息。</returns>
    public static PropertyInfo Property(Type type, string name)
    {
        var key = $"{type.FullName}.{name}";
        return properties.GetOrAdd(key, type.GetProperty(name));
    }

    /// <summary>
    /// 根据选择表达式获取属性信息。
    /// </summary>
    /// <typeparam name="T">类型。</typeparam>
    /// <typeparam name="TValue">属性值类型。</typeparam>
    /// <param name="selector">选择表达式。</param>
    /// <returns>属性信息。</returns>
    /// <exception cref="ArgumentNullException">选择表达式不能为空。</exception>
    /// <exception cref="ArgumentException">表达式不是类型属性成员。</exception>
    public static PropertyInfo Property<T, TValue>(Expression<Func<T, TValue>> selector)
    {
        if (selector is null)
            throw new ArgumentNullException(nameof(selector));

        if (selector.Body is not MemberExpression expression || expression.Member is not PropertyInfo propInfoCandidate)
            throw new ArgumentException($"The parameter selector '{selector}' does not resolve to a public property on the type '{typeof(T)}'.", nameof(selector));

        var type = typeof(T);
        var propertyInfo = propInfoCandidate.DeclaringType != type
                         ? type.GetProperty(propInfoCandidate.Name, propInfoCandidate.PropertyType)
                         : propInfoCandidate;
        if (propertyInfo is null)
            throw new ArgumentException($"The parameter selector '{selector}' does not resolve to a public property on the type '{typeof(T)}'.", nameof(selector));

        return propertyInfo;
    }

    /// <summary>
    /// 根据选择表达式获取属性信息。
    /// </summary>
    /// <typeparam name="T">类型。</typeparam>
    /// <param name="selector">选择表达式。</param>
    /// <returns>属性信息。</returns>
    /// <exception cref="ArgumentNullException">选择表达式不能为空。</exception>
    /// <exception cref="ArgumentException">表达式不是类型属性成员。</exception>
    public static PropertyInfo Property<T>(Expression<Func<T, object>> selector)
    {
        if (selector is null)
            throw new ArgumentNullException(nameof(selector));

        var expression = GetMemberExpression(selector);
        if (expression == null)
            throw new ArgumentException($"The parameter selector '{selector}' does not resolve to a public property on the type '{typeof(T)}'.", nameof(selector));

        var propInfoCandidate = expression.Member as PropertyInfo;
        if (propInfoCandidate == null)
            throw new ArgumentException($"The parameter selector '{selector}' does not resolve to a public property on the type '{typeof(T)}'.", nameof(selector));

        var type = typeof(T);
        var propertyInfo = propInfoCandidate.DeclaringType != type
                         ? type.GetProperty(propInfoCandidate.Name)
                         : propInfoCandidate;
        if (propertyInfo is null)
            throw new ArgumentException($"The parameter selector '{selector}' does not resolve to a public property on the type '{typeof(T)}'.", nameof(selector));

        return propertyInfo;
    }

    private static MemberExpression GetMemberExpression<T>(Expression<Func<T, object>> selector)
    {
        var member = selector.Body as MemberExpression;
        if (member != null)
            return member;

        var unary = selector.Body as UnaryExpression;
        return unary != null ? unary.Operand as MemberExpression : null;
    }

    /// <summary>
    /// 创建一个动态类型。
    /// </summary>
    /// <param name="keyValues">类型属性字典。</param>
    /// <returns>动态类型。</returns>
    public static Type CreateType(Dictionary<string, Type> keyValues)
    {
        var assemblyName = new AssemblyName("DynamicAssembly");
        var dyAssembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
        var dyModule = dyAssembly.DefineDynamicModule("DynamicModule");
        var dyClass = dyModule.DefineType("MyDyClass", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass);
        foreach (var item in keyValues)
        {
            var fb = dyClass.DefineField("_" + item.Key, item.Value, FieldAttributes.Private);

            var gmb = dyClass.DefineMethod("get_" + item.Key, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, item.Value, Type.EmptyTypes);
            var getIL = gmb.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fb);
            getIL.Emit(OpCodes.Ret);

            var smb = dyClass.DefineMethod("set_" + item.Key, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { item.Value });
            var setIL = smb.GetILGenerator();
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, fb);
            setIL.Emit(OpCodes.Ret);

            var pb = dyClass.DefineProperty(item.Key, PropertyAttributes.HasDefault, item.Value, null);
            pb.SetGetMethod(gmb);
            pb.SetSetMethod(smb);
        }
        return dyClass.CreateTypeInfo();
    }
}