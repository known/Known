﻿namespace Known.Razor.Components;

public class Menu : BaseComponent
{
    [Parameter] public string Style { get; set; }
    [Parameter] public MenuItem CurItem { get; set; }
    [Parameter] public MenuItem[] Items { get; set; }
    [Parameter] public EventCallback<MenuItem> OnChanged { get; set; }
    [Parameter] public bool OnlyIcon { get; set; }
    [Parameter] public bool TextIcon { get; set; }
    [Parameter] public int Column { get; set; }

    private int ItemWidth
    {
        get
        {
            if (Column > 0)
                return 100 / Column;

            if (Items == null || Items.Length == 0)
                return 100;

            return 100 / Items.Length;
        }
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Ul(Style, attr =>
        {
            if (Items != null && Items.Length != 0)
            {
                foreach (var item in Items)
                {
                    BuildItem(builder, item);
                }
            }
        });
    }

    private void BuildItem(RenderTreeBuilder builder, MenuItem item)
    {
        builder.Li($"menu-item {Active(item.Id)}", attr =>
        {
            attr.OnClick(Callback(e => OnItemClick(item)));

            if (OnlyIcon)
                BuildOnlyIconItem(builder, item);
            else if (TextIcon)
                BuildTextIconItem(builder, attr, item);
            else
                BuildTextItem(builder, item);
        });
    }

    private static void BuildOnlyIconItem(RenderTreeBuilder builder, MenuItem item)
    {
        builder.Icon(item.Icon, attr =>
        {
            attr.Title(item.Name);
            BuildBadge(builder, item);
        });
    }

    private void BuildTextIconItem(RenderTreeBuilder builder, AttributeBuilder attr, MenuItem item)
    {
        attr.Style($"width:{ItemWidth}%");
        builder.Icon(item.Icon, attr =>
        {
            attr.Style($"background-color:{item.Color}");
            BuildBadge(builder, item);
        });
        builder.Span("name", item.Name);
    }

    private static void BuildTextItem(RenderTreeBuilder builder, MenuItem item)
    {
        builder.Text(item.Name);
        BuildBadge(builder, item);
    }

    private static void BuildBadge(RenderTreeBuilder builder, MenuItem item)
    {
        if (item.Badge > 0)
        {
            builder.Span("badge danger", $"{item.Badge}");
        }
    }

    private void OnItemClick(MenuItem item)
    {
        CurItem = item;
        if (OnChanged.HasDelegate)
            OnChanged.InvokeAsync(item);
    }

    private string Active(string id) => CurItem != null && CurItem.Id == id ? "active" : "";
}

public class MenuItem : MenuInfo
{
    public MenuItem()
    {
        Children = new List<MenuItem>();
    }

    public MenuItem(string code, string name, string icon = null) : this()
    {
        Code = code;
        Name = name;
        Icon = icon;
    }

    public MenuItem(string name, string icon, Type type, string description = null) : base(type.Name, name, icon, description)
    {
        Code = type.Name;
        Target = type.FullName;
        ComType = type;
        Children = new List<MenuItem>();
    }

    public Type ComType { get; set; }
    public Dictionary<string, object> ComParameters { get; set; }
    public MenuItem Previous { get; set; }
    public List<MenuItem> Children { get; set; }

    public static MenuItem From(MenuInfo menu)
    {
        return new MenuItem
        {
            Id = menu.Id,
            ParentId = menu.ParentId,
            Code = menu.Code,
            Name = menu.Name,
            Icon = menu.Icon,
            Description = menu.Description,
            Target = menu.Target,
            Sort = menu.Sort,
            Color = menu.Color,
            Badge = menu.Badge,
            Buttons = menu.Buttons,
            Actions = menu.Actions,
            Columns = menu.Columns
        };
    }
}