namespace Known.Data;

public partial class Database
{
    /// <summary>
    /// 异步执行数据库事务。
    /// </summary>
    /// <param name="name">事务操作名称。</param>
    /// <param name="action">事务操作委托。</param>
    /// <param name="data">事务操作成功返回的扩展对象。</param>
    /// <returns>操作结果。</returns>
    public async Task<Result> TransactionAsync(string name, Func<Database, Task> action, object data = null)
    {
        using (var db = CreateDatabase())
        {
            try
            {
                db.TransId = Utils.GetGuid();
                await db.BeginTransAsync();
                await action.Invoke(db);
                await db.CommitTransAsync();
                return Result.Success($"{name}成功！", data);
            }
            catch (Exception ex)
            {
                await db.RollbackTransAsync();
                HandException(null, ex);
                return Result.Error(ex.Message);
            }
            finally
            {
                db.TransId = string.Empty;
            }
        }
    }

    /// <summary>
    /// 异步开启数据库事务。
    /// </summary>
    /// <returns></returns>
    public virtual Task BeginTransAsync()
    {
        if (conn == null)
            throw new InvalidOperationException("The connection is null.");

        if (conn.State != ConnectionState.Open)
            conn.Open();
        trans = conn.BeginTransaction();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 异步提交数据库事务。
    /// </summary>
    /// <returns></returns>
    public virtual Task CommitTransAsync()
    {
        trans?.Commit();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 异步回滚数据库事务。
    /// </summary>
    /// <returns></returns>
    public virtual Task RollbackTransAsync()
    {
        trans?.Rollback();
        return Task.CompletedTask;
    }
}