using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
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
