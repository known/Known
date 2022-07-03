/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-04-01     KnownChen
 * ------------------------------------------------------------------------------- */

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public abstract class DataComponent<TItem> : BaseComponent
{
    private int pageIndex;

    public DataComponent()
    {
        QueryContext = new FieldContext();
    }

    protected int TotalCount { get; set; }
    protected FieldContext QueryContext { get; }
    protected abstract string ContainerStyle { get; }
    protected abstract string ContentStyle { get; }

    [Parameter] public bool AutoLoad { get; set; } = true;
    [Parameter] public bool ShowPager { get; set; } = true;
    [Parameter] public int PageSize { get; set; } = 10;
    [Parameter] public string Style { get; set; }
    [Parameter] public string EmptyText { get; set; } = Language.NoDataFound;
    [Parameter] public List<TItem> Data { get; set; }
    [Parameter] public RenderFragment FormTemplate { get; set; }
    [Parameter] public RenderFragment QueryTemplate { get; set; }
    [Parameter] public RenderFragment HeadTemplate { get; set; }
    [Parameter] public RenderFragment<TItem> ItemTemplate { get; set; }
    [Parameter] public Func<PagingCriteria, PagingResult<TItem>> OnQuery { get; set; }

    public void QueryData()
    {
        QueryData(1);
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (AutoLoad)
        {
            QueryData();
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var gridTop = QueryTemplate == null ? 0 : 40;
        var gridBottom = !ShowPager ? 0 : 40;

        builder.Div($"{ContainerStyle} {Style}", attr =>
        {
            BuildQuery(builder);
            builder.Div(ContentStyle, attr =>
            {
                attr.Style($"top:{gridTop}px;bottom:{gridBottom}px;");
                BuildContent(builder);
            });
            BuildPager(builder);
        });
    }

    protected abstract void BuildContent(RenderTreeBuilder builder);

    protected void BuildEmpty(RenderTreeBuilder builder)
    {
        builder.Component<Empty>(attr => attr.Add(nameof(Empty.Text), EmptyText));
    }

    private void BuildQuery(RenderTreeBuilder builder)
    {
        if (QueryTemplate == null)
            return;

        builder.Div("query", attr =>
        {
            builder.Component<CascadingValue<FieldContext>>(attr =>
            {
                attr.Add(nameof(CascadingValue<FieldContext>.IsFixed), true)
                    .Add(nameof(CascadingValue<FieldContext>.Value), QueryContext)
                    .Add(nameof(CascadingValue<FieldContext>.ChildContent), QueryTemplate);
            });
        });
    }

    private void BuildPager(RenderTreeBuilder builder)
    {
        if (!ShowPager)
            return;

        builder.Component<Pager>(attr =>
        {
            attr.Add(nameof(Pager.TotalCount), TotalCount)
                .Add(nameof(Pager.PageIndex), pageIndex)
                .Add(nameof(Pager.PageSize), PageSize)
                .Add(nameof(Pager.OnPageChanged), Callback<int>(e => QueryData(e)));
        });
    }

    private void QueryData(int page)
    {
        pageIndex = page;

        if (OnQuery == null)
            return;

        var criteria = new PagingCriteria(pageIndex)
        {
            Parameter = QueryContext.GetData()
        };
        var data = OnQuery(criteria);
        if (data == null)
            return;

        TotalCount = data.TotalCount;
        if (data.PageData != null)
        {
            Data = data.PageData;
        }
    }
}
