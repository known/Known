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

    protected override void SetFieldContext(FieldContext context)
    {
        base.SetFieldContext(context);
        context.FieldItems = GetListItems();
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        if (ReadOnly)
            Enabled = false;

        values.Clear();
        if (ListItems == null || ListItems.Length == 0)
            return;

        builder.Div(attr =>
        {
            foreach (var item in ListItems)
            {
                values[item.Code] = CheckChecked(item.Code);
                BuildRadio(builder, "checkbox", item.Name, item.Code, Enabled, values[item.Code], (isCheck, value) =>
                {
                    values[value] = isCheck;
                    Value = string.Join(",", values.Where(v => v.Value).Select(k => k.Key));
                }, ColumnCount);
            }
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