namespace Known.SqlSugar;

static class SqlSugarExtension
{
    internal static async Task<T> QueryAsync<T>(this SqlSugarScope sugar, string sql, List<SugarParameter> parameters)
    {
        using (var reader = await sugar.Ado.GetDataReaderAsync(sql, parameters))
        {
            return ConvertTo<T>(reader);
        }
    }

    internal static async Task<List<T>> QueryListAsync<T>(this SqlSugarScope sugar, string sql, List<SugarParameter> parameters)
    {
        using (var reader = await sugar.Ado.GetDataReaderAsync(sql, parameters))
        {
            return ConvertToList<T>(reader);
        }
    }

    private static T ConvertTo<T>(System.Data.IDataReader reader)
    {
        if (reader.Read())
            return (T)DBUtils.ConvertTo<T>(reader);

        return default;
    }

    private static List<T> ConvertToList<T>(System.Data.IDataReader reader)
    {
        var lists = new List<T>();
        while (reader.Read())
        {
            var obj = DBUtils.ConvertTo<T>(reader);
            lists.Add((T)obj);
        }
        return lists;
    }
}