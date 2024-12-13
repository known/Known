using AntDesign;

namespace Known.Blazor;

/// <summary>
/// UI组件服务类。
/// </summary>
/// <param name="modalService">模态弹窗服务。</param>
/// <param name="messageService">消息弹窗服务。</param>
/// <param name="noticeService">通知弹窗服务。</param>
public class UIService(ModalService modalService, MessageService messageService, INotificationService noticeService)
{
    private readonly ModalService _modal = modalService;
    private readonly MessageService _message = messageService;
    private readonly INotificationService _notice = noticeService;

    /// <summary>
    /// 取得或设置语言实例。
    /// </summary>
    public Language Language { get; set; }

    /// <summary>
    /// 根据字段类型获取对应的输入组件类型。
    /// </summary>
    /// <param name="dataType">字段属性类型。</param>
    /// <param name="fieldType">字段类型。</param>
    /// <returns></returns>
    internal Type GetInputType(Type dataType, FieldType fieldType)
    {
        if (fieldType == FieldType.AutoComplete)
            return typeof(AntAutoComplete);

        if (fieldType == FieldType.Select)
            return typeof(AntSelectCode);

        if (fieldType == FieldType.CheckBox)
            return typeof(Checkbox);

        if (fieldType == FieldType.CheckList)
            return typeof(AntCheckboxGroup);

        if (fieldType == FieldType.RadioList)
            return typeof(AntRadioGroup);

        if (fieldType == FieldType.Password)
            return typeof(InputPassword);

        if (fieldType == FieldType.TextArea)
            return typeof(AntTextArea);

        if (dataType == typeof(bool))
            return typeof(Switch);

        if (dataType == typeof(short))
            return typeof(AntNumber<short>);

        if (dataType == typeof(short?))
            return typeof(AntNumber<short?>);

        if (dataType == typeof(int))
            return typeof(AntNumber<int>);

        if (dataType == typeof(int?))
            return typeof(AntNumber<int?>);

        if (dataType == typeof(long))
            return typeof(AntNumber<long>);

        if (dataType == typeof(long?))
            return typeof(AntNumber<long?>);

        if (dataType == typeof(float))
            return typeof(AntNumber<float>);

        if (dataType == typeof(float?))
            return typeof(AntNumber<float?>);

        if (dataType == typeof(double))
            return typeof(AntNumber<double>);

        if (dataType == typeof(double?))
            return typeof(AntNumber<double?>);

        if (dataType == typeof(decimal))
            return typeof(AntNumber<decimal>);

        if (dataType == typeof(decimal?))
            return typeof(AntNumber<decimal?>);

        if (dataType == typeof(DateTime))
            return typeof(DatePicker<DateTime>);

        if (dataType == typeof(DateTime?))
            return typeof(AntDatePicker);

        if (dataType == typeof(DateTimeOffset))
            return typeof(DatePicker<DateTimeOffset>);

        if (dataType == typeof(DateTimeOffset?))
            return typeof(DatePicker<DateTimeOffset?>);

        return typeof(AntInput);
        //return typeof(AntInput<>).MakeGenericType(dataType);
    }

    /// <summary>
    /// 添加输入控件扩展参数。
    /// </summary>
    /// <typeparam name="TItem">表达数据类型。</typeparam>
    /// <param name="attributes">输入组件参数字段。</param>
    /// <param name="model">字段组件模型对象实例。</param>
    internal void AddInputAttributes<TItem>(Dictionary<string, object> attributes, FieldModel<TItem> model) where TItem : class, new()
    {
        var column = model.Column;
        if (!string.IsNullOrWhiteSpace(column.Category))
        {
            if (column.Type == FieldType.Select)
                attributes[nameof(AntSelect.DataSource)] = model.GetCodes();

            if (column.Type == FieldType.RadioList)
                attributes[nameof(AntRadioGroup.Codes)] = model.GetCodes("");

            if (column.Type == FieldType.CheckList)
                attributes[nameof(AntCheckboxGroup.Codes)] = model.GetCodes("");
        }

        if (column.Type == FieldType.AutoComplete)
            attributes[nameof(AntAutoComplete.Options)] = model.GetCodes("");

        if (column.Type == FieldType.Date || column.Type == FieldType.DateTime)
            attributes["disabled"] = OneOf.OneOf<bool, bool[]>.FromT0(model.IsReadOnly);
    }

    /// <summary>
    /// 弹出吐司组件提示消息框。
    /// </summary>
    /// <param name="message">提示消息文本。</param>
    /// <param name="style">提示样式，默认Success。</param>
    /// <returns></returns>
    public async Task Toast(string message, StyleType style = StyleType.Success)
    {
        var content = FormatMessage(message);
        switch (style)
        {
            case StyleType.Success:
                await _message.Success(content);
                break;
            case StyleType.Info:
                await _message.Info(content);
                break;
            case StyleType.Warning:
                await _message.Warning(content);
                break;
            case StyleType.Error:
                await _message.Error(content);
                break;
            default:
                await _message.Info(content);
                break;
        }
    }

    /// <summary>
    /// 弹出通知组件提示消息框。
    /// </summary>
    /// <param name="title">通知标题。</param>
    /// <param name="message">提示消息文本。</param>
    /// <param name="style">提示样式，默认Success。</param>
    /// <returns></returns>
    public async Task NoticeAsync(string title, string message, StyleType style = StyleType.Success)
    {
        var config = new NotificationConfig
        {
            Message = title,
            Description = FormatMessage(message),
            Placement = NotificationPlacement.BottomRight
        };
        switch (style)
        {
            case StyleType.Success:
                await _notice.Success(config);
                break;
            case StyleType.Info:
                await _notice.Info(config);
                break;
            case StyleType.Warning:
                await _notice.Warning(config);
                break;
            case StyleType.Error:
                config.Duration = 1000;
                await _notice.Error(config);
                break;
            default:
                await _notice.Info(config);
                break;
        }
    }

    /// <summary>
    /// 弹出消息提示框组件。
    /// </summary>
    /// <param name="message">提示消息文本。</param>
    /// <param name="action">点【确定】按钮的回调方法。</param>
    public bool Alert(string message, Func<Task> action = null)
    {
        var options = new ConfirmOptions
        {
            Title = Language?.GetTitle("Prompt"),
            Content = FormatMessage(message)
        };
        if (action != null)
            options.OnOk = e => action?.Invoke();
        _modal.Info(options);
        return true;
    }

    /// <summary>
    /// 弹出确认消息提示框组件。
    /// </summary>
    /// <param name="message">确认消息文本。</param>
    /// <param name="action">点【确定】按钮的回调方法。</param>
    public bool Confirm(string message, Func<Task> action)
    {
        var options = new ConfirmOptions
        {
            Title = Language?.GetTitle("Question"),
            Icon = b => b.Icon("question-circle"),
            Content = FormatMessage(message)
        };
        if (action != null)
            options.OnOk = e => action?.Invoke();
        _modal.Confirm(options);
        return true;
    }

    private static RenderFragment FormatMessage(string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            message = message.Trim([.. Environment.NewLine]);
            if (message.Contains(Environment.NewLine))
            {
                message = message.Replace(Environment.NewLine, "<br/>");
                message = $"<div class=\"message\">{message}</div>";
            }
        }
        return b => b.Markup(message);
    }

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

        var modal = _modal.CreateModal(option);
        model.OnClose = modal.CloseAsync;
        return true;
    }

    private static RenderFragment BuildTree(Action<RenderTreeBuilder> action)
    {
        return delegate (RenderTreeBuilder builder) { action(builder); };
    }

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
        option.Content = b => b.Component<KModalBody>().Set(c => c.Content, content).Build();

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

        var modal = _modal.CreateModal(option);
        model.OnClose = modal.CloseAsync;
        return true;
    }

    internal static string GetTagColor(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        if (UIConfig.TagColor != null)
        {
            var color = UIConfig.TagColor.Invoke(text);
            if (!string.IsNullOrWhiteSpace(color))
                return color;
        }

        //Module
        if (text == "Menu") return "purple";
        else if (text == "Page") return "blue";
        else if (text == "Custom") return "green";
        //Log
        else if (text == "Login") return "success";
        else if (text == "Logout") return "red";
        //Task
        else if (text == "Pending") return "default";
        else if (text == "Running") return "processing";
        else if (text == "Success") return "success";
        else if (text == "Failed") return "error";
        //User
        else if (text == "Male" || text == "男") return "#108ee9";
        else if (text == "Female" || text == "女") return "hotpink";
        //Flow
        else if (text == "Save") return "default";
        else if (text == "Revoked") return "lime";
        else if (text == "Verifing") return "processing";
        else if (text == "Pass") return "success";
        else if (text == "Fail") return "error";
        //Status
        else if (text.Contains("已完成")) return "success";
        else if (text.Contains("中")) return "processing";
        else if (text.Contains("待") || text.Contains("提交")) return "#2db7f5";
        else if (text.Contains("完成") || text.Contains("结束")) return "#108ee9";
        else if (text.Contains("退回") || text.Contains("不通过") || text.Contains("失败") || text.Contains("异常")) return "#f50";
        else if (text.Contains("已") || text.Contains("通过") || text.Contains("成功") || text == "正常") return "#87d068";

        return "default";
    }
}