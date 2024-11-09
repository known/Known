namespace Known.AntBlazor.Extensions;

static class OptionExtension
{
    internal static SiderTheme ToSiderTheme(this SettingInfo info)
    {
        if (info.MenuTheme == "Light")
            return SiderTheme.Light;

        return SiderTheme.Dark;
    }

    internal static MenuTheme ToMenuTheme(this SettingInfo info)
    {
        if (info.MenuTheme == "Light")
            return MenuTheme.Light;

        return MenuTheme.Dark;
    }

    internal static RadioOption<string>[] ToRadioOptions(this List<CodeInfo> codes)
    {
        if (codes == null || codes.Count == 0)
            return null;

        return codes.Select(a => new RadioOption<string> { Label = a.Name, Value = a.Code }).ToArray();
    }

    internal static CheckboxOption<string>[] ToCheckboxOptions(this List<CodeInfo> codes, Action<CheckboxOption<string>> action = null)
    {
        if (codes == null || codes.Count == 0)
            return null;

        return codes.Select(a =>
        {
            var option = new CheckboxOption<string> { Label = a.Name, Value = a.Code };
            action?.Invoke(option);
            return option;
        }).ToArray();
    }

    internal static SortDirection ToSortDirection(this ColumnInfo colmun)
    {
        if (string.IsNullOrWhiteSpace(colmun.DefaultSort))
            return SortDirection.None;

        if (colmun.DefaultSort == "asc" || colmun.DefaultSort == "Ascend")
            return SortDirection.Ascending;
        else if (colmun.DefaultSort == "desc" || colmun.DefaultSort == "Descend")
            return SortDirection.Descending;

        return SortDirection.None;
    }
}