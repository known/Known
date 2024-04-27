using AntDesign.TableModels;

namespace Known.AntBlazor.Components;

public class AntTable<TItem> : Table<TItem>
{
    [CascadingParameter] public KError Error { get; set; }

    [Parameter] public Context Context { get; set; }
    [Parameter] public PagingCriteria Criteria { get; set; }
    [Parameter] public PagingResult<TItem> Result { get; set; }
    [Parameter] public Func<TItem, string> RowClass { get; set; }

    protected override void OnInitialized()
    {
        Size = TableSize.Small;
        Responsive = true;
        ScrollBarWidth = "8px";
        if (Criteria != null)
        {
            PageIndex = Criteria.PageIndex;
            PageIndexChanged = this.Callback<int>(v => Criteria.PageIndex = v);
            PageSize = Criteria.PageSize;
            PageSizeChanged = this.Callback<int>(v => Criteria.PageSize = v);
        }
        PaginationPosition = "bottomRight";
        if (!OnChange.HasDelegate)
            OnChange = this.Callback<QueryModel<TItem>>(OnDataChange);
        PaginationTemplate = this.BuildTree<(int PageSize, int PageIndex, int Total, string PaginationClass, EventCallback<PaginationEventArgs> HandlePageChange)>(BuildPagination);
        if (RowClass != null)
            RowClassName = r => RowClass.Invoke(r.Data);
        base.OnInitialized();
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

    protected override void OnParametersSet()
    {
        if (Result != null)
        {
            DataSource = Result.PageData;
            Total = Result.TotalCount;
        }
        base.OnParametersSet();
    }

    private async void OnDataChange(QueryModel<TItem> queryModel)
    {
        try
        {
            
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            if (Error != null)
                await Error.HandleAsync(ex);
        }
    }
}