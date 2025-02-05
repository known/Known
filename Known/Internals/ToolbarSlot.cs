namespace Known.Internals;

class ToolbarSlot<TItem> : BaseComponent where TItem : class, new()
{
    private PagingResult<TItem> result;

    [Parameter] public TableModel<TItem> Table { get; set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Table.OnRefreshStatis = OnRefreshStatis;
    }

    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Fragment(Table.TopStatis, result);
    }

    private Task OnRefreshStatis(PagingResult<TItem> result)
    {
        this.result = result;
        return StateChangedAsync();
    }
}