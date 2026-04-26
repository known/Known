namespace Known.AI;

/// <summary>
/// 聊天服务接口。
/// </summary>
public interface IChatService : IService
{
    /// <summary>
    /// 异步发送聊天信息。
    /// </summary>
    /// <param name="info">提问信息。</param>
    /// <returns></returns>
    IAsyncEnumerable<string> SendChatAsync(ChatInfo info);

    /// <summary>
    /// 异步获取用户当前助理聊天信息。
    /// </summary>
    /// <param name="userId">用户ID。</param>
    /// <param name="agentId">助理ID。</param>
    /// <returns></returns>
    Task<List<ChatInfo>> GetChatsAsync(string userId, string agentId);

    /// <summary>
    /// 异步清理会话。
    /// </summary>
    /// <param name="info">聊天信息。</param>
    /// <returns></returns>
    Task<Result> ClearChatsAsync(ChatInfo info);

    /// <summary>
    /// 异步分页查询AI聊天。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<ChatInfo>> QueryChatsAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步删除AI聊天。
    /// </summary>
    /// <param name="infos">AI聊天列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteChatsAsync(List<ChatInfo> infos);

    /// <summary>
    /// 异步删除AI聊天。
    /// </summary>
    /// <param name="info">AI聊天信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveChatAsync(ChatInfo info);
}

[Client]
class ChatClient(HttpClient http) : ClientBase(http), IChatService
{
    public async IAsyncEnumerable<string> SendChatAsync(ChatInfo info)
    {
        var url = http.GetRequestUrl("/Chat/SendChat");
        var response = await Http.PostAsJsonAsync(url, info);
        response.EnsureSuccessStatusCode();

        using var responseStream = await response.Content.ReadAsStreamAsync();
        using var jsonDocument = await JsonDocument.ParseAsync(responseStream);

        foreach (var element in jsonDocument.RootElement.EnumerateArray())
        {
            yield return element.GetString();
        }
    }

    public Task<List<ChatInfo>> GetChatsAsync(string userId, string agentId)
    {
        return Http.GetAsync<List<ChatInfo>>($"/Chat/GetChats?userId={userId}&agentId={agentId}");
    }

    public Task<Result> ClearChatsAsync(ChatInfo info)
    {
        return Http.PostAsync("/Chat/ClearChats", info);
    }

    public Task<PagingResult<ChatInfo>> QueryChatsAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<ChatInfo>("/Chat/QueryChats", criteria);
    }

    public Task<Result> DeleteChatsAsync(List<ChatInfo> infos)
    {
        return Http.PostAsync("/Chat/DeleteChats", infos);
    }

    public Task<Result> SaveChatAsync(ChatInfo info)
    {
        return Http.PostAsync("/Chat/SaveChat", info);
    }
}

[WebApi, Service]
class ChatService(Context context, IExtendService extend) : ServiceBase(context), IChatService
{
    public async IAsyncEnumerable<string> SendChatAsync(ChatInfo info)
    {
        var db = Database;
        await SaveChatAsync(db, info);
        if (info.Agent == null)
        {
            yield return $"这是一条测试数据，你说的是：{info.Context}";
        }
        else
        {
            if (info.Agent.Model == null)
            {
                yield return "模型不存在！";
            }
            else
            {
                var messages = await GetChatMessagesAsync(db, info, info.Agent);
                var chats = GetChatStreamingAsync(info.Agent.Model, messages);
                await foreach (var item in chats)
                    yield return item;
            }
        }
    }

    public Task<List<ChatInfo>> GetChatsAsync(string userId, string agentId)
    {
        return Database.Query<SysChat>()
                       .Where(d => d.UserId == userId && d.AgentId == agentId)
                       .OrderBy(d => d.CreateTime)
                       .ToListAsync<ChatInfo>();
    }

    public async Task<Result> ClearChatsAsync(ChatInfo info)
    {
        if (info == null)
            return Result.Error("聊天记录不存在！");

        await Database.DeleteAsync<SysChat>(d => d.UserId == info.UserId && d.AgentId == info.AgentId);
        return Result.Success("清理成功！");
    }

    public Task<PagingResult<ChatInfo>> QueryChatsAsync(PagingCriteria criteria)
    {
        return Database.Query<SysChat>(criteria).ToPageAsync<ChatInfo>();
    }

    public async Task<Result> DeleteChatsAsync(List<ChatInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteAsync<SysChat>(item.Id);
            }
        });
    }

    public async Task<Result> SaveChatAsync(ChatInfo info)
    {
        await SaveChatAsync(Database, info);
        return Result.Success(Language.SaveSuccess);
    }

    private static async Task<List<ChatMessage>> GetChatMessagesAsync(Database db, ChatInfo info, AgentInfo agent)
    {
        var messages = new List<ChatMessage>();
        if (!string.IsNullOrWhiteSpace(agent.Prompt))
            messages.Add(new ChatMessage("system", agent.Prompt));

        var chats = await db.Query<SysChat>()
                            .Where(o => o.SessionId == info.SessionId)
                            .OrderBy(o => o.CreateTime)
                            .ToListAsync();
        if (chats != null && chats.Count > 0)
        {
            foreach (var item in chats)
            {
                if (string.IsNullOrWhiteSpace(item.Context))
                    continue;

                if (item.IsSend)
                    messages.Add(new ChatMessage("user", item.Context));
                else
                    messages.Add(new ChatMessage("assistant", item.Context));
            }
        }
        return messages;
    }

    private async IAsyncEnumerable<string> GetChatStreamingAsync(ModelInfo model, List<ChatMessage> messages)
    {
        switch (model.Type)
        {
            case ChatType.Ollama:
                var ollama = new OllamaClient(model);
                var chats = ollama.GetChatStreamAsync(messages);
                await foreach (var item in chats)
                    yield return item;
                break;
            case ChatType.OpenAI:
                var openai = new OpenAIClient(model);
                var chats1 = openai.GetChatStreamAsync(messages);
                await foreach (var item in chats1)
                    yield return item;
                break;
            case ChatType.Extend:
                var items = extend.GetChatStreamAsync(model, messages);
                await foreach (var item in items)
                    yield return item;
                break;
            default:
                var mocks = GetChatStreamAsync(messages);
                await foreach (var item in mocks)
                    yield return item;
                break;
        }
    }

    private static async IAsyncEnumerable<string> GetChatStreamAsync(List<ChatMessage> messages)
    {
        await Task.Delay(500);
        yield return $"我是模拟AI模型，这是一条模拟测试数据，你说的是：{messages.LastOrDefault()?.Content}";
    }

    private static async Task SaveChatAsync(Database db, ChatInfo item)
    {
        await db.SaveAsync(new SysChat
        {
            UserId = item.UserId,
            SessionId = item.SessionId,
            AgentId = item.Agent.Id,
            AgentName = item.Agent.Name,
            IsSend = item.IsSend,
            Context = item.Context
        });
    }
}