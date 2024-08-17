namespace Known.SqlSugar;

class SqlSugarFactory
{
    internal static ConnectionConfig Config = new();

    public static SqlSugarScope CreateSugar()
    {
        return new SqlSugarScope(Config);
    }
}