using WebSite.Docus.Inputs.CheckBoxs;

namespace WebSite.Docus.Inputs;

class KCheckBox : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "支持开关类型"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<CheckBox1>("1.默认示例");
        builder.BuildDemo<CheckBox2>("2.开关示例");
        builder.BuildDemo<CheckBox3>("3.事件示例");
        builder.BuildDemo<CheckBox4>("4.控制示例");
    }
}