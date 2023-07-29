namespace Known.Razor;

public class KRContext : Context
{
    private readonly MenuItem account = new("个人中心", "fa fa-user", typeof(SysAccount), "当前用户个人中心和安全设置。");
    private MenuItem current;

    public KRContext() => Check = new CheckInfo();

    internal static Action<MenuItem> OnNavigate { get; set; }
    internal CheckInfo Check { get; set; }
    public UIService UI { get; set; }

    public void Back()
    {
        if (current == null || current.Previous == null)
            return;

        current = current.Previous;
        OnNavigate?.Invoke(current);
    }

    public void NavigateToHome() => Navigate(KRConfig.Home);
    public void NavigateToAccount() => Navigate(account);

    public void Navigate(MenuItem menu, Dictionary<string, object> prevParams = null)
    {
        if (menu == null || string.IsNullOrWhiteSpace(menu.Target))
            return;

        menu.Previous = current;
        if (menu.Previous != null && prevParams != null)
            menu.Previous.ComParameters = prevParams;
        current = menu;
        OnNavigate?.Invoke(current);
    }

    public void Navigate<T>(string name, string icon, Dictionary<string, object> comParams = null) where T : IComponent
    {
        var menu = new MenuItem(name, icon, typeof(T)) { ComParameters = comParams };
        Navigate(menu);
    }

    public void Navigate<T>() where T : IComponent
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

    internal void Navigate<TItem, TForm>(string name, string icon, TItem model, bool readOnly = false) where TItem : EntityBase, new() where TForm : BaseForm<TItem>
    {
        var menu = new MenuItem(name, icon, typeof(TForm))
        {
            ComParameters = new Dictionary<string, object> {
                { nameof(Form.ReadOnly), readOnly },
                { nameof(Form.Model), model }
            }
        };
        var id = model == null || model.IsNew ? string.Empty : model.Id;
        menu.Id = model.Id + name + id;
        Navigate(menu);
    }
}