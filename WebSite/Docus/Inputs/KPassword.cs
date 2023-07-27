using WebSite.Docus.Inputs.Passwords;

namespace WebSite.Docus.Inputs;

class KPassword : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "密码字段组件"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Password1>("1.默认示例");
    }
}