namespace Known.Internals;

internal class MenuTree : BaseComponent
{
    private TreeModel tree;
    private MenuInfo root;
    private MenuInfo current = new();

    [Parameter] public List<MenuInfo> Menus { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        root = Config.App.GetRootMenu();
        root.Children.AddRange(Menus);
        tree = new TreeModel
        {
            ExpandRoot = true,
            Data = [root],
            OnNodeClick = OnTreeClickAsync
        };
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-edit-menu", () =>
        {
            builder.Div("", () => builder.Tree(tree));
            builder.Div("", () => BuildForm(builder));
        });
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            await tree.RefreshAsync();
    }

    internal static void AddMenuRow(FormModel<MenuInfo> model)
    {
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
        if (model.Data?.Type == nameof(MenuType.Link))
        {
            model.AddRow().AddColumn(c => c.Url, c => c.Required = true);
            model.AddRow().AddColumn(c => c.Target, c =>
            {
                c.Name = "目标";
                c.Type = FieldType.RadioList;
                c.Category = nameof(LinkTarget);
            });
        }
    }

    private void BuildForm(RenderTreeBuilder builder)
    {
        if (current?.Id == root.Id)
            return;

        var model = new FormModel<MenuInfo>(this)
        {
            SmallLabel = true,
            Data = current,
            OnFieldChanged = async f =>
            {
                await Platform.SaveMenuAsync(current);
                await StateChangedAsync();
            }
        };
        model.AddRow().AddColumn("类型", b => b.Tag(current.Type));
        AddMenuRow(model);
        model.AddRow().AddColumn(c => c.Sort, c =>
        {
            c.Name = "排序";
            c.Required = true;
        });
        builder.Form(model);
    }

    private Task OnTreeClickAsync(MenuInfo item)
    {
        current = item;
        return StateChangedAsync();
    }
}