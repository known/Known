/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
#if NET35
using System.Collections;
#else
using System.Collections.Concurrent;
#endif

namespace Known
{
    public sealed class Cache
    {
        private const string KEY_CODES = "Known_Codes_{0}";
#if NET35
        private static readonly Hashtable cached = new Hashtable();
#else
        private static readonly ConcurrentDictionary<string, object> cached = new ConcurrentDictionary<string, object>();
#endif
        private Cache() { }

        public static List<T> GetList<T>(string keyPrefix)
        {
            var list = new List<T>();
#if NET35
            foreach (string key in cached.Keys)
            {
                if (key.StartsWith(keyPrefix))
                    list.Add((T)cached[key]);
            }
#else
            foreach (var item in cached)
            {
                if (item.Key.StartsWith(keyPrefix))
                    list.Add((T)item.Value);
            }
#endif
            return list;
        }

        public static T Get<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
                return default;

            if (!cached.ContainsKey(key))
                return default;

            return (T)cached[key];
        }

        public static void Set(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                return;
#if NET35
            lock (cached.SyncRoot)
            {
                cached[key] = value;
            }
#else
            cached[key] = value;
#endif
        }

        public static void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (!cached.ContainsKey(key))
                return;
#if NET35
            lock (cached.SyncRoot)
            {
                cached.Remove(key);
            }
#else
            cached.TryRemove(key, out object _);
#endif
        }

        public static List<CodeInfo> GetCodes(string appId)
        {
            var key = string.Format(KEY_CODES, appId);
            var codes = Get<List<CodeInfo>>(key);
            if (codes == null)
                codes = new List<CodeInfo>();

            return codes;
        }

        public static CodeInfo[] GetCodes(string appId, string category)
        {
            return GetCodes(appId).Where(c => c.Category == category).ToArray();
        }

        public static void SetCodes(string appId, List<CodeInfo> codes)
        {
            var key = string.Format(KEY_CODES, appId);
            Set(key, codes);
        }

        public static void AttachCodes(string appId, List<CodeInfo> codes)
        {
            var datas = new List<CodeInfo>();
            var items = GetCodes(appId);
            if (items != null && items.Count > 0)
            {
                foreach (var item in items)
                {
                    if (!codes.Exists(c => c.Category == item.Category))
                        datas.Add(item);
                }
            }

            datas.AddRange(codes);
            SetCodes(appId, datas);
        }

        public static void AttachCodes(string appId, Type constType)
        {
            var fields = constType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (fields == null || fields.Length == 0)
                return;

            var datas = new List<CodeInfo>();
            foreach (var item in fields)
            {
                var name = item.GetValue(null).ToString();
                var code = new CodeInfo(constType.Name, item.Name, name, null);
                datas.Add(code);
            }
            AttachCodes(appId, datas);
        }
    }

    public class CodeInfo
    {
        public CodeInfo() { }

        public CodeInfo(string code, string name, object data = null)
        {
            Code = code;
            Name = name;
            Data = data;
        }

        public CodeInfo(string category, string code, string name, object data = null)
        {
            Category = category;
            Code = code;
            Name = name;
            Data = data;
        }

        public string Category { get; set; }
        public string CategoryName { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public object Data { get; set; }

        public static CodeInfo[] GetCodes(string category, CodeInfo[] items = null)
        {
            if (items != null && items.Length > 0)
                return items;

            if (string.IsNullOrEmpty(category))
                return null;

            var appId = Config.App == null ? "" : Config.App.AppId;
            var codes = Cache.GetCodes(appId, category);
            if (codes != null && codes.Length > 0)
                return codes;

            return category.Split(',').Select(d => new CodeInfo(d, d)).ToArray();
        }
    }
}