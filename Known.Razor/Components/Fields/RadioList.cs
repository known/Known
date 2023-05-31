namespace Known.Razor.Components.Fields;

public class RadioList : Field
{
    [Parameter] public int ColumnCount { get; set; }
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

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        if (ListItems == null || ListItems.Length == 0)
            return;

        foreach (var item in ListItems)
        {
            BuildRadio(builder, "radio", item.Name, item.Code, Enabled, Value == item.Code, columnCount: ColumnCount);
        }
    }

    protected override void SetFieldContext(FieldContext context)
    {
        base.SetFieldContext(context);
        context.FieldItems = GetListItems();
    }

    private CodeInfo[] GetListItems()
    {
        if (Items != null && Items.Length > 0)
            return Items;

        if (CodeAction != null)
            return CodeAction();

        return CodeInfo.GetCodes(Codes).ToArray();
    }
}