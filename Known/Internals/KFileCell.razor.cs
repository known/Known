namespace Known.Internals;

/// <summary>
/// 附件单元格组件类。
/// </summary>
public partial class KFileCell
{
    private List<AttachInfo> Files = [];
    private bool HasFile => Files != null && Files.Count > 0;

    /// <summary>
    /// 取得或设置附件字段值。
    /// </summary>
    [Parameter] public string Value { get; set; }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender && !string.IsNullOrWhiteSpace(Value))
        {
            Files = await Admin.GetFilesAsync(Value);
            await StateChangedAsync();
        }
    }

    private void OnShowFile()
    {
        var model = new DialogModel
        {
            Title = "预览附件",
            Maximizable = true,
            Content = b => b.Component<KFileView>().Set(c => c.Items, Files).Build()
        };
        UI.ShowDialog(model);
    }
}