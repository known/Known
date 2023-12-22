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
    private readonly TabModel tab = new();
    private readonly List<CodeInfo> actions = Config.Actions.Select(a => new CodeInfo(a.Id, a.Name)).ToList();

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

        tab.Items.Add(new ItemModel("属性") { Content = BuildProperty });
        tab.Items.Add(new ItemModel("工具条") { Content = BuildToolbar });
        tab.Items.Add(new ItemModel("操作列") { Content = BuildAction });
    }

    internal override void SetModel(PageInfo model)
    {
        base.SetModel(model);
        SetModel();
        StateChanged();
    }

    private void BuildView(RenderTreeBuilder builder)
    {
        builder.Div("view", () =>
        {
            builder.Div("kui-top", () =>
            {
                UI.BuildQuery(builder, table);
                UI.BuildToolbar(builder, table?.Toolbar);
            });
            builder.Div("kui-table", () => UI.BuildTable(builder, table));
        });
        builder.Div("setting", () => UI.BuildTabs(builder, tab));
    }

    private void BuildList(RenderTreeBuilder builder) => BuildList(builder, list);
    private void BuildCode(RenderTreeBuilder builder) => BuildCode(builder, code);

    private void BuildProperty(RenderTreeBuilder builder)
    {
        builder.Div("setting-row", () =>
        {
            BuildPropertyItem(builder, "滚动宽度", b => UI.BuildText(b, new InputModel<string>
            {
                Disabled = ReadOnly,
                Value = Model.ScrollX,
                ValueChanged = this.Callback<string>(value => { Model.ScrollX = value; OnPropertyChanged(); })
            }));
            BuildPropertyItem(builder, "滚动高度", b => UI.BuildText(b, new InputModel<string>
            {
                Disabled = ReadOnly,
                Value = Model.ScrollY,
                ValueChanged = this.Callback<string>(value => { Model.ScrollY = value; OnPropertyChanged(); })
            }));
        });
    }

    private void BuildToolbar(RenderTreeBuilder builder)
    {
        UI.BuildCheckList(builder, new InputModel<string[]>
        {
            Disabled = ReadOnly,
            Codes = actions,
            Value = Model.Tools,
            ValueChanged = this.Callback<string[]>(value => { Model.Tools = value; OnPropertyChanged(); })
        });
    }

    private void BuildAction(RenderTreeBuilder builder)
    {
        UI.BuildCheckList(builder, new InputModel<string[]>
        {
            Disabled = ReadOnly,
            Codes = actions,
            Value = Model.Actions,
            ValueChanged = this.Callback<string[]>(value => { Model.Actions = value; OnPropertyChanged(); })
        });
    }

    private void SetModel()
    {
        table = new TableModel<Dictionary<string, object>>(UI, Model) { OnQuery = OnQueryDatas };
        code = Service.GetPage(Model);
    }

    private void OnPropertyChanged()
    {
        SetModel(Model);
        OnChanged?.Invoke(Model);
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