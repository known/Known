namespace Known.Razor;

partial class UIService
{
    public async void Show(ChartOption option) => await InvokeAsync<ChartOption>("KRazor.showChart", option);
}