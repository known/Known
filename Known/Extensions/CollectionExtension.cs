using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Known.Extensions
{
    /// <summary>
    /// 集合对象的扩展类。
    /// </summary>
    public static class CollectionExtension
    {
        #region ICollection
        /// <summary>
        /// 遍历集合操作。
        /// </summary>
        /// <typeparam name="T">集合元素类型。</typeparam>
        /// <param name="collection">集合对象。</param>
        /// <param name="action">元素的操作。</param>
        public static void ForEach<T>(this ICollection collection, Action<T> action)
        {
            if (collection == null || collection.Count == 0)
                return;

            foreach (var item in collection)
            {
                action((T)item);
            }
        }

        /// <summary>
        /// 遍历集合操作，返回索引参数。
        /// </summary>
        /// <typeparam name="T">集合元素类型。</typeparam>
        /// <param name="collection">集合对象。</param>
        /// <param name="action">元素的操作。</param>
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
        /// <summary>
        /// 遍历枚举数操作。
        /// </summary>
        /// <typeparam name="T">枚举数元素类型。</typeparam>
        /// <param name="collection">枚举数对象。</param>
        /// <param name="action">元素的操作。</param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null || collection.Count() == 0)
                return;

            foreach (var item in collection)
            {
                action(item);
            }
        }

        /// <summary>
        /// 遍历枚举数操作，返回索引参数。
        /// </summary>
        /// <typeparam name="T">枚举数元素类型。</typeparam>
        /// <param name="collection">枚举数对象。</param>
        /// <param name="action">元素的操作。</param>
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

        /// <summary>
        /// 获取枚举数中重复字段集合。
        /// </summary>
        /// <typeparam name="T">枚举数元素类型。</typeparam>
        /// <typeparam name="TKey">返回结果类型。</typeparam>
        /// <param name="collection">枚举数对象。</param>
        /// <param name="keySelector">重复字段选择器。</param>
        /// <returns>重复字段集合。</returns>
        public static List<TKey> GetDuplicateValues<T, TKey>(this IEnumerable<T> collection, Func<T, TKey> keySelector)
        {
            return collection.GroupBy(keySelector)
                             .Select(g => new { g.Key, Count = g.Count() })
                             .Where(r => r.Count > 1)
                             .Select(r => r.Key)
                             .ToList();
        }

        /// <summary>
        /// 获取枚举数的分页集合。
        /// </summary>
        /// <typeparam name="T">枚举数元素类型。</typeparam>
        /// <param name="collection">枚举数对象。</param>
        /// <param name="pageIndex">页码。</param>
        /// <param name="pageSize">每页大小。</param>
        /// <returns>分页元素集合。</returns>
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
        /// <summary>
        /// 将键值集合转出字典对象。
        /// </summary>
        /// <param name="collection">键值集合对象。</param>
        /// <returns>字典对象。</returns>
        public static Dictionary<string, object> ToDictionary(this NameValueCollection collection)
        {
            var dict = new Dictionary<string, object>();
            if (collection == null)
                return dict;

            foreach (string key in collection.Keys)
            {
                dict.Add(key, collection[key]);
            }
            return dict;
        }
        #endregion

        #region IDictionary
        /// <summary>
        /// 获取字典中指定类型的数据值。
        /// </summary>
        /// <typeparam name="T">数据值类型。</typeparam>
        /// <param name="dictionary">字典对象。</param>
        /// <param name="key">数据键。</param>
        /// <param name="defValue">为空时的默认值。</param>
        /// <returns>数据值。</returns>
        public static T Value<T>(this IDictionary<string, object> dictionary, string key, T defValue = default(T))
        {
            if (dictionary == null)
                return default;

            if (dictionary.ContainsKey(key))
                return (T)dictionary[key];

            return defValue;
        }

        /// <summary>
        /// 将字典中所有数据按键排序进行 MD5 加签。
        /// </summary>
        /// <param name="dictionary">字典对象。</param>
        /// <returns>MD5 加签字符串。</returns>
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
