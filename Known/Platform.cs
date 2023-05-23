namespace Known;

public interface IPlatform
{
    string GetMacAddress();
    string GetIPAddress();
    void MakeThumbnail(Stream stream, string thumbnailPath, int width, int height);
}

public sealed class Platform
{
    private static readonly IPlatform platform = Container.Resolve<IPlatform>();

    private Platform() { }

    public static string GetMacAddress() => platform.GetMacAddress();
    public static string GetIPAddress() => platform.GetIPAddress();
    public static void MakeThumbnail(Stream stream, string thumbnailPath, int width, int height) => platform.MakeThumbnail(stream, thumbnailPath, width, height);
}