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
    #endregion

    #region Property
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

    /// <summary>
    /// 获取表格栏位宽度。
    /// </summary>
    /// <param name="type">栏位数据类型。</param>
    /// <returns></returns>
    public static int? GetColumnWidth(this Type type)
    {
        var fieldType = type.GetFieldType();
        return fieldType.GetColumnWidth();
    }

    /// <summary>
    /// 获取栏位字段类型。
    /// </summary>
    /// <param name="type">栏位数据类型。</param>
    /// <returns></returns>
    public static FieldType GetFieldType(this Type type)
    {
        if (type == typeof(bool))
            return FieldType.Switch;

        if (type == typeof(short) || type == typeof(int) || type == typeof(long) ||
            type == typeof(short?) || type == typeof(int?) || type == typeof(long?))
            return FieldType.Integer;

        if (type == typeof(float) || type == typeof(double) || type == typeof(decimal) ||
            type == typeof(float?) || type == typeof(double?) || type == typeof(decimal?))
            return FieldType.Number;

        if (type == typeof(DateOnly) || type == typeof(DateOnly?))
            return FieldType.Date;

        if (type == typeof(DateTime) || type == typeof(DateTime?) || 
            type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            return FieldType.DateTime;

        return FieldType.Text;
    }

    internal static string GetFieldName(this MemberInfo info)
    {
        var attr = info?.GetCustomAttribute<ColumnAttribute>();
        return attr?.Field ?? info?.Name;
    }
    #endregion
}