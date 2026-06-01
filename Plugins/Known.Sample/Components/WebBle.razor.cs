using System.Globalization;
using Microsoft.JSInterop;

namespace Known.Sample.Components;

public partial class WebBle : IAsyncDisposable
{
    private DotNetObjectReference<WebBle> invoker;

    [Parameter] public string ServiceUuid { get; set; }
    [Parameter] public string WriteCharacteristicUuid { get; set; }
    [Parameter] public string NotifyCharacteristicUuid { get; set; }
    [Parameter] public string CommandText { get; set; }
    [Parameter] public EventCallback<string> CommandTextChanged { get; set; }
    [Parameter] public EventCallback<string> OnReceive { get; set; }

    public string DeviceName { get; set; }
    public bool IsConnected { get; set; }
    public List<BleMessageInfo> Messages { get; } = [];
    public string StatusText => IsConnected ? "已连接" : "未连接";
    public bool CanSendText => IsConnected && !string.IsNullOrWhiteSpace(WriteCharacteristicUuid) && !string.IsNullOrWhiteSpace(CommandText);
    public bool CanSendHex => CanSendText;
    public bool CanStartNotifications => IsConnected && !string.IsNullOrWhiteSpace(NotifyCharacteristicUuid);
    public bool CanStopNotifications => CanStartNotifications;

    protected override Task OnInitAsync()
    {
        invoker = DotNetObjectReference.Create(this);
        return base.OnInitAsync();
    }

    public async Task ConnectAsync()
    {
        if (string.IsNullOrWhiteSpace(ServiceUuid))
        {
            UI.Warning("请输入服务UUID！");
            return;
        }

        try
        {
            var device = await JSRuntime.InvokeJsAsync<BleDeviceInfo>("KSampleBle.connect", invoker, ServiceUuid, WriteCharacteristicUuid, NotifyCharacteristicUuid);
            if (device == null)
            {
                UI.Warning("蓝牙连接失败！");
                return;
            }

            DeviceName = string.IsNullOrWhiteSpace(device.Name) ? device.Id : device.Name;
            IsConnected = device.IsConnected;
            AddMessage("SYS", $"已连接设备：{DeviceName}");
            await StateChangedAsync();
        }
        catch (Exception ex)
        {
            UI.Error(ex.Message);
        }
    }

    public async Task DisconnectAsync()
    {
        await JSRuntime.InvokeJsAsync("KSampleBle.disconnect");
        IsConnected = false;
        DeviceName = null;
        AddMessage("SYS", "蓝牙已断开。", false);
        await StateChangedAsync();
    }

    public async Task StartNotificationsAsync()
    {
        if (string.IsNullOrWhiteSpace(NotifyCharacteristicUuid))
        {
            UI.Warning("请输入通知特征UUID！");
            return;
        }

        await JSRuntime.InvokeJsAsync("KSampleBle.startNotifications", NotifyCharacteristicUuid);
        AddMessage("SYS", "已开始接收设备通知。", false);
        await StateChangedAsync();
    }

    public async Task StopNotificationsAsync()
    {
        if (string.IsNullOrWhiteSpace(NotifyCharacteristicUuid))
            return;

        await JSRuntime.InvokeJsAsync("KSampleBle.stopNotifications", NotifyCharacteristicUuid);
        AddMessage("SYS", "已停止接收设备通知。", false);
        await StateChangedAsync();
    }

    public async Task SendTextAsync()
    {
        await JSRuntime.InvokeJsAsync("KSampleBle.writeText", WriteCharacteristicUuid, CommandText);
        AddMessage("TX", CommandText);
        await StateChangedAsync();
    }

    public async Task SendHexAsync()
    {
        var hex = NormalizeHex(CommandText);
        if (string.IsNullOrWhiteSpace(hex))
        {
            UI.Warning("请输入有效HEX命令！");
            return;
        }

        await JSRuntime.InvokeJsAsync("KSampleBle.writeHex", WriteCharacteristicUuid, hex);
        AddMessage("TX", hex);
        await StateChangedAsync();
    }

    public void ClearMessages()
    {
        Messages.Clear();
    }

    [JSInvokable]
    public async Task OnReceiveData(string text)
    {
        AddMessage("RX", text);
        if (OnReceive.HasDelegate)
            await OnReceive.InvokeAsync(text);
        await StateChangedAsync();
    }

    [JSInvokable]
    public async Task OnDisconnect()
    {
        IsConnected = false;
        AddMessage("SYS", "设备连接已断开。", false);
        await StateChangedAsync();
    }

    [JSInvokable]
    public async Task OnError(string error)
    {
        AddMessage("ERR", error, false);
        await StateChangedAsync();
    }

    public async Task OnCommandInput(ChangeEventArgs args)
    {
        CommandText = args.Value?.ToString();
        if (CommandTextChanged.HasDelegate)
            await CommandTextChanged.InvokeAsync(CommandText);
    }

    protected override async Task OnDisposeAsync()
    {
        await base.OnDisposeAsync();
        invoker?.Dispose();
        await JSRuntime.InvokeJsAsync("KSampleBle.dispose");
    }

    private void AddMessage(string type, string text, bool trim = true)
    {
        Messages.Insert(0, new BleMessageInfo { Type = type, Text = trim ? text?.Trim() : text });
        if (Messages.Count > 100)
            Messages.RemoveAt(Messages.Count - 1);
    }

    private static string NormalizeHex(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        var value = text.Replace(" ", string.Empty, StringComparison.Ordinal)
                        .Replace("\r", string.Empty, StringComparison.Ordinal)
                        .Replace("\n", string.Empty, StringComparison.Ordinal);
        if (value.Length % 2 != 0)
            return null;

        for (var i = 0; i < value.Length; i += 2)
        {
            if (!byte.TryParse(value.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
                return null;
        }

        return value.ToUpperInvariant();
    }
}