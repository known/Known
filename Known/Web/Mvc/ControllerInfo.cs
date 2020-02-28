using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Known.Core;

namespace Known.Web.Mvc
{
    /// <summary>
    /// 控制器信息类。
    /// </summary>
    public class ControllerInfo
    {
        private ControllerInfo(Type type)
        {
            Type = type;
            Name = type.Name.Replace("Controller", "");
            Actions = new List<ActionInfo>();
        }

        /// <summary>
        /// 取得控制器名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 取得控制器类型。
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 取得控制器所有Action列表。
        /// </summary>
        public List<ActionInfo> Actions { get; }

        internal ActionInfo GetAction(string name)
        {
            if (Actions == null || Actions.Count == 0)
                return null;

            return Actions.FirstOrDefault(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        internal static ControllerInfo Create(Type type)
        {
            var info = new ControllerInfo(type);

            var attr = type.GetCustomAttribute<ModuleAttribute>();
            if (attr != null)
            {
                attr.Code = info.Name;
                AppInfo.Instance.AddModule(attr);
            }

            return info;
        }
    }
}
