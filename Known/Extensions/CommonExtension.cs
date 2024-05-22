namespace Known.Extensions;

public static class CommonExtension
{
    #region String
    internal static void AppendLine(this StringBuilder sb, string format, params object[] args)
    {
        var value = string.Format(format, args);
        sb.AppendLine(value);
    }

    internal static string Format(this string format, params object[] args) => string.Format(format, args);
    #endregion

    #region Enum
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
    #endregion

    #region Dictionary
    public static T GetValue<T>(this IDictionary dic, string key)
    {
        if (dic == null)
            return default;

        if (string.IsNullOrWhiteSpace(key))
            return default;

        if (!dic.Contains(key))
            return default;

        var value = dic[key];
        return Utils.ConvertTo<T>(value);
    }
    #endregion

    #region Object
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

    #region Property
    public static bool IsRequired(this PropertyInfo property)
    {
        return property?.GetCustomAttribute<RequiredAttribute>() is not null;
    }

    public static string DisplayName(this PropertyInfo property)
    {
        return property?.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
    }

    public static int? MinLength(this PropertyInfo property)
    {
        return property?.GetCustomAttribute<MinLengthAttribute>()?.Length;
    }

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

    #region Http
    public static bool CheckMobile(this HttpRequest request)
    {
        if (request == null)
            throw new Exception("Server WebSocket not enabled!");

        var agent = request.Headers["User-Agent"].ToString();
        if (string.IsNullOrWhiteSpace(agent))
            agent = request.Headers["X-Forwarded-For"].ToString();
        return Utils.CheckMobile(agent);
    }

    public static string GetHostUrl(this HttpContext context)
    {
        if (context == null || context.Request == null)
            return string.Empty;

        return context.Request.Scheme + "://" + context.Request.Host;
    }

    public static string GetPageUrl(this NavigationManager navigation)
    {
        var baseUrl = navigation.BaseUri.TrimEnd('/');
        return navigation.Uri.Replace(baseUrl, "");
    }
    #endregion
}