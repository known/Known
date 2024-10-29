namespace Known.Extensions;

/// <summary>
/// 类型扩展类。
/// </summary>
public static class TypeExtension
{
    #region Type
    /// <summary>
    /// 获取类型关联的Route特性的路由模板。
    /// </summary>
    /// <param name="type">类型对象。</param>
    /// <returns>路由模板。</returns>
    public static string RouteTemplate(this Type type)
    {
        return type?.GetCustomAttribute<RouteAttribute>()?.Template;
    }

    /// <summary>
    /// 获取成员关联的DisplayName特性的显示名称。
    /// </summary>
    /// <param name="member">成员对象。</param>
    /// <returns>属性显示名称。</returns>
    public static string DisplayName(this MemberInfo member)
    {
        return member?.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
    }

    /// <summary>
    /// 判断成员是否允许匿名访问。
    /// </summary>
    /// <param name="member">成员对象。</param>
    /// <returns>是否允许匿名访问。</returns>
    public static bool IsAllowAnonymous(this MemberInfo member)
    {
        return member?.GetCustomAttribute<AllowAnonymousAttribute>() is not null;
    }
    #endregion

    #region Property
    //public static bool IsRequired(this PropertyInfo property)
    //{
    //    return property?.GetCustomAttribute<RequiredAttribute>() is not null;
    //}

    /// <summary>
    /// 获取属性关联的MinLength特性的长度。
    /// </summary>
    /// <param name="property">属性对象。</param>
    /// <returns>属性最小长度。</returns>
    public static int? MinLength(this PropertyInfo property)
    {
        return property?.GetCustomAttribute<MinLengthAttribute>()?.Length;
    }

    /// <summary>
    /// 获取属性关联的MaxLength特性的长度。
    /// </summary>
    /// <param name="property">属性对象。</param>
    /// <returns>属性最大长度。</returns>
    public static int? MaxLength(this PropertyInfo property)
    {
        return property?.GetCustomAttribute<MaxLengthAttribute>()?.Length;
    }

    internal static FieldType GetFieldType(this PropertyInfo property)
    {
        var type = property.PropertyType;

        if (type == typeof(bool))
            return FieldType.Switch;

        if (type == typeof(short) || type == typeof(int) || type == typeof(long) || type == typeof(float) || type == typeof(double) || type == typeof(decimal))
            return FieldType.Number;

        if (type == typeof(DateOnly?) || type == typeof(DateOnly))
            return FieldType.Date;

        if (type == typeof(DateTime?) || type == typeof(DateTime) || type == typeof(DateTimeOffset?) || type == typeof(DateTimeOffset))
            return FieldType.DateTime;

        return FieldType.Text;
    }
    #endregion

    #region Method
    /// <summary>
    /// 判断方法是否是匿名访问方法。
    /// </summary>
    /// <param name="method">方法对象。</param>
    /// <returns>是否匿名访问方法。</returns>
    public static bool AllowAnonymous(this MethodInfo method)
    {
        return method?.GetCustomAttribute<AllowAnonymousAttribute>() is not null;
    }
    #endregion
}