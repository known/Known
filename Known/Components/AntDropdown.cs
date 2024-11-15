using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant下拉菜单组件类。
/// </summary>
public class AntDropdown : Dropdown
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 取得或设置下拉框设置模型。
    /// </summary>
    [Parameter] public DropdownModel Model { get; set; }

    /// <summary>
    /// 初始化组件。
    /// </summary>
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(string);
        base.OnInitialized();

        if (!string.IsNullOrWhiteSpace(Model?.Icon))
            ChildContent = BuildIcon;
        else if (!string.IsNullOrWhiteSpace(Model?.Text))
            ChildContent = BuildText;
        else if (!string.IsNullOrWhiteSpace(Model?.TextIcon))
            ChildContent = BuildTextIcon;
        else if (!string.IsNullOrWhiteSpace(Model?.TextButton))
            ChildContent = BuildTextButton;

        if (!string.IsNullOrWhiteSpace(Model?.TriggerType))
            Trigger = GetTriggers(Model?.TriggerType);

        if (Model?.Overlay != null)
            Overlay = Model?.Overlay;
        else if (Model?.Items != null && Model?.Items.Count > 0)
            Overlay = BuildOverlay;
    }

    private void BuildIcon(RenderTreeBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(Model?.Tooltip))
        {
            builder.Icon(Model?.Icon);
        }
        else
        {
            builder.Component<Tooltip>()
                   .Set(c => c.Title, Model?.Tooltip)
                   .Set(c => c.ChildContent, b => b.Icon(Model?.Icon))
                   .Build();
        }
        if (!string.IsNullOrWhiteSpace(Model?.Text))
            builder.Span(Model?.Text);
    }

    private void BuildText(RenderTreeBuilder builder)
    {
        builder.OpenElement("a").Class("ant-dropdown-link").PreventDefault().Children(() =>
        {
            builder.Markup(Model?.Text);
            builder.Component<Icon>().Set(c => c.Type, "down").Build();
        }).Close();
    }

    private void BuildTextIcon(RenderTreeBuilder builder)
    {
        builder.Span().Role("img").Text(Model?.TextIcon).Close();
    }

    private void BuildTextButton(RenderTreeBuilder builder)
    {
        builder.Component<Button>()
               .Set(c => c.ChildContent, b =>
               {
                   b.Markup(Model?.TextButton);
                   b.Component<Icon>().Set(c => c.Type, "down").Build();
               })
               .Build();
    }

    private void BuildOverlay(RenderTreeBuilder builder)
    {
        builder.Component<Menu>().Set(c => c.ChildContent, BuildMenu).Build();
    }

    private void BuildMenu(RenderTreeBuilder builder)
    {
        foreach (var item in Model?.Items)
        {
            builder.Component<MenuItem>()
                   .Set(c => c.Key, item.Id)
                   .Set(c => c.Disabled, !item.Enabled)
                   .Set(c => c.ChildContent, b => BuildMenuItem(b, item))
                   .Build();
        }
    }

    private void BuildMenuItem(RenderTreeBuilder builder, ActionInfo item)
    {
        builder.Div().OnClick(this.Callback<MouseEventArgs>(e => Model?.OnItemClick?.Invoke(item)))
               .Children(() => BuildItemName(builder, item))
               .Close();
    }

    private static void BuildItemName(RenderTreeBuilder builder, ActionInfo item)
    {
        if (!string.IsNullOrWhiteSpace(item.Icon))
        {
            builder.Component<Icon>()
                   .Set(c => c.Type, item.Icon)
                   .Set(c => c.Theme, "outline")
                   .Build();
        }
        builder.Span(item.Name);
    }

    private static Trigger[] GetTriggers(string triggerType)
    {
        if (triggerType == "Click")
            return [AntDesign.Trigger.Click];
        else if (triggerType == "ContextMenu")
            return [AntDesign.Trigger.ContextMenu];
        else if (triggerType == "Hover")
            return [AntDesign.Trigger.Hover];
        else if (triggerType == "Focus")
            return [AntDesign.Trigger.Focus];

        return [AntDesign.Trigger.None];
    }
}