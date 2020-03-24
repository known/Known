using System;
using Known.Extensions;

namespace Known
{
    /// <summary>
    /// 工具条按钮类型。
    /// </summary>
    public enum ToolbarType
    {
        /// <summary>
        /// 新增。
        /// </summary>
        [Button("新增", "fa-plus")]
        Add,
        /// <summary>
        /// 编辑。
        /// </summary>
        [Button("编辑", "fa-pencil")]
        Edit,
        /// <summary>
        /// 保存。
        /// </summary>
        [Button("保存", "fa-save")]
        Save,
        /// <summary>
        /// 删除。
        /// </summary>
        [Button("删除", "fa-minus")]
        Remove,
        /// <summary>
        /// 导入。
        /// </summary>
        [Button("导入", "fa-sign-in")]
        Imports,
        /// <summary>
        /// 导出
        /// </summary>
        [Button("导出", "fa-sign-out")]
        Exports,
        /// <summary>
        /// 上载。
        /// </summary>
        [Button("上载", "fa-upload")]
        Upload,
        /// <summary>
        /// 下载。
        /// </summary>
        [Button("下载", "fa-download")]
        Download
    }

    class ButtonAttribute : Attribute
    {
        public ButtonAttribute(string name, string icon)
        {
            Name = name;
            Icon = icon;
        }

        public string Name { get; }
        public string Icon { get; }
    }

    /// <summary>
    /// 工具条按钮配置特性类。
    /// </summary>
    public class ToolbarAttribute : Attribute
    {
        /// <summary>
        /// 初始化一个工具条按钮配置特性类的实例。
        /// </summary>
        /// <param name="order">显示顺序。</param>
        /// <param name="type">按钮类型。</param>
        /// <param name="isForm">是否是表单按钮。</param>
        public ToolbarAttribute(int order, ToolbarType type, bool isForm = false)
        {
            Order = order;
            IsForm = isForm;
            Id = type.ToString().ToLower();
            var attr = type.GetAttribute<ButtonAttribute>();
            if (attr != null)
            {
                Name = attr.Name;
                Icon = attr.Icon;
            }
        }

        /// <summary>
        /// 初始化一个工具条按钮配置特性类的实例。
        /// </summary>
        /// <param name="order">显示顺序。</param>
        /// <param name="id">ID。</param>
        /// <param name="name">名称。</param>
        /// <param name="icon">图标。</param>
        /// <param name="isForm">是否是表单按钮。</param>
        public ToolbarAttribute(int order, string id, string name, string icon, bool isForm = false)
        {
            Order = order;
            Id = id;
            Name = name;
            Icon = icon;
            IsForm = isForm;
        }

        /// <summary>
        /// 取得显示顺序。
        /// </summary>
        public int Order { get; }

        /// <summary>
        /// 取得ID。
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// 取得名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 取得图标。
        /// </summary>
        public string Icon { get; }

        /// <summary>
        /// 取得是否是表单按钮。
        /// </summary>
        public bool IsForm { get; }

        /// <summary>
        /// 取得URL。
        /// </summary>
        public string Url { get; internal set; }

        /// <summary>
        /// 取得按钮对应的页面Controller类型。
        /// </summary>
        public Type Page { get; internal set; }
    }
}
