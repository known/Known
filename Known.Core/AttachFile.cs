namespace Known.Core;

public class AttachFile
{
    private readonly IAttachFile file;

    public AttachFile(IAttachFile file, UserInfo user, string typePath = null, string timePath = null)
    {
        this.file = file;
        User = user;
        Size = file.Length;
        var names = file.FileName.Replace(@"\", "/").Split('/');
        SourceName = names.Last();
        var index = SourceName.LastIndexOf('.');
        ExtName = SourceName.Substring(index);
        FileName = SourceName;
        var filePath = GetFilePath(user.CompNo, typePath);
        var fileId = Utils.GetGuid();
        var fileName = $"{user.UserName}_{fileId}{ExtName}";
        if (string.IsNullOrEmpty(timePath))
        {
            FilePath = Path.Combine(filePath, fileName);
            ThumbPath = Path.Combine(filePath, "Thumbnails", fileName);
        }
        else
        {
            FilePath = Path.Combine(filePath, timePath, fileName);
            ThumbPath = Path.Combine(filePath, timePath, "Thumbnails", fileName);
        }
    }

    internal AttachFile(UploadInfo info, UserInfo user) : this(new ByteAttachFile(info?.Name, info?.Data), user) { }

    internal UserInfo User { get; }
    internal bool IsWWW { get; set; }
    public long Size { get; }
    public string SourceName { get; }
    public string ExtName { get; }
    public string FileName { get; }
    public string FilePath { get; internal set; }
    public string ThumbPath { get; internal set; }
    public string BizId { get; set; }
    public string BizType { get; set; }
    public string Category1 { get; set; }
    public string Category2 { get; set; }

    internal async Task Save(bool isThumb)
    {
        var filePath = KCConfig.GetUploadPath(FilePath, IsWWW);
        var info = new FileInfo(filePath);
        if (!info.Directory.Exists)
            info.Directory.Create();

        if (!isThumb)
            ThumbPath = string.Empty;

        var bytes = file.GetBytes();
        if (bytes == null)
        {
            await file.SaveAsync(filePath);
            if (isThumb)
                SaveThumbnail(filePath);
        }
        else
        {
            await File.WriteAllBytesAsync(filePath, bytes);
            if (isThumb)
                SaveThumbnail(bytes);
        }
    }

    private void SaveThumbnail(string path)
    {
        var filePath = KCConfig.GetUploadPath(ThumbPath, IsWWW);
        var info = new FileInfo(filePath);
        if (!info.Directory.Exists)
            info.Directory.Create();

        using var stream = File.OpenRead(path);
        Platform.MakeThumbnail(stream, filePath, 100, 100);
    }

    private void SaveThumbnail(byte[] bytes)
    {
        var filePath = KCConfig.GetUploadPath(ThumbPath, IsWWW);
        var info = new FileInfo(filePath);
        if (!info.Directory.Exists)
            info.Directory.Create();

        var stream = new MemoryStream(bytes);
        Platform.MakeThumbnail(stream, filePath, 100, 100);
    }

    internal static void DeleteFile(SysFile file)
    {
        var path = KCConfig.GetUploadPath(file.Path, file.IsWWW);
        Utils.DeleteFile(path);
    }

    public static void DeleteFiles(List<SysFile> files) => files.ForEach(DeleteFile);

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