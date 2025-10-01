namespace Known.Entities;

/// <summary>
/// 系统用户实体类。
/// </summary>
[DisplayName("系统用户")]
public class SysUser : EntityBase
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    public SysUser()
    {
        Gender = "Male";
    }

    /// <summary>
    /// 取得或设置组织编码。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("组织编码")]
    public string OrgNo { get; set; }

    /// <summary>
    /// 取得或设置用户名。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(IsQuery = true, IsViewLink = true)]
    [Form(Row = 1, Column = 1)]
    [DisplayName("用户名")]
    public string UserName { get; set; }

    /// <summary>
    /// 取得或设置密码。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Form(Row = 1, Column = 2, Type = nameof(FieldType.Password))]
    [DisplayName("密码")]
    public string Password { get; set; }

    /// <summary>
    /// 取得或设置姓名。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(IsQuery = true)]
    [Form(Row = 2, Column = 1)]
    [DisplayName("姓名")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置英文名。
    /// </summary>
    [MaxLength(50)]
    [Form(Row = 2, Column = 2)]
    [DisplayName("英文名")]
    public string EnglishName { get; set; }

    /// <summary>
    /// 取得或设置性别。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Category(nameof(GenderType))]
    [Column(Width = 80)]
    [Form(Row = 3, Column = 1, Type = nameof(FieldType.RadioList))]
    [DisplayName("性别")]
    public string Gender { get; set; }

    /// <summary>
    /// 取得或设置固定电话。
    /// </summary>
    [MaxLength(50)]
    [RegularExpression(RegexPattern.Phone, ErrorMessage = "固定电话格式不正确！")]
    [Form(Row = 3, Column = 2)]
    [DisplayName("固定电话")]
    public string Phone { get; set; }

    /// <summary>
    /// 取得或设置移动电话。
    /// </summary>
    [MaxLength(50)]
    [RegularExpression(RegexPattern.Mobile, ErrorMessage = "移动电话格式不正确！")]
    [Column(Width = 120)]
    [Form(Row = 4, Column = 1)]
    [DisplayName("移动电话")]
    public string Mobile { get; set; }

    /// <summary>
    /// 取得或设置电子邮件。
    /// </summary>
    [MaxLength(50)]
    [RegularExpression(RegexPattern.Email, ErrorMessage = "电子邮件格式不正确！")]
    [Column(Width = 150)]
    [Form(Row = 4, Column = 2)]
    [DisplayName("电子邮件")]
    public string Email { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Required]
    [Column(Width = 80)]
    [Form(Row = 5, Column = 1, Type = nameof(FieldType.Switch))]
    [DisplayName("状态")]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column(Width = 200)]
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置首次登录时间。
    /// </summary>
    [DisplayName("首次登录时间")]
    public DateTime? FirstLoginTime { get; set; }

    /// <summary>
    /// 取得或设置首次登录IP。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("首次登录IP")]
    public string FirstLoginIP { get; set; }

    /// <summary>
    /// 取得或设置最近登录时间。
    /// </summary>
    [DisplayName("最近登录时间")]
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 取得或设置最近登录IP。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("最近登录IP")]
    public string LastLoginIP { get; set; }

    /// <summary>
    /// 取得或设置类型。
    /// </summary>
    [MaxLength(500)]
    [DisplayName("类型")]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置角色。
    /// </summary>
    [Column]
    [DisplayName("角色")]
    public string Role { get; set; }

    /// <summary>
    /// 取得或设置数据。
    /// </summary>
    [DisplayName("数据权限")]
    public string Data { get; set; }

    /// <summary>
    /// 取得或设置默认密码。
    /// </summary>
    public virtual string DefaultPassword { get; set; }

    /// <summary>
    /// 取得或设置用户关联的角色ID集合。
    /// </summary>
    [Category("Roles")]
    [DisplayName("角色")]
    public virtual string[] RoleIds { get; set; }

    /// <summary>
    /// 取得或设置系统角色代码表列表。
    /// </summary>
    public virtual List<CodeInfo> Roles { get; set; }

    /// <summary>
    /// 获取数据权限对象。
    /// </summary>
    /// <typeparam name="T">数据权限类型。</typeparam>
    /// <returns></returns>
    public T GetDataPurview<T>() => Utils.FromJson<T>(Data);

    /// <summary>
    /// 设置数据权限对象。
    /// </summary>
    /// <param name="data">数据权限对象。</param>
    public void SetDataPurview(object data) => Data = Utils.ToJson(data);
}