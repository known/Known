using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace Known.AI;

/// <summary>
/// AI聊天记录虚拟化组件类。
/// </summary>
public class ChatVirtualize : BaseComponent
{
    /// <summary>
    /// 取得或设置数据项列表。
    /// </summary>
    [Parameter] public List<ChatInfo> Items { get; set; }

    /// <summary>
    /// 取得或设置内容模板。
    /// </summary>
    [Parameter] public RenderFragment<ChatInfo> ChildContent { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Component<Virtualize<ChatInfo>>()
               .Set(c => c.Items, Items)
               .Set(c => c.ItemContent, ChildContent)
               .Build();
    }
}