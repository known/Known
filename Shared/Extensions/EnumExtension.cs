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
        if (attr != null)
            return attr.Description;

        var display = field.GetCustomAttribute<DisplayAttribute>();
        if (display != null)
            return display.Name;

        var displayName = field.GetCustomAttribute<DisplayNameAttribute>();
        if (displayName != null)
            return displayName.DisplayName;
        return name;
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
    /// 根据字段类型获取表格栏位宽度。
    /// </summary>
    /// <param name="type">字段类型。</param>
    /// <returns></returns>
    public static int? GetColumnWidth(this FieldType type)
    {
        if (type == FieldType.Switch) return 80;
        if (type == FieldType.Number) return 100;
        if (type == FieldType.Date) return 100;
        if (type == FieldType.DateTime) return 140;
        return null;
    }

    /// <summary>
    /// 根据字段类型获取查询类型列表。
    /// </summary>
    /// <param name="type">字段类型。</param>
    /// <param name="language">语言实例。</param>
    /// <returns></returns>
    public static List<CodeInfo> GetQueryTypes(this FieldType type, Language language)
    {
        var types = new List<CodeInfo>();
        switch (type)
        {
            case FieldType.Switch:
            case FieldType.CheckBox:
                AddQueryType(language, types, QueryType.Equal);
                AddQueryType(language, types, QueryType.NotEqual);
                break;
            case FieldType.Number:
                AddQueryType(language, types, QueryType.Equal);
                AddQueryType(language, types, QueryType.NotEqual);
                AddQueryType(language, types, QueryType.LessThan);
                AddQueryType(language, types, QueryType.LessEqual);
                AddQueryType(language, types, QueryType.GreatThan);
                AddQueryType(language, types, QueryType.GreatEqual);
                break;
            case FieldType.Date:
            case FieldType.DateTime:
                AddQueryType(language, types, QueryType.Between);
                AddQueryType(language, types, QueryType.BetweenNotEqual);
                AddQueryType(language, types, QueryType.BetweenLessEqual);
                AddQueryType(language, types, QueryType.BetweenGreatEqual);
                break;
            default:
                AddQueryType(language, types, QueryType.Equal);
                AddQueryType(language, types, QueryType.NotEqual);
                AddQueryType(language, types, QueryType.Contain);
                AddQueryType(language, types, QueryType.NotContain);
                AddQueryType(language, types, QueryType.StartWith);
                AddQueryType(language, types, QueryType.NotStartWith);
                AddQueryType(language, types, QueryType.EndWith);
                AddQueryType(language, types, QueryType.NotEndWith);
                AddQueryType(language, types, QueryType.Batch);
                AddQueryType(language, types, QueryType.In);
                AddQueryType(language, types, QueryType.NotIn);
                break;
        }
        return types;
    }

    private static void AddQueryType(Language language, List<CodeInfo> types, QueryType type)
    {
        var queryTypes = Cache.GetCodes<QueryType>();
        var queryType = queryTypes.FirstOrDefault(t => t.Code == $"{type}");
        queryType.Name = language[type.GetDescription()];
        types.Add(queryType);
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