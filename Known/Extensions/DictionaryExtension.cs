namespace Known.Extensions;

public static class DictionaryExtension
{
    public static T GetValue<T>(this IDictionary dic, string key)
    {
        if (!dic.Contains(key))
            return default;

        var value = dic[key];
        return Utils.ConvertTo<T>(value);
    }
}