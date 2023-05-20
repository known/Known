namespace Known.Razor;

public class KRContext : Context
{
    private MenuItem current;

    public KRContext() => Check = new CheckInfo();

    internal static Action<MenuItem> OnNavigate { get; set; }

    public UIService UI { get; set; }
    public CheckInfo Check { get; set; }

    public void Back()
    {
        if (current == null || current.Previous == null)
            return;

        current = current.Previous;
        OnNavigate?.Invoke(current);
    }

    public void NavigateToHome() => Navigate(KRConfig.Home);

    public void NavigateToAccount()
    {
        var menu = new MenuItem("个人中心", "fa fa-user", typeof(SysAccount), "当前用户个人中心和安全设置。");
        Navigate(menu);
    }

    public void Navigate(MenuItem menu, Dictionary<string, object> prevParms = null)
    {
        if (menu == null)
            return;

        menu.Previous = current;
        if (menu.Previous != null && prevParms != null)
            menu.Previous.ComParameters = prevParms;
        current = menu;
        OnNavigate?.Invoke(current);
    }

    public void Navigate<T>()
    {
        var type = typeof(T);
        var target = type.FullName;
        var menu = KRConfig.UserMenus.FirstOrDefault(m => m.Target == target);
        if (menu == null)
            return;

        var item = MenuItem.From(menu);
        item.ComType = type;
        Navigate(item);
    }
}