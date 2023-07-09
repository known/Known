namespace Known.Razor;

public class DataComponent<TItem> : BaseComponent
{
    private bool isInitialized;
    private bool isQuery;
    protected List<QueryInfo> query;
    protected PagingCriteria criteria;

    public DataComponent()
    {
        isInitialized = false;
        QueryContext = new QueryContext();
        SelectedItems = new List<TItem>();
    }

    internal QueryContext QueryContext { get; }
    internal int TotalCount { get; set; }
    internal int PageSize => criteria.PageSize;
    internal List<TItem> SelectedItems { get; set; }
    public string EmptyText { get; set; } = Language.NoDataFound;
    protected string Style { get; set; }
    protected string ContainerStyle { get; set; }
    protected string ContentStyle { get; set; }
    protected string[] OrderBys { get; set; }
    protected bool ShowQuery { get; set; }
    protected bool ShowPager { get; set; } = true;
    internal Dictionary<string, object> Sums { get; set; }
    protected List<ButtonInfo> Tools { get; set; }
    protected object DefaultQuery { get; set; }
    [Parameter] public List<TItem> Data { get; set; }

    protected virtual void BuildQuerys(RenderTreeBuilder builder) { }
    protected virtual Task<PagingResult<TItem>> OnQueryData(PagingCriteria criteria) => Task.FromResult(new PagingResult<TItem>());
    protected virtual List<string> GetSumColumns() => null;

    protected bool HasButton(ButtonInfo button)
    {
        var user = CurrentUser;
        if (user == null)
            return false;

        return button.IsInMenu(Id);
    }

    protected virtual async void QueryData(bool isQuery = false)
    {
        this.isQuery = isQuery;
        await QueryPageData();
    }

    protected override void OnParametersSet()
    {
        QueryContext.Model = DefaultQuery;
    }

    protected override async Task OnInitializedAsync()
    {
        criteria = new PagingCriteria(1);
        criteria.PageSize = Setting.Info.PageSize;

        await AddVisitLogAsync();
        await InitPageAsync();

        if (Data == null)
            await QueryPageData();

        isInitialized = true;
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!isInitialized)
            return;

        if (!Context.Check.IsCheckKey)
        {
            BuildAuthorize(builder);
            return;
        }

        BuildPage(builder);
    }

    protected virtual Task InitPageAsync() => Task.CompletedTask;
    protected virtual void BuildPage(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default(ContainerStyle).AddClass(Style).Build();
        builder.Div(css, attr =>
        {
            builder.Div("toolbar", attr =>
            {
                BuildQuery(builder);
                BuildTool(builder);
            });

            var css = CssBuilder.Default(ContentStyle)
                                .AddClass("hasToolbar", ShowQuery)
                                .AddClass("hasPager", ShowPager)
                                .Build();
            builder.Div(css, attr => BuildContent(builder));
            BuildPager(builder);
        });
    }

    internal virtual void BuildContent(RenderTreeBuilder builder) { }
    internal void BuildEmpty(RenderTreeBuilder builder) => builder.Component<Empty>().Set(c => c.Text, EmptyText).Build();

    internal virtual void BuildPager(RenderTreeBuilder builder)
    {
        if (!ShowPager)
            return;

        builder.Component<Pager>()
               .Set(c => c.TotalCount, TotalCount)
               .Set(c => c.PageIndex, criteria.PageIndex)
               .Set(c => c.PageSize, criteria.PageSize)
               .Set(c => c.OnPageChanged, QueryPageData)
               .Build();
    }

    private void BuildQuery(RenderTreeBuilder builder)
    {
        if (!ShowQuery)
            return;

        var hasTool = Tools != null && Tools.Count > 0;
        var query = hasTool ? " right" : " left";
        builder.Div($"query{query}", attr =>
        {
            builder.Component<CascadingValue<QueryContext>>(attr =>
            {
                attr.Set(c => c.IsFixed, true)
                    .Set(c => c.Value, QueryContext)
                    .Set(c => c.ChildContent, BuildQuerys);
            });
        });
    }

    private void BuildTool(RenderTreeBuilder builder)
    {
        if (Tools == null || Tools.Count == 0)
            return;

        builder.Div("tool left", attr =>
        {
            foreach (var item in Tools)
            {
                if (item.Children.Any())
                {
                    var items = item.Children.Select(i => new MenuItem(i, () => OnAction(i))).ToList();
                    builder.Component<Dropdown>()
                           .Set(c => c.Style, "button")
                           .Set(c => c.Title, item.Name)
                           .Set(c => c.Items, items)
                           .Build();
                }
                else
                {
                    BuildButton(builder, item);
                }
            }
        });
    }

    private void BuildButton(RenderTreeBuilder builder, ButtonInfo item)
    {
        builder.Button(item.Name, item.Icon, Callback(() => OnAction(item)), item.Style);
    }

    internal void OnAction(ButtonInfo info, object[] parameters = null)
    {
        var method = GetType().GetMethod(info.Id);
        if (method == null)
            UI.Toast($"{info.Name}方法不存在！");
        else
            method.Invoke(this, parameters);
    }

    private async Task QueryPageData(PagingCriteria pc = null)
    {
        SelectedItems.Clear();
        criteria.ExportMode = ExportMode.None;
        criteria.IsQuery = isQuery;
        criteria.PageIndex = 1;
        if (pc != null)
        {
            criteria.PageIndex = pc.PageIndex;
            criteria.PageSize = pc.PageSize;
        }
        criteria.Query = GetQuery();
        criteria.OrderBys = OrderBys;
        criteria.SumColumns = GetSumColumns();

        if (!ShowPager)
            criteria.PageIndex = -1;

        var data = await OnQueryData(criteria);
        if (data == null)
        {
            TotalCount = 0;
            Data = null;
            Sums = null;
        }
        else
        {
            TotalCount = data.TotalCount;
            Data = data.PageData;
            Sums = data.Sums;
        }
        StateChanged();
    }

    private List<QueryInfo> GetQuery()
    {
        if (query != null && query.Count > 0)
            return query;

        query = QueryContext.GetData();
        if (!query.Any() && DefaultQuery != null)
            query = QueryContext.GetData(DefaultQuery);

        return query;
    }
}