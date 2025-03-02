namespace Known;

/// <summary>
/// 数据字典信息类。
/// </summary>
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
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置类别名称。
    /// </summary>
    [MaxLength(50)]
    public string CategoryName { get; set; }

    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    [Required]
    [MaxLength(100)]
    [Column(IsQuery = true, IsViewLink = true)]
    [Form]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [MaxLength(150)]
    [Column(IsQuery = true)]
    [Form]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置顺序。
    /// </summary>
    [Required]
    [Column]
    [Form]
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Required]
    [Column]
    [Form]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column]
    [Form(Type = nameof(FieldType.TextArea))]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置子字典。
    /// </summary>
    public string Child { get; set; }
}

/// <summary>
/// 组织架构信息类。
/// </summary>
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
    public string ParentId { get; set; }

    /// <summary>
    /// 取得或设置编码。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(IsViewLink = true)]
    [Form]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column]
    [Form]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置管理者。
    /// </summary>
    [MaxLength(50)]
    public string ManagerId { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column]
    [Form(Type = nameof(FieldType.TextArea))]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置上级组织名称。
    /// </summary>
    public string ParentName { get; set; }
}

/// <summary>
/// 角色信息类。
/// </summary>
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
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Required]
    [Column]
    [Form]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column]
    [Form(Type = nameof(FieldType.TextArea))]
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
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置操作对象。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column]
    public string Target { get; set; }

    /// <summary>
    /// 取得或设置操作内容。
    /// </summary>
    [Column(IsQuery = true)]
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