using Known.Pages;

namespace Known;

public class Context
{
    private readonly KMenuItem account = new("个人中心", "fa fa-user", typeof(SysAccount), "当前用户个人中心和安全设置。");
    private KMenuItem current;

    internal static Action<KMenuItem> OnNavigate { get; set; }

    public UserInfo CurrentUser { get; set; }

    public void Back()
    {
        if (current == null || current.Previous == null)
            return;

        current = current.Previous;
        OnNavigate?.Invoke(current);
    }

    public void NavigateToHome() => Navigate(KRConfig.Home);
    public void NavigateToAccount() => Navigate(account);

    public void Navigate(KMenuItem menu, Dictionary<string, object> prevParams = null)
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
        var menu = new KMenuItem(name, icon, typeof(T)) { ComParameters = comParams };
        Navigate(menu);
    }

    public void Navigate<T>() where T : IComponent
    {
        var type = typeof(T);
        var target = type.FullName;
        var menu = KRConfig.UserMenus.FirstOrDefault(m => m.Target == target);
        if (menu == null)
            return;

        var item = KMenuItem.From(menu);
        item.ComType = type;
        Navigate(item);
    }

    internal void Navigate<TItem, TForm>(string name, string icon, TItem model, bool readOnly = false, Action<Result> onSuccess = null) where TItem : EntityBase, new() where TForm : BaseForm<TItem>
    {
        var id = model == null || model.IsNew ? name : name + model.Id;
        var menu = new KMenuItem(name, icon, typeof(TForm))
        {
            Id = id,
            ComParameters = new Dictionary<string, object> {
                { nameof(KForm.Model), model },
                { nameof(KForm.ReadOnly), readOnly },
                { nameof(KForm.OnSuccess), onSuccess }
            }
        };
        Navigate(menu);
    }
}