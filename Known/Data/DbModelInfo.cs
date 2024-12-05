namespace Known.Data;

/// <summary>
/// 数据模型配置信息类，适用于EFCore配置模型。
/// </summary>
public class DbModelInfo
{
    /// <summary>
    /// 取得或设置实体类型。
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// 取得或设置主键字段列表。
    /// </summary>
    public List<string> Keys { get; set; }
}