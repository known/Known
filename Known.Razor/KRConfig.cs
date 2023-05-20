namespace Known.Razor;

public class KRConfig
{
    static KRConfig()
    {
        Assemblies = new List<Assembly>();
    }

    public static string ValidDate { get; set; }
    public static string AuthStatus { get; set; }
    public static List<Assembly> Assemblies { get; }
    public static MenuItem Home { get; set; }
    public static bool IsWeb { get; set; }
    public static List<MenuInfo> UserMenus { get; set; }

    private static List<Type> modelTypes;
    public static List<Type> GetModelTypes()
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