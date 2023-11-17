using AntDesign;
using Known;
using Known.Extensions;
using Known.Razor;
using KnownAntDesign.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace KnownAntDesign;

class UIService : IUIService
{
    private ModalService _modal;
    private MessageService _message;

    public UIService(ModalService modal, MessageService message)
    {
        _modal = modal;
        _message = message;
    }

    public Type GetInputType(ColumnAttribute column)
    {
        var type = column.Property.PropertyType;
        var maxLength = column.Property.MaxLength();

        if (type == typeof(bool))
            return typeof(Switch);

        if (type == typeof(short))
            return typeof(InputNumber<short>);

        if (type == typeof(int))
            return typeof(InputNumber<int>);

        if (type == typeof(long))
            return typeof(InputNumber<long>);

        if (type == typeof(float))
            return typeof(InputNumber<float>);

        if (type == typeof(double))
            return typeof(InputNumber<double>);

        if (type == typeof(decimal))
            return typeof(InputNumber<decimal>);

        if (type == typeof(string) && maxLength >= 500)
            return typeof(TextArea);

        if (type == typeof(string))
            return typeof(Input<string>);

        if (type == typeof(DateTime))
            return typeof(DatePicker<DateTime>);

        if (type == typeof(DateTime?))
            return typeof(DatePicker<DateTime?>);

        if (type == typeof(DateTimeOffset))
            return typeof(DatePicker<DateTimeOffset>);

        if (type == typeof(DateTimeOffset?))
            return typeof(DatePicker<DateTimeOffset?>);

        //if (type.IsEnum && !type.IsDefined(typeof(FlagsAttribute), inherit: true))
        //    return typeof(Select<>).MakeGenericType(type);

        return typeof(Input<string>);
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
            Title = "提示",
            Content = message
        });
    }

    public void Confirm(string message, Func<Task> action)
    {
        _modal.Confirm(new ConfirmOptions
        {
            Title = "询问",
            Icon = b => b.Component<Icon>().Set(c => c.Type, "question-circle").Set(c => c.Theme, "outline").Build(),
            Content = message,
            OnOk = e => action?.Invoke()
        });
    }

    public async void ShowModal(ModalOption option)
    {
        var options = new ModalOptions
        {
            Title = option.Title,
            Content = option.Content,
            Footer = null
        };
        if (option.Footer != null)
            options.Footer = option.Footer;

        var modal = await _modal.CreateModalAsync(options);
        option.OnClose = modal.CloseAsync;
    }

    public async void ShowForm<TItem>(FormModel<TItem> model) where TItem : class, new()
    {
        var option = new ModalOptions
        {
            Title = model.Title,
            OkText = "确定",
            CancelText = "取消",
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

        if (model.Option.Width != null)
            option.Width = model.Option.Width.Value;
        if (model.IsView || model.Option.NoFooter)
            option.Footer = null;

        var modal = await _modal.CreateModalAsync(option);
        model.OnClose = modal.CloseAsync;
    }

    public void BuildPage<TItem>(RenderTreeBuilder builder, PageModel<TItem> model) where TItem : class, new()
    {
        builder.Component<WebPage<TItem>>().Set(c => c.Model, model).Build();
    }

    public void BuildTag(RenderTreeBuilder builder, string text, string color)
    {
        builder.Component<Tag>()
               .Set(c => c.Color, color)
               .Set(c => c.ChildContent, b => b.Text(text))
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

    public void BuildButton(RenderTreeBuilder builder, ButtonOption option)
    {
        if (string.IsNullOrWhiteSpace(option.Type))
            option.Type = ButtonType.Primary;

        builder.Component<Button>()
               .Set(c => c.Icon, option.Icon)
               .Set(c => c.Type, option.Type)
               .Set(c => c.OnClick, option.OnClick)
               .Set(c => c.ChildContent, b => b.Text(option.Text))
               .Build();
    }
}