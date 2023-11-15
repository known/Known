using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.Entities;

/// <summary>
/// 企业信息实体类。
/// </summary>
public class SysCompany : EntityBase
{
    /// <summary>
    /// 取得或设置企业编码。
    /// </summary>
    [Column]
    [DisplayName("企业编码")]
    [MaxLength(50)]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置企业名称。
    /// </summary>
    [Column]
    [DisplayName("企业名称")]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置英文名称。
    /// </summary>
    [Column]
    [DisplayName("英文名称")]
    [MaxLength(250)]
    public string NameEn { get; set; }

    /// <summary>
    /// 取得或设置社会信用代码。
    /// </summary>
    [Column]
    [DisplayName("社会信用代码")]
    [MaxLength(18)]
    public string SccNo { get; set; }

    /// <summary>
    /// 取得或设置所属行业。
    /// </summary>
    [Column]
    [DisplayName("所属行业")]
    [MaxLength(50)]
    public string Industry { get; set; }

    /// <summary>
    /// 取得或设置所属区域。
    /// </summary>
    [Column]
    [DisplayName("所属区域")]
    [MaxLength(50)]
    public string Region { get; set; }

    /// <summary>
    /// 取得或设置中文地址。
    /// </summary>
    [Column]
    [DisplayName("中文地址")]
    [MaxLength(500)]
    public string Address { get; set; }

    /// <summary>
    /// 取得或设置英文地址。
    /// </summary>
    [Column]
    [DisplayName("英文地址")]
    [MaxLength(500)]
    public string AddressEn { get; set; }

    /// <summary>
    /// 取得或设置联系人。
    /// </summary>
    [Column]
    [DisplayName("联系人")]
    [MaxLength(50)]
    public string Contact { get; set; }

    /// <summary>
    /// 取得或设置联系人电话。
    /// </summary>
    [Column]
    [DisplayName("联系人电话")]
    [MaxLength(50)]
    public string Phone { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column]
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置系统信息。
    /// </summary>
    [Column]
    public string SystemData { get; set; }

    /// <summary>
    /// 取得或设置企业信息。
    /// </summary>
    [Column]
    public string CompanyData { get; set; }
}