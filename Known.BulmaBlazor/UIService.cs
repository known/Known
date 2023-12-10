using BulmaRazor.Components;
using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.BulmaBlazor;

public class UIService(ToastService toast) : IUIService
{
    private readonly ToastService _toast = toast;

    public Type GetInputType(ColumnInfo column)
    {
        var property = column.GetProperty();
        if (property == null)
            return null;

        var type = property.PropertyType;
        var maxLength = property.MaxLength();

        if (type == typeof(bool))
            return typeof(Switch);

        if (type == typeof(short))
            return typeof(NumberInput<short>);

        if (type == typeof(int))
            return typeof(NumberInput<int>);

        if (type == typeof(long))
            return typeof(NumberInput<long>);

        if (type == typeof(float))
            return typeof(NumberInput<float>);

        if (type == typeof(double))
            return typeof(NumberInput<double>);

        if (type == typeof(decimal))
            return typeof(NumberInput<decimal>);

        if (type == typeof(DateTime))
            return typeof(DatePicker<DateTime>);

        if (type == typeof(DateTime?))
            return typeof(DatePicker<DateTime?>);

        if (type == typeof(DateTimeOffset))
            return typeof(DatePicker<DateTimeOffset>);

        if (type == typeof(DateTimeOffset?))
            return typeof(DatePicker<DateTimeOffset?>);

        if (type == typeof(string[]))
            return typeof(CheckBoxes<string>);

        if (type == typeof(string) && !string.IsNullOrWhiteSpace(column.Category))
            return typeof(Radios<string>);

        if (type == typeof(string) && column.IsPassword)
            return typeof(Input<string>);

        if (type == typeof(string) && maxLength >= 500)
            return typeof(Textarea);

        //if (type.IsEnum && !type.IsDefined(typeof(FlagsAttribute), inherit: true))
        //    return typeof(Select<>).MakeGenericType(type);

        return typeof(Input<string>);
    }

    public void AddInputAttributes<TItem>(Dictionary<string, object> attributes, FieldModel<TItem> model) where TItem : class, new()
    {
        var column = model.Column;
        if (!string.IsNullOrWhiteSpace(column.Category))
        {
            var property = column.GetProperty();
            var type = property.PropertyType;

            //if (type == typeof(string))
            //    attributes[nameof(AntRadioGroup.Codes)] = model.GetCodes("");

            //if (type == typeof(string[]))
            //    attributes[nameof(AntCheckboxGroup.Codes)] = model.GetCodes("");
        }
    }

    public async void Toast(string message, StyleType style = StyleType.Success)
    {
        switch (style)
        {
            case StyleType.Success:
                await _toast.Show(message);
                break;
            case StyleType.Info:
                await _toast.Show(message);
                break;
            case StyleType.Warning:
                await _toast.Show(message);
                break;
            case StyleType.Error:
                await _toast.Show(message);
                break;
            default:
                await _toast.Show(message);
                break;
        }
    }

    public void Alert(string message)
    {
        //_toast.Info(new ConfirmOptions
        //{
        //    Title = "提示",
        //    Content = message
        //});
    }

    public void Confirm(string message, Func<Task> action)
    {
        //_toast.Confirm(new ConfirmOptions
        //{
        //    Title = "询问",
        //    Icon = b => b.Component<Icon>().Set(c => c.Type, "question-circle").Set(c => c.Theme, "outline").Build(),
        //    Content = message,
        //    OnOk = e => action?.Invoke()
        //});
    }

    public void ShowModal(ModalOption option)
    {
        //var options = new ModalOptions
        //{
        //    Title = option.Title,
        //    Content = option.Content
        //};

        //if (option.OnOk != null)
        //{
        //    options.OkText = "确定";
        //    options.CancelText = "取消";
        //    options.OnOk = e => option.OnOk.Invoke();
        //}
        //else
        //{
        //    options.Footer = null;
        //}

        //if (option.Footer != null)
        //    options.Footer = option.Footer;

        //var modal = await _toast.CreateModalAsync(options);
        //option.OnClose = modal.CloseAsync;
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
        //    option.Maximizable = model.Option.Maximizable;
        //    option.DefaultMaximized = model.Option.DefaultMaximized;
        //    noFooter = model.Option.NoFooter;
        //    if (model.Option.Width != null)
        //        option.Width = model.Option.Width.Value;
        //}
        //if (model.IsView || noFooter)
        //    option.Footer = null;

        //var modal = await _toast.CreateModalAsync(option);
        //model.OnClose = modal.CloseAsync;
    }

    public void BuildForm<TItem>(RenderTreeBuilder builder, FormModel<TItem> model) where TItem : class, new()
    {
        //builder.Component<DataForm<TItem>>().Set(c => c.Model, model).Build();
    }

    public void BuildPage(RenderTreeBuilder builder, PageModel model)
    {
        //builder.Component<WebPage>().Set(c => c.Model, model).Build();
    }

    public void BuildPage<TItem>(RenderTreeBuilder builder, TablePageModel<TItem> model) where TItem : class, new()
    {
        //builder.Component<DataTablePage<TItem>>().Set(c => c.Model, model).Build();
    }

    public void BuildTable<TItem>(RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new()
    {
        //builder.Component<DataTable<TItem>>().Set(c => c.Model, model).Build();
    }

    public void BuildTree(RenderTreeBuilder builder, TreeModel model)
    {
        //builder.Component<Tree>().Set(c => c.Model, model).Build();
    }

    public void BuildSteps(RenderTreeBuilder builder, StepModel model)
    {
        //builder.Component<DataSteps>().Set(c => c.Model, model).Build();
    }

    public void BuildTabs(RenderTreeBuilder builder, TabModel model)
    {
        //builder.Component<DataTabs>().Set(c => c.Model, model).Build();
    }

    public void BuildTag(RenderTreeBuilder builder, string text, string color)
    {
        builder.Component<Tag>()
               .Set(c => c.IsLight, true)
               .Set(c => c.Color, ToColor(color))
               .Set(c => c.ChildContent, b => b.Text(text))
               .Build();
    }

    public void BuildIcon(RenderTreeBuilder builder, string type)
    {
        //builder.Component<Icon>().Set(c => c.Type, type).Set(c => c.Theme, "outline").Build();
    }

    public void BuildResult(RenderTreeBuilder builder, string status, string message)
    {
        //builder.Component<AntDesign.Result>()
        //       .Set(c => c.Status, status)
        //       .Set(c => c.Title, status)
        //       .Set(c => c.SubTitle, message)
        //       .Build();
    }

    public void BuildButton(RenderTreeBuilder builder, ActionInfo info)
    {
        builder.Component<Button>()
               //.Set(c => c.Icon, info.Icon)
               .Set(c => c.Color, ToColor(info.Style))
               //.Set(c => c.OnClick, info.OnClick)
               .Set(c => c.ChildContent, b => b.Text(info.Name))
               .Build();
    }

    public void BuildInput<TValue>(RenderTreeBuilder builder, InputOption<TValue> option)
    {
        builder.Component<Input<TValue>>()
               .Set(c => c.Value, option.Value)
               .Set(c => c.ValueChanged, option.ValueChanged)
               .Build();
    }

    public void BuildCheckList(RenderTreeBuilder builder, ListOption<string[]> option)
    {
        //builder.Component<AntCheckboxGroup>()
        //       .Set(c => c.Disabled, option.Disabled)
        //       .Set(c => c.Codes, option.Codes)
        //       .Set(c => c.Value, option.Value)
        //       .Set(c => c.ValueChanged, option.ValueChanged)
        //       .Build();
    }

    internal static Color ToColor(string color)
    {
        if (color == "primary")
            return Color.Primary;
        else if (color == "danger")
            return Color.Danger;

        return Color.Primary;
    }
}