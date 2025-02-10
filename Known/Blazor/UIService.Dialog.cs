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
            Closable = model.Closable,
            Draggable = model.Draggable,
            Resizable = model.Resizable,
            WrapClassName = model.ClassName,
            Maximizable = model.Maximizable,
            DefaultMaximized = model.DefaultMaximized,
            Content = b => b.Component<KModalBody>().Set(c => c.Content, model.Content).Build()
        };

        if (model.OnOk != null)
        {
            if (option.Closable)
            {
                option.OkText = Language?.OK;
                option.CancelText = Language?.Cancel;
                option.OnOk = e => model.OnOk.Invoke();
                option.OnCancel = e => model.CloseAsync();
            }
            else
            {
                option.Footer = BuildTree(b => b.Component<KModalFooter>().Set(c => c.OnOk, model.OnOk).Build());
            }
        }
        else
        {
            option.Footer = null;
        }

        if (model.Width != null)
            option.Width = model.Width.Value;
        if (model.Footer != null)
            option.Footer = model.Footer;

        var dialog = modal.CreateModal(option);
        model.OnClose = dialog.CloseAsync;
        return true;
    }

    private static RenderFragment BuildTree(Action<RenderTreeBuilder> action)
    {
        return delegate (RenderTreeBuilder builder) { action(builder); };
    }
}