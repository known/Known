using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace Known.Components;

/// <summary>
/// 日志控制台组件类。
/// </summary>
public class KConsole : BaseComponent
{
    private readonly List<ConsoleLogInfo> Logs = [];

    private string LogId => $"kc-{Id}";

    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [Parameter] public string BizId { get; set; }

    /// <summary>
    /// 取得或设置SignalR的方法名称。
    /// </summary>
    [Parameter] public string MethodName { get; set; }

    /// <summary>
    /// 显示强制退出登录对话框。
    /// </summary>
    /// <param name="message">退出提示信息。</param>
    [JSInvokable]
    public void ShowLog(string message)
    {
        var info = Utils.FromJson<ConsoleLogInfo>(message);
        if (info != null && info.BizId == BizId)
        {
            Logs.Add(info);
            StateChangedAsync();
            JS.RunVoidAsync($"KUtils.scrollToBottom('{LogId}');");
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            var invoker = DotNetObjectReference.Create(this);
            await JSRuntime.RegisterNotifyAsync(invoker, MethodName, nameof(ShowLog));
        }
    }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div().Id(LogId).Class("kui-console").Child(() =>
        {
            builder.Component<Virtualize<ConsoleLogInfo>>()
                   .Set(c => c.Items, Logs)
                   .Set(c => c.ItemSize, 60)
                   .Set(c => c.OverscanCount, 2)
                   .Set(c => c.ItemContent, this.BuildTree<ConsoleLogInfo>(BuildItem))
                   .Build();
        });
    }

    /// <inheritdoc />
    protected override async Task OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        await JSRuntime.CloseNotifyAsync(MethodName);
    }

    private void BuildItem(RenderTreeBuilder builder, ConsoleLogInfo item)
    {
        builder.Div().Class(item.Type.ToString()).Child(item.Content);
    }
}