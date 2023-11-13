using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.Entities;

/// <summary>
/// 系统用户实体类。
/// </summary>
public class SysUser : EntityBase
{
    public SysUser()
    {
        Enabled = true;
    }

    /// <summary>
    /// 取得或设置组织编码。
    /// </summary>
    [Column]
    [DisplayName("组织编码")]
    [MinLength(1), MaxLength(50)]
    public string OrgNo { get; set; }

    /// <summary>
    /// 取得或设置用户名。
    /// </summary>
    [Column(IsGrid = true, IsQuery = true, IsForm = true, IsViewLink = true)]
    [DisplayName("用户名")]
    [Required(ErrorMessage = "用户名不能为空！")]
    [MinLength(1), MaxLength(50)]
    public string UserName { get; set; }

    /// <summary>
    /// 取得或设置密码。
    /// </summary>
    [Column]
    [Required(ErrorMessage = "密码不能为空！")]
    [MinLength(1), MaxLength(50)]
    public string Password { get; set; }

    /// <summary>
    /// 取得或设置姓名。
    /// </summary>
    [Column(IsGrid = true, IsQuery = true, IsForm = true)]
    [DisplayName("姓名")]
    [Required(ErrorMessage = "姓名不能为空！")]
    [MinLength(1), MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置英文名。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("英文名")]
    [MinLength(1), MaxLength(50)]
    public string EnglishName { get; set; }

    /// <summary>
    /// 取得或设置性别。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("性别")]
    [Required(ErrorMessage = "性别不能为空！")]
    [MinLength(1), MaxLength(50)]
    public string Gender { get; set; }

    /// <summary>
    /// 取得或设置固定电话。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("固定电话")]
    [MinLength(1), MaxLength(50)]
    [Regex(RegexPattern.Phone, "固定电话格式不正确！")]
    public string Phone { get; set; }

    /// <summary>
    /// 取得或设置移动电话。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("移动电话")]
    [MinLength(1), MaxLength(50)]
    [Regex(RegexPattern.Mobile, "移动电话格式不正确！")]
    public string Mobile { get; set; }

    /// <summary>
    /// 取得或设置电子邮件。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("电子邮件")]
    [MinLength(1), MaxLength(50)]
    [Regex(RegexPattern.Email, "电子邮件格式不正确！")]
    public string Email { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("状态")]
    [Required(ErrorMessage = "状态不能为空！")]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column(IsForm = true)]
    [DisplayName("简介")]
    [MinLength(1), MaxLength(500)]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置首次登录时间。
    /// </summary>
    [Column(IsGrid = true)]
    [DisplayName("首次登录时间")]
    public DateTime? FirstLoginTime { get; set; }

    /// <summary>
    /// 取得或设置首次登录IP。
    /// </summary>
    [Column(IsGrid = true)]
    [DisplayName("首次登录IP")]
    [MinLength(1), MaxLength(50)]
    public string FirstLoginIP { get; set; }

    /// <summary>
    /// 取得或设置最近登录时间。
    /// </summary>
    [Column(IsGrid = true)]
    [DisplayName("最近登录时间")]
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 取得或设置最近登录IP。
    /// </summary>
    [Column(IsGrid = true)]
    [DisplayName("最近登录IP")]
    [MinLength(1), MaxLength(50)]
    public string LastLoginIP { get; set; }

    /// <summary>
    /// 取得或设置类型。
    /// </summary>
    [Column]
    [DisplayName("类型")]
    [MinLength(1), MaxLength(500)]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置角色。
    /// </summary>
    [Column(IsGrid = true, IsForm = true)]
    [DisplayName("角色")]
    [MinLength(1), MaxLength(500)]
    public string Role { get; set; }

    /// <summary>
    /// 取得或设置数据。
    /// </summary>
    [Column]
    [DisplayName("数据")]
    public string Data { get; set; }

    public virtual string Department { get; set; }
    [DisplayName("角色")]
    public virtual string[] Roles { get; set; }

    private bool isOperation = false;
    public virtual bool IsOperation
    {
        get
        {
            isOperation = Type == Constants.UTOperation;
            return isOperation;
        }
        set { isOperation = value; }
    }
}