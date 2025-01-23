namespace Known.Blazor;

public partial class JSService
{
    internal Task ShowChartAsync(string id, object option)
    {
        return InvokeVoidAsync("KBlazor.showChart", id, option);
    }

    internal Task ShowBarcodeAsync(string id, string value, object option)
    {
        return InvokeVoidAsync("KBlazor.showBarcode", id, value, option ?? new { });
    }

    internal Task ShowQRCodeAsync(string id, object option)
    {
        return InvokeVoidAsync("KBlazor.showQRCode", id, option ?? new { });
    }
}