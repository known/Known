using Known.Core.Cells;

namespace Known.Core;

public enum DatabaseType
{
    SqlServer,
    Oracle,
    MySql,
    SQLite,
    Access
}

public class Database : IDisposable
{
    private DbConnection conn;
    private DbTransaction trans;

    #region Constructors
    public Database() : this("Default") { }

    public Database(string connName, UserInfo user = null)
    {
        var setting = KCConfig.App.GetConnection(connName);
        if (setting != null)
        {
            Init(setting.ProviderName, setting.ConnectionString, user);
        }
    }

    public Database(string providerName, string connString, UserInfo user = null)
    {
        Init(providerName, connString, user);
    }

    private void Init(string providerName, string connString, UserInfo user = null)
    {
        ProviderName = providerName;
        ConnectionString = connString;
        User = user;

        var factory = DbProviderFactories.GetFactory(providerName);
        conn = factory.CreateConnection();
        conn.ConnectionString = connString;

        if (providerName.Contains("Oracle"))
            DatabaseType = DatabaseType.Oracle;
        else if (providerName.Contains("MySql"))
            DatabaseType = DatabaseType.MySql;
        else if (providerName.Contains("SQLite"))
            DatabaseType = DatabaseType.SQLite;
        else if (providerName.Contains("Access"))
            DatabaseType = DatabaseType.Access;
        else
            DatabaseType = DatabaseType.SqlServer;
    }
    #endregion

    #region Properties
    public DatabaseType DatabaseType { get; private set; }
    public string ProviderName { get; private set; }
    public string ConnectionString { get; private set; }
    public UserInfo User { get; set; }
    public string UserName => User?.UserName;
    #endregion

    #region Static
    public static void RegisterProviders(Dictionary<string, Type> dbFactories)
    {
        if (dbFactories != null && dbFactories.Count > 0)
        {
            foreach (var item in dbFactories)
            {
                if (!DbProviderFactories.GetProviderInvariantNames().Contains(item.Key))
                {
                    DbProviderFactories.RegisterFactory(item.Key, item.Value);
                }
            }
        }
    }
    #endregion

    #region Schema
    public List<string> FindAllTables()
    {
        var sql = string.Empty;
        if (DatabaseType == DatabaseType.MySql)
        {
            var dbName = string.Empty;
            var connStrs = ConnectionString.Split(';');
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
        else if (DatabaseType == DatabaseType.Oracle)
        {
            sql = "select table_name from user_tables";
        }
        else if (DatabaseType == DatabaseType.SqlServer)
        {
            sql = "select Name from SysObjects where XType='U' order by Name";
        }

        if (string.IsNullOrEmpty(sql))
            return null;

        return Scalars<string>(sql);
    }
    #endregion

    #region Public
    public void Open()
    {
        if (conn == null)
            return;

        if (conn.State != ConnectionState.Open)
            conn.Open();
    }

    public void Close()
    {
        if (conn == null)
            return;

        if (conn.State != ConnectionState.Closed)
            conn.Close();
    }

    public void Dispose()
    {
        if (trans != null)
            trans.Dispose();
        trans = null;

        if (conn.State != ConnectionState.Closed)
            conn.Close();
        conn.Dispose();
    }

    public Result Transaction(string name, Action<Database> action, object data = null)
    {
        using (var db = new Database(ProviderName, ConnectionString, User))
        {
            try
            {
                db.BeginTrans();
                action(db);
                db.Commit();
                return Result.Success(Language.XXSuccess.Format(name), data);
            }
            catch (Exception ex)
            {
                db.Rollback();
                Logger.Exception(ex);
                if (ex is CheckException)
                    return Result.Error(ex.Message);
                else
                    return Result.Error(Language.TransError);
            }
        }
    }

    public void InsertTable(DataTable data)
    {
        if (data == null || data.Rows.Count == 0)
            return;

        foreach (DataRow item in data.Rows)
        {
            var info = CommandInfo.GetInsertCommand(DatabaseType, item);
            ExecuteNonQuery(info);
        }
    }
    #endregion

    #region SQL
    public int Execute(string sql, object param = null)
    {
        var info = new CommandInfo(DatabaseType, sql, param);
        return ExecuteNonQuery(info);
    }

    public T Scalar<T>(string sql, object param = null)
    {
        var cmd = conn.CreateCommand();
        var info = new CommandInfo(DatabaseType, sql, param);
        PrepareCommand(conn, cmd, trans, info, out bool close);
        var scalar = cmd.ExecuteScalar();
        cmd.Parameters.Clear();
        if (close)
            conn.Close();

        return Utils.ConvertTo<T>(scalar);
    }

    public List<T> Scalars<T>(string sql, object param = null)
    {
        var data = new List<T>();
        var cmd = conn.CreateCommand();
        var info = new CommandInfo(DatabaseType, sql, param);
        PrepareCommand(conn, cmd, trans, info, out bool close);
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var obj = Utils.ConvertTo<T>(reader[0]);
                data.Add(obj);
            }
        }

        cmd.Parameters.Clear();
        if (close)
            conn.Close();

        return data;
    }

    public T Query<T>(string sql, object param = null)
    {
        var info = new CommandInfo(DatabaseType, sql, param);
        return Query<T>(info);
    }

    public List<T> QueryList<T>(string sql, object param = null)
    {
        var info = new CommandInfo(DatabaseType, sql, param);
        return QueryList<T>(info);
    }

    public List<T> QueryList<T>(int topSize, string sql, object param = null)
    {
        var info = new CommandInfo(DatabaseType, sql, param);
        info.Text = info.GetTopSql(DatabaseType, topSize);
        return QueryList<T>(info);
    }

    public PagingResult<T> QueryPage<T>(string sql, PagingCriteria criteria)
    {
        SetAutoQuery(ref sql, criteria);

        byte[] exportData = null;
        Dictionary<string, object> sums = null;
        var dataTable = new DataTable();
        var pageData = new List<T>();
        var cmd = conn.CreateCommand();
        var info = new CommandInfo(DatabaseType, sql, criteria.ToParameters(User));
        PrepareCommand(conn, cmd, trans, info, out bool close);
        cmd.CommandText = info.CountSql;
        var value = cmd.ExecuteScalar();
        var total = Utils.ConvertTo<int>(value);
        if (total > 0)
        {
            if (criteria.ExportMode == ExportMode.None)
            {
                cmd.CommandText = info.GetPagingSql(DatabaseType, criteria);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var obj = ConvertTo<T>(reader);
                    pageData.Add((T)obj);
                }
            }
            else
            {
                if (criteria.ExportMode != ExportMode.Page)
                    criteria.PageIndex = -1;
                cmd.CommandText = info.GetPagingSql(DatabaseType, criteria);
                cmd.CommandText = CommandInfo.GetExportSql(cmd.CommandText, criteria);
                using (var reader = cmd.ExecuteReader())
                {
                    dataTable.Load(reader);
                }
                foreach (var item in criteria.ExportColumns)
                {
                    dataTable.Columns[item.Key].ColumnName = item.Value;
                }
                exportData = GetExportData(dataTable);
            }

            if (criteria.SumColumns != null && criteria.SumColumns.Count > 0)
            {
                var columns = string.Join(",", criteria.SumColumns.Select(c => $"sum({c}) {c}"));
                sql = $"select {columns} from ({sql}) t";
                sums = Query<Dictionary<string, object>>(sql, criteria.ToParameters(User));
            }
        }

        cmd.Parameters.Clear();
        if (close)
            conn.Close();

        if (pageData.Count > criteria.PageSize && criteria.PageSize > 0)
            pageData = pageData.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize).ToList();

        return new PagingResult<T>
        {
            TotalCount = total,
            PageData = pageData,
            ExportData = exportData,
            Sums = sums
        };
    }

    private void SetAutoQuery(ref string sql, PagingCriteria criteria)
    {
        var querys = new List<QueryInfo>();
        foreach (var item in criteria.Query)
        {
            if (!string.IsNullOrWhiteSpace(item.Value))
            {
                if (item.Value.Contains("~") && item.Type != QueryType.Between)
                    item.Type = QueryType.Between;
            }
            querys.Add(item);
        }
        foreach (var item in querys)
        {
            if (!sql.Contains(item.Id))
                SetQuery(ref sql, criteria, item.Type, item.Id);
        }
    }

    public PagingResult<Dictionary<string, object>> QueryPageDictionary(string sql, PagingCriteria criteria)
    {
        if (conn.State != ConnectionState.Open)
            conn.Open();

        var data = new List<Dictionary<string, object>>();
        var cmd = conn.CreateCommand();
        var info = new CommandInfo(DatabaseType, sql, criteria.ToParameters(User));
        PrepareCommand(conn, cmd, trans, info, out _);
        cmd.CommandText = info.CountSql;
        var total = Utils.ConvertTo<int>(cmd.ExecuteScalar());
        if (total > 0)
        {
            cmd.CommandText = info.GetPagingSql(DatabaseType, criteria);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var dic = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i).Replace("_", "");
                        var value = reader[i];
                        dic.Add(name, value == DBNull.Value ? null : value);
                    }
                    data.Add(dic);
                }
            }
        }

        cmd.Parameters.Clear();
        if (conn.State != ConnectionState.Closed)
            conn.Close();

        if (data.Count > criteria.PageSize && criteria.PageSize > 0)
        {
            data = data.Skip((criteria.PageIndex - 1) * criteria.PageSize)
                       .Take(criteria.PageSize)
                       .ToList();
        }

        return new PagingResult<Dictionary<string, object>>
        {
            TotalCount = total,
            PageData = data
        };
    }

    public DataTable QueryTable(string sql, object param = null)
    {
        bool close;
        var data = new DataTable();

        var info = new CommandInfo(DatabaseType, sql, param);
        using (var reader = ExecuteReader(info, out close))
        {
            data.Load(reader);
        }

        if (close)
            conn.Close();

        return data;
    }
    #endregion

    #region Entity
    public T QueryById<T>(string id) where T : EntityBase
    {
        if (string.IsNullOrEmpty(id))
            return default;

        var tableName = CommandInfo.GetTableName<T>();
        var sql = $"select * from {tableName} where Id=@id";
        return Query<T>(sql, new { id });
    }

    public List<T> QueryList<T>() where T : EntityBase
    {
        var tableName = CommandInfo.GetTableName<T>();
        var sql = $"select * from {tableName} order by CreateTime";
        return QueryList<T>(sql);
    }

    public List<T> QueryListById<T>(string[] ids) where T : EntityBase
    {
        if (ids == null || ids.Length == 0)
            return null;

        var idTexts = new List<string>();
        var paramters = new Dictionary<string, object>();
        for (int i = 0; i < ids.Length; i++)
        {
            idTexts.Add($"Id=@id{i}");
            paramters.Add($"id{i}", ids[i]);
        }

        var tableName = CommandInfo.GetTableName<T>();
        var idText = string.Join(" or ", idTexts.ToArray());
        var sql = $"select * from {tableName} where {idText}";
        var info = new CommandInfo(DatabaseType, sql) { Params = paramters };

        return QueryList<T>(info);
    }

    public int DeleteAll<T>() where T : EntityBase
    {
        var tableName = CommandInfo.GetTableName<T>();
        var sql = $"delete from {tableName}";
        return Execute(sql);
    }

    public int Delete<T>(string id) where T : EntityBase
    {
        if (string.IsNullOrEmpty(id))
            return 0;

        var tableName = CommandInfo.GetTableName<T>();
        var sql = $"delete from {tableName} where Id=@id";
        return Execute(sql, new { id });
    }

    public int Delete<T>(T entity) where T : EntityBase
    {
        if (entity == null)
            return 0;

        return Delete<T>(entity.Id);
    }

    public void InsertDatas<T>(List<T> datas)
    {
        if (datas == null || datas.Count == 0)
            return;

        var close = false;
        var tableName = CommandInfo.GetTableName<T>();

        if (conn.State != ConnectionState.Open)
        {
            close = true;
            conn.Open();
        }

        foreach (var item in datas)
        {
            var cmdParams = CommandInfo.MapToDictionary(item);
            var keys = new List<string>();
            foreach (var key in cmdParams.Keys)
            {
                keys.Add(key);
            }
            var cloumn = string.Join(",", keys.ToArray());
            var value = string.Join(",", keys.Select(k => $"@{k}").ToArray());
            var sql = $"insert into {tableName}({cloumn}) values({value})";
            var info = new CommandInfo(DatabaseType, sql) { Params = cmdParams };
            ExecuteNonQuery(info);
        }

        if (close)
        {
            conn.Close();
        }
    }

    public void InsertData<T>(T data)
    {
        if (data == null)
            return;

        var info = CommandInfo.GetInsertCommand(DatabaseType, data);
        ExecuteNonQuery(info);
    }

    public T Insert<T>(T entity) where T : EntityBase
    {
        if (entity == null)
            return entity;

        entity.Id = Utils.GetGuid();
        entity.IsNew = true;
        entity.Version = 1;
        Save(entity);
        return entity;
    }

    public void Save<T>(T entity) where T : EntityBase
    {
        if (entity == null)
            return;

        if (User == null)
            Check.Throw("the user is not null.");

        if (entity.IsNew)
        {
            if (entity.CreateBy == "temp")
                entity.CreateBy = User.UserId;
            entity.CreateTime = DateTime.Now;
            if (entity.AppId == "temp")
                entity.AppId = User.AppId;
            if (entity.CompNo == "temp")
                entity.CompNo = User.CompNo;
        }
        else
        {
            entity.Version += 1;
        }

        entity.ModifyBy = User.UserId;
        entity.ModifyTime = DateTime.Now;

        var info = CommandInfo.GetSaveCommand(DatabaseType, entity);
        ExecuteNonQuery(info);
        entity.IsNew = false;
    }
    #endregion

    #region Dictionary
    public int Insert(string tableName, Dictionary<string, object> data)
    {
        var info = CommandInfo.GetInsertCommand(DatabaseType, tableName, data);
        return ExecuteNonQuery(info);
    }

    public int Update(string tableName, string keyField, Dictionary<string, object> data)
    {
        var info = CommandInfo.GetUpdateCommand(DatabaseType, tableName, keyField, data);
        return ExecuteNonQuery(info);
    }
    #endregion

    #region Trans
    private void BeginTrans()
    {
        if (conn.State != ConnectionState.Open)
            conn.Open();
        trans = conn.BeginTransaction();
    }

    private void Commit()
    {
        if (trans != null)
            trans.Commit();
    }

    private void Rollback()
    {
        if (trans != null)
            trans.Rollback();
    }
    #endregion

    #region Query
    public void SetQuery(ref string sql, PagingCriteria criteria, QueryType type, string key, string field = null)
    {
        if (criteria.ExportMode == ExportMode.All)
            return;

        field ??= key;
        var keys = key.Split('.');
        if (keys.Length > 1)
            key = keys[1];

        if (!criteria.HasQuery(key))
            return;

        var query = criteria.Query.FirstOrDefault(q => q.Id == key);
        switch (type)
        {
            case QueryType.NotEqual:
                sql += $" and {field}<>@{key}";
                break;
            case QueryType.Equal:
                sql += $" and {field}=@{key}";
                break;
            case QueryType.LessThan:
                sql += $" and {field}<@{key}";
                break;
            case QueryType.LessEqual:
                sql += $" and {field}<=@{key}";
                break;
            case QueryType.GreatThan:
                sql += $" and {field}>@{key}";
                break;
            case QueryType.GreatEqual:
                sql += $" and {field}>=@{key}";
                break;
            case QueryType.Between:
                SetLessQuery(ref sql, criteria, field, key, ">=");
                SetGreatQuery(ref sql, criteria, field, key, "<=");
                break;
            case QueryType.BetweenNotEqual:
                SetLessQuery(ref sql, criteria, field, key, ">");
                SetGreatQuery(ref sql, criteria, field, key, "<");
                break;
            case QueryType.BetweenLessEqual:
                SetLessQuery(ref sql, criteria, field, key, ">=");
                SetGreatQuery(ref sql, criteria, field, key, "<");
                break;
            case QueryType.BetweenGreatEqual:
                SetLessQuery(ref sql, criteria, field, key, ">");
                SetGreatQuery(ref sql, criteria, field, key, "<=");
                break;
            case QueryType.Contain:
                if (DatabaseType == DatabaseType.Access)
                {
                    sql += $" and {field} like '%{query.Value}%'";
                }
                else
                {
                    sql += $" and {field} like @{key}";
                    query.Value = $"%{query.Value}%";
                }
                break;
            case QueryType.StartWith:
                if (DatabaseType == DatabaseType.Access)
                {
                    sql += $" and {field} like '{query.Value}%'";
                }
                else
                {
                    sql += $" and {field} like @{key}";
                    query.Value = $"{query.Value}%";
                }
                break;
            case QueryType.EndWith:
                if (DatabaseType == DatabaseType.Access)
                {
                    sql += $" and {field} like '%{query.Value}'";
                }
                else
                {
                    sql += $" and {field} like @{key}";
                    query.Value = $"%{query.Value}";
                }
                break;
            default:
                break;
        }
    }

    public string GetDateSql(string paramName, bool withTime = true)
    {
        if (DatabaseType == DatabaseType.Oracle)
        {
            var format = "yyyy-mm-dd";
            if (withTime)
                format += " hh24:mi:ss";
            return $"to_date(:{paramName},'{format}')";
        }

        return $"@{paramName}";
    }

    private void SetLessQuery(ref string sql, PagingCriteria criteria, string field, string key, string symbol)
    {
        var paramName = $"L{key}";
        var date = GetDateSql(paramName);
        if (criteria.HasQuery(paramName))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == paramName);
            sql += $" and {field}{symbol}{date}";
            query.Value = $"{query.Value} 00:00:00";
        }
        else if (criteria.HasQuery(key))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == key);
            var value = query.Value.Split('~')[0];
            if (!string.IsNullOrWhiteSpace(value))
            {
                sql += $" and {field}{symbol}{date}";
                criteria.SetQuery(paramName, $"{value} 00:00:00");
            }
        }
    }

    private void SetGreatQuery(ref string sql, PagingCriteria criteria, string field, string key, string symbol)
    {
        var paramName = $"G{key}";
        var date = GetDateSql(paramName);
        if (criteria.HasQuery(paramName))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == paramName);
            sql += $" and {field}{symbol}{date}";
            query.Value = $"{query.Value} 23:59:59";
        }
        else if (criteria.HasQuery(key))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == key);
            var value = query.Value.Split('~')[1];
            if (!string.IsNullOrWhiteSpace(value))
            {
                sql += $" and {field}{symbol}{date}";
                criteria.SetQuery(paramName, $"{value} 23:59:59");
            }
        }
    }
    #endregion

    #region Private
    private int ExecuteNonQuery(CommandInfo info)
    {
        var close = false;
        var cmd = conn.CreateCommand();

        try
        {
            PrepareCommand(conn, cmd, trans, info, out close);
            var value = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            if (close)
                conn.Close();

            return value;
        }
        catch
        {
            Logger.Error(info.ToString());
            if (close)
                conn.Close();
            throw;
        }
    }

    private DbDataReader ExecuteReader(CommandInfo info, out bool close)
    {
        close = false;
        var cmd = conn.CreateCommand();
        try
        {
            PrepareCommand(conn, cmd, trans, info, out close);
            var reader = cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return reader;
        }
        catch
        {
            Logger.Error(info.ToString());
            if (close)
                conn.Close();
            throw;
        }
    }

    private T Query<T>(CommandInfo info)
    {
        bool close;
        T obj = default;
        using (var reader = ExecuteReader(info, out close))
        {
            if (reader.Read())
            {
                obj = (T)ConvertTo<T>(reader);
            }
        }
        if (close)
            conn.Close();
        return obj;
    }

    private List<T> QueryList<T>(CommandInfo info)
    {
        bool close;
        var lists = new List<T>();
        using (var reader = ExecuteReader(info, out close))
        {
            while (reader.Read())
            {
                var obj = ConvertTo<T>(reader);
                lists.Add((T)obj);
            }
        }
        if (close)
            conn.Close();
        return lists;
    }

    private static object ConvertTo<T>(DbDataReader reader)
    {
        var dic = new Dictionary<string, object>();
        for (int i = 0; i < reader.FieldCount; i++)
        {
            var name = reader.GetName(i).Replace("_", "");
            var value = reader[i];
            dic.Add(name, value == DBNull.Value ? null : value);
        }

        var type = typeof(T);
        if (type == typeof(Dictionary<string, object>))
            return dic;

        var obj = Activator.CreateInstance<T>();
        var properties = type.GetProperties();
        foreach (var item in dic)
        {
            var property = properties.FirstOrDefault(p => p.Name == item.Key);
            if (property != null)
            {
                var value = Utils.ConvertTo(property.PropertyType, item.Value);
                property.SetValue(obj, value);
            }
        }
        if (obj is EntityBase)
        {
            (obj as EntityBase).SetOriginal(dic);
        }
        return obj;
    }

    private void PrepareCommand(DbConnection conn, DbCommand cmd, DbTransaction trans, CommandInfo info, out bool close)
    {
        if (conn.State != ConnectionState.Open)
        {
            close = true;
            conn.Open();
        }
        else
        {
            close = false;
        }

        cmd.Connection = conn;
        cmd.CommandText = info.Text;

        if (trans != null)
        {
            if (trans.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof(trans));
            cmd.Transaction = trans;
        }

        if (info.Params != null && info.Params.Count > 0)
        {
            cmd.Parameters.Clear();
            foreach (var item in info.Params)
            {
                var pName = $"{info.Prefix}{item.Key}";
                if (info.Text.Contains(pName))
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = pName;
                    if (item.Value == null)
                        p.Value = DBNull.Value;
                    else if (item.Value is DateTime time)
                        p.Value = DatabaseType == DatabaseType.Access ? time.ToString() : time;
                    else
                        p.Value = item.Value.ToString();
                    cmd.Parameters.Add(p);
                }
            }
        }
    }

    private static byte[] GetExportData(DataTable dataTable)
    {
        if (dataTable == null || dataTable.Rows.Count == 0)
            return null;

        var excel = ExcelFactory.Create();
        var sheet = excel.CreateSheet("Sheet1");
        sheet.ImportDataByExport(dataTable);
        var stream = excel.SaveToStream();
        return stream.ToArray();
    }
    #endregion
}

class CommandInfo
{
    internal CommandInfo(DatabaseType databaseType, string text, object param = null)
    {
        DatabaseType = databaseType;
        Prefix = GetPrefix(databaseType);
        Text = text.Replace("@", Prefix);
        if (param != null)
            Params = MapToDictionary(param);
    }

    internal DatabaseType DatabaseType { get; }
    internal string Prefix { get; }
    internal string Text { get; set; }
    internal Dictionary<string, object> Params { get; set; }
    internal string CountSql => $"select count(*) from ({Text}) t";

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Text);
        if (Params != null && Params.Count > 0)
        {
            foreach (var item in Params)
            {
                sb.AppendLine($"{item.Key}={item.Value}");
            }
        }
        return sb.ToString();
    }

    internal string GetPagingSql(DatabaseType type, PagingCriteria criteria)
    {
        var order = string.Empty;
        if (criteria.PageIndex <= 0)
            return GetPagingSqlByNone(criteria, ref order);

        if (type == DatabaseType.Access)
            return GetPagingSqlByAccess(criteria, out order);

        if (criteria.OrderBys != null)
            order = string.Join(",", criteria.OrderBys.Select(f => string.Format("t1.{0}", f)).ToArray());
        if (string.IsNullOrEmpty(order))
            order = "t1.CreateTime";

        var startNo = criteria.PageSize * (criteria.PageIndex - 1);
        var endNo = startNo + criteria.PageSize;
        if (type == DatabaseType.MySql)
            return $"select t1.* from ({Text}) t1 order by {order} limit {startNo}, {criteria.PageSize}";

        if (type == DatabaseType.SQLite)
            return $"select t1.* from ({Text}) t1 order by {order} limit {criteria.PageSize} offset {startNo}";

        return $@"
select t.* from (
    select t1.*,row_number() over (order by {order}) row_no 
    from ({Text}) t1
) t where t.row_no>{startNo} and t.row_no<={endNo}";
    }

    private string GetPagingSqlByNone(PagingCriteria criteria, ref string order)
    {
        if (criteria.OrderBys != null)
            order = string.Join(",", criteria.OrderBys.Select(f => f).ToArray());

        if (string.IsNullOrEmpty(order))
            return Text;

        return $"{Text} order by {order}";
    }

    private string GetPagingSqlByAccess(PagingCriteria criteria, out string order)
    {
        if (criteria.OrderBys != null)
        {
            order = string.Join(",", criteria.OrderBys);
            if (criteria.OrderBys.Length > 1)
                return $"{Text} order by {order}";
        }
        else
        {
            order = "CreateTime";
        }

        var order1 = $"{order} desc";
        if (order.EndsWith("desc"))
            order1 = order.Replace("desc", "");
        else if (order.EndsWith("asc"))
            order1 = order.Replace("asc", "desc");

        var page = criteria.PageIndex;
        return $@"select t3.* from (
    select top {criteria.PageSize} t2.* from(
        select top {page * criteria.PageSize} t1.* from ({Text}) t1 order by t1.{order}
    ) t2 order by t2.{order1}
) t3 order by t3.{order}";
    }

    internal string GetTopSql(DatabaseType type, int size)
    {
        if (type == DatabaseType.SqlServer)
            return Text.Replace("select", $"select top {size}");
        else if (type == DatabaseType.MySql)
            return $"{Text} limit 0, {size}";

        return $@"
select t.* from (
    select t1.*,row_number() over (order by 1) row_no 
    from ({Text}) t1
) t where t.row_no>0 and t.row_no<={size}";
    }

    internal static string GetTableName<T>()
    {
        var type = typeof(T);
        var attrs = type.GetCustomAttributes(true);
        foreach (var item in attrs)
        {
            if (item is TableAttribute attr)
                return attr.Name;
        }

        return type.Name;
    }

    internal static string GetExportSql(string sql, PagingCriteria criteria)
    {
        var columns = criteria.ExportColumns.Where(c => sql.Contains('*') || sql.Contains(c.Key)).Select(c => $"t.{c.Key}");
        var columnStr = string.Join(",", columns);
        return $"select {columnStr} from ({sql}) t";
    }

    internal static CommandInfo GetInsertCommand(DatabaseType databaseType, DataRow row)
    {
        var tableName = row.Table.TableName;
        var cmdParams = new Dictionary<string, object>();
        var keys = new List<string>();
        foreach (DataColumn item in row.Table.Columns)
        {
            keys.Add(item.ColumnName);
            cmdParams.Add(item.ColumnName, row[item]);
        }
        var cloumn = string.Join(",", keys.Select(k => GetColumnName(databaseType, k)).ToArray());
        var value = string.Join(",", keys.Select(k => $"@{k}").ToArray());
        var sql = $"insert into {tableName}({cloumn}) values({value})";
        return new CommandInfo(databaseType, sql) { Params = cmdParams };
    }

    internal static CommandInfo GetInsertCommand<T>(DatabaseType databaseType, T entity)
    {
        var tableName = GetTableName<T>();
        var cmdParams = MapToDictionary(entity);
        return GetInsertCommand(databaseType, tableName, cmdParams);
    }

    internal static CommandInfo GetInsertCommand(DatabaseType databaseType, string tableName, Dictionary<string, object> cmdParams)
    {
        var changes = new Dictionary<string, object>();
        foreach (var item in cmdParams)
        {
            if (item.Value != null)
                changes[item.Key] = item.Value;
        }

        var keys = new List<string>();
        foreach (var key in changes.Keys)
        {
            keys.Add(key);
        }
        var cloumn = string.Join(",", keys.Select(k => GetColumnName(databaseType, k)).ToArray());
        var value = string.Join(",", keys.Select(k => $"@{k}").ToArray());
        var sql = $"insert into {tableName}({cloumn}) values({value})";
        return new CommandInfo(databaseType, sql) { Params = changes };
    }

    internal static CommandInfo GetSaveCommand<T>(DatabaseType databaseType, T entity) where T : EntityBase
    {
        var tableName = GetTableName<T>();
        var cmdParams = ToDictionary(entity);
        if (entity.IsNew)
            return GetInsertCommand(databaseType, tableName, cmdParams);

        var changes = new Dictionary<string, object>();
        foreach (var item in cmdParams)
        {
            if (entity.IsChanged(item.Key, item.Value))
                changes[item.Key] = item.Value;
        }

        var changeKeys = new List<string>();
        foreach (var key in changes.Keys)
        {
            var columnName = GetColumnName(databaseType, key);
            changeKeys.Add($"{columnName}=@{key}");
        }
        var column = string.Join(",", changeKeys.ToArray());
        var sql = $"update {tableName} set {column} where Id=@Id";
        changes["Id"] = entity.Id;
        return new CommandInfo(databaseType, sql) { Params = changes };
    }

    internal static CommandInfo GetUpdateCommand(DatabaseType databaseType, string tableName, string keyField, Dictionary<string, object> cmdParams)
    {
        var changeKeys = new List<string>();
        foreach (var key in cmdParams.Keys)
        {
            var columnName = GetColumnName(databaseType, key);
            changeKeys.Add($"{columnName}=@{key}");
        }
        var column = string.Join(",", changeKeys.ToArray());
        var sql = $"update {tableName} set {column} where {keyField}=@{keyField}";
        return new CommandInfo(databaseType, sql) { Params = cmdParams };
    }

    internal static Dictionary<string, object> ToDictionary(object value)
    {
        var dic = new Dictionary<string, object>();
        var properties = value.GetType().GetProperties();
        foreach (var item in properties)
        {
            if (item.CanRead && item.CanWrite && !item.GetMethod.IsVirtual)
            {
                dic[item.Name] = item.GetValue(value, null);
            }
        }
        return dic;
    }

    internal static Dictionary<string, object> MapToDictionary(object value)
    {
        var dic = Utils.MapTo<Dictionary<string, object>>(value);
        return dic ?? new Dictionary<string, object>();
    }

    private static string GetPrefix(DatabaseType databaseType)
    {
        return databaseType == DatabaseType.Oracle ? ":" : "@";
    }

    private static string GetColumnName(DatabaseType databaseType, string columnName)
    {
        if (databaseType == DatabaseType.Access)
            return $"`{columnName}`";

        return columnName;
    }
}

public class CheckException : Exception
{
    public CheckException(string message) : base(message) { }
}

public sealed class Check
{
    private Check() { }

    public static void IsNull(string name, object value)
    {
        if (value == null)
            throw new ArgumentNullException(name);
    }

    public static void Throw(string message)
    {
        throw new CheckException(message);
    }
}