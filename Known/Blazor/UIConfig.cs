namespace Known.Blazor;

public class UIConfig
{
    private UIConfig() { }

    public static string SysModuleUrl { get; set; } = "/sys/modules";
    public static string FillHeightScript { get; set; }
    public static List<ActionInfo> Sizes { get; set; } = [];
    public static Dictionary<string, List<string>> Icons { get; set; } = [];

    internal static List<MenuInfo> Menus { get; } = [];

    internal static void SetMenu(MenuInfo info)
    {
        if (!Menus.Exists(m => m.Url == info.Url))
            Menus.Add(info);
    }
}