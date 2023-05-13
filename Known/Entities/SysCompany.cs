namespace Known.Entities;

/// <summary>
/// 企业信息实体类。
/// </summary>
public class SysCompany : EntityBase
{
    /// <summary>
    /// 取得或设置企业编码。
    /// </summary>
    [Column("企业编码", "", false, "1", "50")]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置企业名称。
    /// </summary>
    [Column("企业名称", "", false, "1", "250")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置英文名称。
    /// </summary>
    [Column("英文名称", "", false, "1", "250")]
    public string NameEn { get; set; }

    /// <summary>
    /// 取得或设置社会信用代码。
    /// </summary>
    [Column("社会信用代码", "", false, "1", "18")]
    public string SccNo { get; set; }

    /// <summary>
    /// 取得或设置所属行业。
    /// </summary>
    [Column("所属行业", "", false, "1", "50")]
    public string Industry { get; set; }

    /// <summary>
    /// 取得或设置所属区域。
    /// </summary>
    [Column("所属区域", "", false, "1", "50")]
    public string Region { get; set; }

    /// <summary>
    /// 取得或设置中文地址。
    /// </summary>
    [Column("中文地址", "", false, "1", "500")]
    public string Address { get; set; }

    /// <summary>
    /// 取得或设置英文地址。
    /// </summary>
    [Column("英文地址", "", false, "1", "500")]
    public string AddressEn { get; set; }

    /// <summary>
    /// 取得或设置联系人。
    /// </summary>
    [Column("联系人", "", false, "1", "50")]
    public string Contact { get; set; }

    /// <summary>
    /// 取得或设置联系人电话。
    /// </summary>
    [Column("联系人电话", "", false, "1", "50")]
    public string Phone { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column("备注", "", false)]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置系统信息。
    /// </summary>
    [Column("系统信息", "", false, IsGrid = false)]
    public string SystemData { get; set; }

    /// <summary>
    /// 取得或设置企业信息。
    /// </summary>
    [Column("企业信息", "", false, IsGrid = false)]
    public string CompanyData { get; set; }
}