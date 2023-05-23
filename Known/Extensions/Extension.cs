namespace Known.Extensions;

public static class Extension
{
    //Enum
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

    //Date
    public static DateTime ToDate(this DateTime dateTime, string format = "yyyy-MM-dd")
    {
        var date = dateTime.ToString(format);
        return DateTime.Parse(date);
    }

    //Dictionary
    public static T GetValue<T>(this IDictionary dic, string key)
    {
        if (!dic.Contains(key))
            return default;

        var value = dic[key];
        return Utils.ConvertTo<T>(value);
    }
}