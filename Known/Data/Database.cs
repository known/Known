namespace Known.Data;

public abstract class Database : IDisposable
{
    public DatabaseType DatabaseType { get; set; }
    public string ConnectionString { get; set; }
    public Context Context { get; set; }
    public UserInfo User { get; set; }
    public string UserName => User?.UserName;

    private SqlBuilder builder;
    internal SqlBuilder Builder
    {
        get
        {
            builder ??= SqlBuilder.Create(DatabaseType);
            return builder;
        }
    }

    public abstract Task OpenAsync();
    public abstract Task CloseAsync();
    public abstract Task<Result> TransactionAsync(string name, Func<Database, Task> action, object data = null);

    public abstract Task<PagingResult<T>> QueryPageAsync<T>(string sql, PagingCriteria criteria) where T : class, new();
    public abstract Task<PagingResult<T>> QueryPageAsync<T>(PagingCriteria criteria) where T : class, new();
    public abstract Task<T> QueryAsync<T>(string sql, object param = null);
    public abstract Task<T> QueryAsync<T>(Expression<Func<T, bool>> expression) where T : class, new();
    public abstract Task<List<T>> QueryListAsync<T>() where T : class, new();
    public abstract Task<List<T>> QueryListAsync<T>(string sql, object param = null);
    public abstract Task<List<T>> QueryListAsync<T>(Expression<Func<T, bool>> expression) where T : class, new();
    public abstract Task<List<T>> QueryListByIdAsync<T>(string[] ids);
    public abstract Task<DataTable> QueryTableAsync(string sql, object param = null);

    public abstract Task<int> ExecuteAsync(string sql, object param = null);
    public abstract Task<T> ScalarAsync<T>(string sql, object param = null);
    public abstract Task<List<T>> ScalarsAsync<T>(string sql, object param = null);
    public abstract Task<int> CountAsync<T>() where T : class, new();
    public abstract Task<int> CountAsync<T>(Expression<Func<T, bool>> expression) where T : class, new();
    public abstract Task<int> DeleteAsync<T>(Expression<Func<T, bool>> expression) where T : class, new();
    public abstract Task<int> DeleteAllAsync<T>() where T : class, new();
    public abstract Task<int> InsertAsync<T>(T data) where T : class, new();
    public abstract Task<int> InsertListAsync<T>(List<T> datas) where T : class, new();

    public abstract Task<bool> ExistsAsync(string tableName, string id);
    public abstract Task<int> DeleteAsync(string tableName, string id);
    public abstract Task<int> InsertAsync(string tableName, Dictionary<string, object> data);
    public abstract Task<int> UpdateAsync(string tableName, string keyField, Dictionary<string, object> data);

    protected abstract Task<int> SaveDataAsync<T>(T entity) where T : EntityBase, new();

    public Task<T> QueryByIdAsync<T>(string id) where T : EntityBase, new()
    {
        if (string.IsNullOrEmpty(id))
            return default;

        return QueryAsync<T>(d => d.Id == id);
    }

    public async Task<int> SaveAsync(string tableName, Dictionary<string, object> data)
    {
        if (data == null || data.Count == 0)
            return 0;

        var id = data.GetValue<string>(nameof(EntityBase.Id));
        if (await ExistsAsync(tableName, id))
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

    public async Task SaveAsync<T>(T entity) where T : EntityBase, new()
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
            await SetOriginalAsync(entity);
        }

        entity.ModifyBy = User.UserName;
        entity.ModifyTime = DateTime.Now;
        await SaveDataAsync(entity);
        entity.IsNew = false;
    }

    public virtual async Task SaveDatasAsync<T>(List<T> entities) where T : EntityBase, new()
    {
        if (entities == null || entities.Count == 0)
            return;

        foreach (var entity in entities)
        {
            await SaveAsync(entity);
        }
    }

    public async Task<T> InsertAsync<T>(T entity, bool newId = true) where T : EntityBase, new()
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

    public async Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> expression) where T : class, new()
    {
        var count = await CountAsync(expression);
        return count > 0;
    }

    public Task<int> DeleteAsync<T>(T entity) where T : EntityBase, new()
    {
        return DeleteAsync<T>(entity?.Id);
    }

    public Task<int> DeleteAsync<T>(string id) where T : EntityBase, new()
    {
        if (string.IsNullOrWhiteSpace(id))
            return Task.FromResult(0);

        return DeleteAsync<T>(d => d.Id == id);
    }

    public void Dispose() => Dispose(true);

    public virtual string GetDateSql(string name, bool withTime = true) => Builder?.GetDateSql(name, withTime);

    protected virtual void Dispose(bool isDisposing) { }

    internal virtual Task SetOriginalAsync<T>(T entity) where T : EntityBase, new() => Task.CompletedTask;
    internal virtual Task<T> ScalarAsync<T>(CommandInfo info) => Task.FromResult(default(T));
    internal virtual Task<T> QueryAsync<T>(CommandInfo info) => Task.FromResult(default(T));
    internal virtual Task<List<T>> QueryListAsync<T>(CommandInfo info) => Task.FromResult(new List<T>());
}