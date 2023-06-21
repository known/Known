namespace Known.Razor.Components.Fields;

public class CheckList : Field
{
    private readonly Dictionary<string, bool> values = new();

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

    protected override void SetContext(FieldContext context)
    {
        base.SetContext(context);
        context.FieldItems = GetListItems();
    }

    protected override void BuildText(RenderTreeBuilder builder) => BuildCheckList(builder, false);
    protected override void BuildInput(RenderTreeBuilder builder) => BuildCheckList(builder, Enabled);

    private void BuildCheckList(RenderTreeBuilder builder, bool enabled)
    {
        values.Clear();
        if (ListItems == null || ListItems.Length == 0)
            return;

        foreach (var item in ListItems)
        {
            values[item.Code] = CheckChecked(item.Code);
            BuildRadio(builder, item.Name, item.Code, enabled, values[item.Code], ColumnCount);
        }
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
                attr.Type("checkbox").Name(Id).Disabled(!enabled)
                    .Value(value).Checked(isChecked)
                    .OnChange(EventCallback.Factory.CreateBinder<bool>(this, isCheck =>
                    {
                        values[value] = isCheck;
                        Value = string.Join(",", values.Where(v => v.Value).Select(k => k.Key));
                        OnValueChange();
                    }, isChecked));
            });
            builder.Span(text);
        });
    }

    private bool CheckChecked(string item)
    {
        if (string.IsNullOrWhiteSpace(Value))
            return false;

        return Value.Split(',').Contains(item);
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