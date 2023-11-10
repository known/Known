using AntDesign;
using Known;
using Known.Extensions;
using Known.Razor;
using KnownAntDesign.Components;
using Microsoft.AspNetCore.Components;
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
        int.TryParse(column.MaxLength, out int maxLength);

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

    public Task Toast(string message, StyleType style = StyleType.Success)
    {
        switch (style)
        {
            case StyleType.Success:
                return _message.Success(message);
            case StyleType.Info:
                return _message.Info(message);
            case StyleType.Warning:
                return _message.Warning(message);
            case StyleType.Error:
                return _message.Error(message);
            default:
                return _message.Info(message);
        }
    }

    public async Task Result(Known.Result result, Action action = null)
    {
        if (!result.IsValid)
        {
            await _message.Error(result.Message);
            return;
        }

        action?.Invoke();
        await _message.Success(result.Message);
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

    public void ShowForm<TItem>(FormModel<TItem> model) where TItem : class, new()
    {
        RenderFragment content = null;
        if (model.Type == null)
            content = b => b.Component<DataForm<TItem>>().Set(c => c.Model, model).Build();
        else
            content = b => b.Component(model.Type, model.Parameters);

        _modal.CreateModalAsync(new ModalOptions
        {
            Title = model.Title,
            Content = content,
            OkText = "确定",
            CancelText = "取消"
        });
    }

    public void BuildTag(RenderTreeBuilder builder, string text, string color)
    {
        builder.Component<Tag>()
               .Set(c => c.Color, color)
               .Set(c => c.ChildContent, b => b.Markup(text))
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

    public void BuildPage<TItem>(RenderTreeBuilder builder, PageModel<TItem> model) where TItem : class, new()
    {
        builder.Component<WebPage<TItem>>().Set(c => c.Model, model).Build();
    }
}