namespace Known.Internals;

internal class BrowserFile(byte[] data, string name, string contentType) : IBrowserFile
{
    private readonly byte[] _data = data;
    private readonly string _name = name;
    private readonly string _contentType = contentType;

    public string Name => _name;
    public DateTimeOffset LastModified { get; } = DateTimeOffset.Now;
    public long Size { get; } = data.Length;
    public string ContentType => _contentType;

    public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
        => new MemoryStream(_data);
}