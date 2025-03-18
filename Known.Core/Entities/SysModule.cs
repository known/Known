namespace Known.Entities;

/// <summary>
/// 系统模块实体类。
/// </summary>
public class SysModule : EntityBase
{
    /// <summary>
    /// 取得或设置上级。
    /// </summary>
    [MaxLength(50)]
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置代码。
    /// </summary>
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
    public string Type { get; set; }

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
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 取得或设置页面布局配置数据。
    /// </summary>
    public string LayoutData { get; set; }

    /// <summary>
    /// 取得或设置插件配置数据。
    /// </summary>
    public string PluginData { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [MaxLength(500)]
    public string Note { get; set; }

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

    internal ModuleInfo ToModuleInfo()
    {
        return new ModuleInfo
        {
            Id = Id,
            ParentId = ParentId,
            Name = Name,
            Icon = Icon,
            Type = Type,
            Target = Target,
            Url = Url,
            Sort = Sort,
            Enabled = Enabled,
            Layout = Utils.FromJson<LayoutInfo>(LayoutData),
            Plugins = ZipHelper.UnZipDataFromString<List<PluginInfo>>(PluginData)
        };
    }

    internal static SysModule Load(ModuleInfo info)
    {
        return new SysModule
        {
            ParentId = info.ParentId,
            Code = info.Id,//用于查询上级模块
            Name = info.Name,
            Icon = info.Icon,
            Type = info.Type,
            Target = info.Target,
            Url = info.Url,
            Sort = info.Sort,
            Enabled = info.Enabled,
            LayoutData = Utils.ToJson(info.Layout),
            PluginData = info.Plugins?.ZipDataString()
        };
    }

    internal List<PluginInfo> ToPlugins()
    {
        if (Type == nameof(MenuType.Menu) || Target == nameof(ModuleType.Menu))
            return null;

        var plugins = new List<PluginInfo>();
        plugins.AddPlugin(ToAutoPageInfo());
        return plugins;
    }

    private AutoPageInfo ToAutoPageInfo()
    {
        return new AutoPageInfo
        {
            EntityData = EntityData,
            FlowData = FlowData,
            Page = Utils.FromJson<PageInfo>(PageData),
            Form = Utils.FromJson<FormInfo>(FormData)
        };
    }
}