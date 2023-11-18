using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Known.Extensions;

namespace Known;

[AttributeUsage(AttributeTargets.Class)]
public class CodeInfoAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public class TableAttribute : Attribute
{
    public TableAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute : Attribute
{
    public ColumnAttribute(string columnName = null)
    {
        ColumnName = columnName;
        IsQueryAll = true;
    }

    public string ColumnName { get; }
    public string DateFormat { get; set; }
    public string CodeType { get; set; }
    public string Placeholder { get; set; }
    public bool IsForm { get; set; }
    public bool IsFile { get; set; }
    public bool IsMultiFile { get; set; }
    public bool IsReadOnly { get; set; }
    public bool IsGrid { get; set; }
    public bool IsViewLink { get; set; }
    public bool IsQuery { get; set; }
    public bool IsQueryAll { get; set; }
    public PropertyInfo Property { get; set; }

    internal virtual void Validate(object value, PropertyInfo property, List<string> errors)
    {
        var description = property.DisplayName();
        var valueString = value == null ? "" : value.ToString().Trim();
        if (property.IsRequired() && string.IsNullOrEmpty(valueString))
        {
            errors.Add(Language.NotEmpty.Format(description));
            return;
        }
        else if (!string.IsNullOrEmpty(valueString))
        {
            var minLength = property.MinLength();
            var maxLength = property.MaxLength();
            var length = GetByteLength(valueString);
            if (minLength != null && length < minLength.Value)
                errors.Add(Language.MinLength.Format(description, minLength));
            if (maxLength != null && length > maxLength.Value)
                errors.Add(Language.MaxLength.Format(description, maxLength));

            var typeName = property.PropertyType.FullName;
            if (typeName.Contains("System.Int32"))
            {
                if (!int.TryParse(value.ToString(), out int i))
                    errors.Add(Language.MustInteger.Format(description));
                if (minLength != null && i < minLength.Value)
                    errors.Add(Language.MustMinLength.Format(description, minLength));
                if (maxLength != null && i > maxLength.Value)
                    errors.Add(Language.MustMaxLength.Format(description, maxLength));
            }

            if (typeName.Contains("System.Decimal"))
            {
                if (!decimal.TryParse(value.ToString(), out decimal d))
                    errors.Add(Language.MustNumber.Format(description));
                if (minLength != null && d < minLength.Value)
                    errors.Add(Language.MustMinLength.Format(description, minLength));
                if (maxLength != null && d > maxLength.Value)
                    errors.Add(Language.MustMaxLength.Format(description, maxLength));
            }

            if (typeName.Contains("System.DateTime"))
            {
                if (string.IsNullOrEmpty(DateFormat))
                {
                    if (!DateTime.TryParse(value.ToString(), out _))
                        errors.Add(Language.MustDateTime.Format(description));
                }
                else
                {
                    if (!DateTime.TryParseExact(valueString, DateFormat, null, DateTimeStyles.None, out _))
                        errors.Add(Language.MustDateFormat.Format(description, DateFormat));
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
public class RegexAttribute : Attribute
{
    public RegexAttribute(string pattern, string message)
    {
        Pattern = pattern;
        Message = message;
    }

    public string Pattern { get; }
    public string Message { get; }

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