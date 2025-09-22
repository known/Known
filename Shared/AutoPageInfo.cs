namespace Known;

/// <summary>
/// 自动页面类型。
/// </summary>
public enum AutoPageType
{
    /// <summary>
    /// 新建数据库表。
    /// </summary>
    NewTable,
    /// <summary>
    /// 已有数据库表。
    /// </summary>
    OldTable,
    /// <summary>
    /// SQL查询语句。
    /// </summary>
    SqlQuery
}

/// <summary>
/// 自动页面插件配置信息类。
/// </summary>
public class AutoPageInfo
{
    /// <summary>
    /// 取得或设置页面ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置页面名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置代码模型表前缀。
    /// </summary>
    public string Prefix { get; set; }

    /// <summary>
    /// 取得或设置代码模型命名空间。
    /// </summary>
    public string Namespace { get; set; } = Config.App.Id;

    /// <summary>
    /// 取得或设置页面类型。
    /// </summary>
    public string Type
    {
        get { return PageType.ToString(); }
        set { PageType = Utils.ConvertTo<AutoPageType>(value); }
    }

    /// <summary>
    /// 取得或设置页面类型。
    /// </summary>
    public AutoPageType PageType { get; set; }

    /// <summary>
    /// 取得或设置页面关联实体的数据库连接名。
    /// </summary>
    public string Database { get; set; }

    /// <summary>
    /// 取得或设置页面关联实体的数据库表或SQL语句。
    /// </summary>
    public string Script { get; set; }

    /// <summary>
    /// 取得或设置页面关联实体的数据库表的主键字段，默认Id。
    /// </summary>
    public string IdField { get; set; } = nameof(EntityBase.Id);

    /// <summary>
    /// 取得或设置实体设置。
    /// </summary>
    public string EntityData { get; set; }

    /// <summary>
    /// 取得或设置流程设置。
    /// </summary>
    public string FlowData { get; set; }

    /// <summary>
    /// 取得或设置无代码页面配置信息。
    /// </summary>
    public PageInfo Page { get; set; } = new();

    /// <summary>
    /// 取得或设置无代码表单配置信息。
    /// </summary>
    public FormInfo Form { get; set; } = new();
}