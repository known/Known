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
        Tab.Items.Add(new ItemModel("字段") { Content = BuildView });
        Tab.Items.Add(new ItemModel("实体代码") { Content = BuildCode });
        Tab.Items.Add(new ItemModel("建表脚本") { Content = BuildScript });

        table.ScrollY = "380px";
        table.OnQuery = c=>
        {
            var result = new PagingResult<FieldInfo>(Model?.Fields);
            return Task.FromResult(result);
        };
    }

    private void BuildView(RenderTreeBuilder builder)
    {
        builder.Div("bold", $"{Model?.Name}（{Model?.Id}）");
        UI.BuildTable(builder, table);
    }

    private void BuildCode(RenderTreeBuilder builder) => BuildCode(builder, code);
    private void BuildScript(RenderTreeBuilder builder) => BuildCode(builder, script);

    private void SetViewData(EntityInfo model)
    {
        code = Service.GetEntity(model);
        script = Service.GetSQL(model, dbType);
    }
}