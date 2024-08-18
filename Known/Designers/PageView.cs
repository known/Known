namespace Known.Designers;

class PageView : BaseView<PageInfo>
{
    private readonly TabModel tab = new();
    private TableModel<PageColumnInfo> list;
    private DemoPageModel table;

    [Parameter] public EntityInfo Entity { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();

        Tab.AddTab("Designer.View", BuildView);
        Tab.AddTab("Designer.Fields", BuildList);

        SetTablePage();

        list = new(this, true)
        {
            FixedHeight = "355px",
            OnQuery = c =>
            {
                var result = new PagingResult<PageColumnInfo>(Model?.Columns);
                return Task.FromResult(result);
            }
        };

        tab.AddTab("Designer.Property", BuildProperty);
        if (!ReadOnly)
        {
            tab.Right = b =>
            {
                UI.BuildButton(b, new ActionInfo
                {
                    Name = Language["Designer.Toolbar"],
                    Style = "primary",
                    OnClick = this.Callback<MouseEventArgs>(OnToolbar)
                });
                UI.BuildButton(b, new ActionInfo
                {
                    Name = Language["Designer.Action"],
                    Style = "primary",
                    OnClick = this.Callback<MouseEventArgs>(OnAction)
                });
            };
        }
    }

    internal override async void SetModel(PageInfo model)
    {
        base.SetModel(model);
        SetTablePage();
        await StateChangedAsync();
        await list.RefreshAsync();
    }

    private void BuildView(RenderTreeBuilder builder)
    {
        builder.Div("view", () => builder.Table(table));
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

    private void OnToolbar(MouseEventArgs args)
    {
        ShowActions("Toolbar", Model.Tools, value =>
        {
            Model.Tools = value;
            OnPropertyChanged();
        });
    }

    private void OnAction(MouseEventArgs args)
    {
        ShowActions("Action", Model.Actions, value =>
        {
            Model.Actions = value;
            OnPropertyChanged();
        });
    }

    private void ShowActions(string type, string[] value, Action<string[]> onChange)
    {
        var title = Language[$"Designer.{type}"];
        ActionTable table = null;
        var model = new DialogModel
        {
            Title = Language?.GetFormTitle("Edit", title),
            Content = b => b.Component<ActionTable>()
                            .Set(c => c.Name, title)
                            .Set(c => c.Value, value)
                            .Build(value => table = value),
        };
        model.OnOk = async () =>
        {
            onChange.Invoke(table?.Values);
            await model.CloseAsync();
        };
        UI.ShowDialog(model);
    }

    private void OnPropertyChanged()
    {
        OnChanged?.Invoke(Model);
        table.SetPage(Entity, Model);
        StateChanged();
    }

    private async void SetTablePage()
    {
        table = new DemoPageModel(this)
        {
            Module = Form.Model.Data,
            Entity = Entity
        };
        table.SetPage(Entity, Model);
        table.Result = await table.OnQuery?.Invoke(table.Criteria);
    }
}