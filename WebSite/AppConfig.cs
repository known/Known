using WebSite.Data;

namespace WebSite;

class AppConfig
{
    private AppConfig() { }

    internal static string RootPath { get; set; }

    internal static void Initialize()
    {
        DocumentService.GetMenus();
    }
}