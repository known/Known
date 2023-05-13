namespace Known.Core;

public class FormAttachFile : IAttachFile
{
    private readonly IFormFile file;
    private readonly byte[] bytes;

    public FormAttachFile(IFormFile file)
    {
        this.file = file;
        Length = file.Length;
        FileName = file.FileName;
    }

    public FormAttachFile(IFormFile file, byte[] bytes) : this(file)
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

        return file.OpenReadStream();
    }

    public async void Save(string path)
    {
        await using FileStream fs = new(path, FileMode.Create);
        await file.CopyToAsync(fs);
    }

    public async Task SaveAsync(string path)
    {
        await using FileStream fs = new(path, FileMode.Create);
        await file.CopyToAsync(fs);
    }
}