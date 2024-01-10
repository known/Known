using AntDesign;
using Known.AntBlazor.Components;
using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.AntBlazor;

public class UIService(ModalService modalService, MessageService messageService) : IUIService
{
    private readonly ModalService _modal = modalService;
    private readonly MessageService _message = messageService;

    public Language Language { get; set; }

    public Type GetInputType(ColumnInfo column)
    {
        var type = typeof(string);
        if (column.Property != null)
            type = column.Property.PropertyType;

        if (type == typeof(bool) || column.Type == FieldType.Switch)
            return typeof(Switch);

        if (type == typeof(short))
            return typeof(InputNumber<short>);

        if (type == typeof(int) || column.Type == FieldType.Number)
            return typeof(InputNumber<int>);

        if (type == typeof(long))
            return typeof(InputNumber<long>);

        if (type == typeof(float))
            return typeof(InputNumber<float>);

        if (type == typeof(double))
            return typeof(InputNumber<double>);

        if (type == typeof(decimal))
            return typeof(InputNumber<decimal>);

        if (type == typeof(DateTime?))
            return typeof(DatePicker<DateTime?>);

        if (type == typeof(DateTime) || column.Type == FieldType.Date)
            return typeof(DatePicker<DateTime>);

        if (type == typeof(DateTimeOffset?))
            return typeof(DatePicker<DateTimeOffset?>);

        if (type == typeof(DateTimeOffset))
            return typeof(DatePicker<DateTimeOffset>);

        if (column.Type == FieldType.Select)
            return typeof(AntSelect);

        if (column.Type == FieldType.CheckList)
            return typeof(AntCheckboxGroup);

        if (column.Type == FieldType.RadioList)
            return typeof(AntRadioGroup);

        if (column.Type == FieldType.Password)
            return typeof(InputPassword);

        if (column.Type == FieldType.TextArea)
            return typeof(TextArea);

        return typeof(Input<string>);
    }

    public void AddInputAttributes<TItem>(Dictionary<string, object> attributes, FieldModel<TItem> model) where TItem : class, new()
    {
        var column = model.Column;
        if (!string.IsNullOrWhiteSpace(column.Category))
        {
            if (column.Type == FieldType.Select)
                attributes[nameof(AntSelect.Codes)] = model.GetCodes();

            if (column.Type == FieldType.RadioList)
                attributes[nameof(AntRadioGroup.Codes)] = model.GetCodes("");

            if (column.Type == FieldType.CheckList)
                attributes[nameof(AntCheckboxGroup.Codes)] = model.GetCodes("");
        }
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
            Content = model.Content
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
            option.Content = b => b.Component<DataForm<TItem>>().Set(c => c.Model, model).Build();
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

    public void BuildPage(RenderTreeBuilder builder, PageModel model)
    {
        builder.Component<WebPage>().Set(c => c.Model, model).Build();
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
               .Set(c=>c.ChildContent, delegate (RenderTreeBuilder b)
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

    public void BuildTag(RenderTreeBuilder builder, string text, string color)
    {
        builder.Component<Tag>()
               .Set(c => c.Color, color)
               .Set(c => c.ChildContent, b => b.Text(text))
               .Build();
    }

    public void BuildIcon(RenderTreeBuilder builder, string type)
    {
        builder.Component<Icon>().Set(c => c.Type, type).Set(c => c.Theme, "outline").Build();
    }

    public void BuildResult(RenderTreeBuilder builder, string status, string message)
    {
        builder.Component<AntDesign.Result>()
               .Set(c => c.Status, status)
               .Set(c => c.Title, status)
               .Set(c => c.SubTitle, message)
               .Build();
    }

    public void BuildButton(RenderTreeBuilder builder, ActionInfo info)
    {
        builder.Component<Button>()
               .Set(c => c.Disabled, !info.Enabled)
               .Set(c => c.Icon, info.Icon)
               .Set(c => c.Type, info.Style)
               .Set(c => c.OnClick, info.OnClick)
               .Set(c => c.ChildContent, b => b.Text(info.Name))
               .Build();
    }

    public void BuildText(RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<Input<string>>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildTextArea(RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<TextArea>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Rows, model.Rows)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildPassword(RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<InputPassword>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildDatePicker<TValue>(RenderTreeBuilder builder, InputModel<TValue> model)
    {
        builder.Component<DatePicker<TValue>>()
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
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
               .Set(c => c.Codes, model.Codes)
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
}