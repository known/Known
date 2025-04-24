namespace Known.Extensions;

/// <summary>
/// 枚举扩展类。
/// </summary>
public static class EnumExtension
{
    /// <summary>
    /// 获取枚举字段描述。
    /// </summary>
    /// <param name="value">枚举字段值。</param>
    /// <returns>枚举字段描述。</returns>
    public static string GetDescription(this Enum value)
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        var field = type.GetField(name);
        var attr = field.GetCustomAttribute<DescriptionAttribute>();
        return attr != null ? attr.Description : name;
    }

    /// <summary>
    /// 判断字段类型是否是布尔类型。
    /// </summary>
    /// <param name="type">字段类型。</param>
    /// <returns></returns>
    public static bool IsBoolean(this FieldType type)
    {
        return type == FieldType.Switch ||
               type == FieldType.CheckBox;
    }

    /// <summary>
    /// 判断字段类型是否是日期类型。
    /// </summary>
    /// <param name="type">字段类型。</param>
    /// <returns></returns>
    public static bool IsDateTime(this FieldType type)
    {
        return type == FieldType.Date ||
               type == FieldType.DateTime;
    }

    /// <summary>
    /// 判断字段类型是否是字典类型。
    /// </summary>
    /// <param name="type">字段类型。</param>
    /// <returns></returns>
    public static bool IsDictionary(this FieldType type)
    {
        return type == FieldType.Select ||
               type == FieldType.CheckList ||
               type == FieldType.RadioList;
    }

    /// <summary>
    /// 将查询类型转换成SQL操作符。
    /// </summary>
    /// <param name="type">查询类型</param>
    /// <returns>SQL操作符。</returns>
    public static string ToOperator(this QueryType type)
    {
        return type switch
        {
            QueryType.Equal => "=",
            QueryType.NotEqual => "<>",
            QueryType.LessThan => "<",
            QueryType.LessEqual => "<=",
            QueryType.GreatThan => ">",
            QueryType.GreatEqual => ">=",
            QueryType.Contain or QueryType.StartWith or QueryType.EndWith => " like ",
            QueryType.NotContain or QueryType.NotStartWith or QueryType.NotEndWith => " not like ",
            _ => string.Empty,
        };
    }

    internal static string ToValueFormat(this QueryType type)
    {
        return type switch
        {
            QueryType.Contain or QueryType.NotContain => "%{0}%",
            QueryType.StartWith or QueryType.NotStartWith => "{0}%",
            QueryType.EndWith or QueryType.NotEndWith => "%{0}",
            _ => string.Empty,
        };
    }
}