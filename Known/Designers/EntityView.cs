using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class EntityView : BaseView<EntityInfo>
{
    private readonly TableModel<FieldInfo> table = new();
    private string entity;
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
        Tab.Items.Add(new ItemModel("字段列表") { Content = BuildView });
        Tab.Items.Add(new ItemModel("实体代码") { Content = BuildEntity });
        Tab.Items.Add(new ItemModel("建表脚本") { Content = BuildScript });

        table.FixedHeight = "380px";
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

    private void BuildEntity(RenderTreeBuilder builder) => BuildCode(builder, entity);
    private void BuildScript(RenderTreeBuilder builder) => BuildCode(builder, script);

    private void SetViewData(EntityInfo model)
    {
        entity = Generator.GetEntity(model);
        script = Generator.GetScript(dbType, model);
    }
}