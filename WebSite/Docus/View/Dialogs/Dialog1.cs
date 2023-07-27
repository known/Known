namespace WebSite.Docus.View.Dialogs;

class Dialog1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Button("提示对话框", Callback(OnAlert), StyleType.Primary);
        builder.Button("确认对话框", Callback(OnConfirm), StyleType.Primary);
    }

    private void OnAlert()
    {
        UI.Alert("XXX逻辑校验失败！", size: new(300, 200));
    }

    private void OnConfirm()
    {
        UI.Confirm("确定要XXX操作？", () =>
        {
            UI.Toast("操作成功！", StyleType.Success);
        });
    }
}