using Known;
using Known.Razor;

namespace KApp;

public class Client
{
    public static bool CheckIsAdmin(UserInfo user)
    {
        return user != null && user.Role == "管理员";
    }

    public static MenuItem[] GetMenus(UserInfo user)
    {
        var menus = new List<MenuItem>();
        menus.Add(new MenuItem("CustInfo", "新增客户", "fa fa-user-plus") { Color = "#0096dd" });
        menus.Add(new MenuItem("CustCheckQuery", "客户查重", "fa fa-search") { Color = "#00a0ff" });
        if (CheckIsAdmin(user))
        {
            menus.Add(new MenuItem("CustAdvQuery", "高级查询", "fa fa-search-plus") { Color = "#0066dd" });
            menus.Add(new MenuItem("SysUserList", "用户管理", "fa fa-user") { Color = "#eecc00" });
        }
        menus.Add(new MenuItem("", "功能待加", "fa fa-plus") { Color = "#00a65a" });
        return menus.ToArray();
    }
}
