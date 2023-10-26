using Known.Extensions;

namespace Known.Razor;

public class KImageBox : BaseComponent
{
    [Parameter] public FileUrlInfo Url { get; set; }
    [Parameter] public bool IsShowOriginal { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Img(attr =>
        {
            if (!IsShowOriginal)
                attr.OnClick(Callback(e => OnImageClick()));
            attr.Src(IsShowOriginal ? Url?.OriginalUrl : Url?.ThumbnailUrl)
                .Add("alt", Url?.FileName);
        });
    }

    private void OnImageClick()
    {
        UI.AppendBody($"<div class=\"pic-box\" onclick=\"$(this).remove()\"><img src=\"{Url?.OriginalUrl}\" /></div>");
    }
}