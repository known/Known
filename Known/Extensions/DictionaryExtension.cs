using System.Collections.Generic;

namespace Known.Extensions
{
    public static class DictionaryExtension
    {
        public static T GetValue<T>(this Dictionary<string, T> dictionary, string key)
        {
            return dictionary.GetValue(key, default(T));
        }

        public static T GetValue<T>(this Dictionary<string, T> dictionary, string key, T defaultValue)
        {
            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }

            return defaultValue;
        }
    }
}
