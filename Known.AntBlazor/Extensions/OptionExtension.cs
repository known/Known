namespace Known.AntBlazor.Extensions;

static class OptionExtension
{
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