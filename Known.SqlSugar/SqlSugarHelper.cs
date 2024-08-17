namespace Known.SqlSugar;

class SqlSugarHelper
{
    internal static DatabaseType GetDatabaseType(DbType dbType)
    {
        switch (dbType)
        {
            case DbType.Access:
                return DatabaseType.Access;
            case DbType.MySql:
            case DbType.MySqlConnector:
                return DatabaseType.MySql;
            case DbType.SqlServer:
                return DatabaseType.SqlServer;
            case DbType.Sqlite:
                return DatabaseType.SQLite;
            case DbType.Oracle:
                return DatabaseType.Oracle;
            case DbType.PostgreSQL:
                return DatabaseType.PgSql;
            case DbType.Dm:
            case DbType.Kdbndp:
            case DbType.Oscar:
            case DbType.OpenGauss:
            case DbType.QuestDB:
            case DbType.HG:
            case DbType.ClickHouse:
            case DbType.GBase:
            case DbType.Odbc:
            case DbType.OceanBaseForOracle:
            case DbType.TDengine:
            case DbType.GaussDB:
            case DbType.OceanBase:
            case DbType.Tidb:
            case DbType.Vastbase:
            case DbType.PolarDB:
            case DbType.Doris:
            case DbType.Xugu:
            case DbType.GoldenDB:
            case DbType.Custom:
            default:
                return DatabaseType.Other;
        }
    }

    internal static List<SugarParameter> GetSugarParameters(string sql, PagingCriteria criteria, UserInfo user)
    {
        if (criteria == null)
            return null;

        var list = new List<SugarParameter>();
        var paras = criteria.ToParameters(user);
        foreach (var item in paras)
        {
            if (sql.Contains(item.Key))
                list.Add(new SugarParameter(item.Key, item.Value));
        }
        return list;
    }
}