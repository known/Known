namespace Known;

public partial class ColumnInfo
{
    internal bool IsForm { get; set; }
    internal string DisplayName { get; set; }

    /// <summary>
    /// 取得或设置栏位是否显示标题名称，默认显示。
    /// </summary>
    public bool ShowLabel { get; set; } = true;

    /// <summary>
    /// 取得或设置栏标题名称。
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// 取得或设置表单字段数据字典类别。
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置表单字段控件占位符文本。
    /// </summary>
    public string Placeholder { get; set; }

    /// <summary>
    /// 取得或设置字段默认值。
    /// </summary>
    public string FieldValue { get; set; }

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
    /// 取得或设置栏位字段组件类型。
    /// </summary>
    public FieldType Type { get; set; }

    private string customField;
    /// <summary>
    /// 取得或设置自定义字段组件类型名称。
    /// </summary>
    public string CustomField
    {
        get { return customField; }
        set
        {
            customField = value;
            if (!string.IsNullOrWhiteSpace(value))
                Type = FieldType.Custom;
        }
    }

    /// <summary>
    /// 取得或设置文本域组件行数，默认3。
    /// </summary>
    public uint Rows { get; set; } = 3;

    /// <summary>
    /// 取得或设置计量单位。
    /// </summary>
    public string Unit { get; set; }

    /// <summary>
    /// 取得或设置表单字段附件是否可多选。
    /// </summary>
    public bool MultiFile { get; set; }

    /// <summary>
    /// 取得或设置上传文件的模板URL。
    /// </summary>
    public string TemplateUrl { get; set; }

    /// <summary>
    /// 取得或设置字段是否必填。
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// 取得或设置表单字段是否为只读。
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// 取得或设置表单字段代码表列表。
    /// </summary>
    public List<CodeInfo> Codes { get; set; }

    /// <summary>
    /// 取得字段其他属性字典。
    /// </summary>
    public Dictionary<string, object> Attributes { get; } = [];
}