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
    [MaxLength(50)]
    public string OrgNo { get; set; }

    [Column]
    [DisplayName("部门")]
    public virtual string Department { get; set; }

    /// <summary>
    /// 取得或设置用户名。
    /// </summary>
    [Column(IsGrid = true, IsQuery = true, IsForm = true, IsViewLink = true, Row = 1, Column = 1)]
    [DisplayName("用户名")]
    [Required(ErrorMessage = "用户名不能为空！")]
    [MaxLength(50)]
    public string UserName { get; set; }

    /// <summary>
    /// 取得或设置密码。
    /// </summary>
    [Column]
    [MaxLength(50)]
    public string Password { get; set; }

    /// <summary>
    /// 取得或设置姓名。
    /// </summary>
    [Column(IsGrid = true, IsQuery = true, IsForm = true, Row = 1, Column = 2)]
    [DisplayName("姓名")]
    [Required(ErrorMessage = "姓名不能为空！")]
    [MaxLength(50)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置英文名。
    /// </summary>
    [Column(IsGrid = true, IsForm = true, Row = 2, Column = 1)]
    [DisplayName("英文名")]
    [MaxLength(50)]
    public string EnglishName { get; set; }

    /// <summary>
    /// 取得或设置性别。
    /// </summary>
    [Column(IsGrid = true, IsForm = true, CodeType = "男,女", Row = 2, Column = 2)]
    [DisplayName("性别")]
    [Required(ErrorMessage = "性别不能为空！")]
    [MaxLength(50)]
    public string Gender { get; set; }

    /// <summary>
    /// 取得或设置固定电话。
    /// </summary>
    [Column(IsGrid = true, IsForm = true, Row = 3, Column = 1)]
    [DisplayName("固定电话")]
    [MaxLength(50)]
    [Regex(RegexPattern.Phone, "固定电话格式不正确！")]
    public string Phone { get; set; }

    /// <summary>
    /// 取得或设置移动电话。
    /// </summary>
    [Column(IsGrid = true, IsForm = true, Row = 3, Column = 2)]
    [DisplayName("移动电话")]
    [MaxLength(50)]
    [Regex(RegexPattern.Mobile, "移动电话格式不正确！")]
    public string Mobile { get; set; }

    /// <summary>
    /// 取得或设置电子邮件。
    /// </summary>
    [Column(IsGrid = true, IsForm = true, Row = 4, Column = 1)]
    [DisplayName("电子邮件")]
    [MaxLength(50)]
    [Regex(RegexPattern.Email, "电子邮件格式不正确！")]
    public string Email { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Column(IsGrid = true, IsForm = true, Row = 4, Column = 2)]
    [DisplayName("状态")]
    [Required(ErrorMessage = "状态不能为空！")]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column]
    [DisplayName("简介")]
    [MaxLength(500)]
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
    [MaxLength(50)]
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
    [MaxLength(50)]
    public string LastLoginIP { get; set; }

    /// <summary>
    /// 取得或设置类型。
    /// </summary>
    [Column]
    [DisplayName("类型")]
    [MaxLength(500)]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置角色。
    /// </summary>
    [Column(IsGrid = true)]
    [DisplayName("角色")]
    [MaxLength(500)]
    public string Role { get; set; }

    /// <summary>
    /// 取得或设置数据。
    /// </summary>
    [Column]
    [DisplayName("数据")]
    public string Data { get; set; }

    [Column(IsForm = true, Row = 5, Column = 1)]
    [DisplayName("角色")]
    public virtual string[] RoleIds { get; set; }
    [DisplayName("数据")]
    public virtual string[] DataIds { get; set; }

    public virtual List<CodeInfo> Roles { get; set; }
    public virtual List<CodeInfo> Datas { get; set; }

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