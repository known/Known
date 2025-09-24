namespace Known.Weixins;

/// <summary>
/// 系统用户微信信息类。
/// </summary>
[DisplayName("系统用户微信")]
public class SysWeixin : EntityBase
{
    /// <summary>
    /// 取得或设置公众号ID。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("公众号ID")]
    public string MPAppId { get; set; }

    /// <summary>
    /// 取得或设置系统用户ID。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("系统用户ID")]
    public string UserId { get; set; }

    /// <summary>
    /// 取得或设置微信ID。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("微信ID")]
    public string OpenId { get; set; }

    /// <summary>
    /// 取得或设置微信ID。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("微信ID")]
    public string UnionId { get; set; }

    /// <summary>
    /// 取得或设置昵称。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("昵称")]
    public string NickName { get; set; }

    /// <summary>
    /// 取得或设置性别。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("性别")]
    public string Sex { get; set; }

    /// <summary>
    /// 取得或设置国家。
    /// </summary>
    [DisplayName("国家")]
    [MaxLength(50)]
    public string Country { get; set; }

    /// <summary>
    /// 取得或设置省份。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("省份")]
    public string Province { get; set; }

    /// <summary>
    /// 取得或设置城市。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("城市")]
    public string City { get; set; }

    /// <summary>
    /// 取得或设置头像。
    /// </summary>
    [MaxLength(500)]
    [DisplayName("头像")]
    public string HeadImgUrl { get; set; }

    /// <summary>
    /// 取得或设置特权信息。
    /// </summary>
    [DisplayName("特权信息")]
    public string Privilege { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [DisplayName("备注")]
    public string Note { get; set; }
}