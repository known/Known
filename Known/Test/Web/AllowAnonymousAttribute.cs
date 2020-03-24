using System;

namespace Known.Web
{
    /// <summary>
    /// 允许匿名访问特性类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute
    {
    }
}
