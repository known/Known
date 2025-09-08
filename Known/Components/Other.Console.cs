using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.SignalR.Client;

namespace Known.Components;

/// <summary>
/// 日志控制台组件类。
/// </summary>
public class KConsole : BaseComponent
{
    private readonly List<ConsoleLogInfo> Logs = [];
    private HubConnection connection;

    private string LogId => $"lc-{Id}";

    /// <summary>
    /// 取得或设置业务ID。
    /// </summary>
    [Parameter] public string BizId { get; set; }

    /// <summary>
    /// 取得或设置SignalR的Hub地址。
    /// </summary>
    [Parameter] public string HubUrl { get; set; }

    /// <summary>
    /// 取得或设置SignalR的方法名称。
    /// </summary>
    [Parameter] public string MethodName { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        var url = Navigation.ToAbsoluteUri(HubUrl);
        connection = new HubConnectionBuilder().WithUrl(url).Build();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            connection?.On(MethodName, (string message) =>
            {
                var log = Utils.FromJson<ConsoleLogInfo>(message);
                if (log.BizId == BizId)
                {
                    Logs.Add(log);
                    StateChangedAsync();
                    JS.RunVoidAsync($"KUtils.scrollToBottom('{LogId}');");
                }
            });
            if (connection?.State == HubConnectionState.Disconnected)
                await connection?.StartAsync();
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
    protected override Task OnDisposeAsync()
    {
        connection?.Remove(MethodName);
        return base.OnDisposeAsync();
    }

    private void BuildItem(RenderTreeBuilder builder, ConsoleLogInfo item)
    {
        builder.Div().Class(item.Type.ToString()).Child(item.Content);
    }
}