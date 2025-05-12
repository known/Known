namespace Known;

/// <summary>
/// 在线页面模型配置信息类。
/// </summary>
public class PageInfo
{
    /// <summary>
    /// 取得或设置表格名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置页面类型（暂未使用）。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置表格页面是否显示高级搜索，默认显示。
    /// </summary>
    public bool ShowAdvSearch { get; set; } = true;

    /// <summary>
    /// 取得或设置表格页面是否显示分页。
    /// </summary>
    [DisplayName("分页")]
    public bool ShowPager { get; set; }

    /// <summary>
    /// 取得或设置表格页面是否显示列设置，默认显示。
    /// </summary>
    [DisplayName("列设置")]
    public bool ShowSetting { get; set; } = true;

    /// <summary>
    /// 取得或设置表格页面分页每页大小。
    /// </summary>
    [DisplayName("每页大小")]
    public int? PageSize { get; set; }

    /// <summary>
    /// 取得或设置表格页面工具条按钮显示数量大小。
    /// </summary>
    [DisplayName("工具条大小")]
    public int? ToolSize { get; set; }

    /// <summary>
    /// 取得或设置表格行操作按钮显示数量大小（暂未使用）。
    /// </summary>
    [DisplayName("操作列大小")]
    public int? ActionSize { get; set; }
    //public string FixedWidth { get; set; }
    //public string FixedHeight { get; set; }

    /// <summary>
    /// 取得或设置表格工具条按钮代码集合。
    /// </summary>
    public List<string> Tools { get; set; } = [];

    /// <summary>
    /// 取得或设置表格行操作按钮代码集合。
    /// </summary>
    public List<string> Actions { get; set; } = [];

    /// <summary>
    /// 取得或设置页面表格栏位信息列表。
    /// </summary>
    public List<PageColumnInfo> Columns { get; set; } = [];
}

/// <summary>
/// 在线页面表格栏位模型配置信息类。
/// </summary>
public class PageColumnInfo
{
    /// <summary>
    /// 取得或设置栏位ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置栏位名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是查看连接（设为True，才可在线配置表单，为False，则默认为普通查询表格）。
    /// </summary>
    public bool IsViewLink { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是查询条件。
    /// </summary>
    public bool IsQuery { get; set; }

    /// <summary>
    /// 取得或设置栏位查询条件下拉框是否显示【全部】。
    /// </summary>
    public bool IsQueryAll { get; set; }

    /// <summary>
    /// 取得或设置栏位查询条件默认值类型（固定值,占位符）。
    /// </summary>
    public string QueryValueType { get; set; } = "固定值";

    /// <summary>
    /// 取得或设置栏位查询条件默认值。
    /// </summary>
    public string QueryValue { get; set; }

    /// <summary>
    /// 取得或设置字段类型。
    /// </summary>
    public FieldType Type { get; set; }

    /// <summary>
    /// 取得或设置表单字段数据字典类别类型（Dictionary,Custom）。
    /// </summary>
    public string CategoryType { get; set; }

    /// <summary>
    /// 取得或设置表单字段数据字典类别。
    /// 若CategoryType是Dictionary，则从数据字典中选择；
    /// 若CategoryType是Customer，则自定义可数项目，用逗号分割，如：男,女。
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置栏位文本超出宽度是否显示省略号，显示则文本不换行。
    /// </summary>
    public bool Ellipsis { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是汇总字段。
    /// </summary>
    public bool IsSum { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是排序字段，默认排序。
    /// </summary>
    public bool IsSort { get; set; } = true;

    /// <summary>
    /// 取得或设置栏位默认排序方法（升序/降序）。
    /// </summary>
    public string DefaultSort { get; set; }

    /// <summary>
    /// 取得或设置栏位固定列位置（left/right）。
    /// </summary>
    public string Fixed { get; set; }

    /// <summary>
    /// 取得或设置栏位宽度。
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// 取得或设置栏位对齐方式（left/center/right）。
    /// </summary>
    public string Align { get; set; }

    /// <summary>
    /// 取得或设置栏位默认显示位置。
    /// </summary>
    public int? Position { get; set; }

    /// <summary>
    /// 取得或设置字段长度。
    /// </summary>
    public string Length { get; set; }

    /// <summary>
    /// 取得或设置字段是否必填。
    /// </summary>
    public bool Required { get; set; }
}