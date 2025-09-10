using AntDesign;

namespace Known.Extensions;

/// <summary>
/// 按钮组件扩展类。
/// </summary>
public static class ButtonExtension
{
    /// <summary>
    /// 获取查询结果。
    /// </summary>
    /// <param name="infos">按钮信息列表。</param>
    /// <param name="criteria">查询条件。</param>
    /// <returns>查询结果。</returns>
    public static PagingResult<ButtonInfo> ToQueryResult(this List<ButtonInfo> infos, PagingCriteria criteria)
    {
        var query = criteria.Query.FirstOrDefault(q => q.Id == nameof(ButtonInfo.Id));
        if (query != null && !string.IsNullOrWhiteSpace(query.Value))
        {
            if (query.Type == QueryType.NotIn)
                infos = [.. infos.Where(b => !query.Value.Split(',').Contains(b.Id))];
            else
                infos = [.. infos.Where(b => b.Id.Contains(query.Value))];
        }
        //infos = [.. infos.Contains(m => m.Id, criteria)];
        infos = [.. infos.Contains(m => m.Name, criteria)];
        //infos = [.. infos.Contains(m => m.Position, criteria)];
        var position = criteria.GetQueryValue(nameof(ButtonInfo.Position));
        if (!string.IsNullOrWhiteSpace(position))
            infos = [.. infos.Where(b => b.Position?.Contains(position) == true)];
        return infos.ToPagingResult(criteria);
    }

    /// <summary>
    /// 构建按钮组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="info">操作按钮信息对象。</param>
    public static void Button(this RenderTreeBuilder builder, ActionInfo info)
    {
        builder.Component<KButton>()
               .Set(c => c.Enabled, info.Enabled)
               .Set(c => c.Icon, info.Icon)
               .Set(c => c.Name, info.Name)
               .Set(c => c.Type, GetButtonType(info.Style))
               .Set(c => c.Danger, GetButtonDanger(info.Style))
               .Set(c => c.OnClick, info.OnClick)
               .Build();
    }

    /// <summary>
    /// 呈现一个按钮。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="info">操作信息。</param>
    /// <param name="onClick">按钮单击事件。</param>
    public static void Button(this RenderTreeBuilder builder, ActionInfo info, EventCallback<MouseEventArgs> onClick)
    {
        builder.Component<KButton>()
               .Set(c => c.Id, info.Id)
               .Set(c => c.Name, info.Name)
               .Set(c => c.Icon, info.Icon)
               .Set(c => c.Type, GetButtonType(info.Style))
               .Set(c => c.Danger, GetButtonDanger(info.Style))
               .Set(c => c.Enabled, info.Enabled)
               .Set(c => c.Visible, info.Visible)
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    /// <summary>
    /// 呈现一个按钮。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="name">按钮名称。</param>
    /// <param name="onClick">按钮单击事件。</param>
    /// <param name="type">按钮样式，默认primary。</param>
    public static void Button(this RenderTreeBuilder builder, string name, EventCallback<MouseEventArgs> onClick, string type = "primary")
    {
        builder.Component<KButton>()
               .Set(c => c.Name, name)
               .Set(c => c.Type, GetButtonType(type))
               .Set(c => c.Danger, GetButtonDanger(type))
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    internal static void ButtonMore(this RenderTreeBuilder builder, ActionInfo item, bool showIcon = false)
    {
        builder.Component<Button>()
               .Set(c => c.Type, item.ToType())
               .Set(c => c.ChildContent, b =>
               {
                   b.Div("kui-more", () =>
                   {
                       if (showIcon)
                           b.IconName(item.Icon, item.Name);
                       else
                           b.Span(item.Name);
                       b.Icon("down");
                   });
               })
               .Build();
    }

    internal static ButtonType GetButtonType(string style)
    {
        return style switch
        {
            "Default" => ButtonType.Default,
            "default" => ButtonType.Default,
            "Primary" => ButtonType.Primary,
            "primary" => ButtonType.Primary,
            "Dashed" => ButtonType.Dashed,
            "dashed" => ButtonType.Dashed,
            "Link" => ButtonType.Link,
            "link" => ButtonType.Link,
            "Text" => ButtonType.Text,
            "text" => ButtonType.Text,
            _ => ButtonType.Primary
        };
    }

    internal static bool GetButtonDanger(string style)
    {
        return style == "Danger" || style == "danger";
    }
}