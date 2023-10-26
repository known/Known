namespace WebSite.Docus.View.ImageBoxs;

class ImageBox1 : BaseComponent
{
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var url = new FileUrlInfo
        {
            OriginalUrl = "/img/demo/sunbin.jpg",
            ThumbnailUrl = "/img/demo/sunbin.jpg"
        };
        builder.Component<KImageBox>().Set(c => c.Url, url).Build();
    }
}