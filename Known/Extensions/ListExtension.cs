namespace Known.Extensions;

public static class ListExtension
{
    public static void Add<T>(this List<ColumnInfo> lists, Expression<Func<T, object>> selector, string name = null)
    {
        var property = TypeHelper.Property(selector);
        var attr = property.GetCustomAttribute<ColumnAttribute>(true);
        var column = new ColumnInfo(attr?.Description, property.Name);
        if (!string.IsNullOrWhiteSpace(name))
            column.Name = name;
        lists.Add(column);
    }

    public static void Add<T>(this List<ColumnInfo> lists, Expression<Func<T, object>> selector, ColumnType type)
    {
        var property = TypeHelper.Property(selector);
        var attr = property.GetCustomAttribute<ColumnAttribute>(true);
        var column = new ColumnInfo(attr?.Description, property.Name, type);
        lists.Add(column);
    }
}