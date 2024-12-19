using AntDesign;

namespace Known.Internals;

class NavEdit : BaseComponent
{
    private DropdownModel model;

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        var items = new List<ActionInfo>();
        var menu = new ActionInfo { Id = "Menu", Icon = "menu", Name = "菜单" };
        menu.Children.Add(new ActionInfo { Id = "Group", Icon = "group", Name = "分组" });
        menu.Children.Add(new ActionInfo { Id = "Page", Icon = "file", Name = "页面" });
        menu.Children.Add(new ActionInfo { Id = "Link", Icon = "link", Name = "链接" });
        items.Add(menu);
        items.Add(new ActionInfo { Id = "Nav", Icon = "bars", Name = "导航" });
        model = new DropdownModel
        {
            Icon = "plus",
            Items = items,
            TriggerType = "Click",
            OnItemClick = OnItemClickAsync
        };
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div().Class("kui-edit").Child(() =>
        {
            builder.Component<Tooltip>()
                   .Set(c => c.Title, "添加模块")
                   .Set(c => c.ChildContent, b =>
                   {
                       b.Component<AntDropdown>()
                        .Set(c => c.Model, model)
                        .Set(c => c.Placement, Placement.BottomRight)
                        .Build();
                   })
                   .Build();
        });
    }

    private Task OnItemClickAsync(ActionInfo info)
    {
        if (info.Id == "Nav")
            ShowNavForm();
        else
            ShowMenuForm(info);
        return Task.CompletedTask;
    }

    private void ShowNavForm()
    {
    }

    private void ShowMenuForm(ActionInfo info)
    {
        var menus = Context.UserMenus ?? [];
        var form = new FormModel<MenuInfo>(this)
        {
            Title = $"添加{info.Name}",
            Data = new MenuInfo { Id = Utils.GetGuid() },
            OnSave = Platform.SaveMenuAsync,
            OnSaved = d => App?.AddMenuItem(d)
        };
        form.AddRow().AddColumn(c => c.ParentId, c =>
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
                     form.Data.ParentId = v.Id;
                     form.Data.Sort = v.Children.Count + 1;
                 })
                 .Build();
            };
        });
        form.AddRow().AddColumn(c => c.Name, c => c.Required = true);
        form.AddRow().AddColumn(c => c.Icon, c =>
        {
            c.Required = true;
            c.CustomField = nameof(IconPicker);
        });
        if (info.Id == "Page")
        {
            form.AddRow().AddColumn(c => c.Target, c => c.Required = true);
            form.AddRow().AddColumn(c => c.Url, c => c.Required = true);
        }
        else if (info.Id == "Link")
        {
            form.AddRow().AddColumn(c => c.Url, c => c.Required = true);
        }
        UI.ShowForm(form);
    }
}