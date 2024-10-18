namespace Known.Designers;

class EntityView : BaseView<EntityInfo>
{
    private TableModel<FieldInfo> table;
    private string entity;
    private string script;
    private string htmlEntity;
    private string htmlScript;
    private string tableName;
    private string tableScript;
    private DatabaseType dbType;
    private IAutoService Auto;

    private bool IsCustomPage => Form.Model.Data.IsCustomPage;

    internal override async void SetModel(EntityInfo model)
    {
        base.SetModel(model);
        SetViewData(model);
        await table.RefreshAsync();
    }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Auto = await CreateServiceAsync<IAutoService>();

        table = new(this, true);
        dbType = Database.Create().DatabaseType;
        SetViewData(Model);

        Tab.AddTab("Designer.Fields", BuildView);
        if (IsCustomPage)
            Tab.AddTab("Designer.EntityCode", BuildEntity);
        Tab.AddTab("Designer.TableScript", BuildScript);

        table.FixedHeight = "330px";
        table.OnQuery = c =>
        {
            var result = new PagingResult<FieldInfo>(Model?.Fields);
            return Task.FromResult(result);
        };
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (string.IsNullOrWhiteSpace(htmlEntity) && IsCustomPage)
            htmlEntity = await JS.HighlightAsync(entity, "csharp");
        if (string.IsNullOrWhiteSpace(htmlScript))
            htmlScript = await JS.HighlightAsync(script, "csharp");
        await StateChangedAsync();
    }

    private void BuildView(RenderTreeBuilder builder)
    {
        builder.Div("entity-title", $"{Model?.Name}（{Model?.Id}）");
        UI.BuildTable(builder, table);
    }

    private void BuildEntity(RenderTreeBuilder builder) => BuildCode(builder, htmlEntity);

    private void BuildScript(RenderTreeBuilder builder)
    {
        if (!ReadOnly)
        {
            builder.Div("kui-code-action", () =>
            {
                builder.Button(Language["Designer.Execute"], this.Callback<MouseEventArgs>(OnExecuteScript));
            });
        }
        BuildCode(builder, htmlScript);
    }

    private async void OnExecuteScript(MouseEventArgs args)
    {
        var info = new AutoInfo<string> { PageId = tableName, Data = tableScript };
        var result = await Auto.CreateTableAsync(info);
        UI.Result(result);
    }

    private void SetViewData(EntityInfo model)
    {
        htmlEntity = string.Empty;
        htmlScript = string.Empty;
        if (IsCustomPage)
            entity = Generator?.GetEntity(model);
        script = Generator?.GetScript(dbType, model);
        tableName = model?.Id;
        tableScript = script;
    }
}