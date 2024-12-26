namespace Known.Core;

/// <summary>
/// 系统模块配置信息类。
/// </summary>
public class ModuleInfo
{
    /// <summary>
    /// 取得或设置是否是查看详情。
    /// </summary>
    [JsonIgnore] public bool IsView { get; set; }

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
    /// 取得或设置类型（Menu/Page/Link）。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置目标（None/Blank/IFrame）。
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
    /// 取得或设置备注。
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置无代码页面配置信息。
    /// </summary>
    public PageInfo Page { get; set; }

    /// <summary>
    /// 取得或设置无代码表单配置信息。
    /// </summary>
    public FormInfo Form { get; set; }

    /// <summary>
    /// 取得或设置上级模块名称。
    /// </summary>
    [JsonIgnore] public virtual string ParentName { get; set; }

    /// <summary>
    /// 取得或设置是否上移。
    /// </summary>
    [JsonIgnore] public virtual bool IsMoveUp { get; set; }

    /// <summary>
    /// 取得是否是自定义页面。
    /// </summary>
    [JsonIgnore] public virtual bool IsCustomPage => Target == ModuleType.Custom.ToString();

    /// <summary>
    /// 取得或设置实体模型配置信息。
    /// </summary>
    [JsonIgnore] public virtual EntityInfo Entity { get; set; }

    /// <summary>
    /// 取得或设置工具条按钮列表。
    /// </summary>
    [JsonIgnore] public virtual List<string> Buttons { get; set; }

    /// <summary>
    /// 取得或设置表格操作列按钮列表。
    /// </summary>
    [JsonIgnore] public virtual List<string> Actions { get; set; }

    /// <summary>
    /// 取得或设置表格栏位信息列表。
    /// </summary>
    [JsonIgnore] public virtual List<PageColumnInfo> Columns { get; set; }

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