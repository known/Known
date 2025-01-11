namespace Known.Entities;

/// <summary>
/// 企业信息实体类。
/// </summary>
public class SysCompany : EntityBase
{
    /// <summary>
    /// 取得或设置企业编码。
    /// </summary>
    [MaxLength(50)]
    [Column]
    [Form]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置企业名称。
    /// </summary>
    [MaxLength(50)]
    [Column(IsQuery = true)]
    [Form]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置英文名称。
    /// </summary>
    [MaxLength(250)]
    [Column]
    [Form]
    public string NameEn { get; set; }

    /// <summary>
    /// 取得或设置社会信用代码。
    /// </summary>
    [MaxLength(18)]
    [Column]
    [Form]
    public string SccNo { get; set; }

    /// <summary>
    /// 取得或设置所属行业。
    /// </summary>
    [MaxLength(50)]
    [Column]
    [Form]
    public string Industry { get; set; }

    /// <summary>
    /// 取得或设置所属区域。
    /// </summary>
    [MaxLength(50)]
    [Column]
    [Form]
    public string Region { get; set; }

    /// <summary>
    /// 取得或设置中文地址。
    /// </summary>
    [MaxLength(500)]
    [Column]
    [Form]
    public string Address { get; set; }

    /// <summary>
    /// 取得或设置英文地址。
    /// </summary>
    [MaxLength(500)]
    [Column]
    [Form]
    public string AddressEn { get; set; }

    /// <summary>
    /// 取得或设置联系人。
    /// </summary>
    [MaxLength(50)]
    [Column]
    [Form]
    public string Contact { get; set; }

    /// <summary>
    /// 取得或设置联系人电话。
    /// </summary>
    [MaxLength(50)]
    [Column]
    [Form]
    public string Phone { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Form(Type = nameof(FieldType.TextArea))]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置系统信息。
    /// </summary>
    public string SystemData { get; set; }

    /// <summary>
    /// 取得或设置企业信息。
    /// </summary>
    public string CompanyData { get; set; }
}