namespace WebSite.Docus.Feedback.Loadings;

class Loading1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Button("提交", Callback(OnLoading), StyleType.Primary);
    }

    private void OnLoading()
    {
        UI.ShowLoading("数据提交中...");
        Task.Run(() =>
        {
            Thread.Sleep(2000);
            UI.CloseLoading();
        });
    }
}