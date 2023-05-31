namespace Known.Razor.Components.Fields;

public class Select : Field
{
    private string Text { get; set; }

    [Parameter] public string Icon { get; set; }
    [Parameter] public string EmptyText { get; set; }
    [Parameter] public bool IsAuto { get; set; }
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
        ListItems = GetListItems();
        var code = ListItems.FirstOrDefault(c => c.Code == Value);
        Text = code?.Name ?? Value;
    }

    protected override void SetFieldContext(FieldContext context)
    {
        base.SetFieldContext(context);
        context.FieldItems = GetListItems();
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
        if (!string.IsNullOrWhiteSpace(Icon))
            builder.Icon(Icon);
        if (IsAuto)
            BuildInputSelect(builder);
        else
            BuildSelect(builder);
    }

    protected override void SetInputValue(object value) => Value = value?.ToString();

    private CodeInfo[] GetListItems()
    {
        if (Items != null && Items.Length > 0)
            return Items;

        if (CodeAction != null)
            return CodeAction();

        return CodeInfo.GetCodes(Codes).ToArray();
    }

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