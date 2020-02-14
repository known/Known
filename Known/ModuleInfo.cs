using System;
using System.Collections.Generic;
using Known.Extensions;

namespace Known
{
    public class ModuleAttribute : Attribute
    {
        public ModuleAttribute(string name, string icon)
        {
            Name = name;
            Icon = icon;
        }

        public string Name { get; }

        public string Icon { get; }

        public string Code { get; set; }
    }

    public class PageAttribute : Attribute
    {
        public PageAttribute(int order, string name, string icon)
        {
            Order = order;
            Name = name;
            Icon = icon;
        }

        public int Order { get; }

        public string Name { get; }

        public string Icon { get; }

        public string Code { get; set; }
    }

    /// <summary>
    /// 模块页面视图类型。
    /// </summary>
    public enum ViewType
    {
        /// <summary>
        /// 未设置。
        /// </summary>
        None,
        /// <summary>
        /// 数据网格页面。
        /// </summary>
        DataGridView,
        /// <summary>
        /// 树网格页面。
        /// </summary>
        TreeGridView,
        /// <summary>
        /// 页签导航页面。
        /// </summary>
        TabPageView,
        /// <summary>
        /// 左右分割页面。
        /// </summary>
        SplitPageView
    }

    /// <summary>
    /// 模块信息类。
    /// </summary>
    public class ModuleInfo
    {
        /// <summary>
        /// 初始化一个模块信息类的实例。
        /// </summary>
        public ModuleInfo()
        {
            Enabled = true;
            Children = new List<ModuleInfo>();
        }

        /// <summary>
        /// 取得或设置主键ID。
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 取得或设置应用程序ID。
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 取得或设置上级模块ID。
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 取得或设置代码。
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 取得或设置名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置描述。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 取得或设置视图类型。
        /// </summary>
        public ViewType ViewType { get; set; }

        /// <summary>
        /// 取得或设置地址。
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 取得或设置图标。
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 取得或设置顺序。
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 取得或设置是否可用。
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 取得或设置按钮数据。
        /// </summary>
        public string ButtonData { get; set; }

        /// <summary>
        /// 取得或设置字段数据。
        /// </summary>
        public string FieldData { get; set; }

        /// <summary>
        /// 取得或设置扩展数据。
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// 取得或设置按钮信息列表。
        /// </summary>
        public List<ButtonInfo> Buttons
        {
            get { return ButtonData.FromJson<List<ButtonInfo>>(); }
            set { ButtonData = value.ToJson(); }
        }

        /// <summary>
        /// 取得或设置字段信息列表。
        /// </summary>
        public List<FieldInfo> Fields
        {
            get { return FieldData.FromJson<List<FieldInfo>>(); }
            set { FieldData = value.ToJson(); }
        }

        /// <summary>
        /// 取得或设置上级模块信息对象。
        /// </summary>
        public ModuleInfo Parent { get; set; }

        /// <summary>
        /// 取得或设置子模块信息列表。
        /// </summary>
        public List<ModuleInfo> Children { get; }

        /// <summary>
        /// 取得或设置全模块代码列表。
        /// </summary>
        public List<string> FullCodes
        {
            get
            {
                var fullCodes = new List<string> { this.Code };
                InitFullCodes(this, fullCodes);
                return fullCodes;
            }
        }

        /// <summary>
        /// 添加子模块信息。
        /// </summary>
        /// <param name="info">模块信息。</param>
        public void AddChild(ModuleInfo info)
        {
            info.AppId = AppId;
            info.ParentId = Id;
            info.Parent = this;
            Children.Add(info);
        }

        private void InitFullCodes(ModuleInfo module, List<string> fullCodes)
        {
            if (module.Parent != null)
            {
                fullCodes.Insert(0, module.Parent.Code);
                InitFullCodes(module.Parent, fullCodes);
            }
        }
    }

    #region Button
    /// <summary>
    /// 按钮信息类。
    /// </summary>
    public class ButtonInfo
    {
        /// <summary>
        /// 取得或设置上级按钮。
        /// </summary>
        public string Parent { get; set; }

        /// <summary>
        /// 取得或设置编码。
        /// </summary>
        public string Code { get; set; }

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
        /// 取得或设置顺序。
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 取得或设置是否有分隔符。
        /// </summary>
        public bool IsSplit { get; set; }

        /// <summary>
        /// 取得或设置子按钮对象列表。
        /// </summary>
        public List<ButtonInfo> Children { get; set; }
    }
    #endregion

    #region Field
    /// <summary>
    /// 字段控件类型。
    /// </summary>
    public enum FieldType
    {
        /// <summary>
        /// 单行文本框。
        /// </summary>
        TextBox,
        /// <summary>
        /// 多行文本框。
        /// </summary>
        TextArea,
        /// <summary>
        /// 日期框。
        /// </summary>
        Date,
        /// <summary>
        /// 日期时间框。
        /// </summary>
        DateTime,
        /// <summary>
        /// 整数型文本框。
        /// </summary>
        Integer,
        /// <summary>
        /// 数值型文本框。
        /// </summary>
        Decimal,
        /// <summary>
        /// 下拉框。
        /// </summary>
        ComboBox,
        /// <summary>
        /// 复选框。
        /// </summary>
        CheckBox,
        /// <summary>
        /// 复选框列表。
        /// </summary>
        CheckBoxList,
        /// <summary>
        /// 单选按钮列表。
        /// </summary>
        RadioButtonList
    }

    /// <summary>
    /// 字段查询类型。
    /// </summary>
    public enum QueryType
    {
        /// <summary>
        /// 未设置。
        /// </summary>
        None,
        /// <summary>
        /// 不等于。
        /// </summary>
        NotEqual,
        /// <summary>
        /// 等于。
        /// </summary>
        Equal,
        /// <summary>
        /// 小于。
        /// </summary>
        LessThan,
        /// <summary>
        /// 小于等于。
        /// </summary>
        LessEqual,
        /// <summary>
        /// 大于。
        /// </summary>
        GreatThan,
        /// <summary>
        /// 大于等于。
        /// </summary>
        GreatEqual,
        /// <summary>
        /// 区间，左右包含等于。
        /// </summary>
        Between,
        /// <summary>
        /// 区间，左右不包含等于。
        /// </summary>
        BetweenNotEqual,
        /// <summary>
        /// 区间，包含小于等于。
        /// </summary>
        BetweenLessEqual,
        /// <summary>
        /// 区间，包含大于等于。
        /// </summary>
        BetweenGreatEqual,
        /// <summary>
        /// 包含。
        /// </summary>
        Contain,
        /// <summary>
        /// 开始于。
        /// </summary>
        StartWith,
        /// <summary>
        /// 结束于。
        /// </summary>
        EndWith,
        /// <summary>
        /// 是空。
        /// </summary>
        IsNull,
        /// <summary>
        /// 不是空。
        /// </summary>
        IsNotNull,
        /// <summary>
        /// IN。
        /// </summary>
        In,
        /// <summary>
        /// 非IN。
        /// </summary>
        NotIn,
        /// <summary>
        /// 自定义。
        /// </summary>
        Custom
    }

    /// <summary>
    /// 字段信息类。
    /// </summary>
    public class FieldInfo
    {
        /// <summary>
        /// 取得或设置编码。
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 取得或设置名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置描述。
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 取得或设置控件类型。
        /// </summary>
        public FieldType FieldType { get; set; }

        /// <summary>
        /// 取得或设置查询类型。
        /// </summary>
        public QueryType QueryType { get; set; }

        /// <summary>
        /// 取得或设置顺序。
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 取得或设置是否是导出栏位。
        /// </summary>
        public bool IsExport { get; set; }

        /// <summary>
        /// 取得或设置是否是合并列。
        /// </summary>
        public bool IsMerge { get; set; }

        /// <summary>
        /// 取得或设置是否是表单栏位。
        /// </summary>
        public bool IsForm { get; set; }

        /// <summary>
        /// 取得或设置是否是排序栏位。
        /// </summary>
        public bool IsSort { get; set; }
    }
    #endregion
}
