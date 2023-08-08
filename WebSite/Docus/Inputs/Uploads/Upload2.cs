using Microsoft.AspNetCore.Components.Forms;

namespace WebSite.Docus.Inputs.Uploads;

class Upload2 : BaseComponent
{
    private string? message;

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Field<Upload>("附件1：", "Upload1")
               .ValueChanged(OnValueChanged)
               .Set(f => f.OnFilesChanged, OnFilesChanged)
               .Build();
        builder.Field<Upload>("附件2：", "Upload2")
               .ValueChanged(OnValueChanged)
               .Set(f => f.IsMultiple, true)
               .Set(f => f.OnFilesChanged, OnFilesChanged)
               .Build();
        builder.Div("tips", message);
    }

    private void OnValueChanged(string value)
    {
        message = value;
        StateChanged();
    }

    private void OnFilesChanged(List<IBrowserFile> list)
    {
        foreach (var item in list)
        {
            message += $"大小：{item.Size}";
        }
        StateChanged();
    }
}