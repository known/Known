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

    internal static List<IConditionalModel> GetSugarWhere(string sql, PagingCriteria criteria)
    {
        var models = new List<IConditionalModel>();
        foreach (var item in criteria.Query)
        {
            if (!string.IsNullOrWhiteSpace(item.Value))
            {
                var value = item.Value;
                if (value.Contains('~'))
                {
                    item.Type = QueryType.Between;
                    value = value.Replace('~', ',');
                }
                models.Add(new ConditionalModel
                {
                    FieldName = item.Id,
                    ConditionalType = GetConditionalType(item.Type),
                    FieldValue = value
                });
            }
        }
        return models;
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

    private static ConditionalType GetConditionalType(QueryType type)
    {
        switch (type)
        {
            case QueryType.Equal:
                return ConditionalType.Equal;
            case QueryType.NotEqual:
                return ConditionalType.NoEqual;
            case QueryType.LessThan:
                return ConditionalType.LessThan;
            case QueryType.LessEqual:
                return ConditionalType.LessThanOrEqual;
            case QueryType.GreatThan:
                return ConditionalType.GreaterThan;
            case QueryType.GreatEqual:
                return ConditionalType.GreaterThanOrEqual;
            case QueryType.Between:
            case QueryType.BetweenNotEqual:
            case QueryType.BetweenLessEqual:
            case QueryType.BetweenGreatEqual:
                return ConditionalType.Range;
            case QueryType.Contain:
                return ConditionalType.Like;
            case QueryType.StartWith:
                return ConditionalType.LikeLeft;
            case QueryType.EndWith:
                return ConditionalType.LikeRight;
            case QueryType.Batch:
                return ConditionalType.In;
            default:
                return ConditionalType.Equal;
        }
    }
}