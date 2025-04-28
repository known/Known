using AntDesign;

namespace Known.Extensions;

/// <summary>
/// 字段组件扩展类。
/// </summary>
public static class FieldExtension
{
    /// <summary>
    /// 构建下拉框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">下拉框组件模型对象。</param>
    public static void Dropdown(this RenderTreeBuilder builder, DropdownModel model)
    {
        builder.Component<AntDropdown>().Set(c => c.Model, model).Build();
    }

    /// <summary>
    /// 构建搜索框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    public static void Search(this RenderTreeBuilder builder, InputModel<string> model)
    {
        builder.Component<Search>()
               .Set(c => c.ReadOnly, model.ReadOnly)
               .Set(c => c.Disabled, model.Disabled)
               .Set(c => c.ClassicSearchIcon, true)
               .Set(c => c.Placeholder, model.Placeholder)
               .Set(c => c.Value, model.Value)
               .Set(c => c.ValueChanged, model.ValueChanged)
               .Build();
    }

    /// <summary>
    /// 构建文本框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    public static void TextBox(this RenderTreeBuilder builder, InputModel<string> model) => builder.TextBox(null, model);

    /// <summary>
    /// 构建文本框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="label">字段标题。</param>
    /// <param name="model">输入模型对象。</param>
    public static void TextBox(this RenderTreeBuilder builder, string label, InputModel<string> model)
    {
        builder.FieldItem(label, b =>
        {
            b.Component<Input<string>>()
             .Set(c => c.ReadOnly, model.ReadOnly)
             .Set(c => c.Disabled, model.Disabled)
             .Set(c => c.Placeholder, model.Placeholder)
             .Set(c => c.Value, model.Value)
             .Set(c => c.ValueChanged, model.ValueChanged)
             .Build();
        });
    }

    /// <summary>
    /// 构建多行文本框框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    public static void TextArea(this RenderTreeBuilder builder, InputModel<string> model) => builder.TextArea(null, model);

    /// <summary>
    /// 构建多行文本框框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="label">字段标题。</param>
    /// <param name="model">输入模型对象。</param>
    public static void TextArea(this RenderTreeBuilder builder, string label, InputModel<string> model)
    {
        builder.FieldItem(label, b =>
        {
            b.Component<TextArea>()
             .Set(c => c.ReadOnly, model.ReadOnly)
             .Set(c => c.Disabled, model.Disabled)
             .Set(c => c.Placeholder, model.Placeholder)
             .Set(c => c.Rows, model.Rows)
             .Set(c => c.Value, model.Value)
             .Set(c => c.ValueChanged, model.ValueChanged)
             .Build();
        });
    }

    /// <summary>
    /// 构建密码输入框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    public static void Password(this RenderTreeBuilder builder, InputModel<string> model) => builder.Password(null, model);

    /// <summary>
    /// 构建密码输入框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="label">字段标题。</param>
    /// <param name="model">输入模型对象。</param>
    public static void Password(this RenderTreeBuilder builder, string label, InputModel<string> model)
    {
        builder.FieldItem(label, b =>
        {
            b.Component<InputPassword>()
             .Set(c => c.ReadOnly, model.ReadOnly)
             .Set(c => c.Disabled, model.Disabled)
             .Set(c => c.Placeholder, model.Placeholder)
             .Set(c => c.Value, model.Value)
             .Set(c => c.ValueChanged, model.ValueChanged)
             .Build();
        });
    }

    /// <summary>
    /// 构建日期选择框组件。
    /// </summary>
    /// <typeparam name="TValue">日期类型。</typeparam>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    public static void DatePicker<TValue>(this RenderTreeBuilder builder, InputModel<TValue> model) => builder.DatePicker(null, model);

    /// <summary>
    /// 构建日期选择框组件。
    /// </summary>
    /// <typeparam name="TValue">日期类型。</typeparam>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="label">字段标题。</param>
    /// <param name="model">输入模型对象。</param>
    public static void DatePicker<TValue>(this RenderTreeBuilder builder, string label, InputModel<TValue> model)
    {
        builder.FieldItem(label, b =>
        {
            b.Component<DatePicker<TValue>>()
             .Set(c => c.Disabled, model.Disabled)
             .Set(c => c.Placeholder, model.Placeholder)
             .Set(c => c.Value, model.Value)
             .Set(c => c.ValueChanged, model.ValueChanged)
             .Build();
        });
    }

    /// <summary>
    /// 构建日期范围选择框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    public static void RangePicker(this RenderTreeBuilder builder, InputModel<string> model) => builder.RangePicker(null, model);

    /// <summary>
    /// 构建日期范围选择框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="label">字段标题。</param>
    /// <param name="model">输入模型对象。</param>
    public static void RangePicker(this RenderTreeBuilder builder, string label, InputModel<string> model)
    {
        builder.FieldItem(label, b =>
        {
            b.Component<AntRangePicker>()
             .Set(c => c.Disabled, model.Disabled)
             .Set(c => c.Placeholder, model.Placeholder)
             .Set(c => c.RangeValue, model.Value)
             .Set(c => c.RangeValueChanged, model.ValueChanged)
             .Build();
        });
    }

    /// <summary>
    /// 构建数字输入框组件。
    /// </summary>
    /// <typeparam name="TValue">数字类型。</typeparam>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    public static void Number<TValue>(this RenderTreeBuilder builder, InputModel<TValue> model) => builder.Number(null, model);

    /// <summary>
    /// 构建数字输入框组件。
    /// </summary>
    /// <typeparam name="TValue">数字类型。</typeparam>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="label">字段标题。</param>
    /// <param name="model">输入模型对象。</param>
    public static void Number<TValue>(this RenderTreeBuilder builder, string label, InputModel<TValue> model)
    {
        builder.FieldItem(label, b =>
        {
            b.Component<AntDesign.InputNumber<TValue>>()
             .Set(c => c.Disabled, model.Disabled)
             .Set(c => c.Value, model.Value)
             .Set(c => c.ValueChanged, model.ValueChanged)
             .Build();
        });
    }

    /// <summary>
    /// 构建复选框框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    public static void CheckBox(this RenderTreeBuilder builder, InputModel<bool> model) => builder.CheckBox(null, model);

    /// <summary>
    /// 构建复选框框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="label">字段标题。</param>
    /// <param name="model">输入模型对象。</param>
    public static void CheckBox(this RenderTreeBuilder builder, string label, InputModel<bool> model)
    {
        builder.FieldItem(label, b =>
        {
            b.Component<Checkbox>()
             .Set(c => c.Disabled, model.Disabled)
             .Set(c => c.Checked, model.Value)
             .Set(c => c.Indeterminate, model.Indeterminate)
             .Set(c => c.Label, model.Label)
             .Set(c => c.Value, model.Value)
             .Set(c => c.ValueChanged, model.ValueChanged)
             .Build();
        });
    }

    /// <summary>
    /// 构建开关组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    public static void Switch(this RenderTreeBuilder builder, InputModel<bool> model) => builder.Switch(null, model);

    /// <summary>
    /// 构建开关组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="label">字段标题。</param>
    /// <param name="model">输入模型对象。</param>
    public static void Switch(this RenderTreeBuilder builder, string label, InputModel<bool> model)
    {
        builder.FieldItem(label, b =>
        {
            b.Component<Switch>()
             .Set(c => c.Disabled, model.Disabled)
             .Set(c => c.Checked, model.Value)
             .Set(c => c.Value, model.Value)
             .Set(c => c.ValueChanged, model.ValueChanged)
             .Build();
        });
    }

    /// <summary>
    /// 构建下拉选择框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    public static void Select(this RenderTreeBuilder builder, InputModel<string> model) => builder.Select(null, model);

    /// <summary>
    /// 构建下拉选择框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="label">字段标题。</param>
    /// <param name="model">输入模型对象。</param>
    public static void Select(this RenderTreeBuilder builder, string label, InputModel<string> model)
    {
        builder.FieldItem(label, b =>
        {
            b.Component<AntSelectCode>()
             .Set(c => c.Disabled, model.Disabled)
             .Set(c => c.Placeholder, model.Placeholder)
             .Set(c => c.DataSource, model.Codes)
             .Set(c => c.Value, model.Value)
             .Set(c => c.ValueChanged, model.ValueChanged)
             .Build();
        });
    }

    /// <summary>
    /// 构建单选框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    public static void RadioList(this RenderTreeBuilder builder, InputModel<string> model) => builder.RadioList(null, model);

    /// <summary>
    /// 构建单选框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="label">字段标题。</param>
    /// <param name="model">输入模型对象。</param>
    public static void RadioList(this RenderTreeBuilder builder, string label, InputModel<string> model)
    {
        builder.FieldItem(label, b =>
        {
            b.Component<AntRadioGroup>()
             .Set(c => c.Disabled, model.Disabled)
             .Set(c => c.Codes, model.Codes)
             .Set(c => c.Value, model.Value)
             .Set(c => c.ValueChanged, model.ValueChanged)
             .Build();
        });
    }

    /// <summary>
    /// 构建复选框列表组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    public static void CheckList(this RenderTreeBuilder builder, InputModel<string[]> model) => builder.CheckList(null, model);

    /// <summary>
    /// 构建复选框列表组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="label">字段标题。</param>
    /// <param name="model">输入模型对象。</param>
    public static void CheckList(this RenderTreeBuilder builder, string label, InputModel<string[]> model)
    {
        builder.FieldItem(label, b =>
        {
            b.Component<AntCheckboxGroup>()
             .Set(c => c.Disabled, model.Disabled)
             .Set(c => c.Codes, model.Codes)
             .Set(c => c.Value, model.Value)
             .Set(c => c.ValueChanged, model.ValueChanged)
             .Build();
        });
    }

    /// <summary>
    /// 呈现附件超链接。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="item">系统附件对象。</param>
    public static void FileLink(this RenderTreeBuilder builder, AttachInfo item)
    {
        builder.Component<KFileLink>().Set(c => c.Item, item).Build();
    }

    /// <summary>
    /// 构建字段项组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="label">字段标题。</param>
    /// <param name="child">字段项模板。</param>
    public static void FieldItem(this RenderTreeBuilder builder, string label, RenderFragment child)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            builder.Fragment(child);
            return;
        }

        builder.Component<FieldItem>()
               .Set(c => c.Label, label)
               .Set(c => c.ChildContent, child)
               .Build();
    }
}