using Known.Extensions;

namespace Known.Razor;

public class KSelect : ListField
{
    private string Text { get; set; }

    [Parameter] public string Icon { get; set; }
    [Parameter] public string EmptyText { get; set; }
    [Parameter] public bool IsList { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        var code = ListItems.FirstOrDefault(c => c.Code == Value);
        Text = code?.Name ?? Value;
    }

    protected override void BuildText(RenderTreeBuilder builder)
    {
        var value = Value;
        var code = ListItems.FirstOrDefault(c => c.Code == value);
        if (code != null)
            value = code.Name;
        builder.Span("text", value);
    }

    protected override void BuildInput(RenderTreeBuilder builder)
    {
        if (!string.IsNullOrWhiteSpace(Icon))
            builder.Icon(Icon);
        if (IsList)
            BuildDataList(builder);
        else
            BuildSelect(builder);
    }

    private void BuildDataList(RenderTreeBuilder builder)
    {
        builder.Input(attr =>
        {
            attr.Type("text").Id(Id).Name(Id).Disabled(!Enabled).Class("select")
                .Value(Text).Required(Required)
                .Placeholder(EmptyText)
                .Add("autocomplete", "off")
                .Add("list", $"list{Id}")
                .OnChange(CreateBinder());
        });
        builder.Icon("fa fa-angle-down");
        builder.Element("datalist", attr =>
        {
            attr.Id($"list{Id}");
            if (ListItems != null && ListItems.Length > 0)
            {
                foreach (var item in ListItems)
                {
                    BuildOption(builder, item.Code, item.Name, item.Code == Value);
                }
            }
        });
    }

    private void BuildSelect(RenderTreeBuilder builder, Action onChanged = null)
    {
        builder.Select(attr =>
        {
            attr.Id(Id).Name(Id).Disabled(!Enabled).OnChange(CreateBinder());
            if (!string.IsNullOrWhiteSpace(EmptyText))
            {
                BuildOption(builder, "", EmptyText, true);
            }
            if (ListItems != null && ListItems.Length > 0)
            {
                foreach (var item in ListItems)
                {
                    BuildOption(builder, item.Code, item.Name, item.Code == Value);
                }
            }
        });
    }

    private static void BuildOption(RenderTreeBuilder builder, string value, string text, bool selected)
    {
        builder.Option(attr =>
        {
            attr.Value(value).Selected(selected);
            builder.Text(text);
        });
    }
}