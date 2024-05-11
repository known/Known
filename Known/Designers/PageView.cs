namespace Known.Designers;

class PageView : BaseView<PageInfo>
{
    private readonly TabModel tab = new();
    private TableModel<PageColumnInfo> list;
    private DemoPageModel table;
    private List<CodeInfo> actions;

    [Parameter] public EntityInfo Entity { get; set; }

    protected override void OnInitialized()
    {
        actions = Config.Actions.Select(a =>
        {
            var name = Language.GetString(a);
            return new CodeInfo(a.Id, name);
        }).ToList();
        base.OnInitialized();

        Tab.AddTab("Designer.View", BuildView);
        Tab.AddTab("Designer.Fields", BuildList);

        SetTablePage();

        list = new(Context, true)
        {
            FixedHeight = "355px",
            OnQuery = c =>
            {
                var result = new PagingResult<PageColumnInfo>(Model?.Columns);
                return Task.FromResult(result);
            }
        };

        tab.AddTab("Designer.Property", BuildProperty);
        tab.AddTab("Designer.Toolbar", BuildToolbar);
        tab.AddTab("Designer.Action", BuildAction);
    }

    internal override async void SetModel(PageInfo model)
    {
        base.SetModel(model);
        SetTablePage();
        StateChanged();
        await list.RefreshAsync();
    }

    private void BuildView(RenderTreeBuilder builder)
    {
        builder.Div("view", () => builder.BuildTable(table));
        builder.Div("setting", () => UI.BuildTabs(builder, tab));
    }

    private void BuildList(RenderTreeBuilder builder) => BuildList(builder, list);

    private void BuildProperty(RenderTreeBuilder builder)
    {
        builder.Div("setting-row", () =>
        {
            BuildPropertyItem(builder, "Designer.ShowPager", b => UI.BuildSwitch(b, new InputModel<bool>
            {
                Disabled = ReadOnly,
                Value = Model.ShowPager,
                ValueChanged = this.Callback<bool>(value => { Model.ShowPager = value; OnPropertyChanged(); })
            }));
            BuildPropertyItem(builder, "Designer.FixedWidth", b => UI.BuildText(b, new InputModel<string>
            {
                Disabled = ReadOnly,
                Value = Model.FixedWidth,
                ValueChanged = this.Callback<string>(value => { Model.FixedWidth = value; OnPropertyChanged(); })
            }));
            BuildPropertyItem(builder, "Designer.FixedHeight", b => UI.BuildText(b, new InputModel<string>
            {
                Disabled = ReadOnly,
                Value = Model.FixedHeight,
                ValueChanged = this.Callback<string>(value => { Model.FixedHeight = value; OnPropertyChanged(); })
            }));
        });
    }

    private void BuildToolbar(RenderTreeBuilder builder)
    {
        //TODO:按钮位置左右移动
        UI.BuildCheckList(builder, new InputModel<string[]>
        {
            Disabled = ReadOnly,
            Codes = actions,
            Value = Model.Tools,
            ValueChanged = this.Callback<string[]>(value => { Model.Tools = value; OnPropertyChanged(); })
        });
    }

    private void BuildAction(RenderTreeBuilder builder)
    {
        UI.BuildCheckList(builder, new InputModel<string[]>
        {
            Disabled = ReadOnly,
            Codes = actions,
            Value = Model.Actions,
            ValueChanged = this.Callback<string[]>(value => { Model.Actions = value; OnPropertyChanged(); })
        });
    }

    private void OnPropertyChanged()
    {
        OnChanged?.Invoke(Model);
        table.SetPage(Model);
        StateChanged();
    }

    private async void SetTablePage()
    {
        table = new DemoPageModel(Context)
        {
            Module = Form.Model.Data,
            Entity = Entity
        };
        table.SetPage(Model);
        table.Result = await table.OnQuery?.Invoke(table.Criteria);
    }
}