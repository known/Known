namespace Known.Razor.Components.Fields;

public class ListBox : ListField
{
    private string curItem;

    [Parameter] public Action<CodeInfo> OnItemClick { get; set; }
    [Parameter] public RenderFragment<CodeInfo> ItemTemplace { get; set; }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        var css = CssBuilder.Default("list-box").AddClass("disabled", !Enabled).Build();
        builder.Ul(css, attr =>
        {
            if (ListItems != null && ListItems.Length > 0)
            {
                foreach (var item in ListItems)
                {
                    item.IsActive = curItem == item.Code;
                    var active = item.IsActive ? " active" : "";
                    builder.Li($"item{active}", attr =>
                    {
                        if (Enabled)
                        {
                            attr.OnClick(Callback(e => OnClick(item)));
                        }
                        BuildItem(builder, item);
                    });
                }
            }
        });
    }

    private void BuildItem(RenderTreeBuilder builder, CodeInfo item)
    {
        if (ItemTemplace != null)
            builder.Fragment(ItemTemplace, item);
        else
            builder.Text(item.Name);
    }

    private void OnClick(CodeInfo info)
    {
        curItem = info.Code;
        OnItemClick?.Invoke(info);
    }
}