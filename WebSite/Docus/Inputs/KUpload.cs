using WebSite.Docus.Inputs.Uploads;

namespace WebSite.Docus.Inputs;

class KUpload : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildList(new string[]
        {
            "附件上传"
        });
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<Upload1>("1.默认示例");
        builder.BuildDemo<Upload2>("2.事件示例");
        builder.BuildDemo<Upload3>("3.控制示例");
    }
}