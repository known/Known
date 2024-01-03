using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Known.Extensions;

namespace Known;

[AttributeUsage(AttributeTargets.Method)]
public class ActionAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public class CodeInfoAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public class TableAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}

[AttributeUsage(AttributeTargets.Property)]
public class FormAttribute() : Attribute
{
    public int Row { get; set; } = 1;
    public int Column { get; set; } = 1;
    public string Type { get; set; } = FieldType.Text.ToString();
    public bool ReadOnly { get; set; }
    public string Placeholder { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute(string columnName = null) : Attribute
{
    public string ColumnName { get; } = columnName;
    public string DateFormat { get; set; }
    public PropertyInfo Property { get; set; }

    internal virtual void Validate(Context context, object value, PropertyInfo property, List<string> errors)
    {
        var label = context.Language[property.Name];
        var valueString = value == null ? "" : value.ToString().Trim();
        if (property.IsRequired() && string.IsNullOrEmpty(valueString))
        {
            errors.Add(context.Language.GetString("Valid.Required", label));
            return;
        }
        else if (!string.IsNullOrEmpty(valueString))
        {
            var minLength = property.MinLength();
            var maxLength = property.MaxLength();
            var length = GetByteLength(valueString);
            if (minLength != null && length < minLength.Value)
                errors.Add(context.Language.GetString("Valid.MinLength", label, minLength));
            if (maxLength != null && length > maxLength.Value)
                errors.Add(context.Language.GetString("Valid.MaxLength", label, maxLength));

            var typeName = property.PropertyType.FullName;
            if (typeName.Contains("System.Int32"))
            {
                if (!int.TryParse(value.ToString(), out int i))
                    errors.Add(context.Language.GetString("Valid.MustInteger", label));
                if (minLength != null && i < minLength.Value)
                    errors.Add(context.Language.GetString("Valid.MustMinLength", label, minLength));
                if (maxLength != null && i > maxLength.Value)
                    errors.Add(context.Language.GetString("Valid.MustMaxLength", label, maxLength));
            }

            if (typeName.Contains("System.Decimal"))
            {
                if (!decimal.TryParse(value.ToString(), out decimal d))
                    errors.Add(context.Language.GetString("Valid.MustNumber", label));
                if (minLength != null && d < minLength.Value)
                    errors.Add(context.Language.GetString("Valid.MustMinLength", label, minLength));
                if (maxLength != null && d > maxLength.Value)
                    errors.Add(context.Language.GetString("Valid.MustMaxLength", label, maxLength));
            }

            if (typeName.Contains("System.DateTime"))
            {
                if (string.IsNullOrEmpty(DateFormat))
                {
                    if (!DateTime.TryParse(value.ToString(), out _))
                        errors.Add(context.Language.GetString("Valid.MustDateTime", label));
                }
                else
                {
                    if (!DateTime.TryParseExact(valueString, DateFormat, null, DateTimeStyles.None, out _))
                        errors.Add(context.Language.GetString("Valid.MustDateFormat", label, DateFormat));
                }
            }
        }
    }

    private static int GetByteLength(string value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;

        return Encoding.Default.GetBytes(value).Length;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class RegexAttribute(string pattern, string message) : Attribute
{
    public string Pattern { get; } = pattern;
    public string Message { get; } = message;

    internal virtual void Validate(object value, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(Pattern))
            return;

        var valueString = value == null ? "" : value.ToString().Trim();
        if (string.IsNullOrWhiteSpace(valueString))
            return;

        var isMatch = Regex.IsMatch(valueString, Pattern);
        if (isMatch)
            return;

        errors.Add(Message);
    }
}