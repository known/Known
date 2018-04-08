using System.Collections.Generic;

namespace Known.Platform
{
    /// <summary>
    /// 功能模块。
    /// </summary>
    public class Module
    {
        /// <summary>
        /// 取得或设置ID。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 取得或设置上级功能模块。
        /// </summary>
        public Module Parent { get; set; }

        /// <summary>
        /// 取得或设置名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置描述。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 取得或设置图标。
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 取得或设置访问地址。
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 取得或设置按钮集合。
        /// </summary>
        public virtual List<Button> Buttons { get; set; }

        /// <summary>
        /// 取得或设置列表栏位集合。
        /// </summary>
        public virtual List<Field> Fields { get; set; }
    }
}
