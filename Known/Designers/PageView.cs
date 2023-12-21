using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class PageView : BaseView<PageInfo>
{
    private TableModel<Dictionary<string, object>> table;
    private string code;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetTableModel();
    }

    internal override void SetModel(PageInfo model)
    {
        base.SetModel(model);
        SetTableModel();
        StateChanged();
    }

    protected override void BuildView(RenderTreeBuilder builder)
    {
        builder.Div("kui-top", () =>
        {
            UI.BuildQuery(builder, table);
            UI.BuildToolbar(builder, table?.Toolbar);
        });
        builder.Div("kui-table", () => UI.BuildTable(builder, table));
    }

    protected override void BuildCode(RenderTreeBuilder builder) => BuildCode(builder, code);

    private void SetTableModel()
    {
        table = new TableModel<Dictionary<string, object>>(UI, Model) { OnQuery = OnQueryDatas };
        code = Service.GetPage(Model);
    }

    private Task<PagingResult<Dictionary<string, object>>> OnQueryDatas(PagingCriteria criteria)
    {
        var datas = new List<Dictionary<string, object>>();

        for (int i = 0; i < 5; i++)
        {
            var data = new Dictionary<string, object>();
            foreach (var item in Model.Columns)
            {
                data[item.Id] = $"{item.Id}{i}";
            }
            datas.Add(data);
        }

        var result = new PagingResult<Dictionary<string, object>>(datas);
        return Task.FromResult(result);
    }
}