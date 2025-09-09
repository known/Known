namespace Known.Components;

/// <summary>
/// 蓝牙组件类。
/// </summary>
public partial class KBluetooth
{
    private bool isSupported = false;
    private bool isScanning = false;
    private bool isConnecting = false;
    private bool isDisconnecting = false;
    private bool isConnected = false;
    private string selectedDeviceId = "";
    private string connectedDeviceName = "";

    private List<CodeInfo> devices = [];
    private DotNetObjectReference<KBluetooth> invoker;

    // 回调事件定义
    [Parameter] public EventCallback<CodeInfo> OnConnected { get; set; }

    [Parameter] public EventCallback<CodeInfo> OnDisconnected { get; set; }

    [Parameter] public EventCallback<string> OnDataReceived { get; set; }

    [Parameter] public EventCallback<string> OnError { get; set; }

    protected override async Task OnInitAsync()
    {
        invoker = DotNetObjectReference.Create(this);
        await CheckSupportAsync();
        if (isSupported)
        {
            await LoadPairedDevicesAsync();
        }
    }

    // 检查浏览器是否支持蓝牙
    private async Task CheckSupportAsync()
    {
        try
        {
            isSupported = await JSRuntime.InvokeAsync<bool>("bluetooth.checkSupport");
        }
        catch (Exception ex)
        {
            await HandleErrorAsync($"检查蓝牙支持时出错: {ex.Message}");
            isSupported = false;
        }
    }

    // 加载已配对设备
    private async Task LoadPairedDevicesAsync()
    {
        try
        {
            var pairedDevices = await JSRuntime.InvokeAsync<List<CodeInfo>>("bluetooth.getPairedDevices");
            devices = pairedDevices ?? [];
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync($"加载已配对设备时出错: {ex.Message}");
        }
    }

    // 刷新设备列表
    public async Task RefreshDevicesAsync()
    {
        if (isScanning) return;

        isScanning = true;
        try
        {
            await JSRuntime.InvokeVoidAsync("bluetooth.startDiscovery", invoker);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync($"扫描设备时出错: {ex.Message}");
            isScanning = false;
        }
    }

    // 连接设备
    public async Task ConnectDeviceAsync()
    {
        if (string.IsNullOrEmpty(selectedDeviceId) || isConnecting) return;

        isConnecting = true;
        try
        {
            var device = devices.FirstOrDefault(d => d.Code == selectedDeviceId);
            if (device != null)
            {
                var success = await JSRuntime.InvokeAsync<bool>("bluetooth.connectToDevice",
                    invoker, device.Code);

                if (success)
                {
                    isConnected = true;
                    connectedDeviceName = device.Name;
                    await OnConnected.InvokeAsync(device);
                }
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync($"连接设备时出错: {ex.Message}");
        }
        finally
        {
            isConnecting = false;
        }
    }

    // 断开设备连接
    public async Task DisconnectDeviceAsync()
    {
        if (!isConnected || isDisconnecting) return;

        isDisconnecting = true;
        try
        {
            await JSRuntime.InvokeVoidAsync("bluetooth.disconnectDevice");

            isConnected = false;
            var device = devices.FirstOrDefault(d => d.Code == selectedDeviceId);
            if (device != null)
            {
                await OnDisconnected.InvokeAsync(device);
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync($"断开设备连接时出错: {ex.Message}");
        }
        finally
        {
            isDisconnecting = false;
        }
    }

    // 发送数据到设备
    public async Task SendDataAsync(string data)
    {
        if (!isConnected) return;

        try
        {
            await JSRuntime.InvokeVoidAsync("bluetooth.sendDataToDevice", data);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync($"发送数据时出错: {ex.Message}");
        }
    }

    // JS互操作：发现设备回调
    [JSInvokable]
    public void OnDiscovered(CodeInfo device)
    {
        InvokeAsync(async () =>
        {
            if (!devices.Any(d => d.Code == device.Code))
            {
                devices.Add(device);
                StateHasChanged();
            }
        });
    }

    // JS互操作：扫描完成回调
    [JSInvokable]
    public void OnScanComplete()
    {
        InvokeAsync(() =>
        {
            isScanning = false;
            StateHasChanged();
        });
    }

    // JS互操作：接收数据回调
    [JSInvokable]
    public void OnDataReceivedFromDevice(string data)
    {
        InvokeAsync(async () =>
        {
            await OnDataReceived.InvokeAsync(data);
        });
    }

    // 错误处理
    private async Task HandleErrorAsync(string error)
    {
        await OnError.InvokeAsync(error);
    }

    public void Dispose()
    {
        invoker?.Dispose();
    }
}