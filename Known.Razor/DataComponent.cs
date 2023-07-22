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
    protected bool ShowPager { get; set; } = true;
    internal bool HasTool { get; set; }
    internal bool HasQuery { get; set; }
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
            attr.Id(Id);
            if (HasTool || HasQuery)
            {
                builder.Div("data-top", attr =>
                {
                    BuildTool(builder);
                    BuildQuery(builder);
                });
            }

            var css = CssBuilder.Default(ContentStyle)
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
               .Set(c => c.OnPageChanged, OnPageChanged)
               .Build();
    }

    private void BuildQuery(RenderTreeBuilder builder)
    {
        if (!HasQuery)
            return;

        builder.Div("query", attr =>
        {
            builder.Cascading(QueryContext, BuildQuerys);
        });
    }

    private void BuildTool(RenderTreeBuilder builder)
    {
        if (!HasTool)
            return;

        builder.Component<Toolbar>()
               .Set(c => c.Style, "grid-tool")
               .Set(c => c.Tools, Tools)
               .Set(c => c.OnAction, OnAction)
               .Build();
    }

    private void OnAction(ButtonInfo info) => OnAction(info, null);

    internal void OnAction(ButtonInfo info, object[] parameters)
    {
        var method = GetType().GetMethod(info.Id);
        if (method == null)
            UI.Toast($"{info.Name}方法不存在！");
        else
            method.Invoke(this, parameters);
    }

    private async Task OnPageChanged(PagingCriteria criteria)
    {
        await QueryPageData(criteria);
        StateChanged();
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