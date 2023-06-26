namespace Known.Razor;

public sealed class Setting
{
    private Setting() { }

    public static UserSetting UserSetting { get; set; }
    public static SettingInfo Info { get; set; }

    internal static List<QueryInfo> GetUserQuerys(string id)
    {
        if (!UserSetting.Querys.ContainsKey(id))
            return null;

        return UserSetting.Querys[id];
    }

    internal static List<ColumnInfo> GetUserColumns(string id, List<ColumnInfo> columns)
    {
        var lists = new List<ColumnInfo>();
        var userColumns = UserSetting.Columns.ContainsKey(id)
                        ? UserSetting.Columns[id]
                        : columns;
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
                item.IsAdvQuery = column.IsAdvQuery;
                item.IsSort = column.IsSort;
                item.SetColumnStyle();
                lists.Add(item);
            }
        }
        return lists;
    }
}