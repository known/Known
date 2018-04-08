using System.Collections.Generic;

namespace Known.Platform
{
    /// <summary>
    /// 应用程序。
    /// </summary>
    public class Application
    {
        /// <summary>
        /// 取得或设置ID。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 取得或设置名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置描述。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 取得或设置模块集合。
        /// </summary>
        public List<Module> Modules { get; set; }
    }
}
