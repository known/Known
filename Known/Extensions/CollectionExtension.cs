using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Known.Extensions
{
    /// <summary>
    /// 集合扩展类。
    /// </summary>
    public static class CollectionExtension
    {
        /// <summary>
        /// 将NameValue集合转换成字典集合。
        /// </summary>
        /// <param name="collection">NameValue集合。</param>
        /// <returns>字典集合。</returns>
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
        /// 将字典中的值转换为指定类型实例。
        /// </summary>
        /// <typeparam name="T">值的类型。</typeparam>
        /// <param name="dictionary">字典集合。</param>
        /// <param name="key">键。</param>
        /// <param name="defValue">不存在该键时的默认返回值。</param>
        /// <returns>指定类型实例。</returns>
        public static T Value<T>(this IDictionary<string, object> dictionary, string key, T defValue = default(T))
        {
            if (dictionary == null)
                return default(T);

            if (dictionary.ContainsKey(key))
            {
                return (T)dictionary[key];
            }

            return defValue;
        }

        /// <summary>
        /// 将字典集合按字典Key升序排列生成MD5签名信息。
        /// </summary>
        /// <param name="dictionary">字典集合。</param>
        /// <returns>MD5签名信息。</returns>
        public static string ToMd5Signature(this IDictionary<string, object> dictionary)
        {
            if (dictionary == null)
                return string.Empty;

            var sb = new StringBuilder();
            dictionary.OrderBy(e => e.Key).ToList().ForEach(e => sb.Append(e.Key).Append(e.Value));
            var sort = sb.ToString();
            return Encryptor.ToMd5(sort);
        }
    }
}
