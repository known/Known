namespace Known.Entities;

/// <summary>
/// 企业信息实体类。
/// </summary>
[DisplayName("企业信息")]
public class SysCompany : EntityBase
{
    /// <summary>
    /// 取得或设置企业编码。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("企业编码")]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置企业名称。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("企业名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置英文名称。
    /// </summary>
    [MaxLength(250)]
    [DisplayName("英文名称")]
    public string NameEn { get; set; }

    /// <summary>
    /// 取得或设置社会信用代码。
    /// </summary>
    [MaxLength(18)]
    [DisplayName("社会信用代码")]
    public string SccNo { get; set; }

    /// <summary>
    /// 取得或设置所属行业。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("所属行业")]
    public string Industry { get; set; }

    /// <summary>
    /// 取得或设置所属区域。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("所属区域")]
    public string Region { get; set; }

    /// <summary>
    /// 取得或设置中文地址。
    /// </summary>
    [MaxLength(500)]
    [DisplayName("中文地址")]
    public string Address { get; set; }

    /// <summary>
    /// 取得或设置英文地址。
    /// </summary>
    [MaxLength(500)]
    [DisplayName("英文地址")]
    public string AddressEn { get; set; }

    /// <summary>
    /// 取得或设置联系人。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("联系人")]
    public string Contact { get; set; }

    /// <summary>
    /// 取得或设置联系人电话。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("联系电话")]
    public string Phone { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置系统信息。
    /// </summary>
    [DisplayName("系统信息")]
    public string SystemData { get; set; }

    /// <summary>
    /// 取得或设置企业信息。
    /// </summary>
    [DisplayName("企业信息")]
    public string CompanyData { get; set; }
}