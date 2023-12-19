using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class PageView : BaseView<PageInfo>
{
    private string code;

    internal override void SetModel(PageInfo model)
    {
        base.SetModel(model);
        code = Service.GetPage(Model);
        StateChanged();
    }

    protected override void BuildView(RenderTreeBuilder builder)
    {
        var table = new TableModel<Dictionary<string, object>>(Model);
        builder.Div("kui-top", () =>
        {
            UI.BuildQuery(builder, table);
            UI.BuildToolbar(builder, table.Toolbar);
        });
        builder.Div("kui-table", () => UI.BuildTable(builder, table));
    }

    protected override void BuildCode(RenderTreeBuilder builder) => BuildCode(builder, code);
}