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

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        if (ListItems == null || ListItems.Length == 0)
            return;

        foreach (var item in ListItems)
        {
            BuildRadio(builder, item.Name, item.Code, Enabled, Value == item.Code, ColumnCount);
        }
    }

    protected override void SetContext(FieldContext context)
    {
        base.SetContext(context);
        context.FieldItems = GetListItems();
    }

    private void BuildRadio(RenderTreeBuilder builder, string text, string value, bool enabled, bool isChecked, int? columnCount = null)
    {
        builder.Label("form-radio", attr =>
        {
            if (columnCount != null && columnCount > 0)
            {
                var width = Utils.Round(100.0 / columnCount.Value, 2);
                attr.Style($"width:{width}%;margin-right:0;");
            }
            builder.Input(attr =>
            {
                attr.Type("radio").Name(Id).Disabled(!enabled)
                    .Value(value).Checked(isChecked)
                    .OnChange(CreateBinder());
            });
            builder.Span(text);
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
}