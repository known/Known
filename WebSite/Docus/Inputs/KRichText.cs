using WebSite.Docus.Inputs.RichTexts;

namespace WebSite.Docus.Inputs;

class KRichText : BaseDocu
{
    protected override void BuildOverview(RenderTreeBuilder builder)
    {
        builder.BuildMarkdown(@"
- 基于wangEditor.js实现
- 编辑器配置通过Option参数设置
- 配置选项参考：[wangEditor文档](https://www.wangeditor.com/v4/)
- 图片和视频默认上传本地服务器
- 图片和视频也可上传至云，需在index.html中应用OSS或COS的sdk
- 阿里OSS设置参考：[OSS-SDK](https://help.aliyun.com/zh/oss/developer-reference/installation)
- 腾讯COS设置参考：[COS-SDK](https://cloud.tencent.com/document/product/436/11459)
");
    }

    protected override void BuildCodeDemo(RenderTreeBuilder builder)
    {
        builder.BuildDemo<RichText1>("1.默认示例", "block");
        builder.BuildDemo<RichText2>("2.事件示例", "block");
        builder.BuildDemo<RichText3>("3.控制示例", "block");
        builder.BuildDemo<RichText4>("4.自定义上传", "block");
    }
}