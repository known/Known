namespace Known.Razor;

partial class UIService
{
    internal async void Show(ChartOption option) => await InvokeAsync<ChartOption>("KRazor.showChart", option);
    internal void ShowBarcode(string id, string value, object option) => InvokeVoidAsync("KRazor.showBarcode", id, value, option);
}