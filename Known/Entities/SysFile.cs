using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known.Entities;

/// <summary>
/// 系统文件实体类。
/// </summary>
public class SysFile : EntityBase
{
    /// <summary>
    /// 取得或设置一级分类。
    /// </summary>
    [DisplayName("一级分类")]
    [Required]
    [MaxLength(50)]
    public string Category1 { get; set; }

    /// <summary>
    /// 取得或设置二级分类。
    /// </summary>
    [DisplayName("二级分类")]
    [MaxLength(50)]
    public string Category2 { get; set; }

    /// <summary>
    /// 取得或设置文件名称。
    /// </summary>
    [DisplayName("文件名称")]
    [Required]
    [MaxLength(250)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置文件类型。
    /// </summary>
    [DisplayName("文件类型")]
    [MaxLength(50)]
    public string Type { get; set; }

    /// <summary>
    /// 取得或设置文件路径。
    /// </summary>
    [DisplayName("文件路径")]
    [MaxLength(500)]
    public string Path { get; set; }

    /// <summary>
    /// 取得或设置文件大小。
    /// </summary>
    [DisplayName("文件大小")]
    [Required]
    public long Size { get; set; }

    /// <summary>
    /// 取得或设置原文件名。
    /// </summary>
    [DisplayName("原文件名")]
    [Required]
    [MaxLength(250)]
    public string SourceName { get; set; }

    /// <summary>
    /// 取得或设置扩展名。
    /// </summary>
    [DisplayName("扩展名")]
    [Required]
    [MaxLength(50)]
    public string ExtName { get; set; }

    /// <summary>
    /// 取得或设置备注。
    /// </summary>
    [DisplayName("备注")]
    [MaxLength(500)]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [DisplayName("业务ID")]
    [MaxLength(50)]
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置文件缩略图路径。
    /// </summary>
    [DisplayName("文件缩略图路径")]
    [MaxLength(500)]
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