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

namespace Known.Core
{
    partial class SystemService
    {
        public SystemInfo GetSetting()
        {
            return Platform.GetSystem();
        }

        public Result SaveSetting(string data)
        {
            var info = Utils.FromJson<SystemInfo>(data);
            var result = Platform.SaveSystem(info);
            if (result.IsValid)
            {
                var user = CurrentUser;
                user.AppName = info.AppName;
                user.CompName = info.CompName;
                UserHelper.SetUser(user);
            }
            return result;
        }
    }
}