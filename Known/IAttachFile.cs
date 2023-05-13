namespace Known;

public interface IAttachFile
{
    long Length { get; }
    string FileName { get; }

    byte[] GetBytes();
    Stream GetStream();
    void Save(string path);
    Task SaveAsync(string path);
}