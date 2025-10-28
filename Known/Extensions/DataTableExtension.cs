namespace Known.Extensions;

/// <summary>
/// 数据表扩展类。
/// </summary>
public static class DataTableExtension
{
    /// <summary>
    /// 获取数据表字段值。
    /// </summary>
    /// <typeparam name="T">值类型。</typeparam>
    /// <param name="row">数据行对象。</param>
    /// <param name="columnName">栏位名称。</param>
    /// <returns></returns>
    public static T GetValue<T>(this DataRow row, string columnName)
    {
        if (row == null || row.Table == null)
            return default;

        if (string.IsNullOrWhiteSpace(columnName))
            return default;

        if (!row.Table.Columns.Contains(columnName))
            return default;

        return Utils.ConvertTo<T>(row[columnName]);
    }
}