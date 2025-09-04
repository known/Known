using AntDesign;

namespace Known.Blazor;

public partial class UIService
{
    /// <summary>
    /// 根据当前用户配置显示表单组件（模态框、抽屉、URL）。
    /// </summary>
    /// <typeparam name="TItem">表单数据类型。</typeparam>
    /// <param name="model">表单组件模型对象。</param>
    public bool ShowForm<TItem>(FormModel<TItem> model) where TItem : class, new()
    {
        var openType = model.Info?.OpenType;
        if (openType == FormOpenType.None)
            openType = Utils.ConvertTo<FormOpenType>(model.Context.UserSetting.OpenType);
        switch (openType)
        {
            case FormOpenType.Modal:
                ShowModal(model);
                break;
            case FormOpenType.Drawer:
                _ = ShowDrawerAsync(model);
                break;
            case FormOpenType.Url:
                ShowUrlForm(model);
                break;
            default:
                ShowModal(model);
                break;
        }
        return true;
    }

    /// <summary>
    /// 显示表单组件对话框。
    /// </summary>
    /// <typeparam name="TItem">表单数据类型。</typeparam>
    /// <param name="model">表单组件模型对象。</param>
    public void ShowModal<TItem>(FormModel<TItem> model) where TItem : class, new()
    {
        var option = new ModalOptions
        {
            MaskClosable = false,
            DestroyOnClose = true,
            Draggable = model.Draggable,
            Resizable = model.Resizable,
            Maximizable = model.Info?.Maximizable == true,
            DefaultMaximized = model.Info?.DefaultMaximized == true,
            Title = model.GetFormTitle(),
            OkText = Language.OK,
            CancelText = Language.Cancel,
            WrapClassName = GetWrapperClass(model),
            OnCancel = e => model.CloseAsync(),
            Content = GetFormContent(model)
        };
        if (model.Info?.Width != null)
            option.Width = model.Info.Width.Value;

        if (model.Footer != null)
            option.Footer = model.Footer;
        else
            option.Footer = BuildTree(b => BuildFormFooter(b, model));

        if (model.IsNoFooter && !model.Info?.ShowFooter == true)
            option.Footer = null;
        if (model.IsView)
            option.Footer = null;

        var dialog = modal.CreateModal(option);
        model.OnClose = dialog.CloseAsync;
    }

    /// <summary>
    /// 显示表单组件抽屉。
    /// </summary>
    /// <typeparam name="TItem">表单数据类型。</typeparam>
    /// <param name="model">表单组件模型对象。</param>
    /// <returns></returns>
    public async Task ShowDrawerAsync<TItem>(FormModel<TItem> model) where TItem : class, new()
    {
        var option = new DrawerOptions
        {
            Title = model.GetFormTitle(),
            Width = model.Info?.Width?.ToString() ?? "400px",
            Closable = true,
            MaskClosable = model.IsView,
            Placement = DrawerPlacement.Right,
            WrapClassName = GetWrapperClass(model),
            Content = GetFormContent(model, true)
        };
        var obj = await drawer.CreateAsync(option);
        model.OnClose = obj.CloseAsync;
    }

    /// <summary>
    /// 显示URL表单组件。
    /// </summary>
    /// <typeparam name="TItem">表单数据类型。</typeparam>
    /// <param name="model">表单组件模型对象。</param>
    public void ShowUrlForm<TItem>(FormModel<TItem> model) where TItem : class, new()
    {
        var route = model.Type?.GetCustomAttributes<RouteAttribute>()?.FirstOrDefault();
        if (route == null || string.IsNullOrWhiteSpace(route.Template))
        {
            this.Error(Language.TipFormRouteIsNull);
            return;
        }

        model.Context.NavigateTo(new MenuInfo
        {
            Id = typeof(TItem).FullName,
            Name = model.GetFormTitle(),
            Url = route.Template
        });
    }

    private static string GetWrapperClass<TItem>(FormModel<TItem> model) where TItem : class, new()
    {
        return CssBuilder.Default("kui-form-wrapper")
                         .AddClass(model.WrapClass)
                         .AddClass("kui-view-form", model.IsView)
                         .AddClass("kui-tab-form", model.IsTabForm)
                         .AddClass("kui-step-form", model.IsStepForm)
                         .BuildClass();
    }

    private static RenderFragment GetFormContent<TItem>(FormModel<TItem> model, bool isDrawer = false) where TItem : class, new()
    {
        model.EnableEdit = true;
        return builder => builder.BuildBody(model.Context, b =>
        {
            b.Div("kui-form-body", () =>
            {
                if (model.Type == null)
                {
                    b.Form(model);
                }
                else
                {
                    var parameters = model.Parameters ?? [];
                    parameters[nameof(BaseForm<TItem>.Model)] = model;
                    b.Component(model.Type, parameters);
                }
            });
            if (isDrawer)
                BuildDrawerFooter(b, model);
        });
        //return b => b.Component<KModalBody>().Set(c => c.Content, content).Build();
    }

    private static void BuildDrawerFooter<TItem>(RenderTreeBuilder builder, FormModel<TItem> model) where TItem : class, new()
    {
        if (model.IsView) return;
        if (model.IsNoFooter && !model.Info.ShowFooter) return;

        if (model.Footer != null)
        {
            builder.FormAction(() =>
            {
                if (model.FooterRight != null)
                    builder.Fragment(model.FooterRight);
                builder.Fragment(model.Footer);
            }, model.FooterLeft);
            return;
        }

        BuildFormFooter(builder, model);
    }

    private static void BuildFormFooter<TItem>(RenderTreeBuilder builder, FormModel<TItem> model) where TItem : class, new()
    {
        builder.Component<KModalFooter>()
               .Set(c => c.Closable, true)
               .Set(c => c.Left, model.FooterLeft)
               .Set(c => c.Right, model.FooterRight)
               .Set(c => c.Actions, model.Actions)
               .Set(c => c.OnOk, () => model.SaveAsync())
               .Set(c => c.OnCancel, () => model.CloseAsync())
               .Build();
    }
}