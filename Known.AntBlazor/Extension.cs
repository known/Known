using System.Reflection;
using AntDesign;
using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

namespace Known.AntBlazor;

public static class Extension
{
    public static void AddKnownAntDesign(this IServiceCollection services, Action<AntDesignOption> action = null)
    {
        //添加AntDesign
        services.AddAntDesign();

        AntConfig.Option = new AntDesignOption();
        action?.Invoke(AntConfig.Option);

        Config.AddModule(typeof(Extension).Assembly);
        services.AddScoped<UIService>();

        UIConfig.Icons["AntDesign"] = typeof(IconType.Outline).GetProperties()
            .Select(x => (string)x.GetValue(null))
            .Where(x => x is not null)
            .ToList();
    }

    internal static FormValidationRule[] RuleRequired(this Context context, string id)
    {
        var message = context.Language.GetString("Valid.Required", id);
        var rule = new FormValidationRule { Type = FormFieldType.String, Required = true, Message = message };
        return [rule];
    }

    internal static FormValidationRule[] ToRules<TItem>(this FieldModel<TItem> model, Context context) where TItem : class, new()
    {
        var column = model.Column;
        if (column == null || column.Property == null)
            return [];

        var property = column.Property;
        var rules = new List<FormValidationRule>();
        if (column.Required && property.PropertyType != typeof(bool))
        {
            rules.Add(GetFormRuleRequired(context, column));
        }
        else
        {
            var min = property.MinLength();
            if (min != null)
                rules.Add(GetFormRuleMin(context, column, min.Value));

            var max = property.MaxLength();
            if (max != null)
                rules.Add(GetFormRuleMax(context, column, max.Value));

            var regex = property?.GetCustomAttribute<RegexAttribute>();
            if (regex != null)
                rules.Add(GetFormRuleRegex(regex));
        }

        return [.. rules];
    }

    private static FormValidationRule GetFormRuleRequired(Context context, ColumnInfo column)
    {
        //String,Number,Boolean,Regexp,Integer,Float,Array,Object,Enum,Date,Url,Email
        var property = column.Property;
        var type = FormFieldType.String;
        if (property.PropertyType.IsEnum)
            type = FormFieldType.Enum;
        else if (property.PropertyType == typeof(DateTime))
            type = FormFieldType.Date;
        else if (property.PropertyType.IsArray)
            type = FormFieldType.Array;
        else if (property.PropertyType == typeof(int) || property.PropertyType == typeof(uint))
            type = FormFieldType.Integer;
        else if (property.PropertyType == typeof(float) || property.PropertyType == typeof(double))
            type = FormFieldType.Float;

        var message = context.Language.GetString("Valid.Required", column.Id);
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

    internal static RadioOption<string>[] ToRadioOptions(this List<CodeInfo> codes)
    {
        if (codes == null || codes.Count == 0)
            return null;

        return codes.Select(a => new RadioOption<string> { Label = a.Name, Value = a.Code }).ToArray();
    }

    internal static CheckboxOption[] ToCheckboxOptions(this List<CodeInfo> codes, Action<CheckboxOption> action = null)
    {
        if (codes == null || codes.Count == 0)
            return null;

        return codes.Select(a =>
        {
            var option = new CheckboxOption { Label = a.Name, Value = a.Code };
            action?.Invoke(option);
            return option;
        }).ToArray();
    }
}

public class AntDesignOption
{
    public RenderFragment Footer { get; set; }
}

class AntConfig
{
    public static AntDesignOption Option { get; set; }
}