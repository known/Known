namespace Known.Entities;

/// <summary>
/// 系统模块实体类。
/// </summary>
public class SysModule : EntityBase
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    public SysModule()
    {
        Enabled = true;
    }

    /// <summary>
    /// 取得或设置上级。
    /// </summary>
    [MaxLength(50)]
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置图标。
    /// </summary>
    [MaxLength(50)]
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置描述。
    /// </summary>
    [MaxLength(200)]
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置目标。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置Url地址。
    /// </summary>
    [MaxLength(200)]
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
    [MaxLength(500)]
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
    /// 取得或设置实体模型配置信息。
    /// </summary>
    public virtual EntityInfo Entity { get; set; }

    private PageInfo page;
    internal virtual PageInfo Page
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
    internal virtual FormInfo Form
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

    internal virtual List<string> Buttons { get; set; }
    internal virtual List<string> Actions { get; set; }
    internal virtual List<PageColumnInfo> Columns { get; set; }

    internal void LoadData()
    {
        Buttons = Page?.Tools?.ToList();
        Actions = Page?.Actions?.ToList();
        Columns = Page?.Columns;
    }
}