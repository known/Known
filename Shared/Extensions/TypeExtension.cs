namespace Known.Extensions;

/// <summary>
/// 类型扩展类。
/// </summary>
public static class TypeExtension
{
    #region Type
    /// <summary>
    /// 判断类型是否包含指定属性。
    /// </summary>
    /// <param name="type">类型。</param>
    /// <param name="propertyName">属性名。</param>
    /// <returns>是否包含指定属性。</returns>
    public static bool HasProperty(this Type type, string propertyName)
    {
        return TypeHelper.Property(type, propertyName) != null;
    }

    /// <summary>
    /// 获取成员关联的Table特性的名称。
    /// </summary>
    /// <param name="info">成员对象。</param>
    /// <returns>名称。</returns>
    public static string TableName(this MemberInfo info)
    {
        return info?.GetCustomAttribute<TableAttribute>()?.Name ?? info.Name;
    }

    /// <summary>
    /// 获取成员关联的DisplayName特性的显示名称。
    /// </summary>
    /// <param name="info">成员对象。</param>
    /// <returns>属性显示名称。</returns>
    public static string DisplayName(this MemberInfo info)
    {
        return info?.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
    }

    /// <summary>
    /// 获取成员关联的Description特性的描述信息。
    /// </summary>
    /// <param name="info">成员对象。</param>
    /// <returns>描述信息。</returns>
    public static string Description(this MemberInfo info)
    {
        return info?.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }

    /// <summary>
    /// 获取成员关联的Category特性的显示名称。
    /// </summary>
    /// <param name="info">成员对象。</param>
    /// <returns>属性显示名称。</returns>
    public static string Category(this MemberInfo info)
    {
        return info?.GetCustomAttribute<CategoryAttribute>()?.Category;
    }
    #endregion

    #region Property
    /// <summary>
    /// 获取属性是否关联Key特性。
    /// </summary>
    /// <param name="info">属性对象。</param>
    /// <returns>是否必填。</returns>
    public static bool IsKey(this PropertyInfo info)
    {
        return info?.GetCustomAttribute<KeyAttribute>() is not null;
    }

    /// <summary>
    /// 获取属性是否关联Required特性。
    /// </summary>
    /// <param name="info">属性对象。</param>
    /// <returns>是否必填。</returns>
    public static bool IsRequired(this PropertyInfo info)
    {
        return info?.GetCustomAttribute<RequiredAttribute>() is not null;
    }

    /// <summary>
    /// 获取属性关联的MinLength特性的长度。
    /// </summary>
    /// <param name="info">属性对象。</param>
    /// <returns>属性最小长度。</returns>
    public static int? MinLength(this PropertyInfo info)
    {
        return info?.GetCustomAttribute<MinLengthAttribute>()?.Length;
    }

    /// <summary>
    /// 获取属性关联的MaxLength特性的长度。
    /// </summary>
    /// <param name="info">属性对象。</param>
    /// <returns>属性最大长度。</returns>
    public static int? MaxLength(this PropertyInfo info)
    {
        return info?.GetCustomAttribute<MaxLengthAttribute>()?.Length;
    }

    internal static string GetFieldLength(this PropertyInfo info)
    {
        var type = info.GetFieldType();
        if (type == FieldType.Switch) return "50";
        if (type == FieldType.Number) return "18,2";

        return info.MaxLength()?.ToString();
    }

    internal static int? GetColumnWidth(this PropertyInfo info)
    {
        var type = info.GetFieldType();
        if (type == FieldType.Switch) return 50;
        if (type == FieldType.Number) return 100;
        if (type == FieldType.Date) return 100;
        if (type == FieldType.DateTime) return 140;

        var length = info.MaxLength();
        if (length == null) return null;
        if (length < 100) return length * 2;
        return length;
    }

    internal static FieldType GetFieldType(this PropertyInfo info)
    {
        var form = info.GetCustomAttribute<FormAttribute>();
        if (form != null && !string.IsNullOrWhiteSpace(form.Type))
            return Utils.ConvertTo<FieldType>(form.Type);

        var type = info.PropertyType;
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

    internal static string GetFieldName(this MemberInfo info)
    {
        return info?.GetCustomAttribute<ColumnAttribute>()?.Field ?? info?.Name;
    }
    #endregion
}