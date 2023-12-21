using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class PageView : BaseView<PageInfo>
{
    private TableModel<Dictionary<string, object>> table;
    private readonly TableModel<PageColumnInfo> list = new();
    private string code;

    [Parameter] public EntityInfo Entity { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetModel();
        Tab.Items.Add(new ItemModel("视图") { Content = BuildView });
        Tab.Items.Add(new ItemModel("字段") { Content = BuildList });
        Tab.Items.Add(new ItemModel("代码") { Content = BuildCode });

        list.ScrollY = "380px";
        list.OnQuery = c =>
        {
            var result = new PagingResult<PageColumnInfo>(Model?.Columns);
            return Task.FromResult(result);
        };
    }

    internal override void SetModel(PageInfo model)
    {
        base.SetModel(model);
        SetModel();
        StateChanged();
    }

    private void BuildView(RenderTreeBuilder builder)
    {
        builder.Div("kui-top", () =>
        {
            UI.BuildQuery(builder, table);
            UI.BuildToolbar(builder, table?.Toolbar);
        });
        builder.Div("kui-table", () => UI.BuildTable(builder, table));
    }

    private void BuildList(RenderTreeBuilder builder) => BuildList(builder, list);
    private void BuildCode(RenderTreeBuilder builder) => BuildCode(builder, code);

    private void SetModel()
    {
        table = new TableModel<Dictionary<string, object>>(UI, Model) { OnQuery = OnQueryDatas };
        code = Service.GetPage(Model);
    }

    private Task<PagingResult<Dictionary<string, object>>> OnQueryDatas(PagingCriteria criteria)
    {
        var datas = new List<Dictionary<string, object>>();

        for (int i = 0; i < 3; i++)
        {
            var data = new Dictionary<string, object>();
            foreach (var item in Model.Columns)
            {
                data[item.Id] = GetValue(item, i);
            }
            datas.Add(data);
        }

        var result = new PagingResult<Dictionary<string, object>>(datas);
        return Task.FromResult(result);
    }

    private object GetValue(PageColumnInfo item, int index)
    {
        var value = $"{item.Id}{index}";
        var field = Entity?.Fields?.FirstOrDefault(f => f.Id == item.Id);
        if (field != null)
        {
            switch (field.Type)
            {
                case FieldType.Text:
                    return value;
                case FieldType.TextArea:
                    return "";
                case FieldType.Date:
                    return DateTime.Now;
                case FieldType.Number:
                    return index;
                case FieldType.Switch:
                case FieldType.CheckBox:
                    return true;
                case FieldType.CheckList:
                    return "Item1,Item2";
                case FieldType.RadioList:
                case FieldType.Select:
                    return item.Id;
                case FieldType.File:
                    return "附件";
            }
        }

        if (item.Id.EndsWith("Date") || item.Id.EndsWith("Time"))
            return DateTime.Now;

        return value;
    }
}