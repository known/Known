namespace Known.Data;

public partial class Database
{
    private List<string> Tables { get; set; }

    /// <summary>
    /// 异步判断是否存在数据表。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="isLoadTable">是否加载表格。</param>
    /// <returns>是否存在。</returns>
    public virtual Task<bool> ExistsAsync<T>(bool isLoadTable = false)
    {
        return ExistsAsync(typeof(T), isLoadTable);
    }

    /// <summary>
    /// 异步判断是否存在数据表。
    /// </summary>
    /// <param name="type">实体类型。</param>
    /// <param name="isLoadTable">是否加载表格。</param>
    /// <returns>是否存在。</returns>
    public virtual async Task<bool> ExistsAsync(Type type, bool isLoadTable = false)
    {
        if (isLoadTable)
            Tables = await GetTableNamesAsync();
        var tableName = Provider.GetTableName(type);
        return await ExistsTableAsync(tableName);
    }

    /// <summary>
    /// 异步获取数据库所有数据表信息。
    /// </summary>
    /// <param name="isLoadSchema">是否加载表架构。</param>
    /// <returns></returns>
    public async Task<List<DbTableInfo>> GetTablesAsync(bool isLoadSchema = false)
    {
        if (conn == null)
            return [];

        var sql = Provider.GetTableSql(conn.Database);
        if (string.IsNullOrWhiteSpace(sql))
            return [];

        await OpenAsync();
        var tables = await QueryListAsync<DbTableInfo>(sql);
        if (tables != null && tables.Count > 0 && isLoadSchema)
        {
            foreach (var table in tables)
            {
                table.Fields = await GetTableFieldsAsync(table.Id);
            }
        }
        await CloseAsync();
        return tables;
    }

    /// <summary>
    /// 异步获取表字段架构信息列表。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <returns></returns>
    public async Task<List<FieldInfo>> GetTableFieldsAsync(string tableName)
    {
        var fields = new List<FieldInfo>();
        var info = new CommandInfo() { Text = $"select * from {tableName} where 1=0" };
        using var reader = await ExecuteReaderAsync(info);
        var schema = reader.GetSchemaTable();
        if (schema != null && schema.Rows.Count > 0)
        {
            foreach (DataRow row in schema.Rows)
            {
                fields.Add(CreateField(row));
            }
        }
        return fields;
    }

    /// <summary>
    /// 异步获取数据库所有数据表名。
    /// </summary>
    /// <returns></returns>
    public Task<List<string>> GetTableNamesAsync()
    {
        var sql = Provider.GetTableSql("");
        if (string.IsNullOrWhiteSpace(sql))
            return Task.FromResult(new List<string>());

        return ScalarsAsync<string>(sql);
    }

    /// <summary>
    /// 获取实体模型建表脚本。
    /// </summary>
    /// <param name="info">数据实体模型。</param>
    /// <returns></returns>
    public string GetTableScript(DbModelInfo info)
    {
        var tableName = Provider.GetTableName(info.Type);
        if (info.Fields == null || info.Fields.Count == 0)
            return string.Empty;

        return Provider.GetTableScript(tableName, info);
    }

    /// <summary>
    /// 异步创建数据库表。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <returns></returns>
    public async Task CreateTableAsync<T>()
    {
        var model = DbConfig.Models.FirstOrDefault(m => m.Type == typeof(T));
        if (model == null)
            return;

        await CreateTableAsync(model);
    }

    /// <summary>
    /// 异步创建数据库表。
    /// </summary>
    /// <param name="info">数据实体模型。</param>
    /// <returns></returns>
    public async Task CreateTableAsync(DbModelInfo info)
    {
        var tableName = Provider.GetTableName(info.Type);
        if (await ExistsTableAsync(tableName))
            return;

        var script = GetTableScript(info);
        if (string.IsNullOrWhiteSpace(script))
            return;

        await ExecuteAsync(script);
        Tables.Add(tableName);
    }

    /// <summary>
    /// 异步创建所有实体模型数据库表。
    /// </summary>
    /// <returns></returns>
    public async Task CreateTablesAsync()
    {
        foreach (var item in DbConfig.Models)
        {
            await CreateTableAsync(item);
        }
    }

    /// <summary>
    /// 导出所有实体模型建表脚本。
    /// </summary>
    /// <returns></returns>
    public string ExportTableScripts()
    {
        var sb = new StringBuilder();
        foreach (var item in DbConfig.Models)
        {
            var script = GetTableScript(item);
            if (!string.IsNullOrWhiteSpace(script))
                sb.AppendLine(script);
        }
        return sb.ToString();
    }

    private async Task<bool> ExistsTableAsync(string tableName)
    {
        Tables ??= await GetTableNamesAsync();
        return Tables.Exists(t => t.Equals(tableName, StringComparison.OrdinalIgnoreCase));
    }

    private static FieldInfo CreateField(DataRow row)
    {
        var id = Utils.ConvertTo<string>(row["ColumnName"]);
        var type = Utils.ConvertTo<string>(row["DataType"]);
        var typeName = Utils.ConvertTo<string>(row["DataTypeName"]);
        var size = Utils.ConvertTo<int>(row["ColumnSize"]);
        size = size < 0 ? 50 : size;
        return new FieldInfo
        {
            Id = id,
            Name = id,
            Type = GetFieldType(id, type, typeName, size),
            Length = size > 500 ? "" : $"{size}",
            Required = !Utils.ConvertTo<bool>(row["AllowDBNull"]),
            IsKey = Utils.ConvertTo<bool>(row["IsKey"])
        };
    }

    private static FieldType GetFieldType(string id, string type, string typeName, int size)
    {
        if (type.Contains("DateTime") || typeName.Contains("time") || id.EndsWith("Time"))
            return FieldType.DateTime;
        else if (type.Contains("Date") || typeName.Contains("date") || id.EndsWith("Date"))
            return FieldType.Date;
        else if (type.Contains("Int32") || type.Contains("Int64"))
            return FieldType.Number;
        else if (typeName.Contains("text"))
            return FieldType.TextArea;
        else
            return size > 500 ? FieldType.TextArea : FieldType.Text;
    }
}