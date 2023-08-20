namespace Known.Razor;

public sealed class Setting
{
    private Setting() { }

    public static UserSetting UserSetting { get; set; }
    public static SettingInfo Info { get; set; }

    internal static List<QueryInfo> GetUserQuerys(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        if (UserSetting == null || UserSetting.Querys == null)
            return null;

        if (!UserSetting.Querys.ContainsKey(id))
            return null;

        return UserSetting.Querys[id];
    }

    internal static List<ColumnInfo> GetUserColumns(string id, List<ColumnInfo> columns)
    {
        var lists = new List<ColumnInfo>();
        var userColumns = GetUserColumns(id) ?? columns;
        if (userColumns == null || userColumns.Count == 0)
            return lists;

        foreach (var column in userColumns)
        {
            lists.Add(new ColumnInfo
            {
                Id = column.Id,
                Name = column.Name,
                Align = column.Align,
                Width = column.Width,
                IsVisible = column.IsVisible,
                IsQuery = column.IsQuery,
                IsAdvQuery = column.IsAdvQuery,
                IsSort = column.IsSort
            });
        }
        return lists;
    }

    internal static List<Column<TItem>> GetUserColumns<TItem>(string id, List<Column<TItem>> columns)
    {
        var userColumns = GetUserColumns(id);
        if (userColumns == null)
            return columns;

        var lists = new List<Column<TItem>>();
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
                item.IsAdvQuery = column.IsAdvQuery;
                item.IsSort = column.IsSort;
                item.SetColumnStyle();
                lists.Add(item);
            }
        }
        return lists;
    }

    private static List<ColumnInfo> GetUserColumns(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        if (UserSetting == null || UserSetting.Columns == null)
            return null;

        if (!UserSetting.Columns.ContainsKey(id))
            return null;

        return UserSetting.Columns[id];
    }
}