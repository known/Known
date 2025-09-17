namespace Known;

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
    public AttachFile(FileDataInfo file)
    {
        this.file = file;
        Size = file.Size;
        SourceName = file.Name.Replace(@"\", "/").Split('/').Last();
        ExtName = SourceName[SourceName.LastIndexOf('.')..];
        FileName = SourceName;
    }

    /// <summary>
    /// 构造函数，创建一个附件类的实例。
    /// </summary>
    /// <param name="file">上传的附件信息。</param>
    /// <param name="bizType">附件业务类型。</param>
    /// <param name="bizPath">附件业务存储路径。</param>
    public AttachFile(FileDataInfo file, string bizType, string bizPath = null) : this(file)
    {
        BizType = bizType ?? "Files";
        var fileId = Utils.GetGuid();
        var fileName = $"{fileId}{ExtName}";
        fileName = fileName.Replace(" ", "");
        if (string.IsNullOrEmpty(bizPath))
            FilePath = Path.Combine(BizType, fileName);
        else
            FilePath = Path.Combine(BizType, bizPath, fileName);
    }

    /// <summary>
    /// 构造函数，创建一个附件类的实例，该方法3.3.0版本之后已过时。
    /// </summary>
    /// <param name="file">上传的附件信息。</param>
    /// <param name="user">当前用户信息。</param>
    /// <param name="bizType">附件业务类型。</param>
    /// <param name="bizPath">附件业务存储路径。</param>
    public AttachFile(FileDataInfo file, UserInfo user, string bizType = null, string bizPath = null)
        : this(file, bizType, bizPath) { }

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
    /// 取得或设置备注。
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// 异步保存附件。
    /// </summary>
    /// <returns></returns>
    public Task SaveAsync()
    {
        var filePath = Config.GetUploadPath(FilePath, IsWeb);
        return SaveAsync(filePath);
    }

    /// <summary>
    /// 异步保存附件。
    /// </summary>
    /// <param name="filePath">文件路径。</param>
    /// <returns></returns>
    public async Task SaveAsync(string filePath)
    {
        var info = new FileInfo(filePath);
        info.Directory?.Create();

        if (file.Bytes != null && file.Bytes.Length > 0)
        {
            //await File.WriteAllBytesAsync(filePath, file.Bytes);
            await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await fs.WriteAsync(file.Bytes.AsMemory(0, file.Bytes.Length));
        }
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

    /// <summary>
    /// 物理删除附件。
    /// </summary>
    /// <param name="filePaths">附件路径列表。</param>
    public static void DeleteFiles(List<string> filePaths)
    {
        filePaths.ForEach(DeleteFile);
    }
}