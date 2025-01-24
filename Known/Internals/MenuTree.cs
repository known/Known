namespace Known.Internals;

class MenuTree : BaseComponent
{
    private TreeModel tree;
    private MenuInfo current = new();

    [Parameter] public MenuInfo Parent { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        tree = new TreeModel
        {
            ExpandRoot = true,
            Data = [Parent],
            OnNodeClick = OnTreeClickAsync,
            OnModelChanged = OnTreeChangedAsync
        };
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-edit-menu", () =>
        {
            builder.Div(() => BuildTree(builder));
            builder.Div(() => BuildForm(builder));
        });
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

        var isPage = model.Data?.Type == nameof(MenuType.Page);
        var isLink = model.Data?.Type == nameof(MenuType.Link);
        if (isPage || isLink)
            model.AddRow().AddColumn(c => c.Url, c => c.Required = isLink);
        if (isLink)
        {
            model.AddRow().AddColumn(c => c.Target, c =>
            {
                c.Name = "目标";
                c.Type = FieldType.RadioList;
                c.Category = nameof(LinkTarget);
            });
        }
    }

    private void BuildTree(RenderTreeBuilder builder)
    {
        builder.Component<AntTree>()
               .Set(c => c.Model, tree)
               //.Set(c => c.Draggable, true)
               //.Set(c => c.OnDrop, this.Callback<TreeEventArgs<MenuInfo>>(OnTreeDrop))
               //.Set(c => c.OnDragEnd, this.Callback<TreeEventArgs<MenuInfo>>(OnTreeDropEnd))
               //.Set(c => c.TitleTemplate, this.BuildTree<TreeNode<MenuInfo>>(BuildTreeItem))
               .Build();
    }

    //private void BuildTreeItem(RenderTreeBuilder builder, TreeNode<MenuInfo> node)
    //{
    //    var model = new DropdownModel
    //    {
    //        TriggerType = nameof(Trigger.ContextMenu),
    //        OnItemClick = info => OnItemClick(info, node.DataItem),
    //        Items = [
    //            new ActionInfo { Id = "Delete", Icon = "close", Name = "删除" }
    //        ]
    //    };
    //    builder.Component<AntDropdown>()
    //           .Set(c => c.Model, model)
    //           .Set(c => c.ChildContent, b => b.Span(node.DataItem.Name))
    //           .Build();
    //}

    private void BuildForm(RenderTreeBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(current?.Name) || current?.Id == "0")
            return;

        var model = new FormModel<MenuInfo>(this)
        {
            SmallLabel = true,
            Data = current,
            OnFieldChanged = async f => await SaveMenuAsync(current)
        };
        model.AddRow().AddColumn("类型", b => BuildMenuType(b, current));
        AddMenuRow(model);
        model.AddRow().AddColumn(c => c.Sort, c =>
        {
            c.Name = "排序";
            c.Required = true;
        });
        builder.Form(model);
    }

    private void BuildMenuType(RenderTreeBuilder builder, MenuInfo item)
    {
        builder.Div("type", () =>
        {
            builder.Tag(item.Type);
            builder.Div(() =>
            {
                //if (item.Parent.Children.IndexOf(item) > 0)
                //    builder.Icon("arrow-up", "上移", this.Callback<MouseEventArgs>(e => MoveUpMenuAsync(item)));
                //if (item.Parent.Children.IndexOf(item) < item.Parent.Children.Count - 1)
                //    builder.Icon("arrow-down", "下移", this.Callback<MouseEventArgs>(e => MoveDownMenuAsync(item)));
                //builder.Icon("drag", "移动到", this.Callback<MouseEventArgs>(e => MoveToMenu(item)));
                builder.Span().Class("kui-danger")
                       .Child(() => builder.Icon("delete", "删除", this.Callback<MouseEventArgs>(e => DeleteMenu(item))));
            });
        });
    }

    private Task OnTreeClickAsync(MenuInfo item)
    {
        current = item;
        return StateChangedAsync();
    }

    private Task<TreeModel> OnTreeChangedAsync()
    {
        if (current != null && current.Parent != null)
            current.Parent.Children = [.. current.Parent.Children.OrderBy(m => m.Sort)];
        tree.Data = tree.Data;
        return Task.FromResult(tree);
    }

    //private Task OnTreeDrop(TreeEventArgs<MenuInfo> args)
    //{
    //    var node = args.Node.DataItem;
    //    return Task.CompletedTask;
    //}

    //private Task OnTreeDropEnd(TreeEventArgs<MenuInfo> args)
    //{
    //    var node = args.Node.DataItem;
    //    return Task.CompletedTask;
    //}

    //private Task OnItemClick(ActionInfo info, MenuInfo item)
    //{
    //    UI.Alert(item.Name);
    //    return Task.CompletedTask;
    //}

    private async Task SaveMenuAsync(MenuInfo item)
    {
        await Platform.SaveMenuAsync(item);
        await tree.RefreshAsync();
    }

    //private async Task MoveUpMenuAsync(MenuInfo item)
    //{
    //    var result = await Platform.SaveMenuAsync(item);
    //    UI.Result(result, tree.RefreshAsync);
    //}

    //private async Task MoveDownMenuAsync(MenuInfo item)
    //{
    //    var result = await Platform.SaveMenuAsync(item);
    //    UI.Result(result, tree.RefreshAsync);
    //}

    //private void MoveToMenu(MenuInfo item)
    //{
    //    throw new NotImplementedException();
    //}

    private void DeleteMenu(MenuInfo item)
    {
        if (item.Children != null && item.Children.Count > 0)
        {
            UI.Error("存在子菜单，不能删除！");
            return;
        }

        UI.Confirm($"确定要删除菜单[{item.Name}]？", async () =>
        {
            var result = await Platform.DeleteMenuAsync(item);
            UI.Result(result, () =>
            {
                if (item.Id == Parent.Id)
                    tree.Data.Remove(item);
                if (item.Parent != null)
                {
                    item.Parent.Children.Remove(item);
                    item.Parent.Children.Resort();
                }
                return tree.RefreshAsync();
            });
        });
    }
}