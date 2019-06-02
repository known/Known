using System.Collections.Generic;

namespace Known.Core
{
    /// <summary>
    /// 应用程序信息类。
    /// </summary>
    public class Application
    {
        /// <summary>
        /// 取得或设置主键 Id。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 取得或设置应用程序编码。
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 取得或设置应用程序名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置应用程序版本。
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 取得或设置应用程序描述。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 取得或设置应用程序模块信息列表。
        /// </summary>
        public List<Module> Modules { get; set; }
    }
}