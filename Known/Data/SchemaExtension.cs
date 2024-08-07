namespace Known.Data;

public static class SchemaExtension
{
    public static Task<List<string>> FindAllTablesAsync(this Database db)
    {
        var sql = string.Empty;
        if (db.DatabaseType == DatabaseType.MySql)
        {
            var dbName = string.Empty;
            var connStrs = db.ConnectionString.Split(';');
            foreach (var item in connStrs)
            {
                var items = item.Split('=');
                if (items[0] == "Initial Catalog")
                {
                    dbName = items[1];
                    break;
                }
            }
            sql = $"select table_name from information_schema.tables where table_schema='{dbName}'";
        }
        else if (db.DatabaseType == DatabaseType.Oracle)
        {
            sql = "select table_name from user_tables";
        }
        else if (db.DatabaseType == DatabaseType.SqlServer)
        {
            sql = "select Name from SysObjects where XType='U' order by Name";
        }

        if (string.IsNullOrEmpty(sql))
            return null;

        return db.ScalarsAsync<string>(sql);
    }
}