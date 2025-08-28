namespace Known.Extensions;

static class ModalExtension
{
    internal static void PreviewFile(this UIService service, List<AttachInfo> files)
    {
        var model = new DialogModel
        {
            Title = Language.PreviewFile,
            Width = 800,
            Maximizable = true,
            Content = b => b.Component<KFileView>().Set(c => c.Items, files).Build()
        };
        service.ShowDialog(model);
    }
}