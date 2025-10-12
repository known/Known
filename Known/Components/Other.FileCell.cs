namespace Known.Components;

/// <summary>
/// 附件单元格组件类。
/// </summary>
public class KFileCell : BaseComponent
{
    private bool HasFile => !string.IsNullOrWhiteSpace(Value);

    /// <summary>
    /// 取得或设置附件字段值。
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (!HasFile)
            return;

        builder.Component<KIcon>()
               .Set(c => c.Icon, "file")
               .Set(c => c.Name, Name)
               .Set(c => c.OnClick, this.Callback<MouseEventArgs>(e => OnShowFile()))
               .Build();
    }

    private async Task OnShowFile()
    {
        var files = await Admin.GetFilesAsync(Value);
        UI.PreviewFile(files);
    }
}