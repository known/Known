namespace Known.Entities;

/// <summary>
/// 代码生成实体类。
/// </summary>
[DisplayName("代码生成")]
public class SysCode : EntityBase
{
    /// <summary>
    /// 取得或设置代码。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("代码")]
    public string Code { get; set; }

    /// <summary>
    /// 取得或设置名称。
    /// </summary>
    [Required]
    [MaxLength(100)]
    [DisplayName("名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置配置数据。
    /// </summary>
    [DisplayName("配置数据")]
    public string Data { get; set; }
}