namespace Known.Razor.Components.Fields;

public class CheckBox : Field
{
    [Parameter] public string Text { get; set; }
    [Parameter] public bool Checked { get; set; }

    protected override void BuildChildText(RenderTreeBuilder builder) => BuildRadio(builder, "checkbox", Text, "True", false, IsChecked);

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        BuildRadio(builder, "checkbox", Text, "True", Enabled, IsChecked, (isCheck, value) =>
        {
            Value = isCheck ? "True" : "False";
        });
    }

    private bool IsChecked => Checked || Value == "True";
}

public class CheckList : ListField
{
    private readonly Dictionary<string, bool> values = new();

    protected override void BuildChildText(RenderTreeBuilder builder)
    {
        Enabled = false;
        BuildChildContent(builder);
    }

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        values.Clear();
        if (ListItems == null || ListItems.Length == 0)
            return;

        foreach (var item in ListItems)
        {
            values[item.Code] = CheckChecked(item.Code);
            BuildRadio(builder, "checkbox", item.Name, item.Code, Enabled, values[item.Code], (isCheck, value) =>
            {
                values[value] = isCheck;
                Value = string.Join(",", values.Where(v => v.Value).Select(k => k.Key));
            }, ColumnCount);
        }
    }

    private bool CheckChecked(string item)
    {
        if (string.IsNullOrWhiteSpace(Value))
            return false;

        return Value.Split(',').Contains(item);
    }
}

public class RadioList : ListField
{
    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        if (ListItems == null || ListItems.Length == 0)
            return;

        foreach (var item in ListItems)
        {
            BuildRadio(builder, "radio", item.Name, item.Code, Enabled, Value == item.Code, columnCount: ColumnCount);
        }
    }
}