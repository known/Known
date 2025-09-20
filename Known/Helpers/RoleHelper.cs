namespace Known.Helpers;

class RoleHelper
{
    private const string RoleId = "Role";
    private static List<MenuInfo> Roles { get; } = [];

    internal static void AddRole(Type item)
    {
        var role = item.GetCustomAttribute<RoleAttribute>();
        if (role == null)
            return;

        var target = Constants.Route;
        if (!Roles.Exists(d => d.Id == RoleId))
        {
            var route = new MenuInfo { Id = RoleId, ParentId = "0", Name = "组件", Icon = "block", Target = target, Sort = 998 };
            Roles.Add(route);
        }

        var info = new MenuInfo { Id = item.FullName, Name = role.Name, ParentId = RoleId, Target = target };
        AddActions(info, item);
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

    private static void AddActions(MenuInfo info, Type type)
    {
        var actions = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                          .Where(m => m.IsDefined(typeof(ActionAttribute), false))
                          .Select(m => new ActionInfo(m.Name))
                          .ToList();
        if (actions.Count > 0)
            info.Plugins.AddPlugin(new AutoPageInfo { Page = new PageInfo { Tools = actions } });
    }
}