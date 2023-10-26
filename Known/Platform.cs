namespace Known;

public interface IPlatform
{
    string GetMacAddress();
    string GetIPAddress();
    void MakeThumbnail(Stream stream, string thumbnailPath, int width, int height);
}