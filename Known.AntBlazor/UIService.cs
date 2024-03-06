namespace Known.AntBlazor;

public class UIService(ModalService modalService, MessageService messageService) : IUIService
{
    private readonly ModalService _modal = modalService;
    private readonly MessageService _message = messageService;

    public static Func<string, string> OnTagColor { get; set; }
    public Language Language { get; set; }

    public Type GetInputType(Type dataType, FieldType fieldType)
    {
        if (fieldType == FieldType.Select)
            return typeof(AntSelect);

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

        if (dataType == typeof(int))
            return typeof(AntNumber<int>);

        if (dataType == typeof(long))
            return typeof(AntNumber<long>);

        if (dataType == typeof(float))
            return typeof(AntNumber<float>);

        if (dataType == typeof(double))
            return typeof(AntNumber<double>);

        if (dataType == typeof(decimal))
            return typeof(AntNumber<decimal>);

        if (dataType == typeof(DateTime?))
            return typeof(AntDatePicker<DateTime?>);

        if (dataType == typeof(DateTime))
            return typeof(AntDatePicker<DateTime>);

        if (dataType == typeof(DateTimeOffset?))
            return typeof(AntDatePicker<DateTimeOffset?>);

        if (dataType == typeof(DateTimeOffset))
            return typeof(AntDatePicker<DateTimeOffset>);

        return typeof(AntInput<string>);
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

        if (column.Type == FieldType.Date || column.Type == FieldType.DateTime)
            attributes["disabled"] = OneOf.OneOf<bool, bool[]>.FromT0(model.IsReadOnly);
    }

    public async void Toast(string message, StyleType style = StyleType.Success)
    {
        switch (style)
        {
            case StyleType.Success:
                await _message.Success(message);
                break;
            case StyleType.Info:
                await _message.Info(message);
                break;
            case StyleType.Warning:
                await _message.Warning(message);
                break;
            case StyleType.Error:
                await _message.Error(message);
                break;
            default:
                await _message.Info(message);
                break;
        }
    }

    public void Alert(string message)
    {
        _modal.Info(new ConfirmOptions
        {
            Title = Language?.GetTitle("Prompt"),
            Content = message
        });
    }

    public void Confirm(string message, Func<Task> action)
    {
        _modal.Confirm(new ConfirmOptions
        {
            Title = Language?.GetTitle("Question"),
            Icon = b => b.Component<Icon>().Set(c => c.Type, "question-circle").Set(c => c.Theme, "outline").Build(),
            Content = message,
            OnOk = e => action?.Invoke()
        });
    }

    public async void ShowDialog(DialogModel model)
    {
        var options = new ModalOptions
        {
            Title = model.Title,
            Content = model.Content,
            Maximizable = model.Maximizable,
            DefaultMaximized = model.DefaultMaximized
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

        var modal = await _modal.CreateModalAsync(options);
        model.OnClose = modal.CloseAsync;
    }

    public async void ShowForm<TItem>(FormModel<TItem> model) where TItem : class, new()
    {
        var option = new ModalOptions
        {
            Title = model.GetFormTitle(),
            OkText = Language?.OK,
            CancelText = Language?.Cancel,
            OnOk = e => model.SaveAsync()
        };

        if (model.Type == null)
        {
            option.Content = b => BuildForm(b, model);
        }
        else
        {
            var parameters = new Dictionary<string, object>
            {
                { nameof(BaseForm<TItem>.Model), model }
            };
            option.Content = b => b.Component(model.Type, parameters);
        }

        var noFooter = false;
        if (model.Option != null)
        {
            option.Maximizable = model.Option.Maximizable;
            option.DefaultMaximized = model.Option.DefaultMaximized;
            noFooter = model.Option.NoFooter;
            if (model.Option.Width != null)
                option.Width = model.Option.Width.Value;
        }
        if (model.IsView || noFooter)
            option.Footer = null;

        var modal = await _modal.CreateModalAsync(option);
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

    public void BuildQuery<TItem>(RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new()
    {
        builder.Component<QueryForm<TItem>>().Set(c => c.Model, model).Build();
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
        builder.Component<Steps>()
               .Set(c => c.Current, model.Current)
               .Set(c => c.ChildContent, delegate (RenderTreeBuilder b)
               {
                   foreach (var item in model.Items)
                   {
                       b.Component<Step>().Set(c => c.Title, Language?.GetTitle(item.Title))
                                          .Set(c => c.Subtitle, Language?.GetTitle(item.SubTitle))
                                          .Set(c => c.Description, Language?.GetTitle(item.Description))
                                          .Build();
                   }
               }).Build();
    }

    public void BuildTabs(RenderTreeBuilder builder, TabModel model)
    {
        builder.Component<Tabs>()
               .Set(c => c.Animated, true)
               .Set(c => c.ChildContent, delegate (RenderTreeBuilder b)
               {
                   var key = 1;
                   foreach (var item in model.Items)
                   {
                       b.Component<TabPane>().Set(c => c.Key, $"{key++}")
                                             .Set(c => c.Tab, Language?.GetTitle(item.Title))
                                             .Set(c => c.ChildContent, item.Content)
                                             .Build();
                   }
               }).Build();
    }

    public void BuildTag(RenderTreeBuilder builder, string text)
    {
        var color = GetTagColor(text);
        var name = Language?.GetCode(text);
        builder.Component<Tag>()
               .Set(c => c.Color, color)
               .Set(c => c.ChildContent, b => b.Text(name))
               .Build();
    }

    public void BuildIcon(RenderTreeBuilder builder, string type, EventCallback<MouseEventArgs>? onClick = null)
    {
        if (onClick == null)
        {
            builder.Component<Icon>().Set(c => c.Type, type).Set(c => c.Theme, "outline").Build();
            return;
        }

        builder.Component<Icon>()
               .Set(c => c.Type, type)
               .Set(c => c.Theme, "outline")
               .Set(c => c.OnClick, onClick)
               .Build();
    }

    public void BuildResult(RenderTreeBuilder builder, string status, string message)
    {
        builder.Component<AntDesign.Result>()
               .Set(c => c.Status, status)
               .Set(c => c.Title, status)
               .Set(c => c.SubTitle, message)
               .Build();
    }

    public void BuildButton(RenderTreeBuilder builder, ActionInfo info) => builder.Button(info);

    public void BuildSearch(RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<Search>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.ClassicSearchIcon, true)
               .Set(c => c.Placeholder, model.Placeholder)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildText(RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<Input<string>>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Placeholder, model.Placeholder)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildTextArea(RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<TextArea>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Placeholder, model.Placeholder)
               .Set(c => c.Rows, model.Rows)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildPassword(RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<InputPassword>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Placeholder, model.Placeholder)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildDatePicker<TValue>(RenderTreeBuilder builder, InputModel<TValue> model)
    {
        if (typeof(TValue) == typeof(string))
        {
            builder.Component<AntRangePicker<TValue>>()
                   .Set(c => c.Disabled, model.Disabled)
                   .Set(c => c.Placeholder, model.Placeholder)
                   .Set(c => c.RangeValue, model.Value)
                   .Set(c => c.RangeValueChanged, model.ValueChanged)
                   .Build();
        }
        else
        {
            builder.Component<DatePicker<TValue>>()
                   .Set(c => c.Disabled, model.Disabled)
                   .Set(c => c.Placeholder, model.Placeholder)
                   .Set(c => c.Value, model.Value)
                   .Set(c => c.ValueChanged, model.ValueChanged)
                   .Build();
        }
    }

    public void BuildNumber<TValue>(RenderTreeBuilder builder, InputModel<TValue> model)
    {
        builder.Component<InputNumber<TValue>>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildCheckBox(RenderTreeBuilder builder, InputModel<bool> model)
    {
        builder.Component<Checkbox>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Checked, model.Value)
               .Set(c => c.Label, model.Placeholder)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildSwitch(RenderTreeBuilder builder, InputModel<bool> model)
    {
        builder.Component<Switch>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Checked, model.Value)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildSelect(RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<AntSelect>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Placeholder, model.Placeholder)
               .Set(c => c.DataSource, model.Codes)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildRadioList(RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<AntRadioGroup>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Codes, model.Codes)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildCheckList(RenderTreeBuilder builder, InputModel<string[]> model)
    {
        builder.Component<AntCheckboxGroup>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Codes, model.Codes)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    private static string GetTagColor(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

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
        else if (text == "Male") return "#108ee9";
        else if (text == "Female") return "hotpink";
        //Flow
        else if (text == "Save") return "default";
        else if (text == "Revoked") return "lime";
        else if (text == "Verifing") return "processing";
        else if (text == "Pass") return "success";
        else if (text == "Fail") return "error";
        //Status
        else if (text.Contains("已完成")) return "success";
        else if (text.Contains("在途")) return "processing";
        else if (text.Contains("待") || text.Contains("中") || text.Contains("提交")) return "#2db7f5";
        else if (text.Contains("完成") || text.Contains("结束")) return "#108ee9";
        else if (text.Contains("退回") || text.Contains("不通过") || text.Contains("失败") || text.Contains("异常")) return "#f50";
        else if (text.Contains("已") || text.Contains("通过") || text.Contains("成功") || text == "正常") return "#87d068";

        if (OnTagColor != null)
            return OnTagColor?.Invoke(text);

        return "default";
    }
}