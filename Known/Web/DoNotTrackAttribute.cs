using System;

namespace Known.Web
{
    /// <summary>
    /// 不执行跟踪特性类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class DoNotTrackAttribute : Attribute
    {
    }
}
