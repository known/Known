namespace Known.Data;

/// <summary>
/// 数据库访问类。
/// </summary>
public class Database : IDisposable
{
    private static readonly Dictionary<string, Type> dbTypes = [];

    /// <summary>
    /// 取得或设置框架数据依赖接口实现类的类型。
    /// </summary>
    public static Type RepositoryType { get; set; }

    /// <summary>
    /// 注册第三方数据库访问实现类型。
    /// </summary>
    /// <param name="type">第三方数据库访问实现类型。</param>
    /// <param name="name">数据连接名称，默认为Default。</param>
    public static void Register(Type type, string name = "Default") => dbTypes[name] = type;

    /// <summary>
    /// 创建数据库访问实例。
    /// </summary>
    /// <param name="name">数据库连接名。</param>
    /// <returns>数据库访问实例。</returns>
    /// <exception cref="SystemException">数据库访问实现类不支持。</exception>
    public static Database Create(string name = "Default")
    {
        if (!dbTypes.TryGetValue(name, out Type type))
            return new Database(name);

        if (Activator.CreateInstance(type) is not Database instance)
            throw new SystemException($"The {type} is not implement Database");

        return instance;
    }

    internal static IDataRepository CreateRepository()
    {
        if (RepositoryType == null)
            return new DataRepository();

        if (Activator.CreateInstance(RepositoryType) is not IDataRepository instance)
            throw new SystemException($"The {RepositoryType} is not implement IDatabase");

        return instance;
    }

    /// <summary>
    /// 数据库连接对象。
    /// </summary>
    protected IDbConnection conn;
    private IDbTransaction trans;
    private string TransId { get; set; }

    /// <summary>
    /// 构造函数，创建一个数据库访问实例。
    /// </summary>
    public Database() : this("Default") { }

    /// <summary>
    /// 构造函数，创建一个数据库访问实例。
    /// </summary>
    /// <param name="context">系统上下文对象。</param>
    public Database(Context context) : this()
    {
        Context = context;
        User = context?.CurrentUser;
    }

    /// <summary>
    /// 构造函数，创建一个数据库访问实例。
    /// </summary>
    /// <param name="connName">数据库连接名。</param>
    /// <param name="user">当前操作用户信息。</param>
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

    /// <summary>
    /// 取得或设置数据库类型。
    /// </summary>
    public DatabaseType DatabaseType { get; set; }

    /// <summary>
    /// 取得或设置数据库连接字符串。
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// 取得或设置系统上下文对象。
    /// </summary>
    public Context Context { get; set; }

    /// <summary>
    /// 取得或设置当前操作用户信息。
    /// </summary>
    public UserInfo User { get; set; }

    /// <summary>
    /// 取得当前操作用户账号。
    /// </summary>
    public string UserName => User?.UserName;

    private DbProvider provider;
    internal DbProvider Provider
    {
        get
        {
            provider ??= DbProvider.Create(DatabaseType);
            return provider;
        }
    }

    internal bool EnableLog { get; set; } = true;

    /// <summary>
    /// 异步打开数据库。
    /// </summary>
    /// <returns></returns>
    public virtual Task OpenAsync()
    {
        if (conn != null && conn.State != ConnectionState.Open)
            conn.Open();

        return Task.CompletedTask;
    }

    /// <summary>
    /// 异步关闭数据库。
    /// </summary>
    /// <returns></returns>
    public virtual Task CloseAsync()
    {
        if (conn != null && conn.State != ConnectionState.Closed)
            conn.Close();

        conn.Dispose();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 异步执行数据库事务。
    /// </summary>
    /// <param name="name">事务操作名称。</param>
    /// <param name="action">事务操作委托。</param>
    /// <param name="data">事务操作成功返回的扩展对象。</param>
    /// <returns>操作结果。</returns>
    public virtual async Task<Result> TransactionAsync(string name, Func<Database, Task> action, object data = null)
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

    /// <summary>
    /// 异步查询分页数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页查询结果。</returns>
    public virtual async Task<PagingResult<T>> QueryPageAsync<T>(string sql, PagingCriteria criteria) where T : class, new()
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

    /// <summary>
    /// 异步查询分页数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页查询结果。</returns>
    public virtual Task<PagingResult<T>> QueryPageAsync<T>(PagingCriteria criteria) where T : class, new()
    {
        var tableName = Provider.GetTableName<T>();
        var compNo = nameof(EntityBase.CompNo);
        var compName = Provider.FormatName(compNo);
        var sql = $"select * from {tableName} where {compName}=@{compNo}";
        return QueryPageAsync<T>(sql, criteria);
    }

    /// <summary>
    /// 异步查询单条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>单条数据。</returns>
    public virtual Task<T> QueryAsync<T>(string sql, object param = null)
    {
        var info = Provider.GetCommand(sql, param);
        return QueryAsync<T>(info);
    }

    /// <summary>
    /// 异步查询单条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="expression">查询表达式。</param>
    /// <returns>单条数据。</returns>
    public virtual Task<T> QueryAsync<T>(Expression<Func<T, bool>> expression) where T : class, new()
    {
        var info = Provider.GetSelectCommand(expression);
        return QueryAsync<T>(info);
    }

    /// <summary>
    /// 异步查询多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>多条数据。</returns>
    public virtual Task<List<T>> QueryListAsync<T>() where T : class, new()
    {
        var info = Provider.GetSelectCommand<T>();
        return QueryListAsync<T>(info);
    }

    /// <summary>
    /// 异步查询多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>多条数据。</returns>
    public virtual Task<List<T>> QueryListAsync<T>(string sql, object param = null)
    {
        var info = Provider.GetCommand(sql, param);
        return QueryListAsync<T>(info);
    }

    /// <summary>
    /// 异步查询多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="expression">查询表达式。</param>
    /// <returns>多条数据。</returns>
    public virtual Task<List<T>> QueryListAsync<T>(Expression<Func<T, bool>> expression) where T : class, new()
    {
        var info = Provider.GetSelectCommand(expression);
        return QueryListAsync<T>(info);
    }

    /// <summary>
    /// 异步查询多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="ids">实体ID集合。</param>
    /// <returns>多条数据。</returns>
    public virtual Task<List<T>> QueryListByIdAsync<T>(string[] ids)
    {
        if (ids == null || ids.Length == 0)
            return null;

        var info = Provider.GetSelectCommand<T>(ids);
        return QueryListAsync<T>(info);
    }

    /// <summary>
    /// 异步查询DataTable。
    /// </summary>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>DataTable。</returns>
    public virtual async Task<DataTable> QueryTableAsync(string sql, object param = null)
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

    /// <summary>
    /// 异步执行SQL语句。
    /// </summary>
    /// <param name="sql">SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> ExecuteAsync(string sql, object param = null)
    {
        var info = Provider.GetCommand(sql, param);
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 异步查询标量值。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>标量值。</returns>
    public virtual Task<T> ScalarAsync<T>(string sql, object param = null)
    {
        var info = Provider.GetCommand(sql, param);
        return ScalarAsync<T>(info);
    }

    /// <summary>
    /// 异步查询标量值列表。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>标量值列表。</returns>
    public virtual Task<List<T>> ScalarsAsync<T>(string sql, object param = null)
    {
        var info = Provider.GetCommand(sql, param);
        return ScalarsAsync<T>(info);
    }

    /// <summary>
    /// 异步查询表数据量。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>表数据量。</returns>
    public virtual Task<int> CountAsync<T>() where T : class, new()
    {
        var info = Provider.GetCountCommand<T>();
        return ScalarAsync<int>(info);
    }

    /// <summary>
    /// 异步查询表数据量。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="expression">查询表达式。</param>
    /// <returns>表数据量。</returns>
    public virtual Task<int> CountAsync<T>(Expression<Func<T, bool>> expression) where T : class, new()
    {
        var info = Provider.GetCountCommand(expression);
        return ScalarAsync<int>(info);
    }

    /// <summary>
    /// 异步删除数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="expression">查询表达式。</param>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> DeleteAsync<T>(Expression<Func<T, bool>> expression) where T : class, new()
    {
        var info = Provider.GetDeleteCommand(expression);
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 异步删除全部数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> DeleteAllAsync<T>() where T : class, new()
    {
        var info = Provider.GetDeleteCommand<T>();
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 异步插入单条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="data">数据对象。</param>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> InsertAsync<T>(T data) where T : class, new()
    {
        if (data == null)
            return Task.FromResult(0);

        var info = Provider.GetInsertCommand(data);
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 异步插入多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="datas">数据对象列表。</param>
    /// <returns>影响的行数。</returns>
    public virtual async Task<int> InsertListAsync<T>(List<T> datas) where T : class, new()
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

    /// <summary>
    /// 异步插入DataTable。
    /// </summary>
    /// <param name="data">DataTable对象。</param>
    /// <returns>影响的行数。</returns>
    public virtual async Task<int> InsertTableAsync(DataTable data)
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

    /// <summary>
    /// 异步查询分页数据。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页查询结果。</returns>
    public virtual Task<PagingResult<Dictionary<string, object>>> QueryPageAsync(string tableName, PagingCriteria criteria)
    {
        var sql = Provider.GetSelectSql(tableName);
        return QueryPageAsync<Dictionary<string, object>>(sql, criteria);
    }

    /// <summary>
    /// 异步判断表中是否存在ID。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="id">ID字段值。</param>
    /// <returns>是否存在。</returns>
    public virtual async Task<bool> ExistsAsync(string tableName, string id)
    {
        var info = Provider.GetCountCommand(tableName, id);
        var count = await ScalarAsync<int>(info);
        return count > 0;
    }

    /// <summary>
    /// 异步删除数据。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="id">ID字段值。</param>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> DeleteAsync(string tableName, string id)
    {
        if (string.IsNullOrEmpty(id))
            return Task.FromResult(0);

        var info = Provider.GetDeleteCommand(tableName, id);
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 异步插入字典对象。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="data">字典对象。</param>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> InsertAsync(string tableName, Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            return Task.FromResult(0);

        var info = Provider.GetInsertCommand(tableName, data);
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 异步修改字典对象。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="keyField">主键字段字符串，多个字段用逗号分割。</param>
    /// <param name="data">字典对象。</param>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> UpdateAsync(string tableName, string keyField, Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            return Task.FromResult(0);

        var info = Provider.GetUpdateCommand(tableName, keyField, data);
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 异步保存字典对象。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="data">字典对象。</param>
    /// <returns>影响的行数。</returns>
    public virtual async Task<int> SaveAsync(string tableName, Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            return 0;

        var id = data.GetValue<string>(nameof(EntityBase.Id));
        if (await ExistsAsync(tableName, id))
        {
            var version = data.GetValue<int>(nameof(EntityBase.Version)) + 1;
            DataHelper.SetValue(data, nameof(EntityBase.Version), version);
            return await UpdateAsync(tableName, nameof(EntityBase.Id), data);
        }

        if (string.IsNullOrWhiteSpace(id))
            DataHelper.SetValue(data, nameof(EntityBase.Id), Utils.GetGuid());
        DataHelper.SetValue(data, nameof(EntityBase.CreateBy), User.UserName);
        DataHelper.SetValue(data, nameof(EntityBase.CreateTime), DateTime.Now);
        DataHelper.SetValue(data, nameof(EntityBase.ModifyBy), User.UserName);
        DataHelper.SetValue(data, nameof(EntityBase.ModifyTime), DateTime.Now);
        DataHelper.SetValue(data, nameof(EntityBase.Version), 1);
        DataHelper.SetValue(data, nameof(EntityBase.AppId), User.AppId);
        DataHelper.SetValue(data, nameof(EntityBase.CompNo), User.CompNo);
        return await InsertAsync(tableName, data);
    }

    /// <summary>
    /// 异步保存实体对象。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="entity">实体对象。</param>
    /// <returns></returns>
    /// <exception cref="SystemException">操作用户不能为空。</exception>
    public virtual async Task SaveAsync<T>(T entity) where T : EntityBase, new()
    {
        if (entity == null)
            return;

        var none = "Anonymous";
        if (entity.IsNew)
        {
            if (entity.CreateBy == "temp")
                entity.CreateBy = User?.UserName ?? none;
            entity.CreateTime = DateTime.Now;
            if (entity.AppId == "temp")
                entity.AppId = User?.AppId ?? Config.App.Id;
            if (entity.CompNo == "temp")
                entity.CompNo = User?.CompNo ?? none;
        }
        else
        {
            entity.Version += 1;
            await SetOriginalAsync(entity);
        }

        entity.ModifyBy = User?.UserName ?? none;
        entity.ModifyTime = DateTime.Now;
        await SaveDataAsync(entity);
        entity.IsNew = false;
    }

    /// <summary>
    /// 异步保存多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="entities">对象列表。</param>
    /// <returns></returns>
    public virtual async Task SaveDatasAsync<T>(List<T> entities) where T : EntityBase, new()
    {
        if (entities == null || entities.Count == 0)
            return;

        foreach (var entity in entities)
        {
            await SaveAsync(entity);
        }
    }

    /// <summary>
    /// 异步插入实体对象。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="entity">实体对象。</param>
    /// <param name="newId">是否重新生成新ID。</param>
    /// <returns>实体对象。</returns>
    public virtual async Task<T> InsertAsync<T>(T entity, bool newId) where T : EntityBase, new()
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

    /// <summary>
    /// 异步判断是否存在数据表。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>是否存在。</returns>
    public virtual async Task<bool> ExistsAsync<T>()
    {
        try
        {
            var tableName = Provider.GetTableName<T>();
            var sql = $"select count(*) from {tableName}";
            var value = await ScalarAsync<int?>(sql);
            if (value == null)
                return false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 异步判断是否存在数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="expression">查询表达式。</param>
    /// <returns>是否存在。</returns>
    public virtual async Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> expression) where T : class, new()
    {
        var count = await CountAsync(expression);
        return count > 0;
    }

    /// <summary>
    /// 异步删除实体数据。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="entity">实体对象。</param>
    /// <returns>影响的行数。</returns>
    public virtual Task<int> DeleteAsync<T>(T entity) where T : EntityBase, new()
    {
        return DeleteAsync<T>(entity?.Id);
    }

    /// <summary>
    /// 异步删除实体数据。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="id">ID字段值。</param>
    /// <returns>影响的行数。</returns>
    public Task<int> DeleteAsync<T>(string id) where T : EntityBase, new()
    {
        if (string.IsNullOrWhiteSpace(id))
            return Task.FromResult(0);

        return DeleteAsync<T>(d => d.Id == id);
    }

    /// <summary>
    /// 异步查询实体对象。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="id">ID字段值。</param>
    /// <returns>实体对象。</returns>
    public virtual Task<T> QueryByIdAsync<T>(string id) where T : EntityBase, new()
    {
        if (string.IsNullOrEmpty(id))
            return default;

        return QueryAsync<T>(d => d.Id == id);
    }

    /// <summary>
    /// 释放数据库访问对象。
    /// </summary>
    public void Dispose() => Dispose(true);

    /// <summary>
    /// 获取日期字段转换SQL语句，如Oracle的to_date函数。
    /// </summary>
    /// <param name="name">字段名。</param>
    /// <param name="withTime">是否带时间，默认是。</param>
    /// <returns>日期字段转换SQL语句。</returns>
    public virtual string GetDateSql(string name, bool withTime = true) => Provider?.GetDateSql(name, withTime);

    /// <summary>
    /// 异步保存实体对象。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="entity">实体对象。</param>
    /// <returns>影响的行数。</returns>
    protected virtual Task<int> SaveDataAsync<T>(T entity) where T : EntityBase, new()
    {
        var info = Provider.GetSaveCommand(entity);
        info.IsSave = true;
        return ExecuteNonQueryAsync(info);
    }

    /// <summary>
    /// 释放数据库访问对象。
    /// </summary>
    /// <param name="isDisposing">是否释放。</param>
    protected virtual void Dispose(bool isDisposing)
    {
        trans?.Dispose();
        trans = null;

        if (conn.State != ConnectionState.Closed)
            conn.Close();
        conn.Dispose();
    }

    internal string FormatName(string name) => Provider?.FormatName(name);

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

    private async Task<int> ExecuteNonQueryAsync(CommandInfo info)
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
            HandException(info, ex);
            throw new SystemException(ex.Message, ex);
        }
    }

    private async Task<IDataReader> ExecuteReaderAsync(CommandInfo info)
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
            HandException(info, ex);
            return null;
        }
    }

    internal async Task<T> ScalarAsync<T>(CommandInfo info)
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
            HandException(info, ex);
            return default;
        }
    }

    private async Task<List<T>> ScalarsAsync<T>(CommandInfo info)
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
            HandException(info, ex);
        }
        return data;
    }

    internal async Task<T> QueryAsync<T>(CommandInfo info)
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
            HandException(info, ex);
        }
        return obj;
    }

    internal async Task<List<T>> QueryListAsync<T>(CommandInfo info)
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
            HandException(info, ex);
        }
        return lists;
    }

    private async Task SetOriginalAsync<T>(T entity) where T : EntityBase, new()
    {
        var info = Provider.GetSelectCommand<T>(entity.Id);
        var original = await QueryAsync<Dictionary<string, object>>(info);
        entity.SetOriginal(original);
    }

    private void HandException(CommandInfo info, Exception ex)
    {
        if (!EnableLog)
            return;

        Logger.Error(info.ToString());
        Logger.Exception(ex);
    }

    private Task<IDbCommand> PrepareCommandAsync(IDbConnection conn, IDbTransaction trans, CommandInfo info)
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
}