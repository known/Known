namespace Known.Data;

class PgSqlBuilder : SqlBuilder
{
    public override string FormatName(string name) => $"\"{name}\"";
}