namespace Known.Data;

class PgSqlProvider : DbProvider
{
    //public override string FormatName(string name) => $"\"{name}\"";
    public override object FormatDate(string date) => DateTime.Parse(date);
}