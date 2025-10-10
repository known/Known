using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace Known.Helpers;

/// <summary>
/// 类型帮助者类。
/// </summary>
public sealed class TypeHelper
{
    private TypeHelper() { }

    /// <summary>
    /// 判断对象是否是匿名类型。
    /// </summary>
    /// <param name="obj">对象。</param>
    /// <returns>是否匿名类型。</returns>
    /// <exception cref="ArgumentNullException">对象不能为空。</exception>
    public static bool IsAnonymousType(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        var type = obj.GetType();
        return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
            && type.IsGenericType && type.Name.Contains("AnonymousType")
            && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
            && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
    }

    /// <summary>
    /// 获取枚举类型代码表信息列表。
    /// </summary>
    /// <typeparam name="T">枚举类型。</typeparam>
    /// <returns>代码表信息列表。</returns>
    public static List<CodeInfo> GetEnumCodes<T>() where T : Enum => GetEnumCodes(typeof(T));

    /// <summary>
    /// 获取枚举类型代码表信息列表。
    /// </summary>
    /// <param name="type">枚举类型。</param>
    /// <returns>代码表信息列表。</returns>
    public static List<CodeInfo> GetEnumCodes(Type type)
    {
        var category = type.Name;
        var codes = new List<CodeInfo>();
        var values = Enum.GetValues(type);
        foreach (Enum item in values)
        {
            var fieldName = Enum.GetName(type, item);
            var field = type.GetField(fieldName);
            if (field.GetCustomAttribute<CodeIgnoreAttribute>() != null)
                continue;

            var code = Enum.GetName(type, item);
            var name = item.GetDescription();
            if (string.IsNullOrWhiteSpace(name))
                name = code;
            codes.Add(new CodeInfo(category, code, name, null));
        }
        return codes;
    }

    /// <summary>
    /// 获取实体基类字段信息列表。
    /// </summary>
    /// <returns></returns>
    public static List<FieldInfo> GetBaseFields()
    {
        return
        [
            new() { Id = nameof(EntityBase.Id), Name = "ID", Type = FieldType.Text, Length = "50", Required = true },
            new() { Id = nameof(EntityBase.CreateBy), Name = "创建人", Type = FieldType.Text, Length = "50", Required = true },
            new() { Id = nameof(EntityBase.CreateTime), Name = "创建时间", Type = FieldType.DateTime, Required = true },
            new() { Id = nameof(EntityBase.ModifyBy), Name = "修改人", Type = FieldType.Text, Length = "50" },
            new() { Id = nameof(EntityBase.ModifyTime), Name = "修改时间", Type = FieldType.DateTime },
            new() { Id = nameof(EntityBase.Version), Name = "版本", Type = FieldType.Integer, Required = true },
            new() { Id = nameof(EntityBase.Extension), Name = "扩展数据", Type = FieldType.Text },
            new() { Id = nameof(EntityBase.AppId), Name = "系统ID", Type = FieldType.Text, Length = "50", Required = true },
            new() { Id = nameof(EntityBase.CompNo), Name = "企业编码", Type = FieldType.Text, Length = "50", Required = true }
        ];
    }

    /// <summary>
    /// 获取内存缓存的类型属性集合。
    /// </summary>
    /// <typeparam name="T">类型。</typeparam>
    /// <returns>属性集合。</returns>
    public static PropertyInfo[] Properties<T>() => Properties(typeof(T));

    /// <summary>
    /// 获取内存缓存的类型属性集合。
    /// </summary>
    /// <param name="type">类型。</param>
    /// <returns>属性集合。</returns>
    public static PropertyInfo[] Properties(Type type) => TypeCache.Properties(type);

    /// <summary>
    /// 获取内存缓存的类型属性。
    /// </summary>
    /// <typeparam name="T">类型。</typeparam>
    /// <param name="name">属性名。</param>
    /// <returns>属性信息。</returns>
    public static PropertyInfo Property<T>(string name) => Property(typeof(T), name);

    /// <summary>
    /// 获取内存缓存的类型属性。
    /// </summary>
    /// <param name="type">类型。</param>
    /// <param name="name">属性名。</param>
    /// <returns>属性信息。</returns>
    public static PropertyInfo Property(Type type, string name) => TypeCache.Property(type, name);

    /// <summary>
    /// 根据选择表达式获取属性信息。
    /// </summary>
    /// <typeparam name="T">类型。</typeparam>
    /// <typeparam name="TValue">属性值类型。</typeparam>
    /// <param name="selector">选择表达式。</param>
    /// <returns>属性信息。</returns>
    /// <exception cref="ArgumentNullException">选择表达式不能为空。</exception>
    /// <exception cref="ArgumentException">表达式不是类型属性成员。</exception>
    public static PropertyInfo Property<T, TValue>(Expression<Func<T, TValue>> selector) => ExtractProperty<T>(selector);
    internal static TypeFieldInfo Field<T, TValue>(Expression<Func<T, TValue>> selector) => ExtractField<T>(selector);

    /// <summary>
    /// 根据选择表达式获取属性信息。
    /// </summary>
    /// <typeparam name="T">类型。</typeparam>
    /// <param name="selector">选择表达式。</param>
    /// <returns>属性信息。</returns>
    /// <exception cref="ArgumentNullException">选择表达式不能为空。</exception>
    /// <exception cref="ArgumentException">表达式不是类型属性成员。</exception>
    public static PropertyInfo Property<T>(Expression<Func<T, object>> selector) => ExtractProperty<T>(selector);
    internal static TypeFieldInfo Field<T>(Expression<Func<T, object>> selector) => ExtractField<T>(selector);

    /// <summary>
    /// 获取数据对象属性泛型值。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="model">数据对象。</param>
    /// <param name="name">属性名称。</param>
    /// <returns>属性泛型值。</returns>
    public static T GetPropertyValue<T>(object model, string name)
    {
        var value = GetPropertyValue(model, name);
        return Utils.ConvertTo<T>(value);
    }

    /// <summary>
    /// 获取数据对象属性值。
    /// </summary>
    /// <param name="model">数据对象。</param>
    /// <param name="name">属性名称。</param>
    /// <returns>属性值。</returns>
    public static object GetPropertyValue(object model, string name)
    {
        var info = TypeCache.Model(model.GetType());
        return info?.GetValue(model, name);
    }

    /// <summary>
    /// 设置数据对象属性值。
    /// </summary>
    /// <param name="model">数据对象。</param>
    /// <param name="name">属性名称。</param>
    /// <param name="value">属性值。</param>
    public static void SetPropertyValue(object model, string name, object value)
    {
        var info = TypeCache.Model(model.GetType());
        info?.SetValue(model, name, value);
    }

    private static PropertyInfo ExtractProperty<T>(LambdaExpression selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        var memberExpr = selector.Body switch
        {
            MemberExpression m => m,
            UnaryExpression { NodeType: ExpressionType.Convert, Operand: MemberExpression m } => m,
            _ => throw new ArgumentException($"Invalid property expression: {selector}")
        };

        if (memberExpr.Member is not PropertyInfo property)
            throw new ArgumentException($"Expression does not resolve to a property: {selector}");

        if (property.DeclaringType == typeof(T))
            return property;

        return Property(property.DeclaringType, property.Name);
    }

    private static TypeFieldInfo ExtractField<T>(LambdaExpression selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        var memberExpr = selector.Body switch
        {
            MemberExpression m => m,
            UnaryExpression { NodeType: ExpressionType.Convert, Operand: MemberExpression m } => m,
            _ => throw new ArgumentException($"Invalid property expression: {selector}")
        };

        if (memberExpr.Member is not PropertyInfo property)
            throw new ArgumentException($"Expression does not resolve to a property: {selector}");

        return TypeCache.Field(property.DeclaringType, property.Name);
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

    /// <summary>
    /// 判断一个类型是否继承泛型。
    /// </summary>
    /// <param name="derivedType">继承类型。</param>
    /// <param name="genericBaseType">泛型基类。</param>
    /// <param name="genericArguments">泛型参数。</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static bool IsGenericSubclass(Type derivedType, Type genericBaseType, out Type[] genericArguments)
    {
        // 确保 genericBaseType 是泛型类型
        if (!genericBaseType.IsGenericType)
            throw new ArgumentException("genericBaseType 必须是一个泛型类型");

        // 获取当前基类
        Type baseType = derivedType.BaseType;

        // 检查基类是否是泛型类型
        if (baseType != null && baseType.IsGenericType)
        {
            var baseTypeDefinition = baseType.GetGenericTypeDefinition();

            // 排除直接匹配的泛型类型
            if (baseTypeDefinition == genericBaseType)
            {
                // 找到匹配的泛型基类，获取泛型参数
                genericArguments = baseType.GetGenericArguments();
                return true;
            }

            // 检查是否是指定的泛型类型
            if (genericBaseType.IsAssignableFrom(baseTypeDefinition))
            {
                genericArguments = baseType.GetGenericArguments();
                return true;
            }
        }

        genericArguments = null;
        return false;
    }
}