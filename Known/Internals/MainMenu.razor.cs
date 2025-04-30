using AntDesign;

namespace Known.Internals;

/// <summary>
/// 主菜单组件类。
/// </summary>
public partial class MainMenu
{
    private AntMenu menu;
    private readonly List<ActionInfo> actions = [];

    private bool CanEdit => Parent?.CanEdit == true;

    /// <summary>
    /// 取得或设置菜单模式。
    /// </summary>
    [Parameter] public MenuMode Mode { get; set; }

    /// <summary>
    /// 取得或设置上级菜单。
    /// </summary>
    [Parameter] public MenuInfo Parent { get; set; }

    /// <summary>
    /// 取得或设置添加菜单后委托。
    /// </summary>
    [Parameter] public Action<MenuInfo> OnAdded { get; set; }

    /// <summary>
    /// 取得或设置菜单管理后委托。
    /// </summary>
    [Parameter] public Action<MenuInfo> OnManaged { get; set; }

    /// <summary>
    /// 异步初始化组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        AddAction("Menu", "folder-add", Language.AddMenu, OnAddMenu);
        AddAction("Page", "file-add", Language.AddPage, OnAddPage);
        AddAction("Link", "link", Language.AddLink, OnAddLink);
        AddAction("Manage", "menu", Language.MenuManage, OnManageMenu);
    }

    /// <summary>
    /// 设置菜单数据源。
    /// </summary>
    /// <param name="info">菜单信息。</param>
    /// <returns></returns>
    public void SetData(MenuInfo info)
    {
        if (info == null)
            return;

        Parent = info;
        menu?.SetItems(info.Children);
    }

    private void AddAction(string id, string icon, string name, Action<MouseEventArgs> onClick)
    {
        actions.Add(new ActionInfo
        {
            Id = id,
            Icon = icon,
            Name = name,
            OnClick = this.Callback(onClick)
        });
    }

    private void OnAddMenu(MouseEventArgs e)
    {
        var model = GetMenuFormModel(Language.AddMenu, new MenuInfo { Type = nameof(MenuType.Menu) });
        UI.ShowForm(model);
    }

    private void OnAddPage(MouseEventArgs e)
    {
        var model = GetMenuFormModel(Language.AddPage, new MenuInfo { Type = nameof(MenuType.Page) });
        UI.ShowForm(model);
    }

    private void OnAddLink(MouseEventArgs e)
    {
        var model = GetMenuFormModel(Language.AddLink, new MenuInfo { Type = nameof(MenuType.Link) });
        UI.ShowForm(model);
    }

    private void OnManageMenu(MouseEventArgs e)
    {
        DialogModel model = null;
        model = new DialogModel
        {
            Title = Language.MenuManage,
            Width = 600,
            Content = b => b.Component<MenuTree>().Set(c => c.Parent, Parent).Build(),
            OnOk = () =>
            {
                menu?.SetItems(Parent.Children);
                OnManaged?.Invoke(Parent);
                return model.CloseAsync();
            }
        };
        UI.ShowDialog(model);
    }

    private FormModel<MenuInfo> GetMenuFormModel(string title, MenuInfo data)
    {
        var model = new FormModel<MenuInfo>(this)
        {
            SmallLabel = true,
            Title = title,
            Data = data,
            OnSave = Platform.SaveMenuAsync,
            OnSaved = d =>
            {
                App?.AddMenuItem(d);
                OnAdded?.Invoke(d);
            }
        };
        model.AddRow().AddColumn(c => c.ParentId, c =>
        {
            c.Required = true;
            c.Name = Language.ParentMenu;
            c.Template = b =>
            {
                b.Component<TreePicker>()
                 .Set(c => c.Title, Language.SelectParentMenu)
                 .Set(c => c.Items, [Parent])
                 .Set(c => c.OnChanged, v =>
                 {
                     model.Data.ParentId = v.Id;
                     model.Data.Sort = v.Children.Count + 1;
                 })
                 .Build();
            };
        });
        MenuTree.AddMenuRow(model);
        return model;
    }
}