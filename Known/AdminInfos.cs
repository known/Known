namespace Known;

/// <summary>
/// 数据字典信息类。
/// </summary>
[DisplayName("数据字典")]
public class DictionaryInfo
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    public DictionaryInfo()
    {
        Id = Utils.GetNextId();
        Enabled = true;
    }

    /// <summary>
    /// 取得或设置实体ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置类别。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column]
    [DisplayName("类别")]
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置类别名称。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("类别名称")]
    public string CategoryName { get; set; }

    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column(IsQuery = true, IsViewLink = true)]
    [Form]
    [DisplayName("代码")]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [MaxLength(150)]
    [Column(IsQuery = true)]
    [Form]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置顺序。
    /// </summary>
    [Required]
    [Column]
    [Form]
    [DisplayName("顺序")]
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Required]
    [Column]
    [Form(Type = nameof(FieldType.Switch))]
    [DisplayName("状态")]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column]
    [Form(Type = nameof(FieldType.TextArea))]
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置子字典。
    /// </summary>
    [DisplayName("子字典")]
    public string Child { get; set; }
}

/// <summary>
/// 组织架构信息类。
/// </summary>
[DisplayName("组织架构")]
public class OrganizationInfo
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    public OrganizationInfo()
    {
        Id = Utils.GetNextId();
    }

    /// <summary>
    /// 取得或设置实体ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置上级组织。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("上级组织")]
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置编码。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(IsViewLink = true)]
    [Form]
    [DisplayName("编码")]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column]
    [Form]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置管理者。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("管理者")]
    public string ManagerId { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column]
    [Form(Type = nameof(FieldType.TextArea))]
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置上级组织名称。
    /// </summary>
    public string ParentName { get; set; }
}

/// <summary>
/// 角色信息类。
/// </summary>
[DisplayName("系统角色")]
public class RoleInfo
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    public RoleInfo()
    {
        Id = Utils.GetNextId();
        Enabled = true;
    }

    /// <summary>
    /// 取得或设置实体ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(IsQuery = true, IsViewLink = true)]
    [Form]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Required]
    [Column]
    [Form(Type = nameof(FieldType.Switch))]
    [DisplayName("状态")]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column]
    [Form(Type = nameof(FieldType.TextArea))]
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置角色关联的模块列表。
    /// </summary>
    public List<ModuleInfo> Modules { get; set; } = [];

    /// <summary>
    /// 取得或设置角色关联的菜单ID列表。
    /// </summary>
    public List<string> MenuIds { get; set; } = [];
}

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
        Gender = "Female";
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
    [Column]
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
    [Column]
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
    [Column]
    [Form(Row = 4, Column = 1)]
    [DisplayName("移动电话")]
    public new string Mobile { get; set; }

    /// <summary>
    /// 取得或设置用户Email。
    /// </summary>
    [MaxLength(50)]
    [RegularExpression(RegexPattern.Email, ErrorMessage = "电子邮件格式不正确！")]
    [Column]
    [Form(Row = 4, Column = 2)]
    [DisplayName("电子邮件")]
    public new string Email { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Required]
    [Column]
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
    [MaxLength(500)]
    [Column]
    [DisplayName("角色")]
    public new string Role { get; set; }

    /// <summary>
    /// 取得或设置用户关联的角色ID集合。
    /// </summary>
    [Category("Roles")]
    public string[] RoleIds { get; set; }
    // 取得或设置用户关联的数据权限ID集合。
    //public string[] DataIds { get; set; }

    /// <summary>
    /// 取得或设置系统角色代码表列表。
    /// </summary>
    public List<CodeInfo> Roles { get; set; }
    //public List<CodeInfo> Datas { get; set; }
}

/// <summary>
/// 日志类型。
/// </summary>
public enum LogType
{
    /// <summary>
    /// 登录。
    /// </summary>
    Login,
    /// <summary>
    /// APP登录。
    /// </summary>
    AppLogin,
    /// <summary>
    /// 退出。
    /// </summary>
    Logout,
    /// <summary>
    /// 页面访问。
    /// </summary>
    Page,
    /// <summary>
    /// 操作。
    /// </summary>
    Operate,
    /// <summary>
    /// 其他。
    /// </summary>
    Other
}

/// <summary>
/// 日志目标枚举类。
/// </summary>
public enum LogTarget
{
    /// <summary>
    /// JSON序列化。
    /// </summary>
    JSON,
    /// <summary>
    /// 前端。
    /// </summary>
    FrontEnd,
    /// <summary>
    /// 后端。
    /// </summary>
    BackEnd
}

/// <summary>
/// 系统日志信息类。
/// </summary>
[DisplayName("系统日志")]
public class LogInfo
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    public LogInfo()
    {
        Id = Utils.GetNextId();
    }

    /// <summary>
    /// 取得或设置实体ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置操作类型。
    /// </summary>
    [Category(nameof(LogType))]
    [Required]
    [MaxLength(50)]
    [Column(IsQuery = true, Type = FieldType.Select)]
    [DisplayName("操作类型")]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置操作对象。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column]
    [DisplayName("操作对象")]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置操作内容。
    /// </summary>
    [Column(IsQuery = true)]
    [DisplayName("操作内容")]
    public string Content { get; set; }

    /// <summary>
    /// 取得或设置实体创建人。
    /// </summary>
    [MaxLength(50)]
    [Column]
    public string CreateBy { get; set; }

    /// <summary>
    /// 取得或设置实体创建时间。
    /// </summary>
    [Column(IsQuery = true)]
    public DateTime? CreateTime { get; set; }
}