namespace Known;

/// <summary>
/// 系统附件信息类。
/// </summary>
public class AttachInfo
{
    /// <summary>
    /// 取得或设置ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置创建时间。
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 取得或设置一级分类。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(IsQuery = true)]
    public string Category1 { get; set; }

    /// <summary>
    /// 取得或设置二级分类。
    /// </summary>
    [MaxLength(50)]
    [Column]
    public string Category2 { get; set; }

    /// <summary>
    /// 取得或设置文件名称。
    /// </summary>
    [Required]
    [MaxLength(250)]
    [Column(IsQuery = true)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置文件类型。
    /// </summary>
    [MaxLength(50)]
    [Column]
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
    [Column]
    public long Size { get; set; }

    /// <summary>
    /// 取得或设置原文件名。
    /// </summary>
    [Required]
    [MaxLength(250)]
    [Column]
    public string SourceName { get; set; }

    /// <summary>
    /// 取得或设置扩展名。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column]
    public string ExtName { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column]
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

    /// <summary>
    /// 取得是否是wwwroot附件。
    /// </summary>
    public virtual bool IsWeb => Category1 == "WWW";

    /// <summary>
    /// 取得附件URL。
    /// </summary>
    public virtual string Url
    {
        get
        {
            var path = ThumbPath;
            if (string.IsNullOrWhiteSpace(path))
                path = Path;

            return Config.GetFileUrl(path, IsWeb);
        }
    }

    /// <summary>
    /// 取得附件URL信息。
    /// </summary>
    public virtual FileUrlInfo FileUrl
    {
        get
        {
            return new FileUrlInfo
            {
                FileName = Name,
                ThumbnailUrl = Config.GetFileUrl(ThumbPath, IsWeb),
                OriginalUrl = Config.GetFileUrl(Path, IsWeb)
            };
        }
    }
}