namespace Known.Data;

/// <summary>
/// 数据库连接信息类。
/// </summary>
public class ConnectionInfo
{
    /// <summary>
    /// 取得或设置数据库连接名称。
    /// </summary>
    [Form, Required]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置数据库类型。
    /// </summary>
    [Required]
    [Form(Type = nameof(FieldType.Select))]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置数据库连接字符串。
    /// </summary>
    [Required]
    [Form(Type = nameof(FieldType.TextArea))]
    public string ConnectionString { get; set; }

    /// <summary>
    /// 获取默认连接字符串。
    /// </summary>
    /// <returns></returns>
    public string GetDefaultConnectionString()
    {
        var type = Utils.ConvertTo<DatabaseType>(Type);
        return type switch
        {
            DatabaseType.Access => "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Sample;Jet OLEDB:Database Password=xxx",
            DatabaseType.SQLite => "Data Source=..\\Sample.db",
            DatabaseType.SqlServer => "Data Source=localhost;Initial Catalog=Sample;User Id=xxx;Password=xxx;",
            DatabaseType.Oracle => "Data Source=localhost:1521/orcl;User Id=xxx;Password=xxx;",
            DatabaseType.MySql => "Data Source=localhost;port=3306;Initial Catalog=Sample;user id=xxx;password=xxx;Charset=utf8;SslMode=none;AllowZeroDateTime=True;",
            DatabaseType.PgSql => "Host=localhost;Port=5432;Database=Sample;Username=xxx;Password=xxx;",
            DatabaseType.DM => "Server=localhost;Schema=Sample;DATABASE=Sample;uid=xxx;pwd=xxx;",
            _ => string.Empty,
        };
    }
}