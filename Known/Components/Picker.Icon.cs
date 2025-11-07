namespace Known.Components;

/// <summary>
/// 图标选择器组件类。
/// </summary>
public class IconPicker : BasePicker<string>, ICustomField
{
    private const string KeyCustom = "Custom";
    private TabModel tab;
    private Dictionary<string, List<IconMetaInfo>> iconMetas = [];
    private IconMetaInfo icon;
    private string faType = "Solid";
    private string searchKey;

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        AllowClear = true;
        Title = Language.SelectIcon;
        tab = new TabModel(this);
        foreach (var item in UIConfig.Icons)
        {
            tab.AddTab(item.Key, b => BuildContent(b, item.Key));
        }
        tab.AddTab(KeyCustom, b => BuildContent(b, KeyCustom));
        tab.OnChange = t => icon = null;
        iconMetas = UIConfig.Icons;
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
            else if (key == "FontAwesome")
            {
                BuildFAIconList(builder, key);
            }
            else
            {
                BuildIconList(builder, key);
            }
        });
    }

    private void BuildFAIconList(RenderTreeBuilder builder, string key)
    {
        builder.Div("kui-flex-space", () =>
        {
            BuildIconType(builder, "Solid,Regular,Light,Brand", faType, value => faType = value);
            BuildSearch(builder);
        });

        var items = iconMetas[key];
        icon ??= items.FirstOrDefault();
        var icons = icon.Icons;
        if (!string.IsNullOrWhiteSpace(searchKey))
            icons = [.. icons.Where(i => i.Contains(searchKey))];

        var fa = faType switch
        {
            "Solid" => "fas",
            "Regular" => "far",
            "Light" => "fal",
            "Brand" => "fab",
            _ => "fas",
        };
        builder.Div("font-awesome", () =>
        {
            builder.Div("category", () =>
            {
                foreach (var item in items)
                {
                    item.IsActive = icon.Type == item.Type;
                    var className = "item";
                    if (item.IsActive)
                        className += " active";
                    builder.Div().Class(className)
                           .OnClick(this.Callback(() => icon = items.FirstOrDefault(d => d.Type == item.Type)))
                           .Child(() => builder.Span(item.Type));
                }
            });
            builder.Div("items", () =>
            {
                foreach (var item in icons)
                {
                    var value = $"{fa} fa-{item}";
                    BuildIconItem(builder, value, item);
                }
            });
        });
    }

    private void BuildIconList(RenderTreeBuilder builder, string key)
    {
        var items = iconMetas[key];
        icon ??= items.FirstOrDefault();
        builder.Div("kui-flex-space", () =>
        {
            var category = string.Join(",", items.Select(c => c.Type));
            BuildIconType(builder, category, icon.Type, value => icon = items.FirstOrDefault(d => d.Type == value));
            BuildSearch(builder);
        });
        var icons = icon.Icons;
        if (!string.IsNullOrWhiteSpace(searchKey))
            icons = [.. icons.Where(i => i.Contains(searchKey))];

        builder.Div("items", () =>
        {
            foreach (var item in icons)
            {
                var value = $"{icon.Type},{item}";
                BuildIconItem(builder, value, item);
            }
        });
    }

    private void BuildIconItem(RenderTreeBuilder builder, string value, string name)
    {
        var className = "item";
        if (SelectedItems.Contains(value))
            className += " active";
        builder.Div().Class(className)
               .OnClick(this.Callback(() => OnSelectItem(value)))
               .Child(() =>
               {
                   builder.Icon(value);
                   builder.Span("name", name);
               });
    }

    private void BuildIconType(RenderTreeBuilder builder, string category, string value, Action<string> onChange)
    {
        builder.Component<AntRadioGroup>()
               .Set(c => c.Category, category)
               .Set(c => c.ButtonStyle, AntDesign.RadioButtonStyle.Solid)
               .Set(c => c.Value, value)
               .Set(c => c.OnChange, this.Callback(onChange))
               .Build();
    }

    private void BuildSearch(RenderTreeBuilder builder)
    {
        builder.Search(new InputModel<string>
        {
            Placeholder = "Search",
            Value = searchKey,
            ValueChanged = this.Callback<string>(value => searchKey = value)
        });
    }

    private void OnSelectItem(string item)
    {
        if (!SelectedItems.Remove(item))
            SelectedItems.Add(item);
        StateChanged();
    }
}