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
    }

    internal static FormValidationRule[] ToRules<TItem>(this FieldModel<TItem> model) where TItem : class, new()
    {
        var column = model.Column;
        if (column == null || column.Property == null)
            return [];

        var property = column.Property;
        var rules = new List<FormValidationRule>();
        if (column.Required && property.PropertyType != typeof(bool))
        {
            rules.Add(GetFormRuleRequired(column));
        }
        else
        {
            var min = property.MinLength();
            if (min != null)
                rules.Add(GetFormRuleMin(column, min.Value));

            var max = property.MaxLength();
            if (max != null)
                rules.Add(GetFormRuleMax(column, max.Value));

            var regex = property?.GetCustomAttribute<RegexAttribute>();
            if (regex != null)
                rules.Add(GetFormRuleRegex(regex));
        }

        return [.. rules];
    }

    private static FormValidationRule GetFormRuleRequired(ColumnInfo column)
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
        return new FormValidationRule { Type = type, Required = true, Message = $"{column.Name}不能为空！" };
    }

    private static FormValidationRule GetFormRuleMin(ColumnInfo column, int length)
    {
        return new FormValidationRule { Type = FormFieldType.String, Min = length, Message = $"{column.Name}至少{length}个字符！" };
    }

    private static FormValidationRule GetFormRuleMax(ColumnInfo column, int length)
    {
        return new FormValidationRule { Type = FormFieldType.String, Max = length, Message = $"{column.Name}不能超过{length}个字符！" };
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