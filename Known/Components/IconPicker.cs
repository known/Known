namespace Known.Components;

/// <summary>
/// 图标选择器组件类。
/// </summary>
public class IconPicker : BasePicker<string>, ICustomField
{
    private const string KeyCustom = "Custom";
    private readonly TabModel tab = new();
    private Dictionary<string, List<string>> icons = [];
    private string searchKey;

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        AllowClear = true;
        Title = Language["Title.SelectIcon"];
        foreach (var item in UIConfig.Icons)
        {
            tab.AddTab(item.Key, b => BuildContent(b, item.Key));
        }
        tab.AddTab(KeyCustom, b => BuildContent(b, KeyCustom));
        icons = UIConfig.Icons;
    }

    /// <inheritdoc />
    protected override void BuildTextBox(RenderTreeBuilder builder)
    {
        if (Value != null && !string.IsNullOrWhiteSpace(Value.ToString()))
            builder.Span().Class("kui-pick-icon").Child(() => builder.Icon(Value.ToString()));
        base.BuildTextBox(builder);
    }

    /// <inheritdoc />
    protected override void BuildContent(RenderTreeBuilder builder) => builder.Tabs(tab);

    /// <inheritdoc />
    protected override void OnValueChanged(List<string> items)
    {
        if (ValueChanged.HasDelegate)
            ValueChanged.InvokeAsync(items?.FirstOrDefault());
    }

    private void BuildContent(RenderTreeBuilder builder, string key)
    {
        var value = SelectedItems.Count == 0 ? "" : SelectedItems[0];
        builder.Div("kui-icon-picker", () =>
        {
            if (key == KeyCustom)
            {
                builder.TextBox(new InputModel<string>
                {
                    Value = value,
                    ValueChanged = this.Callback<string>(value =>
                    {
                        SelectedItems.Clear();
                        SelectedItems.Add(value);
                    })
                });
            }
            else
            {
                BuildSearch(builder);
                BuildIconList(builder, key);
            }
        });
    }

    private void BuildIconList(RenderTreeBuilder builder, string key)
    {
        var items = icons[key];
        if (!string.IsNullOrWhiteSpace(searchKey))
            items = items.Where(i => i.Contains(searchKey)).ToList();

        builder.Div("items", () =>
        {
            foreach (var item in items)
            {
                var className = "item";
                if (SelectedItems.Contains(item))
                    className += " active";
                builder.Div().Class(className)
                       .OnClick(this.Callback(() => OnSelectItem(item)))
                       .Child(() =>
                       {
                           //if (key == "FontAwesome")
                           //    builder.Span(item.Icon, "");
                           //else
                           builder.Icon(item);
                           builder.Span("name", item);
                       });
            }
        });
    }

    private void BuildSearch(RenderTreeBuilder builder)
    {
        builder.Div("search", () =>
        {
            builder.Search(new InputModel<string>
            {
                Placeholder = "Search",
                Value = searchKey,
                ValueChanged = this.Callback<string>(value =>
                {
                    searchKey = value;
                    StateChanged();
                })
            });
        });
    }

    private void OnSelectItem(string item)
    {
        if (!SelectedItems.Remove(item))
            SelectedItems.Add(item);
        StateChanged();
    }
}