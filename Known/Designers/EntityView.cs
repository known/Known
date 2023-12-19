using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class EntityView : BaseView<EntityInfo>
{
    private readonly TableModel<FieldInfo> table = new();
    private string code;
    private string script;
    private DatabaseType dbType;

    internal override async void SetModel(EntityInfo model)
    {
        base.SetModel(model);
        SetViewData(model);
        await table.RefreshAsync();
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        dbType = new Database().DatabaseType;
        SetViewData(Model);
        Tab.Items.Add(new ItemModel("脚本") { Content = BuildScript });
        table.OnQuery = OnQuery;
    }

    protected override void BuildView(RenderTreeBuilder builder)
    {
        builder.Div("bold", $"{Model?.Name}（{Model?.Id}）");
        UI.BuildTable(builder, table);
    }

    protected override void BuildCode(RenderTreeBuilder builder) => BuildCode(builder, code);
    private void BuildScript(RenderTreeBuilder builder) => BuildCode(builder, script);

    private Task<PagingResult<FieldInfo>> OnQuery(PagingCriteria criteria)
    {
        var result = new PagingResult<FieldInfo>(Model?.Fields);
        return Task.FromResult(result);
    }

    private void SetViewData(EntityInfo model)
    {
        code = Service.GetEntity(model);
        script = Service.GetSQL(model, dbType);
    }
}