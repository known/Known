using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public class ListBox : BaseComponent
{
    private string curItem;

    [Parameter] public List<CodeInfo> Items { get; set; }
    [Parameter] public Func<CodeInfo, Task> OnItemClick { get; set; }
    [Parameter] public RenderFragment<CodeInfo> ItemTemplate { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Ul().Class("kui-list-box").Children(() =>
        {
            if (Items != null && Items.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(curItem))
                    curItem = Items[0].Code;

                foreach (var item in Items)
                {
                    builder.Li().Class(item.Code == curItem ? "active" : "")
                           .OnClick(this.Callback(() => OnClick(item)))
                           .Children(() => BuildItem(builder, item))
                           .Close();
                }
            }
        }).Close();
    }

    private void BuildItem(RenderTreeBuilder builder, CodeInfo item)
    {
        if (ItemTemplate != null)
            builder.Fragment(ItemTemplate, item);
        else
            builder.Text(item.Name);
    }

    private async Task OnClick(CodeInfo info)
    {
        if (!Enabled)
            return;

        curItem = info.Code;
        await OnItemClick?.Invoke(info);
    }
}