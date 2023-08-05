namespace WebSite.Data;

class FileService
{
    internal static Stream GetPdfStream()
    {
        var path = Path.Combine(AppConfig.RootPath, "files/demo.pdf");
        var bytes = File.ReadAllBytes(path);
        return new MemoryStream(bytes);
    }
}