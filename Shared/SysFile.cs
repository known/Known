namespace Known;

/// <summary>
/// 系统文件实体类。
/// </summary>
[DisplayName("系统文件")]
public class SysFile : EntityBase
{
    /// <summary>
    /// 取得或设置一级分类。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("一级分类")]
    public string Category1 { get; set; }

    /// <summary>
    /// 取得或设置二级分类。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("二级分类")]
    public string Category2 { get; set; }

    /// <summary>
    /// 取得或设置文件名称。
    /// </summary>
    [Required]
    [MaxLength(250)]
    [DisplayName("文件名称")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置文件类型。
    /// </summary>
    [MaxLength(50)]
    [DisplayName("文件类型")]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置文件路径。
    /// </summary>
    [MaxLength(500)]
    [DisplayName("文件路径")]
    public string Path { get; set; }

    /// <summary>
    /// 取得或设置文件大小。
    /// </summary>
    [Required]
    [DisplayName("文件大小")]
    public long Size { get; set; }

    /// <summary>
    /// 取得或设置原文件名。
    /// </summary>
    [Required]
    [MaxLength(250)]
    [DisplayName("源文件名")]
    public string SourceName { get; set; }

    /// <summary>
    /// 取得或设置扩展名。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [DisplayName("扩展名")]
    public string ExtName { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [MaxLength(250)]
    [DisplayName("业务ID")]
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置文件缩略图路径。
    /// </summary>
    [MaxLength(500)]
    [DisplayName("缩略图路径")]
    public string ThumbPath { get; set; }
}