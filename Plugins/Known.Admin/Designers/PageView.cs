namespace Known.Designers;

class PageView : BaseView<PageInfo>
{
    private readonly TabModel tab = new();
    private TableModel<PageColumnInfo> list;
    private DemoModel table;
    private string codePage;
    private string codeService;
    private string codeServiceI;

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();

        Tab.AddTab("Designer.View", BuildView);
        Tab.AddTab("Designer.Fields", BuildList);
        if (IsCustomPage)
        {
            Tab.AddTab("Designer.PageCode", BuildPage);
            Tab.AddTab("Designer.ServiceICode", BuildServiceI);
            Tab.AddTab("Designer.ServiceCode", BuildService);
        }

        await SetTablePageAsync();

        list = new(this, TableColumnMode.Property)
        {
            ShowSetting = false,
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
                b.Button(new ActionInfo
                {
                    Name = Language["Designer.Toolbar"],
                    Style = "primary",
                    OnClick = this.Callback<MouseEventArgs>(OnToolbar)
                });
                b.Button(new ActionInfo
                {
                    Name = Language["Designer.Action"],
                    Style = "primary",
                    OnClick = this.Callback<MouseEventArgs>(OnAction)
                });
            };
        }
    }

    internal override async Task SetModelAsync(PageInfo model)
    {
        await base.SetModelAsync(model);
        await SetTablePageAsync();
        await StateChangedAsync();
        await list.RefreshAsync();
    }

    private void BuildView(RenderTreeBuilder builder)
    {
        builder.Div("view", () => builder.PageTable(table));
        builder.Div("setting", () => builder.Tabs(tab));
    }

    private void BuildList(RenderTreeBuilder builder) => BuildList(builder, list);

    private void BuildPage(RenderTreeBuilder builder)
    {
        var modulePath = AdminOption.Instance.Code?.PagePath ?? ModulePath;
        var className = DataHelper.GetClassName(Module?.Entity?.Id);
        var fileName = $"{className}List.cs";
        var path = Path.Combine(modulePath, "Pages", fileName);
        if (Config.IsDebug)
            BuildAction(builder, Language.Save, () => SaveSourceCode(path, codePage));
        BuildCode(builder, "page", path, fileName, codePage);
    }

    private void BuildService(RenderTreeBuilder builder)
    {
        var modulePath = AdminOption.Instance.Code?.ServicePath ?? ModulePath;
        var className = DataHelper.GetClassName(Module?.Entity?.Id);
        var fileName = $"{className}Service.cs";
        var path = Path.Combine(modulePath, "Services", fileName);
        if (Config.IsDebug)
            BuildAction(builder, Language.Save, () => SaveSourceCode(path, codeService));
        BuildCode(builder, "page", path, fileName, codeService);
    }

    private void BuildServiceI(RenderTreeBuilder builder)
    {
        var modulePath = AdminOption.Instance.Code?.EntityPath ?? ModulePath;
        var className = DataHelper.GetClassName(Module?.Entity?.Id);
        var fileName = $"I{className}Service.cs";
        var path = Path.Combine(modulePath, "Services", fileName);
        if (Config.IsDebug)
            BuildAction(builder, Language.Save, () => SaveSourceCode(path, codeServiceI));
        BuildCode(builder, "page", path, fileName, codeServiceI);
    }

    private void BuildProperty(RenderTreeBuilder builder)
    {
        builder.Div("setting-row", () =>
        {
            BuildPropertyItem(builder, "Designer.ShowPager", b => b.Switch(new InputModel<bool>
            {
                Disabled = ReadOnly,
                Value = Model.ShowPager,
                ValueChanged = this.Callback<bool>(value => { Model.ShowPager = value; OnPropertyChanged(); })
            }));
            BuildPropertyItem(builder, "Designer.PageSize", b => b.Number(new InputModel<int?>
            {
                Disabled = ReadOnly,
                Value = Model.PageSize,
                ValueChanged = this.Callback<int?>(value => { Model.PageSize = value; OnPropertyChanged(); })
            }));
            BuildPropertyItem(builder, "Designer.ToolSize", b => b.Number(new InputModel<int?>
            {
                Disabled = ReadOnly,
                Value = Model.ToolSize,
                ValueChanged = this.Callback<int?>(value => { Model.ToolSize = value; OnPropertyChanged(); })
            }));
            //BuildPropertyItem(builder, "Designer.ActionSize", b => b.BuildNumber(new InputModel<int?>
            //{
            //    Disabled = ReadOnly,
            //    Value = Model.ActionSize,
            //    ValueChanged = this.Callback<int?>(value => { Model.ActionSize = value; OnPropertyChanged(); })
            //}));
            //BuildPropertyItem(builder, "Designer.FixedWidth", b => b.BuildText(new InputModel<string>
            //{
            //    Disabled = ReadOnly,
            //    Value = Model.FixedWidth,
            //    ValueChanged = this.Callback<string>(value => { Model.FixedWidth = value; OnPropertyChanged(); })
            //}));
            //BuildPropertyItem(builder, "Designer.FixedHeight", b => b.BuildText(new InputModel<string>
            //{
            //    Disabled = ReadOnly,
            //    Value = Model.FixedHeight,
            //    ValueChanged = this.Callback<string>(value => { Model.FixedHeight = value; OnPropertyChanged(); })
            //}));
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

    private void ShowActions(string type, List<string> value, Action<List<string>> onChange)
    {
        var title = Language[$"Designer.{type}"];
        ActionTable action = null;
        var model = new DialogModel
        {
            Title = Language?.GetFormTitle("Edit", title),
            Content = b => b.Component<ActionTable>()
                            .Set(c => c.Type, type)
                            .Set(c => c.Name, title)
                            .Set(c => c.Value, value)
                            .Build(value => action = value),
        };
        model.OnOk = async () =>
        {
            onChange.Invoke(action?.Values);
            await model.CloseAsync();
        };
        UI.ShowDialog(model);
    }

    private void OnPropertyChanged()
    {
        OnChanged?.Invoke(Model);
        table.Initialize(new TablePageInfo { Page = Model, Form = Module?.Form }, Module?.Entity);
        SetSourceCode();
        StateChanged();
    }

    private async Task SetTablePageAsync()
    {
        table = new DemoModel(this) { Module = Module, Entity = Module?.Entity };
        table.Initialize(new TablePageInfo { Page = Model, Form = Module?.Form }, Module?.Entity);
        table.Result = await table.OnQuery?.Invoke(table.Criteria);
        SetSourceCode();
    }

    private void SetSourceCode()
    {
        if (!IsCustomPage)
            return;

        codePage = Generator?.GetPage(Model, Module?.Entity);
        codeService = Generator?.GetService(Model, Module?.Entity);
        codeServiceI = Generator?.GetIService(Model, Module?.Entity);
    }
}