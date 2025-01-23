using AntDesign;

namespace Known.Blazor;

public partial class UIService
{
    /// <summary>
    /// 根据字段类型获取对应的输入组件类型。
    /// </summary>
    /// <param name="dataType">字段属性类型。</param>
    /// <param name="fieldType">字段类型。</param>
    /// <returns></returns>
    internal Type GetInputType(Type dataType, FieldType fieldType)
    {
        if (fieldType == FieldType.AutoComplete) return typeof(AntAutoComplete);
        if (fieldType == FieldType.Select) return typeof(AntSelectCode);
        if (fieldType == FieldType.CheckBox) return typeof(Checkbox);
        if (fieldType == FieldType.CheckList) return typeof(AntCheckboxGroup);
        if (fieldType == FieldType.RadioList) return typeof(AntRadioGroup);
        if (fieldType == FieldType.Password) return typeof(InputPassword);
        if (fieldType == FieldType.TextArea) return typeof(AntTextArea);
        if (fieldType == FieldType.Date) return typeof(AntDatePicker);
        if (fieldType == FieldType.DateTime) return typeof(AntDateTimePicker);
        if (dataType == typeof(bool)) return typeof(Switch);
        if (dataType == typeof(short)) return typeof(AntNumber<short>);
        if (dataType == typeof(short?)) return typeof(AntNumber<short?>);
        if (dataType == typeof(int)) return typeof(AntNumber<int>);
        if (dataType == typeof(int?)) return typeof(AntNumber<int?>);
        if (dataType == typeof(long)) return typeof(AntNumber<long>);
        if (dataType == typeof(long?)) return typeof(AntNumber<long?>);
        if (dataType == typeof(float)) return typeof(AntNumber<float>);
        if (dataType == typeof(float?)) return typeof(AntNumber<float?>);
        if (dataType == typeof(double)) return typeof(AntNumber<double>);
        if (dataType == typeof(double?)) return typeof(AntNumber<double?>);
        if (dataType == typeof(decimal)) return typeof(AntNumber<decimal>);
        if (dataType == typeof(decimal?)) return typeof(AntNumber<decimal?>);
        if (dataType == typeof(DateTime)) return typeof(DatePicker<DateTime>);
        if (dataType == typeof(DateTime?)) return typeof(AntDatePicker);
        if (dataType == typeof(DateTimeOffset)) return typeof(DatePicker<DateTimeOffset>);
        if (dataType == typeof(DateTimeOffset?)) return typeof(DatePicker<DateTimeOffset?>);
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
}