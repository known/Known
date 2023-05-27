using WebSite.Data;

namespace WebSite.Core;

class Config
{
    private Config() { }

    internal static string RootPath { get; set; }

    internal static void Initialize()
    {
        DocService.GetDocMenus();
    }
}