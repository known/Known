namespace Known.Razor;

public class KRadioList : ListField
{
    [Parameter] public bool IsPlain { get; set; }
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
        var css = CssBuilder.Default("form-radio").AddClass("plain", IsPlain).Build();
        builder.Label(css, attr =>
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
            builder.Span(IsPlain && isChecked ? "checked" : "", text);
        });
    }
}