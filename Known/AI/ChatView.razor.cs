namespace Known.AI;

/// <summary>
/// AI聊天组件类。
/// </summary>
public partial class ChatView
{
    private List<ChatInfo> chats = [];
    private IChatService Service;
    private string message;
    private bool sendding;
    private string sessionId;
    private string agentId;
    private bool shouldRender = true;
    private ChatInfo resp;

    private string Placeholder => $"给 {Agent?.Name ?? "AI助理"} 发送消息，Enter键发送，Shift+Enter键换行";
    private string TitleSend => string.IsNullOrWhiteSpace(message) ? "请输入你的问题" : "发送消息";
    private string ActionClass => CssBuilder.Default("action").AddClass("disable", sendding || string.IsNullOrWhiteSpace(message)).BuildClass();

    /// <summary>
    /// 取得或设置AI助理信息。
    /// </summary>
    [Parameter] public AgentInfo Agent { get; set; }

    /// <summary>
    /// 取得或设置消息内容模板。
    /// </summary>
    [Parameter] public RenderFragment<ChatInfo> Message { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Service = await CreateServiceAsync<IChatService>();
    }

    /// <inheritdoc />
    protected override async Task OnParameterAsync()
    {
        await base.OnParameterAsync();
        if (agentId != Agent?.Id)
        {
            shouldRender = true;
            agentId = Agent?.Id;
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        //if (firstRender)
        //    await JSRuntime.InvokeVoidAsync("loadChatScript");

        if (shouldRender)
        {
            shouldRender = false;
            sessionId = Utils.GetNextId();
            chats = await Service.GetChatsAsync(CurrentUser?.Id, Agent?.Id);
            if (chats != null && chats.Count > 0)
                sessionId = chats[0].SessionId;
            await StateChangedAsync();
            await JSRuntime.HighlightAllAsync();
        }
    }

    private void OnCopy(string text)
    {
        JSRuntime.CopyTextAsync(text);
        UI.Success(Language.CopySuccess);
    }

    private void OnNew()
    {
        sessionId = Utils.GetNextId();
        chats.Clear();
    }

    private void OnEditSend(ChatInfo info)
    {
        info.IsEdit = false;
        Send(info.Context);
    }

    private void OnRegenerate(ChatInfo info)
    {
    }

    private void OnSend()
    {
        if (sendding || string.IsNullOrWhiteSpace(message))
            return;

        Send(message);
    }

    private void OnMessageChange(ChangeEventArgs args)
    {
        message = args.Value?.ToString();
    }

    private void OnMessageKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !e.ShiftKey)
        {
            OnSend();
        }
    }

    private async Task OnClearAsync()
    {
        if (chats == null || chats.Count == 0)
        {
            UI.Info("没有会话记录！");
            return;
        }

        var result = await Service.ClearChatsAsync(chats[0]);
        UI.Result(result, () =>
        {
            sessionId = Utils.GetNextId();
            chats.Clear();
            return StateChangedAsync();
        });
    }

    private void Send(string context)
    {
        try
        {
            var chat = GetChatInfo(context, true);
            chats.Add(chat);
            sendding = true;
            Task.Run(async () => await SendAsync(chat)).ContinueWith(task =>
            {
                message = "";
                sendding = false;
                var chat = GetChatInfo(resp.Context, false);
                Service.SaveChatAsync(chat);
                StateChangedAsync();
            });
        }
        catch (Exception ex)
        {
            sendding = false;
            UI.Error(ex.Message);
        }
    }

    private async Task SendAsync(ChatInfo info)
    {
        resp = new ChatInfo { Context = "思考中..." };
        chats.Add(resp);
        await StateChangedAsync();
        await Task.Delay(50);

        var result = Service.SendChatAsync(info);
        var sb = new StringBuilder();
        await foreach (var item in result)
        {
            sb.Append(item);
            resp.Context = sb.ToString();
            await Task.Delay(30);
            await StateChangedAsync();
            //await JS.RunVoidAsync("scrollToBottom('kaiScroll');");
        }
        await JSRuntime.HighlightAllAsync();
    }

    private ChatInfo GetChatInfo(string context, bool isSend)
    {
        return new ChatInfo()
        {
            CreateBy = CurrentUser?.UserName,
            UserId = CurrentUser?.Id,
            SessionId = sessionId,
            AgentId = Agent?.Id,
            AgentName = Agent?.Name,
            Context = context,
            IsSend = isSend,
            Agent = Agent
        };
    }
}