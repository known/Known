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
        switch (type)
        {
            case QueryType.Equal:
                return "=";
            case QueryType.NotEqual:
                return "<>";
            case QueryType.LessThan:
                return "<";
            case QueryType.LessEqual:
                return "<=";
            case QueryType.GreatThan:
                return ">";
            case QueryType.GreatEqual:
                return ">=";
            case QueryType.Contain:
            case QueryType.StartWith:
            case QueryType.EndWith:
                return " like ";
            case QueryType.NotContain:
            case QueryType.NotStartWith:
            case QueryType.NotEndWith:
                return " not like ";
            default:
                return string.Empty;
        }
    }

    internal static string ToValueFormat(this QueryType type)
    {
        switch (type)
        {
            case QueryType.Contain:
            case QueryType.NotContain:
                return "%{0}%";
            case QueryType.StartWith:
            case QueryType.NotStartWith:
                return "{0}%";
            case QueryType.EndWith:
            case QueryType.NotEndWith:
                return "%{0}";
            default:
                return string.Empty;
        }
    }
}