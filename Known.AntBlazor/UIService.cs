namespace Known.AntBlazor;

class UIService(ModalService modalService, MessageService messageService, INotificationService noticeService) : IUIService
{
    private readonly ModalService _modal = modalService;
    private readonly MessageService _message = messageService;
    private readonly INotificationService _notice = noticeService;

    public Language Language { get; set; }

    public Type GetInputType(Type dataType, FieldType fieldType)
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

    public void AddInputAttributes<TItem>(Dictionary<string, object> attributes, FieldModel<TItem> model) where TItem : class, new()
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

    public async Task Notice(string message, StyleType style = StyleType.Success)
    {
        var config = new NotificationConfig
        {
            Message = Language?["Title.Error"],
            Description = FormatMessage(message),
            Placement = NotificationPlacement.BottomRight,
            Duration = null
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
                await _notice.Error(config);
                break;
            default:
                await _notice.Info(config);
                break;
        }
    }

    public void Alert(string message, Func<Task> action = null)
    {
        var options = new ConfirmOptions
        {
            Title = Language?.GetTitle("Prompt"),
            Content = FormatMessage(message)
        };
        if (action != null)
            options.OnOk = e => action?.Invoke();
        _modal.Info(options);
    }

    public void Confirm(string message, Func<Task> action)
    {
        var options = new ConfirmOptions
        {
            Title = Language?.GetTitle("Question"),
            Icon = b => b.Component<Icon>().Set(c => c.Type, "question-circle").Set(c => c.Theme, "outline").Build(),
            Content = FormatMessage(message)
        };
        if (action != null)
            options.OnOk = e => action?.Invoke();
        _modal.Confirm(options);
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

    public void ShowDialog(DialogModel model)
    {
        var options = new ModalOptions
        {
            MaskClosable = false,
            Draggable = model.Draggable,
            Resizable = model.Resizable,
            WrapClassName = model.ClassName,
            Title = model.Title,
            Content = model.Content,
            Maximizable = model.Maximizable,
            DefaultMaximized = model.DefaultMaximized,
            OnCancel = e => model.CloseAsync()
        };

        if (model.OnOk != null)
        {
            options.OkText = Language?.OK;
            options.CancelText = Language?.Cancel;
            options.OnOk = e => model.OnOk.Invoke();
        }
        else
        {
            options.Footer = null;
        }

        if (model.Width != null)
            options.Width = model.Width.Value;
        if (model.Footer != null)
            options.Footer = model.Footer;

        var modal = _modal.CreateModal(options);
        model.OnClose = modal.CloseAsync;
    }

    public void ShowForm<TItem>(FormModel<TItem> model) where TItem : class, new()
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

        var isTabForm = false;
        var isStepForm = false;
        if (model.Type == null)
        {
            option.Content = b => BuildForm(b, model);
        }
        else
        {
            isTabForm = model.Type.IsSubclassOf(typeof(BaseTabForm));
            isStepForm = model.Type.IsSubclassOf(typeof(BaseStepForm));
            var parameters = new Dictionary<string, object>
            {
                { nameof(BaseForm<TItem>.Model), model }
            };
            option.Content = b => b.Component(model.Type, parameters);
        }

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
        if (model.IsView || noFooter || isTabForm || isStepForm)
            option.Footer = null;

        var modal = _modal.CreateModal(option);
        model.OnClose = modal.CloseAsync;
    }

    public void BuildForm<TItem>(RenderTreeBuilder builder, FormModel<TItem> model) where TItem : class, new()
    {
        builder.Component<DataForm<TItem>>().Set(c => c.Model, model).Build();
    }

    public void BuildToolbar(RenderTreeBuilder builder, ToolbarModel model)
    {
        builder.Component<Toolbar>().Set(c => c.Model, model).Build();
    }

    public void BuildQuery(RenderTreeBuilder builder, TableModel model)
    {
        builder.Component<QueryForm>().Set(c => c.Model, model).Build();
    }

    public void BuildTable<TItem>(RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new()
    {
        builder.Component<DataTable<TItem>>().Set(c => c.Model, model).Build();
    }

    public void BuildTree(RenderTreeBuilder builder, TreeModel model)
    {
        builder.Component<AntTree>().Set(c => c.Model, model).Build();
    }

    public void BuildSteps(RenderTreeBuilder builder, StepModel model)
    {
        builder.Component<DataSteps>().Set(c => c.Model, model).Build();
    }

    public void BuildTabs(RenderTreeBuilder builder, TabModel model)
    {
        builder.Component<DataTabs>().Set(c => c.Model, model).Build();
    }

    public void BuildDropdown(RenderTreeBuilder builder, DropdownModel model)
    {
        builder.Component<AntDropdown>().Set(c => c.Model, model).Build();
    }

    public void BuildAlert(RenderTreeBuilder builder, string text, StyleType type = StyleType.Info)
    {
        builder.Component<Alert>()
               .Set(c => c.ShowIcon, true)
               .Set(c => c.Style, "margin-bottom:10px;")
               .Set(c => c.Type, type.ToString().ToLower())
               .Set(c => c.Message, text)
               .Build();
    }

    public void BuildTag(RenderTreeBuilder builder, string text, string color = null)
    {
        if (string.IsNullOrWhiteSpace(color))
            color = GetTagColor(text);
        builder.AntTag(text, color);
    }

    public void BuildIcon(RenderTreeBuilder builder, string type, EventCallback<MouseEventArgs>? onClick = null) => builder.AntIcon(type, onClick);
    public void BuildResult(RenderTreeBuilder builder, string status, string message) => builder.AntResult(status, message);
    public void BuildButton(RenderTreeBuilder builder, ActionInfo info) => builder.AntButton(info);
    public void BuildSearch(RenderTreeBuilder builder, InputModel<string> model) => builder.AntSearch(model);
    public void BuildText(RenderTreeBuilder builder, InputModel<string> model) => builder.AntText(model);
    public void BuildTextArea(RenderTreeBuilder builder, InputModel<string> model) => builder.AntTextArea(model);
    public void BuildPassword(RenderTreeBuilder builder, InputModel<string> model) => builder.AntPassword(model);
    public void BuildDatePicker<TValue>(RenderTreeBuilder builder, InputModel<TValue> model) => builder.AntDatePicker(model);
    public void BuildNumber<TValue>(RenderTreeBuilder builder, InputModel<TValue> model) => builder.AntNumber(model);
    public void BuildCheckBox(RenderTreeBuilder builder, InputModel<bool> model) => builder.AntCheckBox(model);
    public void BuildSwitch(RenderTreeBuilder builder, InputModel<bool> model) => builder.AntSwitch(model);
    public void BuildSelect(RenderTreeBuilder builder, InputModel<string> model) => builder.AntSelect(model);
    public void BuildRadioList(RenderTreeBuilder builder, InputModel<string> model) => builder.AntRadioList(model);
    public void BuildCheckList(RenderTreeBuilder builder, InputModel<string[]> model) => builder.AntCheckList(model);

    private static string GetTagColor(string text)
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