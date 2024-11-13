namespace Known.Admin.Entities;

/// <summary>
/// 系统文件实体类。
/// </summary>
public class SysFile : EntityBase
{
    /// <summary>
    /// 取得或设置一级分类。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Category1 { get; set; }

    /// <summary>
    /// 取得或设置二级分类。
    /// </summary>
    [MaxLength(50)]
    public string Category2 { get; set; }

    /// <summary>
    /// 取得或设置文件名称。
    /// </summary>
    [Required]
    [MaxLength(250)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置文件类型。
    /// </summary>
    [MaxLength(50)]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置文件路径。
    /// </summary>
    [MaxLength(500)]
    public string Path { get; set; }

    /// <summary>
    /// 取得或设置文件大小。
    /// </summary>
    [Required]
    public long Size { get; set; }

    /// <summary>
    /// 取得或设置原文件名。
    /// </summary>
    [Required]
    [MaxLength(250)]
    public string SourceName { get; set; }

    /// <summary>
    /// 取得或设置扩展名。
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ExtName { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [MaxLength(250)]
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置文件缩略图路径。
    /// </summary>
    [MaxLength(500)]
    public string ThumbPath { get; set; }
}