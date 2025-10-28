namespace Known.Entities;

/// <summary>
/// 系统模块实体类。
/// </summary>
[DisplayName("系统模块")]
public class SysModule : EntityBase
{
    /// <summary>
    /// 取得或设置上级。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("上级ID")]
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("代码")]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置图标。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("图标")]
    public string Icon { get; set; }

    /// <summary>
    /// 取得或设置描述。
    /// </summary>
    [MaxLength(200)]
    [DisplayName("描述")]
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置类型。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("类型")]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置目标。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("目标")]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置Url地址。
    /// </summary>
    [MaxLength(200)]
    [DisplayName("Url地址")]
    public string Url { get; set; }

    /// <summary>
    /// 取得或设置顺序。
    /// </summary>
    [DisplayName("顺序")]
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Category(nameof(StatusType))]
    [DisplayName("状态")]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 取得或设置页面布局配置数据。
    /// </summary>
    [DisplayName("布局数据")]
    public string LayoutData { get; set; }

    /// <summary>
    /// 取得或设置插件配置数据。
    /// </summary>
    [DisplayName("插件数据")]
    public string PluginData { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [MaxLength(500)]
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置实体设置（V2.0字段）。
    /// </summary>
    [DisplayName("实体设置")]
    public string EntityData { get; set; }

    /// <summary>
    /// 取得或设置流程设置（V2.0字段）。
    /// </summary>
    [DisplayName("流程设置")]
    public string FlowData { get; set; }

    /// <summary>
    /// 取得或设置页面设置（V2.0字段）。
    /// </summary>
    [DisplayName("页面设置")]
    public string PageData { get; set; }

    /// <summary>
    /// 取得或设置表单设置（V2.0字段）。
    /// </summary>
    [DisplayName("表单设置")]
    public string FormData { get; set; }

    /// <summary>
    /// 取得或设置是否是代码生成的模块。
    /// </summary>
    public virtual bool IsCode { get; set; }

    /// <summary>
    /// 取得或设置是否移动模块。
    /// </summary>
    [JsonIgnore]
    public virtual bool IsMoveUp { get; set; }

    /// <summary>
    /// 取得或设置布局信息。
    /// </summary>
    public virtual LayoutInfo Layout { get; set; }

    /// <summary>
    /// 取得或设置插件配置信息列表。
    /// </summary>
    public virtual List<PluginInfo> Plugins { get; set; } = [];

    internal string ParentName { get; set; }

    internal MenuInfo ToMenuInfo()
    {
        var info = new MenuInfo
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
            Data = this
        };
        if (!string.IsNullOrWhiteSpace(PluginData))
            info.Plugins = ZipHelper.UnZipDataAsString<List<PluginInfo>>(PluginData);
        else
            info.Plugins = ToPlugins();
        return info;
    }

    //internal static SysModule Load(UserInfo user, ModuleInfo info)
    //{
    //    return new SysModule
    //    {
    //        AppId = user.AppId,
    //        CompNo = user.CompNo,
    //        CreateBy = user.UserName,
    //        ParentId = info.ParentId,
    //        Code = info.Id,//用于查询上级模块
    //        Name = info.Name,
    //        Icon = info.Icon,
    //        Type = info.Type,
    //        Target = info.Target ?? nameof(LinkTarget.None),
    //        Url = info.Url,
    //        Sort = info.Sort,
    //        Enabled = info.Enabled,
    //        LayoutData = Utils.ToJson(info.Layout),
    //        PluginData = info.Plugins?.ZipDataString()
    //    };
    //}

    // 适用于Admin插件迁移
    internal List<PluginInfo> ToPlugins()
    {
        if (Type == nameof(MenuType.Menu) || Target == nameof(ModuleType.Menu))
            return null;

        var plugins = new List<PluginInfo>();
        plugins.AddPlugin(ToAutoPageInfo());
        return plugins;
    }

    internal AutoPageInfo ToAutoPageInfo()
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