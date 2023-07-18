using Known.Test.Pages.Samples.Forms;

namespace Known.Test.Pages.Samples.Homes;

class DemoHome4 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Button("添加页签", Callback(OnAddTab), StyleType.Primary);
    }

    private void OnAddTab()
    {
        Context.Navigate<DemoForm1>("表单一", "fa fa-table");
    }
}