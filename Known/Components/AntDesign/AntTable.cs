using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant表格组件类。
/// </summary>
/// <typeparam name="TItem">表格数据对象类型。</typeparam>
public class AntTable<TItem> : Table<TItem>, IComContainer where TItem : class, new()
{
    /// <summary>
    /// 取得或设置系统上下文对象实例。
    /// </summary>
    [Parameter] public Context Context { get; set; }

    /// <summary>
    /// 取得或设置表格组件模型对象实例。
    /// </summary>
    [Parameter] public TableModel<TItem> Model { get; set; }

    /// <summary>
    /// 取得或设置是否只读。
    /// </summary>
    [Parameter] public bool IsView { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        Size = TableSize.Small;
        Responsive = true;
        ScrollBarWidth = "8px";
        if (Model != null)
        {
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

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (Model != null)
        {
            if (Model.IsStriped)
                Class = "kui-striped";
            Resizable = Model.Resizable;
            AutoHeight = Model.AutoHeight;
            RowKey = Model.RowKey;
            HidePagination = !Model.ShowPager;
        }
        base.OnParametersSet();
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Cascading<IComContainer>(this, base.BuildRenderTree);
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