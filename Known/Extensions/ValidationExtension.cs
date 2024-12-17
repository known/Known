using AntDesign;

namespace Known.Extensions;

/// <summary>
/// AntDesign表单验证扩展类。
/// </summary>
public static class ValidationExtension
{
    /// <summary>
    /// 获取必填项验证规则集合。
    /// </summary>
    /// <param name="context">系统上下文。</param>
    /// <param name="id">字段ID。</param>
    /// <returns>验证规则集合。</returns>
    public static FormValidationRule[] RuleRequired(this Context context, string id)
    {
        var message = context.Language.Required(id);
        var rule = new FormValidationRule { Type = FormFieldType.String, Required = true, Message = message };
        return [rule];
    }

    internal static FormValidationRule[] ToRules<TItem>(this FieldModel<TItem> model, Context context) where TItem : class, new()
    {
        var column = model.Column;
        if (column == null)
            return [];

        var type = model.GetPropertyType();
        var rules = new List<FormValidationRule>();
        if (column.Required && type != typeof(bool))
            rules.Add(GetFormRuleRequired<TItem>(context, column, type));

        var property = column.Property;
        var min = property?.MinLength();
        if (min != null)
            rules.Add(GetFormRuleMin<TItem>(context, column, min.Value));

        var max = property?.MaxLength();
        if (max != null)
            rules.Add(GetFormRuleMax<TItem>(context, column, max.Value));

        var regex = property?.GetCustomAttribute<RegularExpressionAttribute>();
        if (regex != null)
            rules.Add(GetFormRuleRegex(regex));

        return [.. rules];
    }

    private static FormValidationRule GetFormRuleRequired<TItem>(Context context, ColumnInfo column, Type propertyType)
    {
        var label = context.Language.GetFieldName<TItem>(column);
        var message = context.Language.Required(label);
        return GetFormRuleRequired(message, propertyType);
    }

    private static FormValidationRule GetFormRuleRequired(string message, Type propertyType)
    {
        //String,Number,Boolean,Regexp,Integer,Float,Array,Object,Enum,Date,Url,Email
        var type = FormFieldType.String;
        if (propertyType.IsEnum)
            type = FormFieldType.String;
        else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
            type = FormFieldType.Date;
        else if (propertyType.IsArray)
            type = FormFieldType.Array;
        else if (propertyType == typeof(int) || propertyType == typeof(int?) ||
            propertyType == typeof(long) || propertyType == typeof(long?))
            type = FormFieldType.Integer;
        else if (propertyType == typeof(float) || propertyType == typeof(float?) ||
            propertyType == typeof(decimal) || propertyType == typeof(decimal?) ||
            propertyType == typeof(double) || propertyType == typeof(double?))
            type = FormFieldType.Float;

        return new FormValidationRule { Type = type, Required = true, Message = message };
    }

    private static FormValidationRule GetFormRuleMin<TItem>(Context context, ColumnInfo column, int length)
    {
        var label = context.Language.GetFieldName<TItem>(column);
        var message = context.Language.GetString("Valid.MinLength", label, length);
        return new FormValidationRule { Type = FormFieldType.String, Min = length, Message = message };
    }

    private static FormValidationRule GetFormRuleMax<TItem>(Context context, ColumnInfo column, int length)
    {
        var label = context.Language.GetFieldName<TItem>(column);
        var message = context.Language.GetString("Valid.MaxLength", label, length);
        return new FormValidationRule { Type = FormFieldType.String, Max = length, Message = message };
    }

    private static FormValidationRule GetFormRuleRegex(RegularExpressionAttribute regex)
    {
        return new FormValidationRule { Type = FormFieldType.Regexp, Pattern = regex.Pattern, Message = regex.ErrorMessage };
    }
}