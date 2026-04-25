namespace Known.AI;

/// <summary>
/// AI聊天信息类。
/// </summary>
/// <param name="Role">会话角色。</param>
/// <param name="Content">会话内容。</param>
public record ChatMessage(string Role, string Content);

/// <summary>
/// AI扩展服务接口。
/// </summary>
public interface IExtendService
{
    /// <summary>
    /// 异步获取扩展聊天响应流。
    /// </summary>
    /// <param name="model">AI大模型。</param>
    /// <param name="messages">聊天信息列表。</param>
    /// <returns></returns>
    IAsyncEnumerable<string> GetChatStreamAsync(ModelInfo model, List<ChatMessage> messages);
}

class ExtendService : IExtendService
{
    public async IAsyncEnumerable<string> GetChatStreamAsync(ModelInfo model, List<ChatMessage> messages)
    {
        await Task.Delay(500);
        yield return $"这是一条扩展测试数据，你说的是：{messages.LastOrDefault()?.Content}";
    }
}