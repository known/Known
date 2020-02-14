using System;

namespace Known.Web
{
    /// <summary>
    /// 路由特性类。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RouteAttribute : Attribute
    {
        /// <summary>
        /// 初始化一个路由特性类的实例。
        /// </summary>
        /// <param name="name"></param>
        public RouteAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 取得路由名称。
        /// </summary>
        public string Name { get; }
    }
}
