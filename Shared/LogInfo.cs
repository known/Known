namespace Known;

/// <summary>
/// 日志类型。
/// </summary>
public enum LogType
{
    /// <summary>
    /// 注册。
    /// </summary>
    Register,
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
    /// 调试。
    /// </summary>
    Debug,
    /// <summary>
    /// Task。
    /// </summary>
    Task,
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
    [DisplayName("类型")]
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
    [DisplayName("创建人")]
    public string CreateBy { get; set; }

    /// <summary>
    /// 取得或设置实体创建时间。
    /// </summary>
    [Column(IsQuery = true)]
    [DisplayName("创建时间")]
    public DateTime? CreateTime { get; set; }
}