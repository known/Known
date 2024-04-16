using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class BaseTable<TItem> : BaseComponent where TItem : class, new()
{
    protected TableModel<TItem> Table { get; private set; }

    public IEnumerable<TItem> SelectedRows => Table.SelectedRows;

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Table = new TableModel<TItem>(Context);
    }

    protected override void BuildRender(RenderTreeBuilder builder) => builder.BuildTable(Table);
    protected void OnToolClick(ActionInfo info) => OnAction(info, null);
    protected void OnActionClick(ActionInfo info, TItem item) => OnAction(info, [item]);
    protected void OnActionClick<TModel>(ActionInfo info, TModel item) => OnAction(info, [item]);

    public Task RefreshAsync() => Table.RefreshAsync();
}