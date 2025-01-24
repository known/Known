using AntDesign;

namespace Known.Internals;

/// <summary>
/// 主菜单组件类。
/// </summary>
public partial class MainMenu
{
    private AntMenu menu;
    private readonly List<ActionInfo> actions = [];

    /// <summary>
    /// 取得或设置菜单模式。
    /// </summary>
    [Parameter] public MenuMode Mode { get; set; }

    /// <summary>
    /// 取得或设置上级菜单。
    /// </summary>
    [Parameter] public MenuInfo Parent { get; set; }

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
        AddAction("Menu", "folder-add", "添加菜单", OnAddMenu);
        AddAction("Page", "file-add", "添加页面", OnAddPage);
        AddAction("Link", "link", "添加连接", OnAddLink);
        AddAction("Manage", "menu", "菜单管理", OnManageMenu);
    }

    /// <summary>
    /// 设置菜单数据源。
    /// </summary>
    /// <param name="info">菜单信息。</param>
    /// <returns></returns>
    public void SetData(MenuInfo info)
    {
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
        UI.ShowForm(model);
    }

    private void OnManageMenu(MouseEventArgs e)
    {
        DialogModel model = null;
        model = new DialogModel
        {
            Title = "菜单管理",
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