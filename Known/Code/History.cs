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

using System.Collections.Generic;
using System.IO;

namespace Known
{
    public sealed class History
    {
        private static readonly string path = Path.Combine(Config.RootPath, "data_history.data");
        private static readonly Dictionary<string, string> datas = new Dictionary<string, string>();

        private History() { }

        static History()
        {
            var json = Utils.ReadFile(path);
            if (!string.IsNullOrEmpty(json))
            {
                datas = Utils.FromJson<Dictionary<string, string>>(json);
            }
        }

        internal static string GetHistory(UserInfo user)
        {
            var key = GetKey(user);
            if (datas.ContainsKey(key))
                return datas[key];

            return string.Empty;
        }

        public static T GetHistory<T>(UserInfo user)
        {
            var value = GetHistory(user);
            if (string.IsNullOrEmpty(value))
                return default;

            return Utils.FromJson<T>(value);
        }

        public static void SaveHistory(UserInfo user, object value)
        {
            var key = GetKey(user);
            datas[key] = Utils.ToJson(value);
            var json = Utils.ToJson(datas);
            Utils.SaveFile(path, json);
        }

        private static string GetKey(UserInfo user)
        {
            return $"{user.AppId}_{user.CompNo}_{user.UserName}";
        }
    }
}