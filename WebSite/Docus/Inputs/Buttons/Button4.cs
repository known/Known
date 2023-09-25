namespace WebSite.Docus.Inputs.Buttons;

class Button4 : BaseComponent
{
    private string btnName = "打开";

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Button(btnName, Callback(OnClick), StyleType.Primary);
    }

    private void OnClick()
    {
        btnName = btnName == "打开" ? "关闭" : "打开";
        StateChanged();
    }
}