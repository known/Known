namespace Known;

/// <summary>
/// 系统模块配置信息类。
/// </summary>
public class ModuleInfo
{
    /// <summary>
    /// 取得或设置是否是查看详情。
    /// </summary>
    public bool IsView { get; set; }

    /// <summary>
    /// 取得或设置ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置上级。
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
    /// 取得或设置图标。
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置描述。
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置目标。
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置Url地址。
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// 取得或设置顺序。
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置可用。
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置实体设置。
    /// </summary>
    public string EntityData { get; set; }

    /// <summary>
    /// 取得或设置流程设置。
    /// </summary>
    public string FlowData { get; set; }

    /// <summary>
    /// 取得或设置页面设置。
    /// </summary>
    public string PageData { get; set; }

    /// <summary>
    /// 取得或设置表单设置。
    /// </summary>
    public string FormData { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置上级模块名称。
    /// </summary>
    public virtual string ParentName { get; set; }

    /// <summary>
    /// 取得或设置是否上移。
    /// </summary>
    public virtual bool IsMoveUp { get; set; }

    /// <summary>
    /// 取得是否是自定义页面。
    /// </summary>
    public virtual bool IsCustomPage => Target == ModuleType.Custom.ToString();

    /// <summary>
    /// 取得或设置实体模型配置信息。
    /// </summary>
    public virtual EntityInfo Entity { get; set; }

    private PageInfo page;
    /// <summary>
    /// 取得或设置无代码页面配置信息。
    /// </summary>
    public virtual PageInfo Page
    {
        get
        {
            page ??= Utils.FromJson<PageInfo>(PageData) ?? new();
            return page;
        }
        set
        {
            page = value;
            PageData = Utils.ToJson(value);
        }
    }

    private FormInfo form;
    /// <summary>
    /// 取得或设置无代码表单配置信息。
    /// </summary>
    public virtual FormInfo Form
    {
        get
        {
            form ??= Utils.FromJson<FormInfo>(FormData) ?? new();
            return form;
        }
        set
        {
            form = value;
            FormData = Utils.ToJson(value);
        }
    }

    /// <summary>
    /// 取得或设置工具条按钮列表。
    /// </summary>
    public virtual List<string> Buttons { get; set; }

    /// <summary>
    /// 取得或设置表格操作列按钮列表。
    /// </summary>
    public virtual List<string> Actions { get; set; }

    /// <summary>
    /// 取得或设置表格栏位信息列表。
    /// </summary>
    public virtual List<PageColumnInfo> Columns { get; set; }

    internal void LoadData()
    {
        Buttons = GetToolButtons();
        Actions = GetTableActions();
        Columns = Page?.Columns;
    }

    /// <summary>
    /// 获取模块页面列表栏位信息列表。
    /// </summary>
    /// <returns>栏位信息列表。</returns>
    public List<PageColumnInfo> GetPageColumns() => Page?.Columns;

    /// <summary>
    /// 获取模块表单字段信息列表。
    /// </summary>
    /// <returns>字段信息列表。</returns>
    public List<FormFieldInfo> GetFormFields() => Form?.Fields;

    /// <summary>
    /// 获取工具条按钮列表。
    /// </summary>
    /// <returns></returns>
    public List<string> GetToolButtons() => Page?.Tools?.ToList();

    /// <summary>
    /// 获取表格操作按钮列表。
    /// </summary>
    /// <returns></returns>
    public List<string> GetTableActions() => Page?.Actions?.ToList();
}

/// <summary>
/// 在线实体模型配置信息类。
/// </summary>
public class EntityInfo
{
    /// <summary>
    /// 取得或设置实体ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置实体名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置实体对应的页面URL。
    /// </summary>
    public string PageUrl { get; set; }

    /// <summary>
    /// 取得或设置是否是工作流实体。
    /// </summary>
    public bool IsFlow { get; set; }

    /// <summary>
    /// 取得或设置实体字段信息列表。
    /// </summary>
    public List<FieldInfo> Fields { get; set; } = [];
}

/// <summary>
/// 字段类型枚举。
/// </summary>
public enum FieldType
{
    //添加字段类型，只能在最后添加，否则会影响模块表单配置
    /// <summary>
    /// 文本框。
    /// </summary>
    Text,
    /// <summary>
    /// 多行文本框。
    /// </summary>
    TextArea,
    /// <summary>
    /// 日期选择框。
    /// </summary>
    Date,
    /// <summary>
    /// 数字选择框。
    /// </summary>
    Number,
    /// <summary>
    /// 开关。
    /// </summary>
    Switch,
    /// <summary>
    /// 复选框。
    /// </summary>
    CheckBox,
    /// <summary>
    /// 复选框列表。
    /// </summary>
    CheckList,
    /// <summary>
    /// 单选列表。
    /// </summary>
    RadioList,
    /// <summary>
    /// 选择框。
    /// </summary>
    Select,
    /// <summary>
    /// 密码框。
    /// </summary>
    Password,
    /// <summary>
    /// 附件。
    /// </summary>
    File,
    /// <summary>
    /// 日期时间选择框。
    /// </summary>
    DateTime,
    /// <summary>
    /// 自动完成。
    /// </summary>
    AutoComplete,
    /// <summary>
    /// 自定义。
    /// </summary>
    Custom
}

/// <summary>
/// 在线实体字段配置信息类。
/// </summary>
public class FieldInfo
{
    /// <summary>
    /// 取得或设置字段ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置字段名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置字段类型。
    /// </summary>
    public FieldType Type { get; set; }

    /// <summary>
    /// 取得或设置字段长度。
    /// </summary>
    public string Length { get; set; }

    /// <summary>
    /// 取得或设置字段是否必填。
    /// </summary>
    public bool Required { get; set; }
}

/// <summary>
/// 在线页面模型配置信息类。
/// </summary>
public class PageInfo
{
    /// <summary>
    /// 取得或设置页面类型（暂未使用）。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置表格页面是否显示分页。
    /// </summary>
    public bool ShowPager { get; set; }

    /// <summary>
    /// 取得或设置表格页面分页每页大小。
    /// </summary>
    public int? PageSize { get; set; }

    /// <summary>
    /// 取得或设置表格页面工具条按钮显示数量大小。
    /// </summary>
    public int? ToolSize { get; set; }

    /// <summary>
    /// 取得或设置表格行操作按钮显示数量大小（暂未使用）。
    /// </summary>
    public int? ActionSize { get; set; }
    //public string FixedWidth { get; set; }
    //public string FixedHeight { get; set; }

    /// <summary>
    /// 取得或设置表格工具条按钮代码集合。
    /// </summary>
    public string[] Tools { get; set; }

    /// <summary>
    /// 取得或设置表格行操作按钮代码集合。
    /// </summary>
    public string[] Actions { get; set; }

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
    /// 取得或设置字段类型。
    /// </summary>
    public FieldType Type { get; set; }

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
    /// 取得或设置栏位是否是汇总字段。
    /// </summary>
    public bool IsSum { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是排序字段。
    /// </summary>
    public bool IsSort { get; set; }

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
}

/// <summary>
/// 在线表单模型配置信息类。
/// </summary>
public class FormInfo
{
    /// <summary>
    /// 取得或设置表单对话框显示宽度。
    /// </summary>
    public double? Width { get; set; }

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
    /// 取得或设置表单字段是否为只读。
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// 取得或设置表单字段附件是否可多选。
    /// </summary>
    public bool MultiFile { get; set; }
}