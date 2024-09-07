namespace Known.Data;

/// <summary>
/// 数据库类型枚举。
/// </summary>
public enum DatabaseType
{
    /// <summary>
    /// Access数据库。
    /// </summary>
    Access,
    /// <summary>
    /// SQLite数据库。
    /// </summary>
    SQLite,
    /// <summary>
    /// SqlServer数据库。
    /// </summary>
    SqlServer,
    /// <summary>
    /// Oracle数据库。
    /// </summary>
    Oracle,
    /// <summary>
    /// MySql数据库。
    /// </summary>
    MySql,
    /// <summary>
    /// PostgreSQL数据库。
    /// </summary>
    PgSql,
    /// <summary>
    /// 国产达蒙数据库。
    /// </summary>
    DM,
    /// <summary>
    /// 其他数据库。
    /// </summary>
    Other
}