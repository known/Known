using AntDesign;

namespace Known.Internals;

public partial class MainMenu
{
    private AntMenu menu;
    private readonly List<ActionInfo> actions = [];

    /// <summary>
    /// 取得或设置菜单模式。
    /// </summary>
    [Parameter] public MenuMode Mode { get; set; }

    /// <summary>
    /// 取得或设置菜单用户设置。
    /// </summary>
    [Parameter] public UserSettingInfo Setting { get; set; }

    /// <summary>
    /// 异步初始化组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        AddAction("Menu", "folder-add", "添加菜单", OnAddMenu);
        AddAction("Page", "file-add", "添加页面", OnAddPage);
        AddAction("Link", "link", "添加连接", OnAddLink);
        AddAction("Manage", "menu", "管理菜单", OnManageMenu);
    }

    /// <summary>
    /// 设置菜单数据源。
    /// </summary>
    /// <param name="items">菜单信息列表。</param>
    /// <returns></returns>
    public void SetItems(List<MenuInfo> items)
    {
        menu?.SetItems(items);
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
        var model = GetMenuFormModel("添加菜单", new MenuInfo { Type = nameof(MenuType.Menu) });
        UI.ShowForm(model);
    }

    private void OnAddPage(MouseEventArgs e)
    {
        var model = GetMenuFormModel("添加页面", new MenuInfo { Type = nameof(MenuType.Page) });
        UI.ShowForm(model);
    }

    private void OnAddLink(MouseEventArgs e)
    {
        var model = GetMenuFormModel("添加连接", new MenuInfo
        {
            Type = nameof(MenuType.Link),
            Target = nameof(LinkTarget.None)
        });
        model.AddRow().AddColumn(c => c.Url, c => c.Required = true);
        model.AddRow().AddColumn(c => c.Target, c =>
        {
            c.Name = "目标";
            c.Type = FieldType.RadioList;
            c.Category = nameof(LinkTarget);
        });
        UI.ShowForm(model);
    }

    private void OnManageMenu(MouseEventArgs e)
    {
        UI.Alert("管理菜单");
    }

    private FormModel<MenuInfo> GetMenuFormModel(string title, MenuInfo data)
    {
        var menus = Context.UserMenus ?? [];
        var model = new FormModel<MenuInfo>(this)
        {
            Title = title,
            Data = data,
            OnSave = Platform.SaveMenuAsync,
            OnSaved = d => App?.AddMenuItem(d)
        };
        model.AddRow().AddColumn(c => c.ParentId, c =>
        {
            c.Required = true;
            c.Name = "上级";
            c.Template = b =>
            {
                b.Component<TreePicker>()
                 .Set(c => c.Title, "选择上级菜单")
                 .Set(c => c.Items, menus.ToMenuItems(true))
                 .Set(c => c.OnChanged, v =>
                 {
                     model.Data.ParentId = v.Id;
                     model.Data.Sort = v.Children.Count + 1;
                 })
                 .Build();
            };
        });
        model.AddRow().AddColumn(c => c.Name, c =>
        {
            c.Name = "名称";
            c.Required = true;
        });
        model.AddRow().AddColumn(c => c.Icon, c =>
        {
            c.Name = "图标";
            c.Required = true;
            c.CustomField = nameof(IconPicker);
        });
        return model;
    }
}