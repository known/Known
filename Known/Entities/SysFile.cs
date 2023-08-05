namespace Known.Entities;

/// <summary>
/// 系统文件实体类。
/// </summary>
public class SysFile : EntityBase
{
    /// <summary>
    /// 取得或设置一级分类。
    /// </summary>
    [Column("一级分类", "", true, "1", "50")]
    public string Category1 { get; set; }

    /// <summary>
    /// 取得或设置二级分类。
    /// </summary>
    [Column("二级分类", "", false, "1", "50")]
    public string Category2 { get; set; }

    /// <summary>
    /// 取得或设置文件名称。
    /// </summary>
    [Column("文件名称", "", true, "1", "250")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置文件类型。
    /// </summary>
    [Column("文件类型", "", false, "1", "50")]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置文件路径。
    /// </summary>
    [Column("文件路径", "", true, "1", "500")]
    public string Path { get; set; }

    /// <summary>
    /// 取得或设置文件大小。
    /// </summary>
    [Column("文件大小", "", true)]
    public long Size { get; set; }

    /// <summary>
    /// 取得或设置原文件名。
    /// </summary>
    [Column("原文件名", "", true, "1", "250")]
    public string SourceName { get; set; }

    /// <summary>
    /// 取得或设置扩展名。
    /// </summary>
    [Column("扩展名", "", true, "1", "50")]
    public string ExtName { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [Column("备注", "", false, "1", "500")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [Column("业务ID", "", false, "1", "50", IsGrid = false)]
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置文件缩略图路径。
    /// </summary>
    [Column("文件缩略图路径", "", false, "1", "500")]
    public string ThumbPath { get; set; }

    public virtual bool IsWeb => Category1 == "WWW";

    public virtual string Url
    {
        get
        {
            var path = ThumbPath;
            if (string.IsNullOrWhiteSpace(path))
                path = Path;

            return GetFileUrl(path, IsWeb);
        }
    }

    public virtual FileUrlInfo FileUrl
    {
        get
        {
            return new FileUrlInfo
            {
                FileName = Name,
                ThumbnailUrl = GetFileUrl(ThumbPath, IsWeb),
                OriginalUrl = GetFileUrl(Path, IsWeb)
            };
        }
    }

    private static string GetFileUrl(string filePath, bool isWeb = false)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return string.Empty;

        var path = filePath.Replace("\\", "/");
        return isWeb ? $"Files/{path}" : $"UploadFiles/{path}";
    }
}