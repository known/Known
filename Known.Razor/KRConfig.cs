namespace Known.Razor;

public sealed class KRConfig
{
    private KRConfig() { }
    static KRConfig() => Assemblies = new List<Assembly>();

    internal static string ValidDate { get; set; }
    internal static string AuthStatus { get; set; }
    public static bool IsWeb { get; set; }
    public static bool IsProductKey { get; set; }
    public static bool IsEditCopyright { get; set; } = true;
    public static string Copyright { get; set; } = $"©2020-{DateTime.Now:yyyy} 普漫科技。保留所有权利。";
    public static string SoftTerms { get; set; } = "您对该软件的使用受您为获得该软件而签订的许可协议的条款和条件的约束。如果您是批量许可客户，则您对该软件的使用应受批量许可协议的约束。如果您未从普漫科技或其许可的分销商处获得该软件的有效许可，则不得使用该软件。";
    public static string AppJsPath { get; set; }
    public static List<Assembly> Assemblies { get; }
    public static MenuItem Home { get; set; }
    public static List<MenuInfo> UserMenus { get; set; }
    public static Action<IMyFlow> ShowMyFlow { get; set; }

    private static List<Type> modelTypes;
    internal static List<Type> GetModelTypes()
    {
        if (modelTypes != null)
            return modelTypes;

        var types = typeof(Config).Assembly.GetTypes().ToList();
        if (Config.AppAssembly != null)
            types.AddRange(Config.AppAssembly.GetTypes());

        modelTypes = types.Where(t => t.BaseType == typeof(EntityBase) || t.BaseType == typeof(ModelBase)).ToList();
        return modelTypes;
    }

    public static List<MenuItem> GetMenus(List<string> menuIds)
    {
        if (menuIds == null || menuIds.Count == 0)
            return new List<MenuItem>();

        var menus = new List<MenuItem>();
        foreach (var menuId in menuIds)
        {
            var menu = UserMenus.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.Target) && m.Name == menuId);
            if (menu != null)
                menus.Add(MenuItem.From(menu));
        }
        return menus;
    }

    public static Type GetType(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            return null;

        var type = typeof(KRContext).Assembly.GetType(typeName);
        if (type != null)
            return type;

        if (Home != null && Home.ComType != null)
        {
            type = Home.ComType.Assembly.GetType(typeName);
            if (type != null)
                return type;
        }

        foreach (var asm in Assemblies)
        {
            type = asm.GetType(typeName);
            if (type != null)
                return type;
        }

        return null;
    }
}