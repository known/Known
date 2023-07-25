using WebSite.Docus.Input.CheckBoxs;

namespace WebSite.Docus.Input;

class KCheckBox : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "支持开关"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<CheckBox1>();
        builder.BuildDemo<CheckBox2>();
    }
}