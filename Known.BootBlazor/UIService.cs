using BootstrapBlazor.Components;
using Known.Blazor;
using Known.BootBlazor.Components;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Known.BootBlazor;

public class UIService(DialogService dialogService, MessageService messageService) : IUIService
{
    private readonly DialogService _dialog = dialogService;
    private readonly MessageService _message = messageService;

    public Language Language { get; set; }

    public Type GetInputType(Type dataType, FieldType fieldType)
    {
        if (dataType == typeof(bool))
            return typeof(Switch);

        if (dataType == typeof(short))
            return typeof(BootstrapInputNumber<short>);

        if (dataType == typeof(int))
            return typeof(BootstrapInputNumber<int>);

        if (dataType == typeof(long))
            return typeof(BootstrapInputNumber<long>);

        if (dataType == typeof(float))
            return typeof(BootstrapInputNumber<float>);

        if (dataType == typeof(double))
            return typeof(BootstrapInputNumber<double>);

        if (dataType == typeof(decimal))
            return typeof(BootstrapInputNumber<decimal>);

        if (dataType == typeof(DateTime?))
            return typeof(DateTimePicker<DateTime?>);

        if (dataType == typeof(DateTime))
            return typeof(DateTimePicker<DateTime>);

        if (dataType == typeof(DateTimeOffset?))
            return typeof(DateTimePicker<DateTimeOffset?>);

        if (dataType == typeof(DateTimeOffset))
            return typeof(DateTimePicker<DateTimeOffset>);

        if (fieldType == FieldType.Select)
            return typeof(BootSelect);

        if (fieldType == FieldType.CheckBox)
            return typeof(Checkbox<bool>);

        if (fieldType == FieldType.CheckList)
            return typeof(BootCheckboxList);

        if (fieldType == FieldType.RadioList)
            return typeof(BootRadioList);

        if (fieldType == FieldType.Password)
            return typeof(BootstrapPassword);

        if (fieldType == FieldType.TextArea)
            return typeof(Textarea);

        return typeof(BootstrapInput<string>);
    }

    public void AddInputAttributes<TItem>(Dictionary<string, object> attributes, FieldModel<TItem> model) where TItem : class, new()
    {
        var column = model.Column;
        if (!string.IsNullOrWhiteSpace(column.Category))
        {
            if (column.Type == FieldType.Select)
                attributes[nameof(BootSelect.Codes)] = model.GetCodes();

            if (column.Type == FieldType.RadioList)
                attributes[nameof(BootRadioList.Codes)] = model.GetCodes("");

            if (column.Type == FieldType.CheckList)
                attributes[nameof(BootCheckboxList.Codes)] = model.GetCodes("");
        }
    }

    public void Toast(string message, StyleType style = StyleType.Success)
    {
        //switch (style)
        //{
        //    case StyleType.Success:
        //        await _message.Show(new MessageOption { Content = message });
        //        break;
        //    case StyleType.Info:
        //        await _message.Show(new MessageOption { Content = message });
        //        break;
        //    case StyleType.Warning:
        //        await _message.Show(new MessageOption { Content = message });
        //        break;
        //    case StyleType.Error:
        //        await _message.Show(new MessageOption { Content = message });
        //        break;
        //    default:
        //        await _message.Show(new MessageOption { Content = message });
        //        break;
        //}
    }

    public void Alert(string message)
    {
        //_dialog.Info(new ConfirmOptions
        //{
        //    Title = "提示",
        //    Content = message
        //});
    }

    public void Confirm(string message, Func<Task> action)
    {
        action?.Invoke();
        //_dialog.Show(new DialogOption
        //{
        //    Title = "询问",
        //    BodyTemplate = b => b.Text(message)
        //});
        //_dialog.Confirm(new ConfirmOptions
        //{
        //    Title = "询问",
        //    Icon = b => b.Component<Icon>().Set(c => c.Type, "question-circle").Set(c => c.Theme, "outline").Build(),
        //    Content = message,
        //    OnOk = e => action?.Invoke()
        //});
    }

    public void ShowDialog(DialogModel model)
    {
        //var options = new ModalOptions
        //{
        //    Title = model.Title,
        //    Content = model.Content
        //};

        //if (model.OnOk != null)
        //{
        //    options.OkText = "确定";
        //    options.CancelText = "取消";
        //    options.OnOk = e => model.OnOk.Invoke();
        //}
        //else
        //{
        //    options.Footer = null;
        //}

        //if (model.Footer != null)
        //    options.Footer = model.Footer;

        //var modal = await _dialog.CreateModalAsync(options);
        //model.OnClose = modal.CloseAsync;
    }

    public void ShowForm<TItem>(FormModel<TItem> model) where TItem : class, new()
    {
        //var option = new ModalOptions
        //{
        //    Title = model.Title,
        //    OkText = "确定",
        //    CancelText = "取消",
        //    OnOk = e => model.SaveAsync()
        //};

        //if (model.Type == null)
        //{
        //    option.Content = b => b.Component<DataForm<TItem>>().Set(c => c.Model, model).Build();
        //}
        //else
        //{
        //    var parameters = new Dictionary<string, object>
        //    {
        //        { nameof(BaseForm<TItem>.Model), model }
        //    };
        //    option.Content = b => b.Component(model.Type, parameters);
        //}

        //var noFooter = false;
        //if (model.Option != null)
        //{
        //    noFooter = model.Option.NoFooter;
        //    if (model.Option.Width != null)
        //        option.Width = model.Option.Width.Value;
        //}
        //if (model.IsView || noFooter)
        //    option.Footer = null;

        //var modal = await _dialog.CreateModalAsync(option);
        //model.OnClose = modal.CloseAsync;
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
        builder.Component<BootTree>().Set(c => c.Model, model).Build();
    }

    public void BuildSteps(RenderTreeBuilder builder, StepModel model)
    {
        builder.Component<Step>().Set(c => c.Items, model?.Items?.Select(m => new StepOption
        {
            Text = Language?.GetTitle(m.Title),
            Title = Language?.GetTitle(m.SubTitle),
            Description = Language?.GetTitle(m.Description)
        }).ToList()).Build();
    }

    public void BuildTabs(RenderTreeBuilder builder, TabModel model)
    {
        builder.Component<Tab>().Set(c => c.ChildContent, delegate (RenderTreeBuilder b)
        {
            foreach (var item in model.Items)
            {
                b.Component<TabItem>().Set(c => c.Text, Language?.GetTitle(item.Title))
                                      .Set(c => c.ChildContent, item.Content)
                                      .Build();
            }
        }).Build();
    }

    public void BuildTag(RenderTreeBuilder builder, string text)
    {
        var name = Language?.GetCode(text);
        builder.Component<Tag>()
               .Set(c => c.Color, Color.Primary)
               .Set(c => c.ChildContent, b => b.Text(name))
               .Build();
    }

    public void BuildIcon(RenderTreeBuilder builder, string type, EventCallback<MouseEventArgs>? onClick = null)
    {
        builder.Span().Class(type).OnClick(onClick).Close();
    }

    public void BuildResult(RenderTreeBuilder builder, string status, string message)
    {
        builder.Component<Empty>()
               .Set(c => c.Image, "_content/Known.Demo/img/none.png")
               .Set(c => c.Text, message)
               .Build();
    }

    public void BuildButton(RenderTreeBuilder builder, ActionInfo info)
    {
        builder.Component<Button>()
               .Set(c => c.IsDisabled, !info.Enabled)
               .Set(c => c.Icon, info.Icon)
               .Set(c => c.Color, info.ToColor())
               .Set(c => c.OnClick, info.OnClick)
               .Set(c => c.ChildContent, b => b.Text(info.Name))
               .Build();
    }

    public void BuildText(RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<BootstrapInput<string>>()
               .Set(c => c.IsDisabled, model.Disabled)
               .Set(c => c.PlaceHolder, model.Placeholder)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildTextArea(RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<Textarea>()
               .Set(c => c.IsDisabled, model.Disabled)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildPassword(RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<BootstrapPassword>()
               .Set(c => c.IsDisabled, model.Disabled)
               .Set(c => c.PlaceHolder, model.Placeholder)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildDatePicker<TValue>(RenderTreeBuilder builder, InputModel<TValue> model)
    {
        builder.Component<DateTimePicker<TValue>>()
               .Set(c => c.IsDisabled, model.Disabled)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildNumber<TValue>(RenderTreeBuilder builder, InputModel<TValue> model)
    {
        builder.Component<BootstrapInput<TValue>>()
               .Set(c => c.PlaceHolder, model.Placeholder)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildCheckBox(RenderTreeBuilder builder, InputModel<bool> model)
    {
        builder.Component<Checkbox<bool>>()
               .Set(c => c.IsDisabled, model.Disabled)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildSwitch(RenderTreeBuilder builder, InputModel<bool> model)
    {
        builder.Component<Switch>()
               .Set(c => c.IsDisabled, model.Disabled)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildSelect(RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<BootSelect>()
               .Set(c => c.IsDisabled, model.Disabled)
               .Set(c => c.PlaceHolder, model.Placeholder)
               .Set(c => c.Codes, model.Codes)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildRadioList(RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<BootRadioList>()
               .Set(c => c.IsDisabled, model.Disabled)
               .Set(c => c.Codes, model.Codes)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    public void BuildCheckList(RenderTreeBuilder builder, InputModel<string[]> option)
    {
        var value = option.Value != null ? string.Join(',', option.Value) : "";
        builder.Component<BootCheckboxList>()
               .Set(c => c.IsDisabled, option.Disabled)
               .Set(c => c.Codes, option.Codes)
               .Set(c => c.Value, value)
               //.Set(c => c.ValueChanged, e=> option.ValueChanged?.InvokeAsync(e.sp)
               .Build();
    }
}