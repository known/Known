namespace Known.Helpers;

partial class CodeGenerator
{
    public string GetScript(DatabaseType dbType, EntityInfo entity)
    {
        if (entity == null)
            return string.Empty;

        List<FieldInfo> columns = entity.IsEntity ? TypeHelper.GetBaseFields() : [];
        columns.AddRange(entity.Fields);
        var keys = entity.IsEntity
                 ? [nameof(EntityBase.Id)]
                 : entity.Fields.Where(f => f.IsKey).Select(f => f.Id).ToList();
        var tableName = Model.EntityName ?? entity.Id;
        var maxLength = columns.Count > 0 ? columns.Select(f => (f.Id ?? "").Length).Max() : 0;
        return dbType switch
        {
            DatabaseType.Access => AccessProvider.GetTableScript(tableName, columns, keys, maxLength),
            DatabaseType.SQLite => SQLiteProvider.GetTableScript(tableName, columns, keys, maxLength),
            DatabaseType.SqlServer => SqlServerProvider.GetTableScript(tableName, columns, keys, maxLength),
            DatabaseType.Oracle => OracleProvider.GetTableScript(tableName, columns, keys, maxLength),
            DatabaseType.MySql => MySqlProvider.GetTableScript(tableName, columns, keys, maxLength),
            DatabaseType.PgSql => PgSqlProvider.GetTableScript(tableName, columns, keys, maxLength),
            DatabaseType.DM => DMProvider.GetTableScript(tableName, columns, keys, maxLength),
            _ => string.Empty,
        };
    }
}