using System;

namespace Known.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class DoNotTrackAttribute : Attribute
    {
    }
}
