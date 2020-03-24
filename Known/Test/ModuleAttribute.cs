using System;
using System.Collections.Generic;

namespace Known
{
    /// <summary>
    /// 模块配置特性类。
    /// </summary>
    public class ModuleAttribute : Attribute
    {
        /// <summary>
        /// 初始化一个模块配置特性类的实例。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="icon">图标。</param>
        public ModuleAttribute(string name, string icon)
        {
            Name = name;
            Icon = icon;
        }

        /// <summary>
        /// 初始化一个模块配置特性类的实例。
        /// </summary>
        /// <param name="order">显示顺序。</param>
        /// <param name="name">名称。</param>
        /// <param name="icon">图标。</param>
        /// <param name="page">页面按钮对应的Controller类型。</param>
        public ModuleAttribute(int order, string name, string icon, Type page)
        {
            Order = order;
            Name = name;
            Icon = icon;
            Page = page;
        }

        /// <summary>
        /// 取得显示顺序。
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// 取得名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 取得图标。
        /// </summary>
        public string Icon { get; }

        /// <summary>
        /// 取得页面按钮对应的Controller类型。
        /// </summary>
        public Type Page { get; }

        /// <summary>
        /// 取得代码。
        /// </summary>
        public string Code { get; internal set; }

        /// <summary>
        /// 取得上级模块代码。
        /// </summary>
        public string Parent { get; internal set; }

        /// <summary>
        /// 取得URL。
        /// </summary>
        public string Url { get; internal set; }

        /// <summary>
        /// 取得模块按钮集合。
        /// </summary>
        public List<ToolbarAttribute> Buttons { get; internal set; }

        /// <summary>
        /// 取得模块列表栏位集合。
        /// </summary>
        public List<string> Columns { get; internal set; }
    }
}
