namespace Known.Razor;

public class Setting
{
    public static UserSetting UserSetting { get; set; }

    internal static SettingInfo Info => UserSetting.Info ?? SettingInfo.Default;

    internal static List<QueryInfo> GetUserQuerys(string id)
    {
        if (!UserSetting.Querys.ContainsKey(id))
            return null;

        return UserSetting.Querys[id];
    }

    internal static List<ColumnInfo> GetUserColumns(string id)
    {
        if (!UserSetting.Columns.ContainsKey(id))
            return null;

        return UserSetting.Columns[id];
    }

    internal static List<Column<TItem>> GetUserColumns<TItem>(string id, List<Column<TItem>> columns)
    {
        if (!UserSetting.Columns.ContainsKey(id))
            return columns;

        var lists = new List<Column<TItem>>();
        var userColumns = UserSetting.Columns[id];
        foreach (var column in userColumns)
        {
            var item = columns.FirstOrDefault(c => c.Id == column.Id);
            if (item != null)
            {
                item.Name = column.Name;
                item.Align = column.Align;
                item.Width = column.Width;
                item.IsVisible = column.IsVisible;
                item.IsQuery = column.IsQuery;
                item.IsSort = column.IsSort;
                item.SetColumnStyle();
                lists.Add(item);
            }
        }
        return lists;
    }
}