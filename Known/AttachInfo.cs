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
    public string Category1 { get; set; }

    /// <summary>
    /// 取得或设置二级分类。
    /// </summary>
    public string Category2 { get; set; }

    /// <summary>
    /// 取得或设置文件名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置文件类型。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置文件路径。
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 取得或设置文件大小。
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// 取得或设置原文件名。
    /// </summary>
    public string SourceName { get; set; }

    /// <summary>
    /// 取得或设置扩展名。
    /// </summary>
    public string ExtName { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
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