using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.Winxins;

/// <summary>
/// 系统用户微信信息类。
/// </summary>
public class SysWinxin : EntityBase
{
    /// <summary>
    /// 取得或设置公众号ID。
    /// </summary>
    [DisplayName("公众号ID")]
    [MaxLength(50)]
    public string MPAppId { get; set; }

    /// <summary>
    /// 取得或设置系统用户ID。
    /// </summary>
    [DisplayName("系统用户ID")]
    [MaxLength(50)]
    public string UserId { get; set; }

    /// <summary>
    /// 取得或设置微信ID。
    /// </summary>
    [DisplayName("微信ID")]
    [MaxLength(50)]
    public string OpenId { get; set; }

    /// <summary>
    /// 取得或设置微信ID。
    /// </summary>
    [DisplayName("微信ID")]
    [MaxLength(50)]
    public string UnionId { get; set; }

    /// <summary>
    /// 取得或设置昵称。
    /// </summary>
    [DisplayName("昵称")]
    [MaxLength(50)]
    public string NickName { get; set; }

    /// <summary>
    /// 取得或设置性别。
    /// </summary>
    [DisplayName("性别")]
    [MaxLength(50)]
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
    [DisplayName("省份")]
    [MaxLength(50)]
    public string Province { get; set; }

    /// <summary>
    /// 取得或设置城市。
    /// </summary>
    [DisplayName("城市")]
    [MaxLength(50)]
    public string City { get; set; }

    /// <summary>
    /// 取得或设置头像。
    /// </summary>
    [DisplayName("头像")]
    [MaxLength(500)]
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