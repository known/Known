using AntDesign;

namespace Known.Extensions;

static class OptionExtension
{
    internal static SiderTheme ToSiderTheme(this UserSettingInfo info)
    {
        if (info.MenuTheme == "Light")
            return SiderTheme.Light;

        return SiderTheme.Dark;
    }

    internal static MenuTheme ToMenuTheme(this UserSettingInfo info)
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

    internal static bool ToEllipsis(this ColumnInfo info)
    {
        if (UIConfig.IsEllipsisTable)
            return true;

        return info.Ellipsis;
    }

    internal static SortDirection ToSortDirection(this ColumnInfo info)
    {
        if (string.IsNullOrWhiteSpace(info.DefaultSort))
            return SortDirection.None;

        if (info.DefaultSort == "asc" || info.DefaultSort == "Ascend")
            return SortDirection.Ascending;
        else if (info.DefaultSort == "desc" || info.DefaultSort == "Descend")
            return SortDirection.Descending;

        return SortDirection.None;
    }

    internal static ColumnFixPlacement? ToColumnFixPlacement(this ColumnInfo info)
    {
        return info.Fixed switch
        {
            "Left" => ColumnFixPlacement.Left,
            "left" => ColumnFixPlacement.Left,
            "Right" => ColumnFixPlacement.Right,
            "right" => ColumnFixPlacement.Right,
            _ => null
        };
    }

    internal static ColumnAlign ToColumnAlign(this ColumnInfo info)
    {
        return info.Align switch
        {
            "Center" => ColumnAlign.Center,
            "center" => ColumnAlign.Center,
            "Right" => ColumnAlign.Right,
            "right" => ColumnAlign.Right,
            _ => ColumnAlign.Left
        };
    }
}