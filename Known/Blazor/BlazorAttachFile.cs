namespace Known.Blazor;

class BlazorAttachFile(IBrowserFile file) : IAttachFile
{
    private readonly IBrowserFile file = file;
    private byte[] bytes = null;

    public long Length { get; } = file.Size;
    public string FileName { get; } = file.Name;
    public byte[] Bytes => bytes;

    public async Task ReadAsync()
    {
        using var stream = new MemoryStream();
        await file.OpenReadStream(Config.App.UploadMaxSize).CopyToAsync(stream);
        bytes = stream.GetBuffer();
    }
}