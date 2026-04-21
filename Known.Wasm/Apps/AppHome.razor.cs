namespace Known.Wasm.Apps;

public partial class AppHome
{
    private string testNo;

    private void OnScan()
    {
        UI.ShowScanner(async (res, err) =>
        {
            if (!string.IsNullOrWhiteSpace(err))
            {
                UI.Error(err);
                return;
            }

            testNo = res;
            await StateChangedAsync();
        }, () =>
        {
            UI.Info("停止扫码！");
            return Task.CompletedTask;
        });
    }
}