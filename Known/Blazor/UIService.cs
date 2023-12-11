using Known.Extensions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

public interface IUIService
{
    Type GetInputType(ColumnInfo column);
    void AddInputAttributes<TItem>(Dictionary<string, object> attributes, FieldModel<TItem> model) where TItem : class, new();
    void Toast(string message, StyleType style = StyleType.Success);
    void Alert(string message);
    void Confirm(string message, Func<Task> action);
    void ShowModal(ModalOption option);
    void ShowForm<TItem>(FormModel<TItem> model) where TItem : class, new();
    void BuildForm<TItem>(RenderTreeBuilder builder, FormModel<TItem> model) where TItem : class, new();
    void BuildPage(RenderTreeBuilder builder, PageModel model);
    void BuildPage<TItem>(RenderTreeBuilder builder, TablePageModel<TItem> model) where TItem : class, new();
    void BuildTable<TItem>(RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new();
    void BuildTree(RenderTreeBuilder builder, TreeModel model);
    void BuildSteps(RenderTreeBuilder builder, StepModel model);
    void BuildTabs(RenderTreeBuilder builder, TabModel model);
    void BuildTag(RenderTreeBuilder builder, string text, string color);
    void BuildIcon(RenderTreeBuilder builder, string type);
    void BuildResult(RenderTreeBuilder builder, string status, string message);
    void BuildButton(RenderTreeBuilder builder, ActionInfo info);
    void BuildInput<TValue>(RenderTreeBuilder builder, InputOption<TValue> option);
    void BuildCheckList(RenderTreeBuilder builder, ListOption<string[]> option);
}

public class UIService : IUIService
{
    public Type GetInputType(ColumnInfo column)
    {
        var property = column.GetProperty();
        if (property == null)
            return null;

        var type = property.PropertyType;
        var maxLength = property.MaxLength();

        if (type == typeof(bool))
            return typeof(InputCheckbox);

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

        if (type == typeof(DateTime))
            return typeof(InputDate<DateTime>);

        if (type == typeof(DateTime?))
            return typeof(InputDate<DateTime?>);

        if (type == typeof(DateTimeOffset))
            return typeof(InputDate<DateTimeOffset>);

        if (type == typeof(DateTimeOffset?))
            return typeof(InputDate<DateTimeOffset?>);

        if (type == typeof(string[]))
            return typeof(InputCheckbox);

        if (type == typeof(string) && !string.IsNullOrWhiteSpace(column.Category))
            return typeof(InputRadioGroup<string>);

        if (type == typeof(string) && column.IsPassword)
            return typeof(InputText);

        if (type == typeof(string) && maxLength >= 500)
            return typeof(InputTextArea);

        //if (type.IsEnum && !type.IsDefined(typeof(FlagsAttribute), inherit: true))
        //    return InputSelect typeof(Select<>).MakeGenericType(type);

        return typeof(InputText);
    }

    public void AddInputAttributes<TItem>(Dictionary<string, object> attributes, FieldModel<TItem> model) where TItem : class, new()
    {
        var column = model.Column;
        if (column.IsPassword)
            attributes["type"] = "password";

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

    public void Toast(string message, StyleType style = StyleType.Success)
    {

    }

    public void Alert(string message)
    {

    }

    public void Confirm(string message, Func<Task> action)
    {

    }

    public void ShowModal(ModalOption option)
    {

    }

    public void ShowForm<TItem>(FormModel<TItem> model) where TItem : class, new()
    {

    }

    public void BuildForm<TItem>(RenderTreeBuilder builder, FormModel<TItem> model) where TItem : class, new()
    {
        builder.Component<KForm<TItem>>().Set(c => c.Form, model).Build();
    }

    public void BuildPage(RenderTreeBuilder builder, PageModel model)
    {

    }

    public void BuildPage<TItem>(RenderTreeBuilder builder, TablePageModel<TItem> model) where TItem : class, new()
    {

    }

    public void BuildTable<TItem>(RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new()
    {

    }

    public void BuildTree(RenderTreeBuilder builder, TreeModel model)
    {

    }

    public void BuildSteps(RenderTreeBuilder builder, StepModel model)
    {

    }

    public void BuildTabs(RenderTreeBuilder builder, TabModel model)
    {

    }

    public void BuildTag(RenderTreeBuilder builder, string text, string color)
    {

    }

    public void BuildIcon(RenderTreeBuilder builder, string type)
    {

    }

    public void BuildResult(RenderTreeBuilder builder, string status, string message)
    {

    }

    public void BuildButton(RenderTreeBuilder builder, ActionInfo info)
    {
        builder.OpenElement("button").Id(info.Id).Class($"kui-button {info.Style}").OnClick(info.OnClick).Text(info.Name).CloseElement();
    }

    public void BuildInput<TValue>(RenderTreeBuilder builder, InputOption<TValue> option)
    {
        //builder.Component<InputBase<string>>().Set(c => c.Value, option.Value).Set(c => c.ValueChanged, option.ValueChanged).Build();
    }

    public void BuildCheckList(RenderTreeBuilder builder, ListOption<string[]> option)
    {

    }
}