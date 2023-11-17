namespace Known;

public class Context
{
    private MenuItem current;

    internal static Action<MenuItem> OnNavigate { get; set; }

    public InstallInfo Install { get; internal set; }
    public UserInfo CurrentUser { get; internal set; }
    public List<MenuInfo> UserMenus { get; internal set; }

    public void Back()
    {
        if (current == null || current.Previous == null)
            return;

        current = current.Previous;
        OnNavigate?.Invoke(current);
    }

    public List<MenuItem> GetMenus(List<string> menuIds)
    {
        if (menuIds == null || menuIds.Count == 0)
            return [];

        var menus = new List<MenuItem>();
        foreach (var menuId in menuIds)
        {
            var menu = UserMenus.FirstOrDefault(m => m.Name == menuId);
            if (menu != null)
                menus.Add(MenuItem.From(menu));
        }
        return menus;
    }

    public void NavigateToHome() => Navigate(Config.GetHomeMenu());

    public void Navigate(MenuItem menu, Dictionary<string, object> prevParams = null)
    {
        if (menu == null)
            return;

        menu.Previous = current;
        if (menu.Previous != null && prevParams != null)
            menu.Previous.ComParameters = prevParams;
        current = menu;
        OnNavigate?.Invoke(current);
    }

    //public void Navigate<T>(string name, string icon, Dictionary<string, object> comParams = null) where T : IComponent
    //{
    //    var menu = new KMenuItem(name, icon, typeof(T)) { ComParameters = comParams };
    //    Navigate(menu);
    //}

    //public void Navigate<T>() where T : IComponent
    //{
    //    var type = typeof(T);
    //    var target = type.FullName;
    //    var menu = UserMenus.FirstOrDefault(m => m.Target == target);
    //    if (menu == null)
    //        return;

    //    var item = KMenuItem.From(menu);
    //    item.ComType = type;
    //    Navigate(item);
    //}

    //internal void Navigate<TItem, TForm>(string name, string icon, TItem model, bool readOnly = false, Action<Result> onSuccess = null) where TItem : EntityBase, new() where TForm : BaseForm<TItem>
    //{
    //    var id = model == null || model.IsNew ? name : name + model.Id;
    //    var menu = new KMenuItem(name, icon, typeof(TForm))
    //    {
    //        Id = id,
    //        ComParameters = new Dictionary<string, object> {
    //            { nameof(KForm.Model), model },
    //            { nameof(KForm.ReadOnly), readOnly },
    //            { nameof(KForm.OnSuccess), onSuccess }
    //        }
    //    };
    //    Navigate(menu);
    //}
}