namespace Known.Razor.Components.Fields;

public class RadioList : ListField
{
    [Parameter] public int ColumnCount { get; set; }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        if (ListItems == null || ListItems.Length == 0)
            return;

        foreach (var item in ListItems)
        {
            BuildRadio(builder, item.Name, item.Code, Enabled, Value == item.Code, ColumnCount);
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
                attr.Type("radio").Name(Id).Disabled(!enabled)
                    .Value(value).Checked(isChecked)
                    .OnChange(CreateBinder());
            });
            builder.Span(text);
        });
    }
}