using WebSite.Data;

namespace WebSite;

class Config
{
    private Config() { }

    internal static string RootPath { get; set; }

    internal static void Initialize()
    {
        DocumentService.GetMenus();
    }
}