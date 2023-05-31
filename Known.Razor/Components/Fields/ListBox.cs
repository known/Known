namespace Known.Razor.Components.Fields;

public class ListBox : Field
{
    private string curItem;

    [Parameter] public Action<CodeInfo> OnItemClick { get; set; }
    [Parameter] public RenderFragment<CodeInfo> ItemTemplace { get; set; }
    [Parameter] public string Codes { get; set; }
    [Parameter] public CodeInfo[] Items { get; set; }
    [Parameter] public Func<CodeInfo[]> CodeAction { get; set; }

    protected CodeInfo[] ListItems { get; private set; }

    public void SetCodes(string codes)
    {
        Codes = codes;
        StateChanged();
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ListItems = GetListItems();
    }

    protected override void SetFieldContext(FieldContext context)
    {
        base.SetFieldContext(context);
        context.FieldItems = GetListItems();
    }

    protected override void BuildChildContent(RenderTreeBuilder builder)
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

    private CodeInfo[] GetListItems()
    {
        if (Items != null && Items.Length > 0)
            return Items;

        if (CodeAction != null)
            return CodeAction();

        return CodeInfo.GetCodes(Codes).ToArray();
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