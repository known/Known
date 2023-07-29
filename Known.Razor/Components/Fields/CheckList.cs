namespace Known.Razor.Components.Fields;

public class CheckList : ListField
{
    private readonly Dictionary<string, bool> values = new();

    [Parameter] public int ColumnCount { get; set; }

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
}