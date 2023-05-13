namespace Known.Entities;

/// <summary>
/// 系统设置实体类。
/// </summary>
public class SysSetting : EntityBase
{
    /// <summary>
    /// 取得或设置业务类型。
    /// </summary>
    [Column("业务类型", "", true, "1", "50")]
    public string BizType { get; set; }

    /// <summary>
    /// 取得或设置业务名称。
    /// </summary>
    [Column("业务名称", "", false, "1", "250")]
    public string BizName { get; set; }

    /// <summary>
    /// 取得或设置业务数据。
    /// </summary>
    [Column("业务数据", "")]
    public string BizData { get; set; }

    public T DataAs<T>() => Utils.FromJson<T>(BizData);
}