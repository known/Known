namespace Known.Core;

/// <summary>
/// 系统附件类。
/// </summary>
public class AttachFile
{
    private readonly FileDataInfo file;

    /// <summary>
    /// 构造函数，创建一个附件类的实例。
    /// </summary>
    /// <param name="file">上传的附件信息。</param>
    /// <param name="user">当前用户信息。</param>
    /// <param name="bizType">附件业务类型。</param>
    /// <param name="bizPath">附件业务存储路径。</param>
    public AttachFile(FileDataInfo file, UserInfo user, string bizType = null, string bizPath = null)
    {
        this.file = file;
        Size = file.Size;
        var names = file.Name.Replace(@"\", "/").Split('/');
        SourceName = names.Last();
        var index = SourceName.LastIndexOf('.');
        ExtName = SourceName.Substring(index);
        FileName = SourceName;
        var filePath = GetFilePath(user.CompNo, bizType);
        var fileId = Utils.GetGuid();
        var fileName = $"{user.UserName}_{fileId}{ExtName}";
        fileName = fileName.Replace(" ", "");
        if (string.IsNullOrEmpty(bizPath))
            FilePath = Path.Combine(filePath, fileName);
        else
            FilePath = Path.Combine(filePath, bizPath, fileName);
    }

    internal bool IsWeb { get; set; }

    /// <summary>
    /// 取得附件大小。
    /// </summary>
    public long Size { get; }

    /// <summary>
    /// 取得附件原始文件名。
    /// </summary>
    public string SourceName { get; }

    /// <summary>
    /// 取得附件扩展名。
    /// </summary>
    public string ExtName { get; }

    /// <summary>
    /// 取得附件文件名，同原始文件名。
    /// </summary>
    public string FileName { get; }

    /// <summary>
    /// 取得附件路径。
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// 取得图片附件缩略图路径。
    /// </summary>
    public string ThumbPath { get; internal set; }

    /// <summary>
    /// 取得或设置附件关联的业务数据ID。
    /// </summary>
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置附件关联的业务类型。
    /// </summary>
    public string BizType { get; set; }

    /// <summary>
    /// 取得或设置附件类别1。
    /// </summary>
    public string Category1 { get; set; }

    /// <summary>
    /// 取得或设置附件类别2。
    /// </summary>
    public string Category2 { get; set; }

    /// <summary>
    /// 异步报错附件。
    /// </summary>
    /// <returns></returns>
    public async Task SaveAsync()
    {
        var filePath = Config.GetUploadPath(FilePath, IsWeb);
        var info = new FileInfo(filePath);
        if (!info.Directory.Exists)
            info.Directory.Create();

        if (file.Bytes != null)
            await File.WriteAllBytesAsync(filePath, file.Bytes);
    }

    /// <summary>
    /// 物理删除附件。
    /// </summary>
    /// <param name="file">附件对象。</param>
    public static void DeleteFile(SysFile file)
    {
        var path = Config.GetUploadPath(file.Path);
        Utils.DeleteFile(path);
    }

    /// <summary>
    /// 根据路径物理删除附件。
    /// </summary>
    /// <param name="filePath">附件路径。</param>
    public static void DeleteFile(string filePath)
    {
        var path = Config.GetUploadPath(filePath);
        Utils.DeleteFile(path);
    }

    private static string GetFilePath(string compNo, string type = null)
    {
        var filePath = compNo;

        if (!string.IsNullOrEmpty(type))
        {
            filePath += $@"\{type}";
        }

        return filePath;
    }
}