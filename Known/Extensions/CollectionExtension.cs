using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Known.Extensions
{
    public static class CollectionExtension
    {
        #region ICollection
        public static void ForEach<T>(this ICollection collection, Action<T> action)
        {
            if (collection == null || collection.Count == 0)
                return;

            foreach (var item in collection)
            {
                action((T)item);
            }
        }

        public static void ForEach<T>(this ICollection collection, Action<T, int> action)
        {
            if (collection == null || collection.Count == 0)
                return;

            var i = 0;
            foreach (var item in collection)
            {
                action((T)item, i++);
            }
        }
        #endregion

        #region IEnumerable
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null || collection.Count() == 0)
                return;

            foreach (var item in collection)
            {
                action(item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T, int> action)
        {
            if (collection == null || collection.Count() == 0)
                return;

            var i = 0;
            foreach (var item in collection)
            {
                action(item, i++);
            }
        }

        public static List<TKey> GetDuplicateValues<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> keySelector)
        {
            return collection.GroupBy(keySelector)
                             .Select(g => new { g.Key, Count = g.Count() })
                             .Where(r => r.Count > 1)
                             .Select(r => r.Key)
                             .ToList();
        }

        public static List<T> ToPageList<T>(this IEnumerable<T> collection, int pageIndex, int pageSize)
        {
            if (collection == null || collection.Count() == 0)
                return null;

            return collection.Skip(pageIndex * pageSize)
                             .Take(pageSize)
                             .ToList();
        }
        #endregion

        #region NameValueCollection
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
        #endregion

        #region IDictionary
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
            dictionary.OrderBy(e => e.Key)
                      .ForEach(e => sb.Append(e.Key).Append(e.Value));
            var sort = sb.ToString();
            return Encryptor.ToMd5(sort);
        }
        #endregion
    }
}
