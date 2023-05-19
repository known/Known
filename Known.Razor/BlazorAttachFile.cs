namespace Known.Razor;

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

        return file.OpenReadStream(Upload.MaxLength);
    }

    public async void Save(string path)
    {
        await using FileStream fs = new(path, FileMode.Create);
        await file.OpenReadStream(Upload.MaxLength).CopyToAsync(fs);
    }

    public async Task SaveAsync(string path)
    {
        await using FileStream fs = new(path, FileMode.Create);
        await file.OpenReadStream(Upload.MaxLength).CopyToAsync(fs);
    }
}