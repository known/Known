namespace Known.Data;

/// <summary>
/// 系统数据库信息类。
/// </summary>
public class DatabaseInfo
{
    /// <summary>
    /// 取得或设置数据库连接名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置数据库类型。
    /// </summary>
    public DatabaseType DatabaseType { get; set; }

    /// <summary>
    /// 取得或设置数据库访问的ADO.NET提供者类型。
    /// </summary>
    public Type ProviderType { get; set; }

    /// <summary>
    /// 取得或设置数据库连接字符串。
    /// </summary>
    public string ConnectionString { get; set; }
}