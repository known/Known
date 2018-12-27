using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Known.Extensions
{
    public static class CollectionExtension
    {
        public static List<T> ToPageList<T>(this IEnumerable<T> datas, int pageIndex, int pageSize)
        {
            if (datas == null || datas.Count() == 0)
                return null;

            return datas.Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public static Dictionary<string, object> ToDictionary(this NameValueCollection collection)
        {
            if (collection == null)
                return null;

            var dict = new Dictionary<string, object>();
            foreach (string key in collection.Keys)
            {
                dict.Add(key, collection[key]);
            }
            return dict;
        }

        public static T Value<T>(this IDictionary<string, object> dictionary, string key, T defValue = default(T))
        {
            if (dictionary == null)
                return default(T);

            if (dictionary.ContainsKey(key))
                return (T)dictionary[key];

            return defValue;
        }

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
