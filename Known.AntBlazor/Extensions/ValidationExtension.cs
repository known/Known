namespace Known.AntBlazor.Extensions;

static class ValidationExtension
{
    internal static FormValidationRule[] RuleRequired(this Context context, string id)
    {
        var message = context.Language.Required(id);
        var rule = new FormValidationRule { Type = FormFieldType.String, Required = true, Message = message };
        return [rule];
    }

    internal static FormValidationRule[] ToRules(this DataItem item, Context context = null)
    {
        var rules = new List<FormValidationRule>();
        if (item.Required)
        {
            var message = context != null
                        ? context.Language.Required(item.Label)
                        : $"{item.Label}不能为空！";
            rules.Add(GetFormRuleRequired(message, item.Type));
        }
        return [.. rules];
    }

    internal static FormValidationRule[] ToRules<TItem>(this FieldModel<TItem> model, Context context) where TItem : class, new()
    {
        var column = model.Column;
        if (column == null)
            return [];

        var type = model.GetPropertyType();
        var rules = new List<FormValidationRule>();
        if (column.Required && type != typeof(bool))
        {
            //TODO：动态数据表单验证问题
            rules.Add(GetFormRuleRequired(context, column, type));
        }
        else
        {
            var property = column.Property;
            var min = property?.MinLength();
            if (min != null)
                rules.Add(GetFormRuleMin(context, column, min.Value));

            var max = property?.MaxLength();
            if (max != null)
                rules.Add(GetFormRuleMax(context, column, max.Value));

            var regex = property?.GetCustomAttribute<RegexAttribute>();
            if (regex != null)
                rules.Add(GetFormRuleRegex(regex));
        }

        return [.. rules];
    }

    private static FormValidationRule GetFormRuleRequired(Context context, ColumnInfo column, Type propertyType)
    {
        var label = context.Language.GetString(column);
        var message = context.Language.Required(label);
        return GetFormRuleRequired(message, propertyType);
    }

    private static FormValidationRule GetFormRuleRequired(string message, Type propertyType)
    {
        //String,Number,Boolean,Regexp,Integer,Float,Array,Object,Enum,Date,Url,Email
        var type = FormFieldType.String;
        if (propertyType.IsEnum)
            type = FormFieldType.Enum;
        else if (propertyType == typeof(DateTime))
            type = FormFieldType.Date;
        else if (propertyType.IsArray)
            type = FormFieldType.Array;
        else if (propertyType == typeof(int) || propertyType == typeof(uint))
            type = FormFieldType.Integer;
        else if (propertyType == typeof(float) || propertyType == typeof(double))
            type = FormFieldType.Float;

        return new FormValidationRule { Type = type, Required = true, Message = message };
    }

    private static FormValidationRule GetFormRuleMin(Context context, ColumnInfo column, int length)
    {
        var message = context.Language.GetString("Valid.MinLength", column.Id, length);
        return new FormValidationRule { Type = FormFieldType.String, Min = length, Message = message };
    }

    private static FormValidationRule GetFormRuleMax(Context context, ColumnInfo column, int length)
    {
        var message = context.Language.GetString("Valid.MaxLength", column.Id, length);
        return new FormValidationRule { Type = FormFieldType.String, Max = length, Message = message };
    }

    private static FormValidationRule GetFormRuleRegex(RegexAttribute regex)
    {
        return new FormValidationRule { Type = FormFieldType.Regexp, Pattern = regex.Pattern, Message = regex.Message };
    }
}