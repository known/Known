namespace Known.AI;

/// <summary>
/// AI聊天记录页面组件类。
/// </summary>
[Route("/sys/aichats")]
[Menu(Constants.System, "聊天记录", "unordered-list", 7)]
//[PagePlugin("聊天记录", "unordered-list", PagePluginType.Module, AiLanguage.AiManage, Sort = 5)]
public class ChatList : BaseTablePage<ChatInfo>
{
    private IChatService Service;

    /// <inheritdoc />
    protected override async Task OnInitPageAsync()
    {
        await base.OnInitPageAsync();
        Service = await CreateServiceAsync<IChatService>();

        Table.OnQuery = Service.QueryChatsAsync;
    }

    /// <summary>
    /// 查看一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    [Action] public void View(ChatInfo row) => Table.ViewForm(row);

    /// <summary>
    /// 删除一条数据。
    /// </summary>
    /// <param name="row">表格行绑定的对象。</param>
    [Action] public void Delete(ChatInfo row) => Table.Delete(Service.DeleteChatsAsync, row);

    /// <summary>
    /// 批量删除多条数据。
    /// </summary>
    [Action] public void DeleteM() => Table.DeleteM(Service.DeleteChatsAsync);
}