namespace Known.Razor.Components.Fields;

public class ListBox : ListField
{
    private string curItem;

    [Parameter] public Action<CodeInfo> OnItemClick { get; set; }
    [Parameter] public RenderFragment<CodeInfo> ItemTemplace { get; set; }

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

public class Select : ListField
{
    private string Text { get; set; }

    [Parameter] public string Icon { get; set; }
    [Parameter] public string EmptyText { get; set; }
    [Parameter] public bool IsAuto { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        var code = ListItems.FirstOrDefault(c => c.Code == Value);
        Text = code?.Name ?? Value;
    }

    protected override void BuildChildText(RenderTreeBuilder builder)
    {
        var value = Value;
        var code = ListItems.FirstOrDefault(c => c.Code == value);
        if (code != null)
            value = code.Name;
        builder.Span("text", value);
    }

    protected override void BuildChildContent(RenderTreeBuilder builder)
    {
        Input.BuildIcon(builder, Icon);
        if (IsAuto)
            BuildInputSelect(builder);
        else
            BuildSelect(builder);
    }

    protected override void SetInputValue(object value) => Value = value?.ToString();

    private void BuildInputSelect(RenderTreeBuilder builder)
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