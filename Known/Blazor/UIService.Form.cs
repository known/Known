using AntDesign;

namespace Known.Blazor;

public partial class UIService
{
    /// <summary>
    /// 弹出表单组件对话框。
    /// </summary>
    /// <typeparam name="TItem">表单数据类型。</typeparam>
    /// <param name="model">表单组件模型对象。</param>
    public bool ShowForm<TItem>(FormModel<TItem> model) where TItem : class, new()
    {
        var option = new ModalOptions
        {
            MaskClosable = false,
            Draggable = model.Draggable,
            Resizable = model.Resizable,
            Title = model.GetFormTitle(),
            OkText = Language?.OK,
            CancelText = Language?.Cancel
        };
        if (model.Footer != null)
        {
            option.Footer = model.Footer;
        }
        else
        {
            option.OnOk = e => model.SaveAsync();
            option.OnCancel = e => model.CloseAsync();
        }

        RenderFragment content = null;
        var isTabForm = false;
        var isStepForm = false;
        if (model.Type == null)
        {
            content = b => b.Form(model);
        }
        else
        {
            isTabForm = model.Type.IsSubclassOf(typeof(BaseTabForm));
            isStepForm = model.Type.IsSubclassOf(typeof(BaseStepForm));
            var parameters = new Dictionary<string, object>
            {
                { nameof(BaseForm<TItem>.Model), model }
            };
            content = b => b.Component(model.Type, parameters);
        }
        option.Content = content;
        //option.Content = b => b.Component<KModalBody>().Set(c => c.Content, content).Build();

        if (isTabForm)
            option.WrapClassName = "kui-tab-form";
        else if (isStepForm)
            option.WrapClassName = "kui-step-form";
        if (model.Info != null)
        {
            option.Maximizable = model.Info.Maximizable;
            option.DefaultMaximized = model.Info.DefaultMaximized;
            if (model.Info.Width != null)
                option.Width = model.Info.Width.Value;
        }
        var noFooter = model.Info != null && model.Info.NoFooter;
        if (model.IsView || noFooter || (isTabForm || isStepForm) && model.Info?.ShowFooter == false)
            option.Footer = null;

        var dialog = modal.CreateModal(option);
        model.OnClose = dialog.CloseAsync;
        return true;
    }
}