using System.ComponentModel.DataAnnotations.Schema;

namespace Known;

public enum DatabaseType
{
    Access,
    SQLite,
    SqlServer,
    Oracle,
    MySql,
    Npgsql
}

public class Database : IDisposable
{
    private DbConnection conn;
    private DbTransaction trans;

    private string TransId { get; set; }

    #region Constructors
    public Database() : this("Default") { }

    public Database(Context context) : this()
    {
        Context = context;
        User = context?.CurrentUser;
    }

    public Database(string connName, UserInfo user = null)
    {
        var setting = Config.App.GetConnection(connName);
        if (setting != null)
        {
            Init(setting.DatabaseType, setting.ConnectionString, user);
        }
    }

    internal Database(DatabaseType databaseType, string connString, UserInfo user = null)
    {
        Init(databaseType, connString, user);
    }
    #endregion

    #region Properties
    public DatabaseType DatabaseType { get; private set; }
    public string ConnectionString { get; private set; }
    public UserInfo User { get; set; }
    public string UserName => User?.UserName;
    internal Context Context { get; set; }
    #endregion

    #region Static
    internal static void RegisterConnections()
    {
        var connections = Config.App.Connections;
        if (connections == null || connections.Count == 0)
            return;

        AppHelper.LoadConnections(connections);
        var dbFactories = connections.ToDictionary(k => k.DatabaseType.ToString(), v => v.ProviderType);
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

    internal static async Task InitializeAsync()
    {
        var db = new Database();
        int? count = null;
        try
        {
            count = await db.ScalarAsync<int?>("select count(*) from SysModule");
        }
        catch
        {
        }

        if (count == null)
        {
            Logger.Info("Data table is initializing...");
            var name = db.DatabaseType == DatabaseType.Npgsql ? "MySql" : db.DatabaseType.ToString();
            foreach (var item in Config.CoreAssemblies)
            {
                var script = Utils.GetResource(item, $"{name}.sql");
                if (string.IsNullOrWhiteSpace(script))
                    return;

                await db.ExecuteAsync(script);
            }
            Logger.Info("Data table is initialized");
        }
    }
    #endregion

    #region Schema
    public Task<List<string>> FindAllTablesAsync()
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

        return ScalarsAsync<string>(sql);
    }
    #endregion

    #region Public
    public Task OpenAsync()
    {
        if (conn != null && conn.State != ConnectionState.Open)
            conn.Open();

        return Task.CompletedTask;
    }

    public Task CloseAsync()
    {
        if (conn != null && conn.State != ConnectionState.Closed)
            conn.Close();
        conn.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        trans?.Dispose();
        trans = null;

        if (conn.State != ConnectionState.Closed)
            conn.Close();
        conn.Dispose();
    }

    public async Task<Result> TransactionAsync(string name, Func<Database, Task> action, object data = null)
    {
        using (var db = new Database(DatabaseType, ConnectionString, User))
        {
            try
            {
                db.TransId = Utils.GetGuid();
                await db.BeginTransAsync();
                await action.Invoke(db);
                await db.CommitAsync();
                return Result.Success(Context?.Language.Success(name), data);
            }
            catch (Exception ex)
            {
                await db.RollbackAsync();
                Logger.Exception(ex);
                if (ex is SystemException)
                    return Result.Error(ex.Message);
                else
                    return Result.Error(Context?.Language["Tip.TransError"]);
            }
            finally
            {
                db.TransId = string.Empty;
            }
        }
    }

    public async Task InsertTableAsync(DataTable data)
    {
        if (data == null || data.Rows.Count == 0)
            return;

        foreach (DataRow item in data.Rows)
        {
            var info = CommandInfo.GetInsertCommand(DatabaseType, item);
            await ExecuteNonQueryAsync(info);
        }
    }
    #endregion

    #region SQL
    public Task<int> ExecuteAsync(string sql, object param = null)
    {
        var info = new CommandInfo(DatabaseType, sql, param);
        return ExecuteNonQueryAsync(info);
    }

    public async Task<T> ScalarAsync<T>(string sql, object param = null)
    {
        var info = new CommandInfo(DatabaseType, sql, param);
        var cmd = await PrepareCommandAsync(conn, trans, info);
        var scalar = cmd.ExecuteScalar();
        cmd.Parameters.Clear();
        if (info.IsClose)
            conn.Close();
        return Utils.ConvertTo<T>(scalar);
    }

    public async Task<List<T>> ScalarsAsync<T>(string sql, object param = null)
    {
        var data = new List<T>();
        var info = new CommandInfo(DatabaseType, sql, param);
        var cmd = await PrepareCommandAsync(conn, trans, info);
        using (var reader = cmd.ExecuteReader())
        {
            while (reader.Read())
            {
                var obj = Utils.ConvertTo<T>(reader[0]);
                data.Add(obj);
            }
        }

        cmd.Parameters.Clear();
        if (info.IsClose)
            conn.Close();
        return data;
    }

    public Task<T> QueryAsync<T>(string sql, object param = null)
    {
        var info = new CommandInfo(DatabaseType, sql, param);
        return QueryAsync<T>(info);
    }

    public Task<List<T>> QueryListAsync<T>(string sql, object param = null)
    {
        var info = new CommandInfo(DatabaseType, sql, param);
        return QueryListAsync<T>(info);
    }

    public Task<List<T>> QueryListAsync<T>(int topSize, string sql, object param = null)
    {
        var info = new CommandInfo(DatabaseType, sql, param);
        info.Text = info.GetTopSql(DatabaseType, topSize);
        return QueryListAsync<T>(info);
    }

    public async Task<PagingResult<T>> QueryPageAsync<T>(string sql, PagingCriteria criteria)
    {
        var watch = Stopwatcher.Start<T>();
        SetAutoQuery(ref sql, criteria);

        if (conn.State != ConnectionState.Open)
            conn.Open();

        byte[] exportData = null;
        Dictionary<string, object> sums = null;
        var pageData = new List<T>();
        var info = new CommandInfo(DatabaseType, sql, criteria.ToParameters(User));
        var cmd = await PrepareCommandAsync(conn, trans, info);
        cmd.CommandText = info.CountSql;
        var value = cmd.ExecuteScalar();
        var total = Utils.ConvertTo<int>(value);
        watch.Watch("Total");
        if (total > 0)
        {
            if (criteria.ExportMode != ExportMode.None && criteria.ExportMode != ExportMode.Page)
                criteria.PageIndex = -1;

            cmd.CommandText = info.GetPagingSql(DatabaseType, criteria);
            watch.Watch("Paging");
            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var obj = ConvertTo<T>(reader);
                        pageData.Add((T)obj);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
            watch.Watch("Convert");
            if (criteria.ExportMode == ExportMode.None)
            {
                if (criteria.SumColumns != null && criteria.SumColumns.Count > 0)
                {
                    cmd.CommandText = info.GetSumSql(criteria);
                    watch.Watch("Suming");
                    using (var reader1 = cmd.ExecuteReader())
                    {
                        if (reader1 != null && reader1.Read())
                            sums = GetDictionary(reader1);
                    }
                    watch.Watch("Sum");
                }
            }
        }

        cmd.Parameters.Clear();
        if (conn.State != ConnectionState.Closed)
            conn.Close();

        if (criteria.ExportMode != ExportMode.None)
        {
            exportData = GetExportData(criteria, pageData);
            watch.Watch("Export");
        }

        if (pageData.Count > criteria.PageSize && criteria.PageSize > 0)
            pageData = pageData.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize).ToList();

        watch.WriteLog();
        return new PagingResult<T>(total, pageData) { ExportData = exportData, Sums = sums };
    }

    private void SetAutoQuery(ref string sql, PagingCriteria criteria)
    {
        var querys = new List<QueryInfo>();
        foreach (var item in criteria.Query)
        {
            if (!string.IsNullOrWhiteSpace(item.Value))
            {
                if (item.Value.Contains('~') && item.Type != QueryType.Between)
                    item.Type = QueryType.Between;
                item.ParamValue = item.Value;
                querys.Add(item);
            }
        }
        foreach (var item in querys)
        {
            if (!sql.Contains($"@{item.Id}"))
                SetQuery(ref sql, criteria, item.Type, item.Id);
        }
    }

    public async Task<PagingResult<Dictionary<string, object>>> QueryPageDictionaryAsync(string sql, PagingCriteria criteria)
    {
        var watch = Stopwatcher.Start<Dictionary<string, object>>();
        if (conn.State != ConnectionState.Open)
            conn.Open();

        byte[] exportData = null;
        Dictionary<string, object> sums = null;
        var pageData = new List<Dictionary<string, object>>();
        var info = new CommandInfo(DatabaseType, sql, criteria.ToParameters(User));
        var cmd = await PrepareCommandAsync(conn, trans, info);
        cmd.CommandText = info.CountSql;
        var scalar = cmd.ExecuteScalar();
        var total = Utils.ConvertTo<int>(scalar);
        watch.Watch("Total");
        if (total > 0)
        {
            cmd.CommandText = info.GetPagingSql(DatabaseType, criteria);
            watch.Watch("Paging");
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var dic = GetDictionary(reader);
                    pageData.Add(dic);
                }
            }
            watch.Watch("Convert");
            if (criteria.ExportMode == ExportMode.None)
            {
                if (criteria.SumColumns != null && criteria.SumColumns.Count > 0)
                {
                    cmd.CommandText = info.GetSumSql(criteria);
                    watch.Watch("Suming");
                    using (var reader1 = cmd.ExecuteReader())
                    {
                        if (reader1 != null && reader1.Read())
                            sums = GetDictionary(reader1);
                    }
                    watch.Watch("Sum");
                }
            }
        }

        cmd.Parameters.Clear();
        if (conn.State != ConnectionState.Closed)
            conn.Close();

        if (criteria.ExportMode != ExportMode.None)
        {
            exportData = GetExportData(criteria, pageData);
            watch.Watch("Export");
        }

        if (pageData.Count > criteria.PageSize && criteria.PageSize > 0)
            pageData = pageData.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize).ToList();

        watch.WriteLog();
        return new PagingResult<Dictionary<string, object>>(total, pageData) { ExportData = exportData, Sums = sums };
    }

    public async Task<DataTable> QueryTableAsync(string sql, object param = null)
    {
        var data = new DataTable();
        var info = new CommandInfo(DatabaseType, sql, param);
        using (var reader = await ExecuteReaderAsync(info))
        {
            if (reader != null)
                data.Load(reader);
        }
        return data;
    }
    #endregion

    #region Entity
    public Task<T> QueryByIdAsync<T>(string id) where T : EntityBase
    {
        if (string.IsNullOrEmpty(id))
            return default;

        var tableName = CommandInfo.GetTableName<T>();
        var sql = $"select * from {tableName} where Id=@id";
        return QueryAsync<T>(sql, new { id });
    }

    public Task<List<T>> QueryListAsync<T>() where T : EntityBase
    {
        var tableName = CommandInfo.GetTableName<T>();
        var sql = $"select * from {tableName} order by CreateTime";
        return QueryListAsync<T>(sql);
    }

    public Task<List<T>> QueryListByIdAsync<T>(string[] ids) where T : EntityBase
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

        return QueryListAsync<T>(info);
    }

    public Task<int> DeleteAllAsync<T>() where T : EntityBase
    {
        var tableName = CommandInfo.GetTableName<T>();
        var sql = $"delete from {tableName}";
        return ExecuteAsync(sql);
    }

    public Task<int> DeleteAsync<T>(string id) where T : EntityBase
    {
        var tableName = CommandInfo.GetTableName<T>();
        return DeleteAsync(tableName, id);
    }

    public Task<int> DeleteAsync<T>(T entity) where T : EntityBase
    {
        if (entity == null)
            return Task.FromResult(0);

        return DeleteAsync<T>(entity.Id);
    }

    public Task<int> InsertDataAsync<T>(T data)
    {
        if (data == null)
            return Task.FromResult(0);

        var info = CommandInfo.GetInsertCommand(DatabaseType, data);
        return ExecuteNonQueryAsync(info);
    }

    public async Task InsertDatasAsync<T>(List<T> datas)
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
            await ExecuteNonQueryAsync(info);
        }

        if (close)
            conn.Close();
    }

    public async Task<T> InsertAsync<T>(T entity, bool newId = true) where T : EntityBase
    {
        if (entity == null)
            return entity;

        if (newId)
            entity.Id = Utils.GetGuid();
        entity.IsNew = true;
        entity.Version = 1;
        await SaveAsync(entity);
        return entity;
    }

    public async Task SaveAsync<T>(T entity) where T : EntityBase
    {
        if (entity == null)
            return;

        if (User == null)
            throw new SystemException("the user is not null.");

        if (entity.IsNew)
        {
            if (entity.CreateBy == "temp")
                entity.CreateBy = User.UserName;
            entity.CreateTime = DateTime.Now;
            if (entity.AppId == "temp")
                entity.AppId = User.AppId;
            if (entity.CompNo == "temp")
                entity.CompNo = User.CompNo;
        }
        else
        {
            entity.Version += 1;
            var tableName = CommandInfo.GetTableName<T>();
            var sql = $"select * from {tableName} where Id=@Id";
            var original = await QueryAsync<Dictionary<string, object>>(sql, new { entity.Id });
            entity.SetOriginal(original);
        }

        entity.ModifyBy = User.UserName;
        entity.ModifyTime = DateTime.Now;

        var info = CommandInfo.GetSaveCommand(DatabaseType, entity);
        info.IsSave = true;
        await ExecuteNonQueryAsync(info);
        entity.IsNew = false;
    }

    public async Task SaveDatasAsync<T>(List<T> entities) where T : EntityBase
    {
        if (entities == null || entities.Count == 0)
            return;

        foreach (var entity in entities)
        {
            await SaveAsync(entity);
        }
    }
    #endregion

    #region Dictionary
    internal async Task<int> SaveAsync(string tableName, Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            return 0;

        var id = data.GetValue<string>("Id");
        var sql = $"select count(*) from {tableName} where Id=@id";
        var count = await ScalarAsync<int>(sql, new { id });
        if (count > 0)
        {
            data["Version"] = data.GetValue<int>("Version") + 1;
            return await UpdateAsync(tableName, "Id", data);
        }

        if (string.IsNullOrWhiteSpace(id))
            data["Id"] = Utils.GetGuid();
        data["CreateBy"] = User.UserName;
        data["CreateTime"] = DateTime.Now;
        data["ModifyBy"] = User.UserName;
        data["ModifyTime"] = DateTime.Now;
        data["Version"] = 1;
        data["AppId"] = User.AppId;
        data["CompNo"] = User.CompNo;
        return await InsertAsync(tableName, data);
    }

    internal Task<int> DeleteAsync(string tableName, string id)
    {
        if (string.IsNullOrEmpty(id))
            return Task.FromResult(0);

        var sql = $"delete from {tableName} where Id=@id";
        return ExecuteAsync(sql, new { id });
    }

    public Task<int> InsertAsync(string tableName, Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            return Task.FromResult(0);

        var info = CommandInfo.GetInsertCommand(DatabaseType, tableName, data);
        return ExecuteNonQueryAsync(info);
    }

    public Task<int> UpdateAsync(string tableName, string keyField, Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            return Task.FromResult(0);

        var info = CommandInfo.GetUpdateCommand(DatabaseType, tableName, keyField, data);
        return ExecuteNonQueryAsync(info);
    }
    #endregion

    #region Trans
    private Task BeginTransAsync()
    {
        if (conn.State != ConnectionState.Open)
            conn.Open();
        trans = conn.BeginTransaction();
        return Task.CompletedTask;
    }

    private Task CommitAsync()
    {
        trans?.Commit();
        return Task.CompletedTask;
    }

    private Task RollbackAsync()
    {
        trans?.Rollback();
        return Task.CompletedTask;
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

        if (criteria.Fields.ContainsKey(key))
            field = criteria.Fields[key];

        switch (type)
        {
            case QueryType.Equal:
                sql += $" and {field}=@{key}";
                break;
            case QueryType.NotEqual:
                sql += $" and {field}<>@{key}";
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
                SetLikeQuery(ref sql, criteria, field, key, "%{0}%");
                break;
            case QueryType.StartWith:
                SetLikeQuery(ref sql, criteria, field, key, "{0}%");
                break;
            case QueryType.EndWith:
                SetLikeQuery(ref sql, criteria, field, key, "%{0}");
                break;
            case QueryType.Batch:
                SetBatchQuery(ref sql, criteria, field, key);
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
            query.ParamValue = $"{query.Value} 00:00:00";
        }
        else if (criteria.HasQuery(key))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == key);
            var value = query.Value.Split('~')[0];
            if (!string.IsNullOrWhiteSpace(value))
            {
                sql += $" and {field}{symbol}{date}";
                var query1 = criteria.SetQuery(paramName, value);
                query1.ParamValue = $"{value} 00:00:00";
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
            query.ParamValue = $"{query.Value} 23:59:59";
        }
        else if (criteria.HasQuery(key))
        {
            var query = criteria.Query.FirstOrDefault(q => q.Id == key);
            var value = query.Value.Split('~')[1];
            if (!string.IsNullOrWhiteSpace(value))
            {
                sql += $" and {field}{symbol}{date}";
                var query1 = criteria.SetQuery(paramName, value);
                query1.ParamValue = $"{value} 23:59:59";
            }
        }
    }

    private void SetLikeQuery(ref string sql, PagingCriteria criteria, string field, string key, string format)
    {
        var query = criteria.Query.FirstOrDefault(q => q.Id == key);
        query.ParamValue = string.Format(format, query.Value);
        if (DatabaseType == DatabaseType.Access)
            sql += $" and {field} like '{query.Value}'";
        else
            sql += $" and {field} like @{key}";
    }

    private static void SetBatchQuery(ref string sql, PagingCriteria criteria, string field, string key)
    {
        var query = criteria.Query.FirstOrDefault(q => q.Id == key);
        var value = query.Value;
        if (string.IsNullOrWhiteSpace(value))
            return;

        var values = value.Split(',', '，');
        var wheres = new List<string>();
        for (int i = 0; i < values.Length; i++)
        {
            var pkey = $"{key}{i}";
            wheres.Add($"{field}=@{pkey}");
            criteria.SetQuery(pkey, QueryType.Equal, values[i]);
        }
        var where = string.Join(" or ", wheres);
        sql += $" and ({where})";
    }
    #endregion

    #region Private
    private async Task<int> ExecuteNonQueryAsync(CommandInfo info)
    {
        var cmd = await PrepareCommandAsync(conn, trans, info);
        var value = cmd.ExecuteNonQuery();
        cmd.Parameters.Clear();
        if (info.IsClose)
            conn.Close();
        return value;
    }

    private async Task<DbDataReader> ExecuteReaderAsync(CommandInfo info)
    {
        var cmd = await PrepareCommandAsync(conn, trans, info);
        var reader = info.IsClose
                   ? cmd.ExecuteReader(CommandBehavior.CloseConnection)
                   : cmd.ExecuteReader();
        cmd.Parameters.Clear();
        return reader;
    }

    private async Task<T> QueryAsync<T>(CommandInfo info)
    {
        T obj = default;
        using (var reader = await ExecuteReaderAsync(info))
        {
            if (reader != null && reader.Read())
            {
                obj = (T)ConvertTo<T>(reader);
            }
        }
        if (info.IsClose)
            conn.Close();
        return obj;
    }

    private async Task<List<T>> QueryListAsync<T>(CommandInfo info)
    {
        var lists = new List<T>();
        using (var reader = await ExecuteReaderAsync(info))
        {
            while (reader.Read())
            {
                var obj = ConvertTo<T>(reader);
                lists.Add((T)obj);
            }
        }
        if (info.IsClose)
            conn.Close();
        return lists;
    }

    private static object ConvertTo<T>(DbDataReader reader)
    {
        var dic = GetDictionary(reader);
        var type = typeof(T);
        if (type == typeof(Dictionary<string, object>))
            return dic;

        var obj = Activator.CreateInstance<T>();
        var properties = TypeHelper.Properties(type);
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

    private static Dictionary<string, object> GetDictionary(DbDataReader reader)
    {
        var dic = new Dictionary<string, object>();
        for (int i = 0; i < reader.FieldCount; i++)
        {
            var name = reader.GetName(i).Replace("_", "");
            var value = reader[i];
            dic.Add(name, value == DBNull.Value ? null : value);
        }
        return dic;
    }

    private Task<DbCommand> PrepareCommandAsync(DbConnection conn, DbTransaction trans, CommandInfo info)
    {
        info.IsClose = false;
        var cmd = conn.CreateCommand();
        cmd.CommandText = FormatSQL(info.Text);

        if (trans != null)
        {
            if (trans.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof(trans));

            cmd.Transaction = trans;
        }
        else
        {
            if (conn.State != ConnectionState.Open)
            {
                info.IsClose = true;
                conn.Open();
            }
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
                    p.Value = GetParameterValue(item, info.IsSave);
                    cmd.Parameters.Add(p);
                }
            }
        }

        return Task.FromResult(cmd);
    }

    private object GetParameterValue(KeyValuePair<string, object> item, bool isTrim)
    {
        if (item.Value == null)
            return DBNull.Value;

        if (item.Value is DateTime time)
            return DatabaseType == DatabaseType.Access ? time.ToString() : time;

        var value = item.Value.ToString();
        if (isTrim)
            value = value.Trim('\r', '\n').Trim();

        return value;
    }

    private string FormatSQL(string text)
    {
        if (DatabaseType == DatabaseType.Npgsql)
        {
            text = text.Replace(" Type", " \"Type\"")
                       .Replace(" Name", " \"Name\"");
        }

        return text;
    }

    private static byte[] GetExportData<T>(PagingCriteria criteria, List<T> pageData)
    {
        if (criteria.ExportColumns == null || criteria.ExportColumns.Count == 0 || pageData.Count == 0)
            return null;

        var excel = ExcelFactory.Create();
        var sheet = excel.CreateSheet("Sheet1");
        var index = 0;
        var headStyle = new StyleInfo { IsBorder = true, IsBold = true, FontColor = Color.White, BackgroundColor = Utils.FromHtml("#6D87C1") };
        foreach (var item in criteria.ExportColumns)
        {
            sheet.SetCellValue(0, index++, item.Name, headStyle);
        }

        var rowIndex = 0;
        var isDictionary = typeof(T) == typeof(Dictionary<string, object>);
        foreach (var data in pageData)
        {
            rowIndex++;
            index = 0;
            foreach (var item in criteria.ExportColumns)
            {
                var cellStyle = new StyleInfo { IsBorder = true };
                var value = isDictionary
                          ? (data as Dictionary<string, object>).GetValue(item.Id)
                          : TypeHelper.GetPropertyValue(data, item.Id);
                if (item.Type == FieldType.Switch || item.Type == FieldType.CheckBox)
                    value = Utils.ConvertTo<bool>(value) ? "是" : "否";
                else if (item.Type == FieldType.Date)
                {
                    value = Utils.ConvertTo<DateTime?>(value)?.Date;
                    cellStyle.Custom = Config.DateFormat;
                }
                else if (item.Type == FieldType.DateTime)
                    cellStyle.Custom = Config.DateTimeFormat;
                else if (item.Type == FieldType.Number)
                    value = GetNumberValue(value);
                else if (!string.IsNullOrWhiteSpace(item.Category))
                    value = Cache.GetCodeName(item.Category, value?.ToString());
                sheet.SetCellValue(rowIndex, index++, value, cellStyle);
            }
        }

        var stream = excel.SaveToStream();
        return stream.ToArray();
    }

    private static object GetNumberValue(object value)
    {
        if (decimal.TryParse(value?.ToString(), out var number))
            return number;

        return value;
    }

    private void Init(DatabaseType databaseType, string connString, UserInfo user = null)
    {
        DatabaseType = databaseType;
        ConnectionString = connString;
        User = user;

        var factory = DbProviderFactories.GetFactory(databaseType.ToString());
        conn = factory.CreateConnection();
        conn.ConnectionString = connString;
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

    internal bool IsSave { get; set; }
    internal bool IsClose { get; set; }
    internal DatabaseType DatabaseType { get; }
    internal string Prefix { get; }
    internal string Text { get; set; }
    internal Dictionary<string, object> Params { get; set; }
    internal string CountSql => $"select count(*) {Text.Substring(Text.IndexOf("from"))}";

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

    internal string GetSumSql(PagingCriteria criteria)
    {
        var columns = string.Join(",", criteria.SumColumns.Select(c => $"sum({c}) as {c}"));
        return $"select {columns} from ({Text}) t";
    }

    internal string GetPagingSql(DatabaseType type, PagingCriteria criteria)
    {
        if (type == DatabaseType.Access)
            return GetPagingSqlByAccess(criteria);

        var order = string.Empty;
        if (criteria.OrderBys != null && criteria.OrderBys.Length > 0)
            order = string.Join(",", criteria.OrderBys);

        if (string.IsNullOrWhiteSpace(order))
            order = "CreateTime desc";

        if (criteria.PageIndex <= 0)
            return GetPagingSqlByNone(criteria, order);

        var startNo = criteria.PageSize * (criteria.PageIndex - 1);
        var endNo = startNo + criteria.PageSize;
        if (type == DatabaseType.MySql)
            return $"{Text} order by {order} limit {startNo}, {criteria.PageSize}";

        if (type == DatabaseType.SQLite)
            return $"{Text} order by {order} limit {criteria.PageSize} offset {startNo}";

        return $@"
select t.* from (
    select t1.*,row_number() over (order by {order}) row_no 
    from ({Text}) t1
) t where t.row_no>{startNo} and t.row_no<={endNo}";
    }

    private string GetPagingSqlByNone(PagingCriteria criteria, string order)
    {
        if (string.IsNullOrEmpty(order))
            return Text;

        return $"{Text} order by {order}";
    }

    private string GetPagingSqlByAccess(PagingCriteria criteria)
    {
        var order = string.Empty;
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
        var properties = TypeHelper.Properties(value.GetType());
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
        else if (databaseType == DatabaseType.Npgsql)
            return $"\"{columnName}\"";

        return columnName;
    }
}