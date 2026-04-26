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
    private string attachmentName;
    private string attachmentText;

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

    /// <summary>
    /// 取得或设置发送区域左下角工具栏区域。
    /// </summary>
    [Parameter] public RenderFragment Toolbar { get; set; }

    /// <summary>
    /// 发送消息前回调，用于注入附件和解析结构化输出。
    /// </summary>
    [Parameter] public Func<ChatInfo, Task<ChatInfo>> OnSendingAsync { get; set; }

    /// <summary>
    /// 接收消息后回调。
    /// </summary>
    [Parameter] public EventCallback<ChatInfo> OnReceivedAsync { get; set; }

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

    private async Task OnEditSend(ChatInfo info)
    {
        info.IsEdit = false;
        await SendAsync(info.Context);
    }

    private async Task OnSend()
    {
        if (sendding || string.IsNullOrWhiteSpace(message))
            return;

        await SendAsync(message);
    }

    private async void OnMessageKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Enter" && !e.ShiftKey)
            await OnSend();
    }

    private async Task SendAsync(string context)
    {
        try
        {
            var chat = GetChatInfo(context, true);
            if (!string.IsNullOrWhiteSpace(attachmentText))
                chat.Context = $"附件[{attachmentName}]：\n{attachmentText}\n\n{chat.Context}";

            if (OnSendingAsync != null)
                chat = await OnSendingAsync.Invoke(chat) ?? chat;

            chats.Add(chat);
            sendding = true;
            await StateChangedAsync();

            resp = new ChatInfo { Context = "思考中..." };
            chats.Add(resp);
            await StateChangedAsync();
            await Task.Delay(50);

            var result = Service.SendChatAsync(chat);
            var sb = new StringBuilder();
            await foreach (var item in result)
            {
                sb.Append(item);
                resp.Context = sb.ToString();
                await Task.Delay(30);
                await StateChangedAsync();
            }
            await JSRuntime.HighlightAllAsync();

            message = "";
            sendding = false;
            ClearAttachment();
            var receive = GetChatInfo(resp.Context, false);
            await Service.SaveChatAsync(receive);
            if (OnReceivedAsync.HasDelegate)
                await OnReceivedAsync.InvokeAsync(receive);
            await StateChangedAsync();
        }
        catch (Exception ex)
        {
            sendding = false;
            UI.Error(ex.Message);
            await StateChangedAsync();
        }
    }

    private async Task OnAttachmentChanged(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file == null)
            return;

        attachmentName = file.Name;
        try
        {
            using var stream = file.OpenReadStream(Agent?.FileMaxSize ?? 2 * 1024 * 1024);
            using var reader = new StreamReader(stream);
            attachmentText = await reader.ReadToEndAsync();
            await StateChangedAsync();
        }
        catch (Exception ex)
        {
            attachmentName = null;
            attachmentText = null;
            UI.Error(ex.Message);
        }
    }

    private Task OnClearAttachment(MouseEventArgs e)
    {
        ClearAttachment();
        return StateChangedAsync();
    }

    private void ClearAttachment()
    {
        attachmentName = null;
        attachmentText = null;
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

    private void OnMessageChange(ChangeEventArgs args)
    {
        message = args.Value?.ToString();
    }
}