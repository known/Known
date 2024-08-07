﻿namespace Known.AntBlazor.Components;

public class AntTable<TItem> : Table<TItem> where TItem : class, new()
{
    [CascadingParameter] public KError Error { get; set; }

    [Parameter] public Context Context { get; set; }
    [Parameter] public TableModel<TItem> Model { get; set; }

    protected override void OnInitialized()
    {
        Size = TableSize.Small;
        Responsive = true;
        ScrollBarWidth = "8px";
        if (Model != null)
        {
            Resizable = Model.Resizable;
            RowKey = Model.RowKey;
            HidePagination = !Model.ShowPager;
            PageSizeChanged = this.Callback<int>(e => PageIndex = 1);
            if (Model.TreeChildren != null)
                TreeChildren = Model.TreeChildren;
            if (Model.RowClass != null)
                RowClassName = r => Model.RowClass.Invoke(r.Data);
        }
        PaginationPosition = "bottomRight";
        PaginationTemplate = this.BuildTree<(int PageSize, int PageIndex, int Total, string PaginationClass, EventCallback<PaginationEventArgs> HandlePageChange)>(BuildPagination);
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        if (Model != null)
        {
            if (Model.ShowPager)
            {
                PageIndex = Model.Criteria.PageIndex;
                PageSize = Model.Criteria.PageSize;
            }

            if (Model.Result != null)
            {
                DataSource = Model.Result.PageData;
                Total = Model.Result.TotalCount;
            }
        }
        base.OnParametersSet();
    }

    private void BuildPagination(RenderTreeBuilder builder, (int PageSize, int PageIndex, int Total, string PaginationClass, EventCallback<PaginationEventArgs> HandlePageChange) tuple)
    {
        Func<PaginationTotalContext, string> showTotal = ctx => Context?.Language["Page.Total"].Replace("{total}", $"{ctx.Total}");
        builder.Component<Pagination>()
               .Set(c => c.Class, tuple.PaginationClass)
               .Set(c => c.Total, tuple.Total)
               .Set(c => c.PageSize, tuple.PageSize)
               .Set(c => c.Current, tuple.PageIndex)
               .Set(c => c.ShowTotal, showTotal)
               .Set(c => c.ShowSizeChanger, true)
               .Set(c => c.ShowQuickJumper, true)
               .Set(c => c.OnChange, tuple.HandlePageChange)
               .Build();
    }
}