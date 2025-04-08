namespace Known.Internals;

/// <summary>
/// 附件单元格组件类。
/// </summary>
public partial class KFileCell
{
    /// <summary>
    /// 取得或设置附件字段值。
    /// </summary>
    [Parameter] public string Value { get; set; }

    private async Task OnShowFile()
    {
        var files = await Admin.GetFilesAsync(Value);
        if (files == null || files.Count == 0)
        {
            UI.Info("暂无附件！");
            return;
        }

        var model = new DialogModel
        {
            Title = "预览附件",
            Maximizable = true,
            Content = b => b.Component<KFileView>().Set(c => c.Items, files).Build()
        };
        UI.ShowDialog(model);
    }
}