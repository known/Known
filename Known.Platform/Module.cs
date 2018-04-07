using Known.Mapping;
using System.Collections.Generic;

namespace Known.Platform
{
    /// <summary>
    /// 功能模块。
    /// </summary>
    public class Module : EntityBase
    {
        /// <summary>
        /// 取得或设置应用程序。
        /// </summary>
        public Application Application { get; set; }

        /// <summary>
        /// 取得或设置上级功能模块。
        /// </summary>
        public Module Parent { get; set; }

        /// <summary>
        /// 取得或设置功能模块名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置功能模块描述。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 取得或设置功能模块图标。
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 取得或设置功能模块访问地址。
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 取得或设置功能模块的按钮集合。
        /// </summary>
        public virtual List<Button> Buttons { get; set; }

        /// <summary>
        /// 取得或设置功能模块的列表栏位集合。
        /// </summary>
        public virtual List<View> Views { get; set; }
    }
}
