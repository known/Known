namespace Known.Entities;

/// <summary>
/// 系统设置实体类。
/// </summary>
public class SysSetting : EntityBase
{
    /// <summary>
    /// 取得或设置业务类型。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string BizType { get; set; }

    /// <summary>
    /// 取得或设置业务名称。
    /// </summary>
    [MaxLength(250)]
    public string BizName { get; set; }

    /// <summary>
    /// 取得或设置业务数据。
    /// </summary>
    public string BizData { get; set; }

    /// <summary>
    /// 将业务数据JSON转换成泛型对象。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>泛型对象。</returns>
    public T DataAs<T>() => Utils.FromJson<T>(BizData);
}