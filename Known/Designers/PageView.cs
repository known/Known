﻿namespace Known.Designers;

class PageView : BaseView<PageInfo>
{
    private readonly TabModel tab = new();
    private TableModel<PageColumnInfo> list;
    private DemoPageModel table;
    private string codePage;
    private string codeService;
    private string codeServiceI;
    private string htmlPage;
    private string htmlService;
    private string htmlServiceI;

    private bool IsCustomPage => Form.Model.Data.IsCustomPage;

    [Parameter] public EntityInfo Entity { get; set; }

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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (IsCustomPage)
        {
            if (string.IsNullOrWhiteSpace(htmlPage))
                htmlPage = await JS.HighlightAsync(codePage, "csharp");
            if (string.IsNullOrWhiteSpace(htmlService))
                htmlService = await JS.HighlightAsync(codeService, "csharp");
            if (string.IsNullOrWhiteSpace(htmlServiceI))
                htmlServiceI = await JS.HighlightAsync(codeServiceI, "csharp");
            //await StateChangedAsync();
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

    private void BuildPage(RenderTreeBuilder builder)
    {
        var className = CodeGenerator.GetClassName(Entity?.Id);
        var path = Path.Combine(ModulePath, "Pages", "", $"{className}List.cs");
        BuildAction(builder, Language.Save, () => SaveSourceCode(path, codePage));
        BuildCode(builder, "page", path, htmlPage);
    }

    private void BuildService(RenderTreeBuilder builder)
    {
        var className = CodeGenerator.GetClassName(Entity?.Id);
        var path = Path.Combine(Config.App.ContentRoot, "Services", $"{className}Service.cs");
        BuildAction(builder, Language.Save, () => SaveSourceCode(path, codeService));
        BuildCode(builder, "page", path, htmlService);
    }

    private void BuildServiceI(RenderTreeBuilder builder)
    {
        var className = CodeGenerator.GetClassName(Entity?.Id);
        var path = Path.Combine(ModulePath, "Services", $"I{className}Service.cs");
        BuildAction(builder, Language.Save, () => SaveSourceCode(path, codeServiceI));
        BuildCode(builder, "page", path, htmlServiceI);
    }

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
            BuildPropertyItem(builder, "Designer.PageSize", b => UI.BuildNumber(b, new InputModel<int?>
            {
                Disabled = ReadOnly,
                Value = Model.PageSize,
                ValueChanged = this.Callback<int?>(value => { Model.PageSize = value; OnPropertyChanged(); })
            }));
            BuildPropertyItem(builder, "Designer.ToolSize", b => UI.BuildNumber(b, new InputModel<int?>
            {
                Disabled = ReadOnly,
                Value = Model.ToolSize,
                ValueChanged = this.Callback<int?>(value => { Model.ToolSize = value; OnPropertyChanged(); })
            }));
            //BuildPropertyItem(builder, "Designer.ActionSize", b => UI.BuildNumber(b, new InputModel<int?>
            //{
            //    Disabled = ReadOnly,
            //    Value = Model.ActionSize,
            //    ValueChanged = this.Callback<int?>(value => { Model.ActionSize = value; OnPropertyChanged(); })
            //}));
            //BuildPropertyItem(builder, "Designer.FixedWidth", b => UI.BuildText(b, new InputModel<string>
            //{
            //    Disabled = ReadOnly,
            //    Value = Model.FixedWidth,
            //    ValueChanged = this.Callback<string>(value => { Model.FixedWidth = value; OnPropertyChanged(); })
            //}));
            //BuildPropertyItem(builder, "Designer.FixedHeight", b => UI.BuildText(b, new InputModel<string>
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

    private void ShowActions(string type, string[] value, Action<string[]> onChange)
    {
        var title = Language[$"Designer.{type}"];
        ActionTable table = null;
        var model = new DialogModel
        {
            Title = Language?.GetFormTitle("Edit", title),
            Content = b => b.Component<ActionTable>()
                            .Set(c => c.Type, type)
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
        table.SetQueryColumns();
        SetSourceCode();
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
        table.SetQueryColumns();
        table.Result = await table.OnQuery?.Invoke(table.Criteria);
        SetSourceCode();
    }

    private void SetSourceCode()
    {
        htmlPage = string.Empty;
        htmlService = string.Empty;
        if (!IsCustomPage)
            return;

        codePage = Generator?.GetPage(Model, Entity);
        codeService = Generator?.GetService(Model, Entity);
        codeServiceI = Generator?.GetIService(Model, Entity);
    }
}