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
    public override Task<int> ExecuteAsync(string sql, object param = null)
    {
        var info = Builder.GetCommand(sql, param);
        return ExecuteNonQueryAsync(info);
    }

    public override Task<T> ScalarAsync<T>(string sql, object param = null)
    {
        var info = Builder.GetCommand(sql, param);
        return ScalarAsync<T>(info);
    }

    public override async Task<List<T>> ScalarsAsync<T>(string sql, object param = null)
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

    public override Task<T> QueryAsync<T>(string sql, object param = null)
    {
        var info = Builder.GetCommand(sql, param);
        return QueryAsync<T>(info);
    }

    public override Task<List<T>> QueryListAsync<T>(string sql, object param = null)
    {
        var info = Builder.GetCommand(sql, param);
        return QueryListAsync<T>(info);
    }

    public Task<List<T>> QueryListAsync<T>(int topSize, string sql, object param = null)
    {
        var info = Builder.GetTopCommand(topSize, sql, param);
        return QueryListAsync<T>(info);
    }

    public override async Task<PagingResult<T>> QueryPageAsync<T>(string sql, PagingCriteria criteria)
    {
        try
        {
            QueryHelper.SetAutoQuery<T>(ref sql, Builder, criteria);

            if (criteria.ExportMode != ExportMode.None && criteria.ExportMode != ExportMode.Page)
                criteria.PageIndex = -1;

            if (conn.State != ConnectionState.Open)
                conn.Open();

            byte[] exportData = null;
            Dictionary<string, object> sums = null;
            var watch = Stopwatcher.Start<T>();
            var pageData = new List<T>();
            var info = Builder.GetCommand(sql, criteria, User);
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
                    if (criteria.SumColumns != null && criteria.SumColumns.Count > 0)
                    {
                        cmd.CommandText = info.SumSql;
                        watch.Watch("Suming");
                        using (var reader1 = cmd.ExecuteReader())
                        {
                            if (reader1 != null && reader1.Read())
                                sums = DBUtils.GetDictionary(reader1);
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
            return new PagingResult<T>(total, pageData) { ExportData = exportData, Sums = sums };
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            Logger.Error(sql);
            return new PagingResult<T>();
        }
    }

    public async Task<PagingResult<Dictionary<string, object>>> QueryPageDictionaryAsync(string sql, PagingCriteria criteria)
    {
        try
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
                        var dic = DBUtils.GetDictionary(reader);
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
                                sums = DBUtils.GetDictionary(reader1);
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
            return new PagingResult<Dictionary<string, object>>(total, pageData) { ExportData = exportData, Sums = sums };
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            Logger.Error(sql);
            return new PagingResult<Dictionary<string, object>>();
        }
    }

    public override async Task<DataTable> QueryTableAsync(string sql, object param = null)
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
    public override Task<int> CountAsync<T>()
    {
        var info = Builder.GetCountCommand<T>();
        return ScalarAsync<int>(info);
    }

    public override Task<List<T>> QueryListAsync<T>()
    {
        var info = Builder.GetSelectCommand<T>();
        return QueryListAsync<T>(info);
    }

    public override Task<List<T>> QueryListByIdAsync<T>(string[] ids)
    {
        if (ids == null || ids.Length == 0)
            return null;

        var info = Builder.GetSelectCommand<T>(ids);
        return QueryListAsync<T>(info);
    }

    public override Task<int> DeleteAllAsync<T>()
    {
        var info = Builder.GetDeleteCommand<T>();
        return ExecuteNonQueryAsync(info);
    }

    public override Task<int> InsertAsync<T>(T data)
    {
        if (data == null)
            return Task.FromResult(0);

        var info = Builder.GetInsertCommand(data);
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
        var info = Builder.GetInsertCommand<T>();
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
        var info = Builder.GetSaveCommand(entity);
        info.IsSave = true;
        return ExecuteNonQueryAsync(info);
    }

    internal override async Task SetOriginalAsync<T>(T entity)
    {
        var info = Builder.GetSelectCommand<T>(entity.Id);
        var original = await QueryAsync<Dictionary<string, object>>(info);
        entity.SetOriginal(original);
    }
    #endregion

    #region Expression
    public override Task<PagingResult<T>> QueryPageAsync<T>(PagingCriteria criteria)
    {
        var tableName = Builder.GetTableName<T>(true);
        var compNo = nameof(EntityBase.CompNo);
        var compName = Builder.FormatName(compNo);
        var sql = $"select * from {tableName} where {compName}=@{compNo}";
        return QueryPageAsync<T>(sql, criteria);
    }

    public override Task<T> QueryAsync<T>(Expression<Func<T, bool>> expression)
    {
        var info = Builder.GetSelectCommand(expression);
        return QueryAsync<T>(info);
    }

    public override Task<List<T>> QueryListAsync<T>(Expression<Func<T, bool>> expression)
    {
        var info = Builder.GetSelectCommand(expression);
        return QueryListAsync<T>(info);
    }

    public override Task<int> CountAsync<T>(Expression<Func<T, bool>> expression)
    {
        var info = Builder.GetCountCommand(expression);
        return ScalarAsync<int>(info);
    }

    public override Task<int> DeleteAsync<T>(Expression<Func<T, bool>> expression)
    {
        var info = Builder.GetDeleteCommand(expression);
        return ExecuteNonQueryAsync(info);
    }
    #endregion

    #region Dictionary
    public override async Task<bool> ExistsAsync(string tableName, string id)
    {
        var info = Builder.GetCountCommand(tableName, id);
        var count = await ScalarAsync<int>(info);
        return count > 0;
    }

    public override Task<int> DeleteAsync(string tableName, string id)
    {
        if (string.IsNullOrEmpty(id))
            return Task.FromResult(0);

        var info = Builder.GetDeleteCommand(tableName, id);
        return ExecuteNonQueryAsync(info);
    }

    public override Task<int> InsertAsync(string tableName, Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            return Task.FromResult(0);

        var info = Builder.GetInsertCommand(tableName, data);
        return ExecuteNonQueryAsync(info);
    }

    public override Task<int> UpdateAsync(string tableName, string keyField, Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            return Task.FromResult(0);

        var info = Builder.GetUpdateCommand(tableName, keyField, data);
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

    internal override async Task<T> ScalarAsync<T>(CommandInfo info)
    {
        var cmd = await PrepareCommandAsync(conn, trans, info);
        var scalar = cmd.ExecuteScalar();
        cmd.Parameters.Clear();
        if (info.IsClose)
            conn.Close();
        return Utils.ConvertTo<T>(scalar);
    }

    internal override async Task<T> QueryAsync<T>(CommandInfo info)
    {
        T obj = default;
        using (var reader = await ExecuteReaderAsync(info))
        {
            if (reader != null && reader.Read())
            {
                obj = (T)DBUtils.ConvertTo<T>(reader);
            }
        }
        if (info.IsClose)
            conn.Close();
        return obj;
    }

    internal override async Task<List<T>> QueryListAsync<T>(CommandInfo info)
    {
        var lists = new List<T>();
        using (var reader = await ExecuteReaderAsync(info))
        {
            while (reader.Read())
            {
                var obj = DBUtils.ConvertTo<T>(reader);
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