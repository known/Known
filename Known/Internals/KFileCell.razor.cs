namespace Known.Internals;

/// <summary>
/// 附件单元格组件类。
/// </summary>
public partial class KFileCell
{
    private bool HasFile => !string.IsNullOrWhiteSpace(Value);

    /// <summary>
    /// 取得或设置附件字段值。
    /// </summary>
    [Parameter] public string Value { get; set; }

    private async Task OnShowFile()
    {
        var files = await Admin.GetFilesAsync(Value);
        var model = new DialogModel
        {
            Title = Language.PreviewFile,
            Width = 800,
            Maximizable = true,
            Content = b => b.Component<KFileView>().Set(c => c.Items, files).Build()
        };
        UI.ShowDialog(model);
    }
}