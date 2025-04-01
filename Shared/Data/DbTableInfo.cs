namespace Known.Data;

/// <summary>
/// 数据库表信息类。
/// </summary>
public class DbTableInfo
{
    /// <summary>
    /// 取得或设置数据库表名。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置数据库表描述名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置数据库表字段列表。
    /// </summary>
    public List<FieldInfo> Fields { get; set; } = [];
}