namespace Known.Extensions;

/// <summary>
/// 通用扩展类。
/// </summary>
public static class CommonExtension
{
    #region Enum
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
    #endregion

    #region Object
    /// <summary>
    /// 合并两个对象。
    /// </summary>
    /// <param name="obj1">对象1。</param>
    /// <param name="obj2">对象2。</param>
    /// <returns>合并后的对象。</returns>
    public static object Merge(this object obj1, object obj2)
    {
        if (obj1 == null) return null;
        if (obj2 == null) return obj1;

        var obj1Type = obj1.GetType();
        var obj2Type = obj2.GetType();
        var obj1Properties = obj1Type.GetProperties();
        var obj2Properties = obj2Type.GetProperties();

        var keyValues = new Dictionary<string, Type>();
        foreach (var prop in obj1Properties)
            keyValues[prop.Name] = prop.PropertyType;
        foreach (var prop in obj2Properties)
            keyValues[prop.Name] = prop.PropertyType;

        var mergedType = TypeHelper.CreateType(keyValues);
        var mergedObject = Activator.CreateInstance(mergedType);

        foreach (var property in obj1Properties)
        {
            var value = obj1Type.GetProperty(property.Name).GetValue(obj1, null);
            mergedType.GetProperty(property.Name).SetValue(mergedObject, value, null);
        }

        foreach (var property in obj2Properties)
        {
            var value = obj2Type.GetProperty(property.Name).GetValue(obj2, null);
            mergedType.GetProperty(property.Name).SetValue(mergedObject, value, null);
        }

        return mergedObject;
    }

    /// <summary>
    /// 合并两个泛型对象，返回动态对象。
    /// </summary>
    /// <typeparam name="TLeft">对象1类型。</typeparam>
    /// <typeparam name="TRight">对象2类型。</typeparam>
    /// <param name="left">对象1。</param>
    /// <param name="right">对象2。</param>
    /// <returns>合并后的对象。</returns>
    public static ExpandoObject Merge<TLeft, TRight>(this TLeft left, TRight right)
    {
        var expando = new ExpandoObject();
        IDictionary<string, object> dict = expando;
        foreach (var p in typeof(TLeft).GetProperties())
            dict[p.Name] = p.GetValue(left);
        foreach (var p in typeof(TRight).GetProperties())
            dict[p.Name] = p.GetValue(right);
        return expando;
    }
    #endregion
}