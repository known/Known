using AntDesign;

namespace Known.Blazor;

public partial class UIService
{
    /// <summary>
    /// 弹出模态对话框组件。
    /// </summary>
    /// <param name="model">对话框组件模型对象。</param>
    public bool ShowDialog(DialogModel model)
    {
        var option = new ModalOptions
        {
            Title = model.Title,
            MaskClosable = false,
            DestroyOnClose = true,
            Closable = model.Closable,
            Draggable = model.Draggable,
            Resizable = model.Resizable,
            WrapClassName = model.ClassName,
            Style = model.Style,
            Maximizable = model.Maximizable,
            DefaultMaximized = model.DefaultMaximized,
            OnCancel = e => model.CloseAsync(),
            Content = model.Content,
            //Content = b => b.Component<KModalBody>().Set(c => c.Content, model.Content).Build()
        };
        if (model.Width != null)
            option.Width = model.Width.Value;

        if (model.Footer != null)
            option.Footer = model.Footer;
        else if (model.OnOk != null)
            option.Footer = BuildTree(b => BuildDialogFooter(b, model));
        else
            option.Footer = null;

        var dialog = modal.CreateModal(option);
        model.OnClose = dialog.CloseAsync;
        return true;
    }

    private static void BuildDialogFooter(RenderTreeBuilder builder, DialogModel model)
    {
        builder.Component<KModalFooter>()
               .Set(c => c.Closable, model.Closable)
               .Set(c => c.Left, model.FooterLeft)
               .Set(c => c.Actions, model.Actions)
               .Set(c => c.OnOk, model.OnOk)
               .Set(c => c.OnCancel, () => model.CloseAsync())
               .Build();
    }
}