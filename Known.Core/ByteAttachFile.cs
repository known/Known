namespace Known.Core;

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
    public void Save(string path) => File.WriteAllBytes(path, buffer);
    public Task SaveAsync(string path) => File.WriteAllBytesAsync(path, buffer);
}