﻿namespace Known.Internals;

class IconInfo
{
    public string Icon { get; set; }

    public override string ToString() => Icon;
}

class IconPicker : BasePicker<IconInfo>, ICustomField
{
    private const string KeyCustom = "Custom";
    private readonly TabModel tab = new();
    private Dictionary<string, List<IconInfo>> icons = [];
    private string searchKey;

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
        icons = UIConfig.Icons.ToDictionary(k => k.Key, v => v.Value.Select(x => new IconInfo { Icon = x }).ToList());
    }

    protected override void BuildTextBox(RenderTreeBuilder builder)
    {
        if (Value != null && !string.IsNullOrWhiteSpace(Value.ToString()))
            builder.Span().Class("kui-pick-icon").Child(() => builder.Icon(Value.ToString()));
        base.BuildTextBox(builder);
    }

    protected override void BuildContent(RenderTreeBuilder builder) => builder.Tabs(tab);

    protected override void OnValueChanged(List<IconInfo> items)
    {
        if (ValueChanged.HasDelegate)
            ValueChanged.InvokeAsync(items?.FirstOrDefault()?.Icon);
    }

    private void BuildContent(RenderTreeBuilder builder, string key)
    {
        var value = SelectedItems.Count == 0 ? "" : SelectedItems[0].Icon;
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
                        SelectedItems.Add(new IconInfo { Icon = value });
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
            items = items.Where(i => i.Icon.Contains(searchKey)).ToList();

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
                           builder.Icon(item.Icon);
                           builder.Span("name", item.Icon);
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

    private void OnSelectItem(IconInfo item)
    {
        if (!SelectedItems.Remove(item))
            SelectedItems.Add(item);
        StateChanged();
    }
}