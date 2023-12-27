using System.Reflection;
using AntDesign;
using Known.Blazor;
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
            //String,Number,Boolean,Regexp,Integer,Float,Array,Object,Enum,Date,Url,Email
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
            rules.Add(new FormValidationRule()
            {
                Type = type,
                Required = true,
                Message = $"{column.Name}不能为空！"
            });
        }
        else
        {
            var regex = property?.GetCustomAttribute<RegexAttribute>();
            if (regex != null)
            {
                rules.Add(new FormValidationRule()
                {
                    Type = FormFieldType.Regexp,
                    Pattern = regex.Pattern,
                    Message = regex.Message
                });
            }
        }

        return [.. rules];
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