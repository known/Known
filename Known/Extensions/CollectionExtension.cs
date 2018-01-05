using System.Collections.Generic;
using System.Collections.Specialized;

namespace Known.Extensions
{
    /// <summary>
    /// 集合扩展类。
    /// </summary>
    public static class CollectionExtension
    {
        /// <summary>
        /// 将NameValue集合转换成键值对集合。
        /// </summary>
        /// <param name="collection">NameValue集合。</param>
        /// <returns>键值对集合。</returns>
        public static IDictionary<string, string> ToDictionary(this NameValueCollection collection)
        {
            if (collection == null)
                return null;

            var dict = new Dictionary<string, string>();
            foreach (string key in collection.Keys)
            {
                dict.Add(key, collection[key]);
            }
            return dict;
        }

        /// <summary>
        /// 将键/值对中的值转换为指定类型实例。
        /// </summary>
        /// <typeparam name="T">值的类型。</typeparam>
        /// <param name="dictionary">键/值对集合。</param>
        /// <param name="key">键。</param>
        /// <param name="defValue">不存在该键时的默认返回值。</param>
        /// <returns></returns>
        public static T Value<T>(this IDictionary<string, object> dictionary, string key, T defValue = default(T))
        {
            if (dictionary.ContainsKey(key))
            {
                return (T)dictionary[key];
            }

            return defValue;
        }
    }
}
