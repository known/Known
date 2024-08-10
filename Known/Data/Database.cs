namespace Known.Data;

public class Database : IDisposable
{
    private DbConnection conn;
    private DbTransaction trans;

    private string TransId { get; set; }
    internal SqlBuilder Builder { get; set; }

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

        var info = Builder.GetInsertCommand(data);
        foreach (DataRow item in data.Rows)
        {
            info.SetParameters(item);
            await ExecuteNonQueryAsync(info);
        }
    }
    #endregion

    #region SQL
    public Task<int> ExecuteAsync(string sql, object param = null)
    {
        var info = Builder.GetCommand(sql, param);
        return ExecuteNonQueryAsync(info);
    }

    public Task<T> ScalarAsync<T>(string sql, object param = null)
    {
        var info = Builder.GetCommand(sql, param);
        return ScalarAsync<T>(info);
    }

    public async Task<List<T>> ScalarsAsync<T>(string sql, object param = null)
    {
        var data = new List<T>();
        var info = Builder.GetCommand(sql, param);
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
        var info = Builder.GetCommand(sql, param);
        return QueryAsync<T>(info);
    }

    public Task<List<T>> QueryListAsync<T>(string sql, object param = null)
    {
        var info = Builder.GetCommand(sql, param);
        return QueryListAsync<T>(info);
    }

    public Task<List<T>> QueryListAsync<T>(int topSize, string sql, object param = null)
    {
        var info = Builder.GetTopCommand(topSize, sql, param);
        return QueryListAsync<T>(info);
    }

    public async Task<PagingResult<T>> QueryPageAsync<T>(string sql, PagingCriteria criteria)
    {
        var watch = Stopwatcher.Start<T>();
        QueryHelper.SetAutoQuery(ref sql, Builder, criteria);

        if (conn.State != ConnectionState.Open)
            conn.Open();

        byte[] exportData = null;
        Dictionary<string, object> sums = null;
        var pageData = new List<T>();
        var info = Builder.GetCommand(sql, criteria, User);
        var cmd = await PrepareCommandAsync(conn, trans, info);
        cmd.CommandText = info.CountSql;
        var value = cmd.ExecuteScalar();
        var total = Utils.ConvertTo<int>(value);
        watch.Watch("Total");
        if (total > 0)
        {
            if (criteria.ExportMode != ExportMode.None && criteria.ExportMode != ExportMode.Page)
                criteria.PageIndex = -1;

            cmd.CommandText = info.PageSql;
            watch.Watch("Paging");
            try
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var obj = DBHelper.ConvertTo<T>(reader);
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
                    cmd.CommandText = info.SumSql;
                    watch.Watch("Suming");
                    using (var reader1 = cmd.ExecuteReader())
                    {
                        if (reader1 != null && reader1.Read())
                            sums = DBHelper.GetDictionary(reader1);
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
            exportData = DBHelper.GetExportData(criteria, pageData);
            watch.Watch("Export");
        }

        if (pageData.Count > criteria.PageSize && criteria.PageSize > 0)
            pageData = pageData.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize).ToList();

        watch.WriteLog();
        return new PagingResult<T>(total, pageData) { ExportData = exportData, Sums = sums };
    }

    public async Task<PagingResult<Dictionary<string, object>>> QueryPageDictionaryAsync(string sql, PagingCriteria criteria)
    {
        var watch = Stopwatcher.Start<Dictionary<string, object>>();
        if (conn.State != ConnectionState.Open)
            conn.Open();

        byte[] exportData = null;
        Dictionary<string, object> sums = null;
        var pageData = new List<Dictionary<string, object>>();
        var info = Builder.GetCommand(sql, criteria, User);
        var cmd = await PrepareCommandAsync(conn, trans, info);
        cmd.CommandText = info.CountSql;
        var scalar = cmd.ExecuteScalar();
        var total = Utils.ConvertTo<int>(scalar);
        watch.Watch("Total");
        if (total > 0)
        {
            cmd.CommandText = info.PageSql;
            watch.Watch("Paging");
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var dic = DBHelper.GetDictionary(reader);
                    pageData.Add(dic);
                }
            }
            watch.Watch("Convert");
            if (criteria.ExportMode == ExportMode.None)
            {
                if (criteria.SumColumns != null && criteria.SumColumns.Count > 0)
                {
                    cmd.CommandText = info.SumSql;
                    watch.Watch("Suming");
                    using (var reader1 = cmd.ExecuteReader())
                    {
                        if (reader1 != null && reader1.Read())
                            sums = DBHelper.GetDictionary(reader1);
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
            exportData = DBHelper.GetExportData(criteria, pageData);
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
        var info = Builder.GetCommand(sql, param);
        using (var reader = await ExecuteReaderAsync(info))
        {
            if (reader != null)
                data.Load(reader);
        }
        return data;
    }
    #endregion

    #region Entity
    public Task<int?> CountAsync<T>()
    {
        var info = Builder.GetCountCommand<T>();
        return ScalarAsync<int?>(info);
    }

    public Task<T> QueryByIdAsync<T>(string id) where T : EntityBase
    {
        if (string.IsNullOrEmpty(id))
            return default;

        var info = Builder.GetSelectCommand<T>(id);
        return QueryAsync<T>(info);
    }

    public Task<List<T>> QueryListAsync<T>() where T : EntityBase
    {
        var info = Builder.GetSelectCommand<T>();
        return QueryListAsync<T>(info);
    }

    public Task<List<T>> QueryListByIdAsync<T>(string[] ids) where T : EntityBase
    {
        if (ids == null || ids.Length == 0)
            return null;

        var info = Builder.GetSelectCommand<T>(ids);
        return QueryListAsync<T>(info);
    }

    public Task<int> DeleteAllAsync<T>() where T : EntityBase
    {
        var info = Builder.GetDeleteCommand<T>();
        return ExecuteNonQueryAsync(info);
    }

    public Task<int> DeleteAsync<T>(string id) where T : EntityBase
    {
        var info = Builder.GetDeleteCommand<T>(id);
        return ExecuteNonQueryAsync(info);
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

        var info = Builder.GetInsertCommand(data);
        return ExecuteNonQueryAsync(info);
    }

    public async Task InsertDatasAsync<T>(List<T> datas)
    {
        if (datas == null || datas.Count == 0)
            return;

        var close = false;
        if (conn.State != ConnectionState.Open)
        {
            close = true;
            conn.Open();
        }

        var info = Builder.GetInsertCommand<T>();
        foreach (var item in datas)
        {
            info.SetParameters(item);
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
            var info1 = Builder.GetSelectCommand<T>(entity.Id);
            var original = await QueryAsync<Dictionary<string, object>>(info1);
            entity.SetOriginal(original);
        }

        entity.ModifyBy = User.UserName;
        entity.ModifyTime = DateTime.Now;

        var info = Builder.GetSaveCommand(entity);
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

        var id = data.GetValue<string>(nameof(EntityBase.Id));
        var info = Builder.GetCountCommand(tableName, id);
        var count = await ScalarAsync<int>(info);
        if (count > 0)
        {
            data[nameof(EntityBase.Version)] = data.GetValue<int>(nameof(EntityBase.Version)) + 1;
            return await UpdateAsync(tableName, nameof(EntityBase.Id), data);
        }

        if (string.IsNullOrWhiteSpace(id))
            data[nameof(EntityBase.Id)] = Utils.GetGuid();
        data[nameof(EntityBase.CreateBy)] = User.UserName;
        data[nameof(EntityBase.CreateTime)] = DateTime.Now;
        data[nameof(EntityBase.ModifyBy)] = User.UserName;
        data[nameof(EntityBase.ModifyTime)] = DateTime.Now;
        data[nameof(EntityBase.Version)] = 1;
        data[nameof(EntityBase.AppId)] = User.AppId;
        data[nameof(EntityBase.CompNo)] = User.CompNo;
        return await InsertAsync(tableName, data);
    }

    internal Task<int> DeleteAsync(string tableName, string id)
    {
        if (string.IsNullOrEmpty(id))
            return Task.FromResult(0);

        var info = Builder.GetDeleteCommand(tableName, id);
        return ExecuteNonQueryAsync(info);
    }

    public Task<int> InsertAsync(string tableName, Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            return Task.FromResult(0);

        var info = Builder.GetInsertCommand(tableName, data);
        return ExecuteNonQueryAsync(info);
    }

    public Task<int> UpdateAsync(string tableName, string keyField, Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            return Task.FromResult(0);

        var info = Builder.GetUpdateCommand(tableName, keyField, data);
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

    #region Private
    internal async Task<int> ExecuteNonQueryAsync(CommandInfo info)
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

    internal async Task<T> ScalarAsync<T>(CommandInfo info)
    {
        var cmd = await PrepareCommandAsync(conn, trans, info);
        var scalar = cmd.ExecuteScalar();
        cmd.Parameters.Clear();
        if (info.IsClose)
            conn.Close();
        return Utils.ConvertTo<T>(scalar);
    }

    internal async Task<T> QueryAsync<T>(CommandInfo info)
    {
        T obj = default;
        using (var reader = await ExecuteReaderAsync(info))
        {
            if (reader != null && reader.Read())
            {
                obj = (T)DBHelper.ConvertTo<T>(reader);
            }
        }
        if (info.IsClose)
            conn.Close();
        return obj;
    }

    internal async Task<List<T>> QueryListAsync<T>(CommandInfo info)
    {
        var lists = new List<T>();
        using (var reader = await ExecuteReaderAsync(info))
        {
            while (reader.Read())
            {
                var obj = DBHelper.ConvertTo<T>(reader);
                lists.Add((T)obj);
            }
        }
        if (info.IsClose)
            conn.Close();
        return lists;
    }

    private Task<DbCommand> PrepareCommandAsync(DbConnection conn, DbTransaction trans, CommandInfo info)
    {
        info.IsClose = false;
        var cmd = conn.CreateCommand();
        cmd.CommandText = info.Text;

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

        if (item.Value is bool boolean)
            return boolean.ToString();

        if (item.Value is DateTime time)
            return DatabaseType == DatabaseType.Access ? time.ToString() : time;

        if (item.Value is string value)
            return isTrim ? TrimValue(value) : value;

        if (item.Value is JsonElement element)
        {
            var valueString = element.ToString();
            return isTrim ? TrimValue(valueString) : valueString;
        }

        return item.Value;
    }

    private static string TrimValue(string value) => value.Trim('\r', '\n').Trim();

    private void Init(DatabaseType databaseType, string connString, UserInfo user = null)
    {
        Builder = SqlBuilder.Create(databaseType);
        DatabaseType = databaseType;
        ConnectionString = connString;
        User = user;

        var factory = DbProviderFactories.GetFactory(databaseType.ToString());
        conn = factory.CreateConnection();
        conn.ConnectionString = connString;
    }
    #endregion
}