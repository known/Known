namespace WebSite.Data;

class MenuService
{
    internal static List<MenuItem> GetDocMenus()
    {
        return new List<MenuItem>
        {
            new MenuItem("概述")
            {
                Children = new List<MenuItem>
                {
                    new MenuItem("简介", "/doc/profile"),
                    new MenuItem("快速开始", "/doc"),
                }
            }
        };
    }
}