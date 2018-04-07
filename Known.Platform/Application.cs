using Known.Mapping;
using System.Collections.Generic;

namespace Known.Platform
{
    /// <summary>
    /// 应用程序。
    /// </summary>
    public class Application : EntityBase
    {
        /// <summary>
        /// 取得或设置应用程序名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置应用程序描述。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 取得或设置应用程序的模块集合。
        /// </summary>
        public virtual List<Module> Modules { get; set; }
    }
}
