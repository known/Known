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

using Known.Entities;

namespace Known
{
    class Constants
    {
        private Constants() { }

        internal const string KeyToken = "Known-Token";
        internal const string KeyClient = "Known-Client";
        internal const string KeyDownload = "Known-Download";
        internal const string SysUserName = "Admin";
        internal const string FrameDictCode = "KnownDict";

        internal static SysDictionary FrameDict = new SysDictionary()
        {
            Code = FrameDictCode,
            Name = "框架字典",
            Sort = 0
        };

        internal const string UMTypeReceive = "收件";
        internal const string UMTypeSend = "发件";
        internal const string UMTypeDelete = "删除";
        internal const string UMStatusRead = "已读";
        internal const string UMStatusUnread = "未读";

        internal const string NSPublished = "已发布";
    }
}