using Known.Entities;

namespace Known;

public class AttachFile
{
    private readonly IAttachFile file;

    internal AttachFile(IAttachFile file, UserInfo user, string bizType = null, string bizPath = null)
    {
        this.file = file;
        User = user;
        Size = file.Length;
        var names = file.FileName.Replace(@"\", "/").Split('/');
        SourceName = names.Last();
        var index = SourceName.LastIndexOf('.');
        ExtName = SourceName.Substring(index);
        FileName = SourceName;
        var filePath = GetFilePath(user.CompNo, bizType);
        var fileId = Utils.GetGuid();
        var fileName = $"{user.UserName}_{fileId}{ExtName}";
        if (string.IsNullOrEmpty(bizPath))
            FilePath = Path.Combine(filePath, fileName);
        else
            FilePath = Path.Combine(filePath, bizPath, fileName);
    }

    internal UserInfo User { get; }
    internal bool IsWeb { get; set; }
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

    internal async Task SaveAsync()
    {
        var filePath = Config.GetUploadPath(FilePath, IsWeb);
        var info = new FileInfo(filePath);
        if (!info.Directory.Exists)
            info.Directory.Create();

        var bytes = file.GetBytes();
        if (bytes == null)
            await file.SaveAsync(filePath);
        else
            await File.WriteAllBytesAsync(filePath, bytes);
    }

    internal static void DeleteFile(SysFile file)
    {
        var path = Config.GetUploadPath(file.Path);
        Utils.DeleteFile(path);
    }

    internal static void DeleteFile(string filePath)
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

public interface IAttachFile
{
    long Length { get; }
    string FileName { get; }

    byte[] GetBytes();
    Stream GetStream();
    Task SaveAsync(string path);
}