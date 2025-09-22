namespace Known.Helpers;

class RoleHelper
{
    private const string RoleId = "Role";
    private static List<MenuInfo> Roles { get; } = [];

    internal static void AddRole(Type type, RoleAttribute role)
    {
        var target = Constants.Route;
        if (!Roles.Exists(d => d.Id == RoleId))
        {
            var route = new MenuInfo { Id = RoleId, ParentId = "0", Name = "组件", Icon = "block", Target = target, Sort = 998 };
            Roles.Add(route);
        }

        var info = new MenuInfo { Id = type.FullName, Name = role.Name, Icon = "file", ParentId = RoleId, Target = target };
        var table = AppData.CreateAutoPage(type);
        if (table != null)
            info.Plugins.AddPlugin(table);
        Roles.Add(info);
    }

    internal static void AddTo(List<MenuInfo> modules)
    {
        if (Roles.Count == 0)
            return;

        var items = Roles.Where(d => !modules.Exists(m => m.Id == d.Id)).ToList();
        if (items != null && items.Count > 0)
            modules.AddRange(items);
    }
}