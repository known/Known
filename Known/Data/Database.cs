namespace Known.Data;

/// <summary>
/// 数据库访问抽象类。
/// </summary>
public abstract class Database : IDisposable
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
            return new DefaultDatabase(name);

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

    /// <summary>
    /// 异步打开数据库。
    /// </summary>
    /// <returns></returns>
    public abstract Task OpenAsync();

    /// <summary>
    /// 异步关闭数据库。
    /// </summary>
    /// <returns></returns>
    public abstract Task CloseAsync();

    /// <summary>
    /// 异步执行数据库事务。
    /// </summary>
    /// <param name="name">事务操作名称。</param>
    /// <param name="action">事务操作委托。</param>
    /// <param name="data">事务操作成功返回的扩展对象。</param>
    /// <returns>操作结果。</returns>
    public abstract Task<Result> TransactionAsync(string name, Func<Database, Task> action, object data = null);

    /// <summary>
    /// 异步查询分页数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页查询结果。</returns>
    public abstract Task<PagingResult<T>> QueryPageAsync<T>(string sql, PagingCriteria criteria) where T : class, new();

    /// <summary>
    /// 异步查询分页数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页查询结果。</returns>
    public abstract Task<PagingResult<T>> QueryPageAsync<T>(PagingCriteria criteria) where T : class, new();

    /// <summary>
    /// 异步查询单条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>单条数据。</returns>
    public abstract Task<T> QueryAsync<T>(string sql, object param = null);

    /// <summary>
    /// 异步查询单条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="expression">查询表达式。</param>
    /// <returns>单条数据。</returns>
    public abstract Task<T> QueryAsync<T>(Expression<Func<T, bool>> expression) where T : class, new();

    /// <summary>
    /// 异步查询多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>多条数据。</returns>
    public abstract Task<List<T>> QueryListAsync<T>() where T : class, new();

    /// <summary>
    /// 异步查询多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>多条数据。</returns>
    public abstract Task<List<T>> QueryListAsync<T>(string sql, object param = null);

    /// <summary>
    /// 异步查询多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="expression">查询表达式。</param>
    /// <returns>多条数据。</returns>
    public abstract Task<List<T>> QueryListAsync<T>(Expression<Func<T, bool>> expression) where T : class, new();

    /// <summary>
    /// 异步查询多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="ids">实体ID集合。</param>
    /// <returns>多条数据。</returns>
    public abstract Task<List<T>> QueryListByIdAsync<T>(string[] ids);

    /// <summary>
    /// 异步查询DataTable。
    /// </summary>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>DataTable。</returns>
    public abstract Task<DataTable> QueryTableAsync(string sql, object param = null);

    /// <summary>
    /// 异步执行SQL语句。
    /// </summary>
    /// <param name="sql">SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>影响的行数。</returns>
    public abstract Task<int> ExecuteAsync(string sql, object param = null);

    /// <summary>
    /// 异步查询标量值。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>标量值。</returns>
    public abstract Task<T> ScalarAsync<T>(string sql, object param = null);

    /// <summary>
    /// 异步查询标量值列表。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="sql">查询SQL语句。</param>
    /// <param name="param">查询参数。</param>
    /// <returns>标量值列表。</returns>
    public abstract Task<List<T>> ScalarsAsync<T>(string sql, object param = null);

    /// <summary>
    /// 异步查询表数据量。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>表数据量。</returns>
    public abstract Task<int> CountAsync<T>() where T : class, new();

    /// <summary>
    /// 异步查询表数据量。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="expression">查询表达式。</param>
    /// <returns>表数据量。</returns>
    public abstract Task<int> CountAsync<T>(Expression<Func<T, bool>> expression) where T : class, new();

    /// <summary>
    /// 异步删除数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="expression">查询表达式。</param>
    /// <returns>影响的行数。</returns>
    public abstract Task<int> DeleteAsync<T>(Expression<Func<T, bool>> expression) where T : class, new();

    /// <summary>
    /// 异步删除全部数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>影响的行数。</returns>
    public abstract Task<int> DeleteAllAsync<T>() where T : class, new();

    /// <summary>
    /// 异步插入单条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="data">数据对象。</param>
    /// <returns>影响的行数。</returns>
    public abstract Task<int> InsertAsync<T>(T data) where T : class, new();

    /// <summary>
    /// 异步插入多条数据。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="datas">数据对象列表。</param>
    /// <returns>影响的行数。</returns>
    public abstract Task<int> InsertListAsync<T>(List<T> datas) where T : class, new();

    /// <summary>
    /// 异步插入DataTable。
    /// </summary>
    /// <param name="data">DataTable对象。</param>
    /// <returns>影响的行数。</returns>
    public abstract Task<int> InsertTableAsync(DataTable data);

    /// <summary>
    /// 异步查询分页数据。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页查询结果。</returns>
    public abstract Task<PagingResult<Dictionary<string, object>>> QueryPageAsync(string tableName, PagingCriteria criteria);

    /// <summary>
    /// 异步判断表中是否存在ID。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="id">ID字段值。</param>
    /// <returns>是否存在。</returns>
    public abstract Task<bool> ExistsAsync(string tableName, string id);

    /// <summary>
    /// 异步删除数据。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="id">ID字段值。</param>
    /// <returns>影响的行数。</returns>
    public abstract Task<int> DeleteAsync(string tableName, string id);

    /// <summary>
    /// 异步插入字典对象。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="data">字典对象。</param>
    /// <returns>影响的行数。</returns>
    public abstract Task<int> InsertAsync(string tableName, Dictionary<string, object> data);

    /// <summary>
    /// 异步修改字典对象。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="keyField">主键字段字符串，多个字段用逗号分割。</param>
    /// <param name="data">字典对象。</param>
    /// <returns>影响的行数。</returns>
    public abstract Task<int> UpdateAsync(string tableName, string keyField, Dictionary<string, object> data);

    /// <summary>
    /// 异步保存字典对象。
    /// </summary>
    /// <param name="tableName">数据库表名。</param>
    /// <param name="data">字典对象。</param>
    /// <returns>影响的行数。</returns>
    public async Task<int> SaveAsync(string tableName, Dictionary<string, object> data)
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
    /// <returns>影响的行数。</returns>
    protected abstract Task<int> SaveDataAsync<T>(T entity) where T : EntityBase, new();

    /// <summary>
    /// 异步保存实体对象。
    /// </summary>
    /// <typeparam name="T">实体类型。</typeparam>
    /// <param name="entity">实体对象。</param>
    /// <returns></returns>
    /// <exception cref="SystemException">操作用户不能为空。</exception>
    public async Task SaveAsync<T>(T entity) where T : EntityBase, new()
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
    public async Task<T> InsertAsync<T>(T entity, bool newId) where T : EntityBase, new()
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
    public async Task<bool> ExistsAsync<T>()
    {
        try
        {
            var tableName = Provider.GetTableName<T>();
            var sql = $"select count(*) from {tableName}";
            var count = await ScalarAsync<int?>(sql);
            return count > 0;
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
    public async Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> expression) where T : class, new()
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
    public Task<int> DeleteAsync<T>(T entity) where T : EntityBase, new()
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
    public Task<T> QueryByIdAsync<T>(string id) where T : EntityBase, new()
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
    /// 释放数据库访问对象。
    /// </summary>
    /// <param name="isDisposing">是否释放。</param>
    protected virtual void Dispose(bool isDisposing) { }

    internal string FormatName(string name) => Provider?.FormatName(name);
    internal virtual Task SetOriginalAsync<T>(T entity) where T : EntityBase, new() => Task.CompletedTask;
    internal virtual Task<T> ScalarAsync<T>(CommandInfo info) => Task.FromResult(default(T));
    internal virtual Task<List<T>> ScalarsAsync<T>(CommandInfo info) => Task.FromResult(new List<T>());
    internal virtual Task<T> QueryAsync<T>(CommandInfo info) => Task.FromResult(default(T));
    internal virtual Task<List<T>> QueryListAsync<T>(CommandInfo info) => Task.FromResult(new List<T>());
}