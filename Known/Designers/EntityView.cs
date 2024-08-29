namespace Known.Designers;

class EntityView : BaseView<EntityInfo>
{
    private TableModel<FieldInfo> table;
    private string entity;
    private string script;
    private string tableName;
    private string tableScript;
    private DatabaseType dbType;
    private IAutoService Auto;

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
        Tab.AddTab("Designer.EntityCode", BuildEntity);
        Tab.AddTab("Designer.TableScript", BuildScript);

        table.FixedHeight = "330px";
        table.OnQuery = c=>
        {
            var result = new PagingResult<FieldInfo>(Model?.Fields);
            return Task.FromResult(result);
        };
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            entity = await JS.HighlightAsync(entity, "csharp");
            script = await JS.HighlightAsync(script, "csharp");
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void BuildView(RenderTreeBuilder builder)
    {
        builder.Div("entity-title", $"{Model?.Name}（{Model?.Id}）");
        UI.BuildTable(builder, table);
    }

    private void BuildEntity(RenderTreeBuilder builder) => BuildCode(builder, entity);

    private void BuildScript(RenderTreeBuilder builder)
    {
        if (!ReadOnly)
        {
            builder.Div("kui-code-action", () =>
            {
                builder.Button(Language["Designer.Execute"], this.Callback<MouseEventArgs>(OnExecuteScript));
            });
        }
        BuildCode(builder, script);
    }

    private async void OnExecuteScript(MouseEventArgs args)
    {
        var info = new AutoInfo<string> { PageId = tableName, Data = tableScript };
        var result = await Auto.CreateTableAsync(info);
        UI.Result(result);
    }

    private void SetViewData(EntityInfo model)
    {
        entity = Generator?.GetEntity(model);
        script = Generator?.GetScript(dbType, model);
        tableName = model?.Id;
        tableScript = script;
    }
}