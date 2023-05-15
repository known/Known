﻿namespace Known.Razor;

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
    internal List<TItem> SelectedItems { get; set; }
    protected string EmptyText { get; set; } = Language.NoDataFound;
    protected string Style { get; set; }
    protected string ContainerStyle { get; set; }
    protected string ContentStyle { get; set; }
    protected string[] OrderBys { get; set; }
    protected bool ShowQuery { get; set; }
    protected bool ShowCustQuery { get; set; }
    protected bool ShowPager { get; set; } = true;
    protected Dictionary<string, object> Sums { get; set; }
    protected List<ButtonInfo> Tools { get; set; }
    protected object DefaultQuery { get; set; }
    [Parameter] public List<TItem> Data { get; set; }

    protected virtual void BuildQuerys(RenderTreeBuilder builder) { }
    protected virtual Task<PagingResult<TItem>> OnQueryData(PagingCriteria criteria) => Task.FromResult(new PagingResult<TItem>());

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

        var style = string.Empty;
        if (ShowQuery)
            style += " hasQuery";
        if (ShowPager)
            style += " hasPager";

        builder.Div($"{ContainerStyle} {Style}", attr =>
        {
            BuildQuery(builder);
            BuildTool(builder);

            if (Tools != null && Tools.Count > 0 && !ReadOnly)
                style += ShowQuery ? " hasTool" : " onlyTool";

            builder.Div($"{ContentStyle}{style}", attr => BuildContent(builder));
            BuildPager(builder);
        });
        BuildOther(builder);
    }

    protected virtual void BuildContent(RenderTreeBuilder builder) { }
    protected virtual void BuildOther(RenderTreeBuilder builder) { }
    protected void BuildEmpty(RenderTreeBuilder builder) => builder.Component<Empty>().Set(c => c.Text, EmptyText).Build();

    protected virtual List<string> GetSumColumns() => null;

    protected bool HasButton(ButtonInfo button)
    {
        var user = CurrentUser;
        if (user == null)
            return false;

        return button.IsInMenu(Id);
    }

    private void BuildQuery(RenderTreeBuilder builder)
    {
        if (!ShowQuery)
            return;

        builder.Div("query", attr =>
        {
            builder.Component<CascadingValue<QueryContext>>(attr =>
            {
                attr.Set(c => c.IsFixed, true)
                    .Set(c => c.Value, QueryContext)
                    .Set(c => c.ChildContent, BuildTree(BuildQuerys));
            });
        });
    }

    private void BuildTool(RenderTreeBuilder builder)
    {
        if (Tools == null || Tools.Count == 0)
            return;

        var only = ShowQuery ? "" : " only";
        builder.Div($"tool{only}", attr =>
        {
            foreach (var item in Tools)
            {
                builder.Button(item.Name, item.Icon, Callback(() =>
                {
                    var method = GetType().GetMethod(item.Id);
                    if (method == null)
                        UI.Tips($"{item.Name}方法不存在！");
                    else
                        method.Invoke(this, null);
                }), item.Style);
            }
        });
    }

    private void BuildPager(RenderTreeBuilder builder)
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