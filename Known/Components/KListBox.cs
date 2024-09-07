namespace Known.Components;

/// <summary>
/// 列表框组件类。
/// </summary>
public class KListBox : BaseComponent
{
    private string curItem;

    /// <summary>
    /// 取得或设置列表项对象列表。
    /// </summary>
    [Parameter] public List<CodeInfo> Items { get; set; }

    /// <summary>
    /// 取得或设置列表项单击事件。
    /// </summary>
    [Parameter] public Func<CodeInfo, Task> OnItemClick { get; set; }

    /// <summary>
    /// 取得或设置列表项呈现模板。
    /// </summary>
    [Parameter] public RenderFragment<CodeInfo> ItemTemplate { get; set; }

    /// <summary>
    /// 呈现列表框组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
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