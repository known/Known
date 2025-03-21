namespace Known.Data;

/// <summary>
/// 数据库连接信息类。
/// </summary>
public class ConnectionInfo
{
    /// <summary>
    /// 取得或设置数据库连接名称。
    /// </summary>
    [Required]
    [Column(IsQuery = true, IsViewLink = true)]
    [Form]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置数据库类型。
    /// </summary>
    [Required]
    [Column]
    [Form(Type = nameof(FieldType.Select))]
    [DisplayName("类型")]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置数据库连接字符串。
    /// </summary>
    [Required]
    [Column]
    [Form(Type = nameof(FieldType.TextArea))]
    [DisplayName("连接字符串")]
    public string ConnectionString { get; set; }

    /// <summary>
    /// 获取默认连接字符串。
    /// </summary>
    /// <returns></returns>
    public string GetDefaultConnectionString()
    {
        var type = Utils.ConvertTo<DatabaseType>(Type);
        return DbUtils.GetConnectionString(type);
    }
}