using System.Collections.Generic;

namespace Known.Core
{
    /// <summary>
    /// 模块。
    /// </summary>
    public class Module
    {
        /// <summary>
        /// 取得或设置ID。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 取得或设置上级模块。
        /// </summary>
        public Module Parent { get; set; }

        /// <summary>
        /// 取得或设置名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置图标。
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 取得或设置访问地址。
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 取得或设置子模块集合。
        /// </summary>
        public List<Module> Children { get; set; }

        /// <summary>
        /// 取得或设置按钮集合。
        /// </summary>
        public List<Button> Buttons { get; set; }
    }
}
