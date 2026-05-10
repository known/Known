namespace Known.Extensions;

/// <summary>
/// 附件扩展类。
/// </summary>
public static class AttachExtension
{
    /// <summary>
    /// 将附件数据转换成附件类的实例。
    /// </summary>
    /// <param name="file">附件信息。</param>
    /// <param name="form">附件表单信息。</param>
    /// <returns></returns>
    public static AttachFile ToAttachFile(this FileDataInfo file, FileFormInfo form)
    {
        return new AttachFile(file, form.BizType, form.BizPath) { Category2 = form.Category };
    }

    /// <summary>
    /// 获取附件字段的文件对象列表。
    /// </summary>
    /// <param name="files">表单的附件字典。</param>
    /// <param name="key">字段名。</param>
    /// <param name="bizType">业务类型。</param>
    /// <param name="bizPath">业务路径。</param>
    /// <returns>文件对象列表。</returns>
    public static List<AttachFile> GetAttachFiles(this Dictionary<string, List<FileDataInfo>> files, string key, string bizType, string bizPath = null)
    {
        return files?.GetAttachFiles(key, new FileFormInfo { BizType = bizType, BizPath = bizPath });
    }

    /// <summary>
    /// 获取附件字段的文件对象列表。
    /// </summary>
    /// <param name="files">表单的附件字典。</param>
    /// <param name="key">字段名。</param>
    /// <param name="form">附件表单对象。</param>
    /// <returns>文件对象列表。</returns>
    public static List<AttachFile> GetAttachFiles(this Dictionary<string, List<FileDataInfo>> files, string key, FileFormInfo form)
    {
        if (files == null || files.Count == 0)
            return null;

        if (!files.TryGetValue(key, out List<FileDataInfo> value))
            return null;

        var attaches = new List<AttachFile>();
        foreach (var item in value)
        {
            var attach = item.ToAttachFile(form);
            if (form.BizType != key) //支持一个表单多个附件字段
                attach.BizType = $"{form.BizType}.{key}";
            attaches.Add(attach);
        }
        return attaches;
    }

    /// <summary>
    /// 将附件数据转换成附件类的实例，该方法3.3.0版本之后已过时。
    /// </summary>
    /// <param name="file">附件信息。</param>
    /// <param name="user">当前用户信息。</param>
    /// <param name="form">附件表单信息。</param>
    /// <returns></returns>
    public static AttachFile ToAttachFile(this FileDataInfo file, UserInfo user, FileFormInfo form)
    {
        return file.ToAttachFile(form);
    }

    /// <summary>
    /// 获取附件字段的文件对象列表，该方法3.3.0版本之后已过时。
    /// </summary>
    /// <param name="files">表单的附件字典。</param>
    /// <param name="user">当前用户。</param>
    /// <param name="key">字段名。</param>
    /// <param name="bizType">业务类型。</param>
    /// <param name="bizPath">业务路径。</param>
    /// <returns>文件对象列表。</returns>
    public static List<AttachFile> GetAttachFiles(this Dictionary<string, List<FileDataInfo>> files, UserInfo user, string key, string bizType, string bizPath = null)
    {
        return files?.GetAttachFiles(key, bizType, bizPath);
    }

    /// <summary>
    /// 获取附件字段的文件对象列表，该方法3.3.0版本之后已过时。
    /// </summary>
    /// <param name="files">表单的附件字典。</param>
    /// <param name="user">当前用户。</param>
    /// <param name="key">字段名。</param>
    /// <param name="form">附件表单对象。</param>
    /// <returns>文件对象列表。</returns>
    public static List<AttachFile> GetAttachFiles(this Dictionary<string, List<FileDataInfo>> files, UserInfo user, string key, FileFormInfo form)
    {
        return files?.GetAttachFiles(key, form);
    }

    /// <summary>
    /// 获取附件物理路径。
    /// </summary>
    /// <param name="item">附件信息。</param>
    /// <returns></returns>
    internal static string GetLocalPath(this AttachInfo item)
    {
        if (item == null || string.IsNullOrWhiteSpace(item.Path))
            return string.Empty;

        return Config.GetUploadPath(item.Path, item.IsWeb);
    }

    /// <summary>
    /// 解析桌面端文件物理路径。
    /// </summary>
    /// <param name="filePath">文件路径或URL路径。</param>
    /// <returns></returns>
    internal static string ResolveLocalPath(this string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return null;

        if (Path.IsPathRooted(filePath))
            return filePath;

        var path = filePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        if (path.StartsWith($"UploadFiles{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            return Config.GetUploadPath(path[("UploadFiles".Length + 1)..]);

        if (path.StartsWith($"Files{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            return Config.GetUploadPath(path[("Files".Length + 1)..], true);

        return null;
    }

    /// <summary>
    /// 获取附件图片URL。
    /// </summary>
    /// <param name="item">附件信息。</param>
    /// <returns></returns>
    internal static string GetImageUrl(this AttachInfo item)
    {
        if (item == null)
            return string.Empty;

        if (Config.App.Type == AppType.Web)
            return item.FileUrl?.OriginalUrl;

        var path = item.GetLocalPath();
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            return string.Empty;

        var bytes = File.ReadAllBytes(path);
        return GetImageDataUrl(bytes, item.SourceName);
    }

    /// <summary>
    /// 异步获取附件图片URL。
    /// </summary>
    /// <param name="item">附件信息。</param>
    /// <returns></returns>
    internal static async Task<string> GetImageUrlAsync(this AttachInfo item)
    {
        if (item == null)
            return string.Empty;

        if (Config.App.Type == AppType.Web)
            return item.FileUrl?.OriginalUrl;

        var path = item.GetLocalPath();
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            return string.Empty;

        var bytes = await File.ReadAllBytesAsync(path);
        return GetImageDataUrl(bytes, item.SourceName);
    }

    /// <summary>
    /// 将图片字节数组转换为Data URL。
    /// </summary>
    /// <param name="bytes">图片字节。</param>
    /// <param name="fileName">文件名。</param>
    /// <returns></returns>
    private static string GetImageDataUrl(byte[] bytes, string fileName)
    {
        if (bytes == null || bytes.Length == 0)
            return string.Empty;

        var contentType = System.IO.Path.GetExtension(fileName)?.ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            _ => "image/jpeg"
        };
        return $"data:{contentType};base64,{Convert.ToBase64String(bytes)}";
    }
}