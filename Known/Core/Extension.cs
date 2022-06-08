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
using Known.Entities;

namespace Known.Core
{
    public static class Extension
    {
        public static object ToTree(this MenuInfo menu)
        {
            return new
            {
                id = menu.Id,
                app = menu.AppId,
                pid = menu.ParentId == "0" ? menu.Code : menu.ParentId,
                type = menu.Type,
                code = menu.Code,
                name = menu.Name,
                title = menu.Name,
                icon = menu.Icon,
                url = menu.Url,
                target = menu.Target,
                hid = GetHelpId(menu.Type, menu.Code, menu.Url),
                @checked = menu.Checked
            };
        }

        public static string GetHelpId(string type, string code, string url)
        {
            var hid = string.Empty;

            if (type == "menu")
                hid = url;
            else if (type == "page")
                hid = string.IsNullOrEmpty(url) ? code : url;

            if (!string.IsNullOrEmpty(hid))
                hid = hid.GetHashCode().ToString().Replace("-", "");

            return hid;
        }

        public static object ToTree(this CodeInfo code)
        {
            return new
            {
                id = code.Code,
                pid = code.Category,
                code = code.Code,
                name = code.Name,
                title = code.Name,
                open = string.IsNullOrEmpty(code.Category) || code.Category == "0",
                data = code.Data
            };
        }

        internal static object ToTree(this SysOrganization entity)
        {
            return new
            {
                id = entity.Code,
                pid = entity.ParentId,
                code = entity.Code,
                name = entity.Name,
                title = entity.Name,
                open = string.IsNullOrEmpty(entity.ParentId) || entity.ParentId == "0",
                data = entity
            };
        }

        public static DateTime ToDate(this DateTime dateTime, string format = "yyyy-MM-dd")
        {
            var date = dateTime.ToString(format);
            return DateTime.Parse(date);
        }
    }
}