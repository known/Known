namespace Known;

/// <summary>
/// 在线表单模型配置信息类。
/// </summary>
public class FormInfo
{
    /// <summary>
    /// 取得或设置表单打开方式。
    /// </summary>
    public FormOpenType OpenType { get; set; }

    /// <summary>
    /// 取得或设置表单对话框显示宽度。
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// 取得或设置表单是否窄宽标题。
    /// </summary>
    public bool SmallLabel { get; set; }

    /// <summary>
    /// 取得或设置表单对话框是否显示最大化按钮。
    /// </summary>
    public bool Maximizable { get; set; }

    /// <summary>
    /// 取得或设置表单对话框是否默认最大化。
    /// </summary>
    public bool DefaultMaximized { get; set; }

    /// <summary>
    /// 取得或设置表单是否连续添加数据，设为True，则显示【确定继续】和【确定关闭】按钮。
    /// </summary>
    public bool IsContinue { get; set; }

    /// <summary>
    /// 取得或设置表单对话框是否隐藏底部按钮。
    /// </summary>
    public bool NoFooter { get; set; }

    /// <summary>
    /// 取得或设置对话框表单是否显示底部按钮。
    /// </summary>
    public bool ShowFooter { get; set; }

    /// <summary>
    /// 取得或设置表单字段信息列表。
    /// </summary>
    public List<FormFieldInfo> Fields { get; set; } = [];
}

/// <summary>
/// 在线表单字段模型配置信息类，继承实体字段配置类。
/// </summary>
public class FormFieldInfo : FieldInfo
{
    /// <summary>
    /// 取得或设置表单字段行号，默认1。
    /// </summary>
    public int Row { get; set; } = 1;

    /// <summary>
    /// 取得或设置表单字段列号，默认1。
    /// </summary>
    public int Column { get; set; } = 1;

    /// <summary>
    /// 取得或设置表单字段跨度大小，整行跨度为24。
    /// </summary>
    public int? Span { get; set; }

    /// <summary>
    /// 取得或设置自定义字段组件类型。
    /// </summary>
    public string CustomField { get; set; }

    /// <summary>
    /// 取得或设置表单字段数据字典类别类型（Dictionary/Custom）。
    /// </summary>
    public string CategoryType { get; set; }

    /// <summary>
    /// 取得或设置表单字段数据字典类别。
    /// 若CategoryType是Dictionary，则从数据字典中选择；
    /// 若CategoryType是Customer，则自定义可数项目，用逗号分割，如：男,女。
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置表单字段控件占位符文本。
    /// </summary>
    public string Placeholder { get; set; }

    /// <summary>
    /// 取得或设置文本域组件行数，默认3。
    /// </summary>
    public uint Rows { get; set; } = 3;

    /// <summary>
    /// 取得或设置计量单位。
    /// </summary>
    public string Unit { get; set; }

    /// <summary>
    /// 取得或设置表单字段是否为只读。
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// 取得或设置表单字段附件是否可多选。
    /// </summary>
    public bool MultiFile { get; set; }
}