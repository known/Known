namespace Known.Helpers;

partial class CodeGenerator
{
    public string GetScript(DatabaseType dbType, EntityInfo entity)
    {
        if (entity == null)
            return string.Empty;

        var columns = TypeHelper.GetBaseFields();
        columns.AddRange(entity.Fields);

        var maxLength = columns.Select(f => (f.Id ?? "").Length).Max();
        return dbType switch
        {
            DatabaseType.Access => AccessProvider.GetTableScript(entity.Id, columns, [nameof(EntityBase.Id)], maxLength),
            DatabaseType.SQLite => SQLiteProvider.GetTableScript(entity.Id, columns, [nameof(EntityBase.Id)], maxLength),
            DatabaseType.SqlServer => SqlServerProvider.GetTableScript(entity.Id, columns, [nameof(EntityBase.Id)], maxLength),
            DatabaseType.Oracle => OracleProvider.GetTableScript(entity.Id, columns, [nameof(EntityBase.Id)], maxLength),
            DatabaseType.MySql => MySqlProvider.GetTableScript(entity.Id, columns, [nameof(EntityBase.Id)], maxLength),
            DatabaseType.PgSql => PgSqlProvider.GetTableScript(entity.Id, columns, [nameof(EntityBase.Id)], maxLength),
            DatabaseType.DM => DMProvider.GetTableScript(entity.Id, columns, [nameof(EntityBase.Id)], maxLength),
            _ => string.Empty,
        };
    }
}