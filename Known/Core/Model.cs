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

namespace Known.Core
{
    public class MenuInfo
    {
        public string Id { get; set; }
        public string AppId { get; set; }
        public string ParentId { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public string Target { get; set; }
        public bool Checked { get; set; }
        public int Order { get; set; }
    }

    public class NoRuleInfo
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    public class OrganizationInfo
    {
        public string AppId { get; set; }
        public string CompNo { get; set; }
        public string CompName { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ManagerId { get; set; }
        public string Note { get; set; }
        public string Extension { get; set; }
    }

    public class TaskInfo
    {
        public const string Pending = "待执行";
        public const string Running = "执行中";
        public const string Success = "执行成功";
        public const string Failed = "执行失败";

        public string Id { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateTime { get; set; }
        public string CompNo { get; set; }
        public string AppId { get; set; }
        public string BizId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Target { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DateTime? BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    public class InstallInfo
    {
        public string CompNo { get; set; }
        public string CompName { get; set; }
        public string AppName { get; set; }
        public string ProductId { get; set; }
        public string ProductKey { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Password1 { get; set; }
    }

    public class SystemInfo
    {
        public string CompNo { get; set; }
        public string CompName { get; set; }
        public string AppName { get; set; }
        public string ProductId { get; set; }
        public string ProductKey { get; set; }
        public string ValidDate { get; set; }
        public string UserDefaultPwd { get; set; }
    }
}