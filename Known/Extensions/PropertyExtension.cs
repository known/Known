namespace Known.Extensions;

static class PropertyExtension
{
    internal static void Validate(this PropertyInfo property, Language language, object value, List<string> errors)
    {
        var label = property.DisplayName();
        if (string.IsNullOrWhiteSpace(label))
            label = language.GetString(property.Name);
        var required = property?.GetCustomAttribute<RequiredAttribute>();
        if (required != null && !required.IsValid(value))
        {
            errors.Add(language.Required(label));
            return;
        }

        var valueString = value == null ? "" : value.ToString().Trim();
        if (string.IsNullOrEmpty(valueString))
            return;

        var minLength = property.MinLength();
        var maxLength = property.MaxLength();
        var typeName = property.PropertyType.FullName;
        if (typeName.Contains("System.Int32"))
        {
            if (!int.TryParse(valueString, out int i))
                errors.Add(language.GetString("Valid.MustInteger", label));
            if (minLength != null && i < minLength.Value)
                errors.Add(language.GetString("Valid.MustMinLength", label, minLength));
            if (maxLength != null && i > maxLength.Value)
                errors.Add(language.GetString("Valid.MustMaxLength", label, maxLength));
        }
        else if (typeName.Contains("System.Decimal"))
        {
            if (!decimal.TryParse(valueString, out decimal d))
                errors.Add(language.GetString("Valid.MustNumber", label));
            if (minLength != null && d < minLength.Value)
                errors.Add(language.GetString("Valid.MustMinLength", label, minLength));
            if (maxLength != null && d > maxLength.Value)
                errors.Add(language.GetString("Valid.MustMaxLength", label, maxLength));
        }
        else if (typeName.Contains("System.DateTime"))
        {
            //if (string.IsNullOrEmpty(DateFormat))
            //{
            //    if (!DateTime.TryParse(valueString, out _))
            //        errors.Add(language.GetString("Valid.MustDateTime", label));
            //}
            //else
            //{
            //    if (!DateTime.TryParseExact(valueString, DateFormat, null, DateTimeStyles.None, out _))
            //        errors.Add(language.GetString("Valid.MustDateFormat", label, DateFormat));
            //}
        }
        else
        {
            var length = GetByteLength(valueString);
            if (minLength != null && length < minLength.Value)
                errors.Add($"{label}至少{minLength}个字符！");
            if (maxLength != null && length > maxLength.Value)
                errors.Add($"{label}不能超过{maxLength}个字符！");
        }

        var regex = property?.GetCustomAttribute<RegularExpressionAttribute>();
        if (regex != null && !regex.IsValid(value))
            errors.Add(regex.ErrorMessage);
    }

    private static int GetByteLength(string value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;

        return Encoding.Default.GetBytes(value).Length;
    }
}