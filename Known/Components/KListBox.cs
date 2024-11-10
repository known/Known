namespace Known.Components;

/// <summary>
/// 列表框组件类。
/// </summary>
public class KListBox : BaseComponent
{
    private string curItem;
    private string searchKey;
    private List<CodeInfo> items = [];

    /// <summary>
    /// 取得或设置是否显示搜索。
    /// </summary>
    [Parameter] public bool ShowSearch { get; set; }

    /// <summary>
    /// 取得或设置列表项数据源。
    /// </summary>
    [Parameter] public List<CodeInfo> DataSource { get; set; }

    /// <summary>
    /// 取得或设置列表项单击事件。
    /// </summary>
    [Parameter] public Func<CodeInfo, Task> OnItemClick { get; set; }

    /// <summary>
    /// 取得或设置列表项呈现模板。
    /// </summary>
    [Parameter] public RenderFragment<CodeInfo> ItemTemplate { get; set; }

    /// <summary>
    /// 异步设置组件参数。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        items = DataSource;
    }

    /// <summary>
    /// 呈现列表框组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-list-box", () =>
        {
            if (ShowSearch)
                BuildSearch(builder);

            var style = ShowSearch ? $"top:32px" : "";
            builder.Ul().Class("kui-list-box-body").Style(style).Children(() =>
            {
                if (items != null && items.Count > 0)
                {
                    if (string.IsNullOrWhiteSpace(curItem))
                        curItem = items[0].Code;

                    foreach (var item in items)
                    {
                        builder.Li().Class(item.Code == curItem ? "active" : "")
                               .OnClick(this.Callback(() => OnClick(item)))
                               .Children(() => BuildItem(builder, item))
                               .Close();
                    }
                }
            }).Close();
        });
    }

    private void BuildSearch(RenderTreeBuilder builder)
    {
        builder.Div("kui-list-box-search", () =>
        {
            UI.BuildSearch(builder, new InputModel<string>
            {
                Placeholder = Language["Tip.EnterKeyword"],
                Value = searchKey,
                ValueChanged = this.Callback<string>(value =>
                {
                    searchKey = value;
                    items = DataSource;
                    if (!string.IsNullOrWhiteSpace(searchKey))
                        items = items?.Where(c => c.Code.Contains(searchKey) || c.Name.Contains(searchKey)).ToList();
                    StateChanged();
                })
            });
        });
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