﻿namespace Known.Designers;

class EntityView : BaseView<EntityInfo>
{
    private TableModel<FieldInfo> table;
    private string entity;
    private string script;
    private string tableName;
    private string tableScript;
    private DatabaseType dbType;
    private IAutoService Auto;

    internal override async Task SetModelAsync(EntityInfo model)
    {
        await base.SetModelAsync(model);
        SetViewData(model);
        await table.RefreshAsync();
    }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Auto = await CreateServiceAsync<IAutoService>();

        dbType = Database.Create().DatabaseType;
        SetViewData(Model);

        Tab.AddTab("Designer.Fields", BuildView);
        if (IsCustomPage)
            Tab.AddTab("Designer.EntityCode", BuildEntity);
        Tab.AddTab("Designer.TableScript", BuildScript);

        table = new(this, TableColumnMode.Property)
        {
            ShowSetting = false,
            FixedHeight = "330px",
            OnQuery = c =>
            {
                var result = new PagingResult<FieldInfo>(Model?.Fields);
                return Task.FromResult(result);
            }
        };
    }

    private void BuildView(RenderTreeBuilder builder)
    {
        builder.Div("entity-title", $"{Model?.Name}（{Model?.Id}）");
        builder.Table(table);
    }

    private void BuildEntity(RenderTreeBuilder builder)
    {
        var modulePath = AdminOption.Instance.Code?.EntityPath ?? ModulePath;
        var path = Path.Combine(modulePath, "Entities", $"{Model?.Id}.cs");
        if (Config.IsDebug)
            BuildAction(builder, Language.Save, () => SaveSourceCode(path, entity));
        BuildCode(builder, "entity", path, entity);
    }

    private void BuildScript(RenderTreeBuilder builder)
    {
        BuildAction(builder, Language["Designer.Execute"], async () =>
        {
            var info = new AutoInfo<string> { PageId = tableName, Data = tableScript };
            var result = await Auto.CreateTableAsync(info);
            UI.Result(result);
        });
        BuildCode(builder, "script", "", script);
    }

    private void SetViewData(EntityInfo model)
    {
        if (IsCustomPage)
            entity = Generator?.GetEntity(model);
        script = Generator?.GetScript(dbType, model);
        tableName = model?.Id;
        tableScript = script;
    }
}