namespace Known.Sample.Pages.Demo;

public partial class Ble
{
    private string serviceUuid = "0000fff0-0000-1000-8000-00805f9b34fb";
    private string writeCharacteristicUuid = "0000fff1-0000-1000-8000-00805f9b34fb";
    private string notifyCharacteristicUuid = "0000fff2-0000-1000-8000-00805f9b34fb";
    private string commandText = "AA5501";
    private string receiveText;

    private Task OnReceiveAsync(string text)
    {
        receiveText = text;
        return Task.CompletedTask;
    }
}