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
using System.IO;
using Known.Web;

namespace Known.Dev.Services;

class TestService : ServiceBase, IService
{
    public ApiFile GetTestPdf()
    {
        var bytes = File.ReadAllBytes(@"D:\Documents\RTSP和HTTP URL介绍.pdf");
        return new ApiFile
        {
            ContentType = MimeTypes.ApplicationPdf,
            Bytes = bytes
        };
    }

    public string GetCache()
    {
        var value = Cache.Get<string>("Test");
        return value;
    }

    public Result SetCache(string value)
    {
        Cache.Set("Test", $"{value}:{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        return Result.Success("设置成功！");
    }
}