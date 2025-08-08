namespace Known;

/// <summary>
/// 用户表单信息类。
/// </summary>
[DisplayName("系统用户")]
public class UserDataInfo : UserInfo
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    public UserDataInfo()
    {
        Id = Utils.GetNextId();
        IsNew = true;
        Enabled = true;
        Gender = nameof(GenderType.Male);
    }

    /// <summary>
    /// 取得或设置是否是新增实体。
    /// </summary>
    public bool IsNew { get; set; }

    /// <summary>
    /// 取得或设置用户登录名。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(IsQuery = true, IsViewLink = true)]
    [Form(Row = 1, Column = 1)]
    [DisplayName("用户名")]
    public new string UserName { get; set; }

    /// <summary>
    /// 取得或设置密码。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Form(Row = 1, Column = 2, Type = nameof(FieldType.Password))]
    [DisplayName("密码")]
    public string Password { get; set; }

    /// <summary>
    /// 取得或设置用户姓名。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(IsQuery = true)]
    [Form(Row = 2, Column = 1)]
    [DisplayName("姓名")]
    public new string Name { get; set; }

    /// <summary>
    /// 取得或设置用户英文名。
    /// </summary>
    [MaxLength(50)]
    [Form(Row = 2, Column = 2)]
    [DisplayName("英文名")]
    public new string EnglishName { get; set; }

    /// <summary>
    /// 取得或设置用户性别。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Category(nameof(GenderType))]
    [Column(Width = 80)]
    [Form(Row = 3, Column = 1, Type = nameof(FieldType.RadioList))]
    [DisplayName("性别")]
    public new string Gender { get; set; }

    /// <summary>
    /// 取得或设置用户固定电话。
    /// </summary>
    [MaxLength(50)]
    [RegularExpression(RegexPattern.Phone, ErrorMessage = "固定电话格式不正确！")]
    [Form(Row = 3, Column = 2)]
    [DisplayName("固定电话")]
    public new string Phone { get; set; }

    /// <summary>
    /// 取得或设置用户移动电话。
    /// </summary>
    [MaxLength(50)]
    [RegularExpression(RegexPattern.Mobile, ErrorMessage = "移动电话格式不正确！")]
    [Column(Width = 120)]
    [Form(Row = 4, Column = 1)]
    [DisplayName("移动电话")]
    public new string Mobile { get; set; }

    /// <summary>
    /// 取得或设置用户Email。
    /// </summary>
    [MaxLength(50)]
    [RegularExpression(RegexPattern.Email, ErrorMessage = "电子邮件格式不正确！")]
    [Column(Width = 150)]
    [Form(Row = 4, Column = 2)]
    [DisplayName("电子邮件")]
    public new string Email { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Required]
    [Column(Width = 80)]
    [Form(Row = 5, Column = 1, Type = nameof(FieldType.Switch))]
    [DisplayName("状态")]
    public new bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置默认密码。
    /// </summary>
    public string DefaultPassword { get; set; }

    /// <summary>
    /// 取得或设置用户所属组织编码。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("所属组织")]
    public new string OrgNo { get; set; }

    /// <summary>
    /// 取得或设置用户角色。
    /// </summary>
    [Column]
    [DisplayName("角色")]
    public new string Role { get; set; }

    /// <summary>
    /// 取得或设置用户数据权限。
    /// </summary>
    [DisplayName("数据")]
    public string Data { get; set; }

    /// <summary>
    /// 取得或设置用户关联的角色ID集合。
    /// </summary>
    [Category("Roles")]
    [DisplayName("角色")]
    public string[] RoleIds { get; set; }
    // 取得或设置用户关联的数据权限ID集合。
    //public string[] DataIds { get; set; }

    /// <summary>
    /// 取得或设置系统角色代码表列表。
    /// </summary>
    public List<CodeInfo> Roles { get; set; }
    //public List<CodeInfo> Datas { get; set; }

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