namespace Known.Helpers;

class RoleHelper
{
    private const string RoleId = "Role";

    internal static void AddRole(Type type, RoleAttribute role)
    {
        var target = Constants.Route;
        if (!DataHelper.Roles.Exists(d => d.Id == RoleId))
        {
            var route = new MenuInfo { Id = RoleId, ParentId = "0", Name = "组件", Icon = "block", Target = target, Sort = 998 };
            DataHelper.Roles.Add(route);
        }

        var info = new MenuInfo { Id = type.FullName, Name = role.Name, Icon = "file", ParentId = RoleId, Target = target };
        var table = RouteHelper.CreateAutoPage(type);
        if (table != null)
            info.Plugins.AddPlugin(table);
        DataHelper.Roles.Add(info);
    }
}