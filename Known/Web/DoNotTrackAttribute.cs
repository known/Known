using System;

namespace Known.Web
{
    /// <summary>
    /// 无需跟踪特性，表示类或方法不记录跟踪日志。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class DoNotTrackAttribute : Attribute
    {
    }
}
