namespace Known.Data;

class DefaultDatabase : Database
{
    private DbConnection conn;
    private DbTransaction trans;

    private string TransId { get; set; }

    #region Constructors
    public DefaultDatabase() : this("Default") { }

    public DefaultDatabase(Context context) : this()
    {
        Context = context;
        User = context?.CurrentUser;
    }

    public DefaultDatabase(string connName, UserInfo user = null)
    {
        var setting = Config.App.GetConnection(connName);
        if (setting != null)
        {
            Init(setting.DatabaseType, setting.ConnectionString, user);
        }
    }

    internal DefaultDatabase(DatabaseType databaseType, string connString, UserInfo user = null)
    {
        Init(databaseType, connString, user);
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
    public override Task OpenAsync()
    {
        if (conn != null && conn.State != ConnectionState.Open)
            conn.Open();

        return Task.CompletedTask;
    }

    public override Task CloseAsync()
    {
        if (conn != null && conn.State != ConnectionState.Closed)
            conn.Close();
        conn.Dispose();
        return Task.CompletedTask;
    }

    public override async Task<Result> TransactionAsync(string name, Func<Database, Task> action, object data = null)
    {
        using (var db = new DefaultDatabase(DatabaseType, ConnectionString, User))
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

    public override async Task<int> InsertTableAsync(DataTable data)
    {
        if (data == null || data.Rows.Count == 0)
            return 0;

        var count = 0;
        var info = Provider.GetInsertCommand(data);
        foreach (DataRow item in data.Rows)
        {
            info.SetParameters(item);
            count += await ExecuteNonQueryAsync(info);
        }
        return count;
    }
    #endregion

    #region SQL
    public override Task<int> ExecuteAsync(string sql, object param = null)
    {
        var info = Provider.GetCommand(sql, param);
        return ExecuteNonQueryAsync(info);
    }

    public override Task<T> ScalarAsync<T>(string sql, object param = null)
    {
        var info = Provider.GetCommand(sql, param);
        return ScalarAsync<T>(info);
    }

    public override Task<List<T>> ScalarsAsync<T>(string sql, object param = null)
    {
        var info = Provider.GetCommand(sql, param);
        return ScalarsAsync<T>(info);
    }

    public override Task<T> QueryAsync<T>(string sql, object param = null)
    {
        var info = Provider.GetCommand(sql, param);
        return QueryAsync<T>(info);
    }

    public override Task<List<T>> QueryListAsync<T>(string sql, object param = null)
    {
        var info = Provider.GetCommand(sql, param);
        return QueryListAsync<T>(info);
    }

    public override async Task<PagingResult<T>> QueryPageAsync<T>(string sql, PagingCriteria criteria)
    {
        try
        {
            QueryHelper.SetAutoQuery<T>(this, ref sql, criteria);
            if (criteria.ExportMode != ExportMode.None && criteria.ExportMode != ExportMode.Page)
                criteria.PageIndex = -1;

            if (conn.State != ConnectionState.Open)
                conn.Open();

            byte[] exportData = null;
            Dictionary<string, object> statis = null;
            var watch = Stopwatcher.Start<T>();
            var pageData = new List<T>();
            var info = Provider.GetCommand(sql, criteria, User);
            var cmd = await PrepareCommandAsync(conn, trans, info);
            cmd.CommandText = info.CountSql;
            var value = cmd.ExecuteScalar();
            var total = Utils.ConvertTo<int>(value);
            watch.Watch("Total");
            if (total > 0)
            {
                cmd.CommandText = info.PageSql;
                watch.Watch("Paging");
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var obj = DBUtils.ConvertTo<T>(reader);
                        pageData.Add((T)obj);
                    }
                }
                watch.Watch("Convert");
                if (criteria.ExportMode == ExportMode.None)
                {
                    if (criteria.StatisColumns != null && criteria.StatisColumns.Count > 0)
                    {
                        cmd.CommandText = info.StatSql;
                        watch.Watch("Suming");
                        using (var reader1 = cmd.ExecuteReader())
                        {
                            if (reader1 != null && reader1.Read())
                                statis = DBUtils.GetDictionary(reader1);
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
                exportData = DBUtils.GetExportData(criteria, pageData);
                watch.Watch("Export");
            }

            if (pageData.Count > criteria.PageSize && criteria.PageSize > 0)
                pageData = pageData.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize).ToList();

            watch.WriteLog();
            return new PagingResult<T>(total, pageData) { ExportData = exportData, Statis = statis };
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            Logger.Error(sql);
            return new PagingResult<T>();
        }
    }

    public override async Task<DataTable> QueryTableAsync(string sql, object param = null)
    {
        var data = new DataTable();
        var info = Provider.GetCommand(sql, param);
        using (var reader = await ExecuteReaderAsync(info))
        {
            if (reader != null)
                data.Load(reader);
        }
        return data;
    }
    #endregion

    #region Entity
    public override Task<int> CountAsync<T>()
    {
        var info = Provider.GetCountCommand<T>();
        return ScalarAsync<int>(info);
    }

    public override Task<List<T>> QueryListAsync<T>()
    {
        var info = Provider.GetSelectCommand<T>();
        return QueryListAsync<T>(info);
    }

    public override Task<List<T>> QueryListByIdAsync<T>(string[] ids)
    {
        if (ids == null || ids.Length == 0)
            return null;

        var info = Provider.GetSelectCommand<T>(ids);
        return QueryListAsync<T>(info);
    }

    public override Task<int> DeleteAllAsync<T>()
    {
        var info = Provider.GetDeleteCommand<T>();
        return ExecuteNonQueryAsync(info);
    }

    public override Task<int> InsertAsync<T>(T data)
    {
        if (data == null)
            return Task.FromResult(0);

        var info = Provider.GetInsertCommand(data);
        return ExecuteNonQueryAsync(info);
    }

    public override async Task<int> InsertListAsync<T>(List<T> datas)
    {
        if (datas == null || datas.Count == 0)
            return 0;

        var close = false;
        if (conn.State != ConnectionState.Open)
        {
            close = true;
            conn.Open();
        }

        var count = 0;
        var info = Provider.GetInsertCommand<T>();
        foreach (var item in datas)
        {
            info.SetParameters(item);
            count += await ExecuteNonQueryAsync(info);
        }

        if (close)
            conn.Close();

        return count;
    }

    protected override Task<int> SaveDataAsync<T>(T entity)
    {
        var info = Provider.GetSaveCommand(entity);
        info.IsSave = true;
        return ExecuteNonQueryAsync(info);
    }

    internal override async Task SetOriginalAsync<T>(T entity)
    {
        var info = Provider.GetSelectCommand<T>(entity.Id);
        var original = await QueryAsync<Dictionary<string, object>>(info);
        entity.SetOriginal(original);
    }
    #endregion

    #region Expression
    public override Task<PagingResult<T>> QueryPageAsync<T>(PagingCriteria criteria)
    {
        var tableName = Provider.GetTableName<T>();
        var compNo = nameof(EntityBase.CompNo);
        var compName = Provider.FormatName(compNo);
        var sql = $"select * from {tableName} where {compName}=@{compNo}";
        return QueryPageAsync<T>(sql, criteria);
    }

    public override Task<T> QueryAsync<T>(Expression<Func<T, bool>> expression)
    {
        var info = Provider.GetSelectCommand(expression);
        return QueryAsync<T>(info);
    }

    public override Task<List<T>> QueryListAsync<T>(Expression<Func<T, bool>> expression)
    {
        var info = Provider.GetSelectCommand(expression);
        return QueryListAsync<T>(info);
    }

    public override Task<int> CountAsync<T>(Expression<Func<T, bool>> expression)
    {
        var info = Provider.GetCountCommand(expression);
        return ScalarAsync<int>(info);
    }

    public override Task<int> DeleteAsync<T>(Expression<Func<T, bool>> expression)
    {
        var info = Provider.GetDeleteCommand(expression);
        return ExecuteNonQueryAsync(info);
    }
    #endregion

    #region Dictionary
    public override Task<PagingResult<Dictionary<string, object>>> QueryPageAsync(string tableName, PagingCriteria criteria)
    {
        var sql = Provider.GetSelectSql(tableName);
        return QueryPageAsync<Dictionary<string, object>>(sql, criteria);
    }

    public override async Task<bool> ExistsAsync(string tableName, string id)
    {
        var info = Provider.GetCountCommand(tableName, id);
        var count = await ScalarAsync<int>(info);
        return count > 0;
    }

    public override Task<int> DeleteAsync(string tableName, string id)
    {
        if (string.IsNullOrEmpty(id))
            return Task.FromResult(0);

        var info = Provider.GetDeleteCommand(tableName, id);
        return ExecuteNonQueryAsync(info);
    }

    public override Task<int> InsertAsync(string tableName, Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            return Task.FromResult(0);

        var info = Provider.GetInsertCommand(tableName, data);
        return ExecuteNonQueryAsync(info);
    }

    public override Task<int> UpdateAsync(string tableName, string keyField, Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            return Task.FromResult(0);

        var info = Provider.GetUpdateCommand(tableName, keyField, data);
        return ExecuteNonQueryAsync(info);
    }
    #endregion

    protected override void Dispose(bool isDisposing)
    {
        trans?.Dispose();
        trans = null;

        if (conn.State != ConnectionState.Closed)
            conn.Close();
        conn.Dispose();
    }

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
        try
        {
            var cmd = await PrepareCommandAsync(conn, trans, info);
            var value = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            if (info.IsClose)
                conn.Close();
            return value;
        }
        catch (Exception ex)
        {
            Logger.Error(info.ToString());
            throw new SystemException(ex.Message, ex);
        }
    }

    private async Task<DbDataReader> ExecuteReaderAsync(CommandInfo info)
    {
        try
        {
            var cmd = await PrepareCommandAsync(conn, trans, info);
            var reader = info.IsClose
                       ? cmd.ExecuteReader(CommandBehavior.CloseConnection)
                       : cmd.ExecuteReader();
            cmd.Parameters.Clear();
            return reader;
        }
        catch (Exception ex)
        {
            Logger.Error(info.ToString());
            Logger.Exception(ex);
            return null;
        }
    }

    internal override async Task<T> ScalarAsync<T>(CommandInfo info)
    {
        try
        {
            var cmd = await PrepareCommandAsync(conn, trans, info);
            var scalar = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            if (info.IsClose)
                conn.Close();
            return Utils.ConvertTo<T>(scalar);
        }
        catch (Exception ex)
        {
            Logger.Error(info.ToString());
            Logger.Exception(ex);
            return default;
        }
    }

    internal override async Task<List<T>> ScalarsAsync<T>(CommandInfo info)
    {
        var data = new List<T>();
        try
        {
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
        }
        catch (Exception ex)
        {
            Logger.Error(info.ToString());
            Logger.Exception(ex);
        }
        return data;
    }

    internal override async Task<T> QueryAsync<T>(CommandInfo info)
    {
        T obj = default;
        try
        {
            using (var reader = await ExecuteReaderAsync(info))
            {
                if (reader != null && reader.Read())
                {
                    obj = (T)DBUtils.ConvertTo<T>(reader);
                }
            }
            if (info.IsClose)
                conn.Close();
        }
        catch (Exception ex)
        {
            Logger.Error(info.ToString());
            Logger.Exception(ex);
        }
        return obj;
    }

    internal override async Task<List<T>> QueryListAsync<T>(CommandInfo info)
    {
        var lists = new List<T>();
        try
        {
            using (var reader = await ExecuteReaderAsync(info))
            {
                while (reader != null && reader.Read())
                {
                    var obj = DBUtils.ConvertTo<T>(reader);
                    lists.Add((T)obj);
                }
            }
            if (info.IsClose)
                conn.Close();
        }
        catch (Exception ex)
        {
            Logger.Error(info.ToString());
            Logger.Exception(ex);
        }
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

        if (item.Value is Enum)
            return item.Value.ToString();

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
        DatabaseType = databaseType;
        ConnectionString = connString;
        User = user;

        var factory = DbProviderFactories.GetFactory(databaseType.ToString());
        conn = factory.CreateConnection();
        conn.ConnectionString = connString;
    }
    #endregion
}