using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class PageView : BaseView<PageInfo>
{
    private TableModel<Dictionary<string, object>> table;
    private readonly TableModel<PageColumnInfo> list = new();
    private string codePage;
    private string codeService;
    private string codeRepository;
    private readonly TabModel tab = new();
    private readonly List<CodeInfo> actions = Config.Actions.Select(a => new CodeInfo(a.Id, a.Name)).ToList();

    [Parameter] public EntityInfo Entity { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        SetModel();
        Tab.Items.Add(new ItemModel("视图") { Content = BuildView });
        Tab.Items.Add(new ItemModel("字段") { Content = BuildList });
        Tab.Items.Add(new ItemModel("页面层代码") { Content = BuildPageCode });
        Tab.Items.Add(new ItemModel("服务层代码") { Content = BuildServiceCode });
        Tab.Items.Add(new ItemModel("数据层代码") { Content = BuildRepositoryCode });

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
    private void BuildPageCode(RenderTreeBuilder builder) => BuildCode(builder, codePage);
    private void BuildServiceCode(RenderTreeBuilder builder) => BuildCode(builder, codeService);
    private void BuildRepositoryCode(RenderTreeBuilder builder) => BuildCode(builder, codeRepository);

    private void BuildProperty(RenderTreeBuilder builder)
    {
        builder.Div("setting-row", () =>
        {
            BuildPropertyItem(builder, "显示分页", b => UI.BuildSwitch(b, new InputModel<bool>
            {
                Disabled = ReadOnly,
                Value = Model.ShowPager,
                ValueChanged = this.Callback<bool>(value => { Model.ShowPager = value; OnPropertyChanged(); })
            }));
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
        table = new DemoPageModel(UI, Model, Entity);
        codePage = Service.GetPage(Model, Entity);
        codeService = Service.GetService(Model, Entity);
        codeRepository = Service.GetRepository(Model, Entity);
    }

    private void OnPropertyChanged()
    {
        SetModel(Model);
        OnChanged?.Invoke(Model);
    }
}