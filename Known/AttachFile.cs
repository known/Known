using Known.Entities;
using Microsoft.AspNetCore.Components.Forms;

namespace Known;

public class AttachFile
{
    private readonly IAttachFile file;

    internal AttachFile(IAttachFile file, UserInfo user, string typePath = null, string timePath = null)
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
            FilePath = Path.Combine(filePath, fileName);
        else
            FilePath = Path.Combine(filePath, timePath, fileName);
    }

    //internal AttachFile(UploadInfo info, UserInfo user) : this(new ByteAttachFile(info?.Name, info?.Data), user) { }

    public static long MaxLength { get; set; } = 1024 * 1024 * 50;

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

class BlazorAttachFile : IAttachFile
{
    private readonly IBrowserFile file;
    private readonly byte[] bytes;

    internal BlazorAttachFile(IBrowserFile file)
    {
        this.file = file;
        Length = file.Size;
        FileName = file.Name;
    }

    internal BlazorAttachFile(IBrowserFile file, byte[] bytes) : this(file)
    {
        this.bytes = bytes;
    }

    public long Length { get; }
    public string FileName { get; }

    public byte[] GetBytes() => bytes;

    public Stream GetStream()
    {
        if (bytes != null)
            return new MemoryStream(bytes);

        return file.OpenReadStream(AttachFile.MaxLength);
    }

    public async Task SaveAsync(string path)
    {
        await using FileStream fs = new(path, FileMode.Create);
        await file.OpenReadStream(AttachFile.MaxLength).CopyToAsync(fs);
    }
}

//class FormAttachFile : IAttachFile
//{
//    private readonly IFormFile file;
//    private readonly byte[] bytes;

//    public FormAttachFile(IFormFile file)
//    {
//        this.file = file;
//        Length = file.Length;
//        FileName = file.FileName;
//    }

//    public FormAttachFile(IFormFile file, byte[] bytes) : this(file)
//    {
//        this.bytes = bytes;
//    }

//    public long Length { get; }
//    public string FileName { get; }

//    public byte[] GetBytes() => bytes;

//    public Stream GetStream()
//    {
//        if (bytes != null)
//            return new MemoryStream(bytes);

//        return file.OpenReadStream();
//    }

//    public async Task SaveAsync(string path)
//    {
//        await using FileStream fs = new(path, FileMode.Create);
//        await file.CopyToAsync(fs);
//    }
//}

class ByteAttachFile : IAttachFile
{
    private readonly byte[] buffer;

    public ByteAttachFile(string name, byte[] bytes)
    {
        buffer = bytes;
        if (bytes != null)
            Length = bytes.Length;
        FileName = name;
    }

    public long Length { get; }
    public string FileName { get; }

    public byte[] GetBytes() => buffer;
    public Stream GetStream() => new MemoryStream(buffer);
    public Task SaveAsync(string path) => File.WriteAllBytesAsync(path, buffer);
}