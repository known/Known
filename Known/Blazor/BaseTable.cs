using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class BaseTable<TItem> : BaseComponent where TItem : class, new()
{
    protected TableModel<TItem> Table { get; private set; }

    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Table = new TableModel<TItem>(Context);
    }

    protected override void BuildRender(RenderTreeBuilder builder) => UI.BuildTable(builder, Table);
}