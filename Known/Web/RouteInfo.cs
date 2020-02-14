using System;
using System.Collections.Generic;
using System.Reflection;

namespace Known.Web
{
    /// <summary>
    /// 路由信息。
    /// </summary>
    public class RouteInfo
    {
        /// <summary>
        /// 取得或设置路由名。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置控制器类型。
        /// </summary>
        public Type Controller { get; set; }

        /// <summary>
        /// 取得或设置方法信息。
        /// </summary>
        public MethodInfo Action { get; set; }

        /// <summary>
        /// 取得或设置路由参数。
        /// </summary>
        public Dictionary<string, object> Datas { get; set; }
    }
}
