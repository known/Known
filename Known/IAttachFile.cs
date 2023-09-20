namespace Known;

public interface IAttachFile
{
    long Length { get; }
    string FileName { get; }

    byte[] GetBytes();
    Stream GetStream();
    Task SaveAsync(string path);
}