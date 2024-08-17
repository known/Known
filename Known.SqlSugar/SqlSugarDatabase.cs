namespace Known.SqlSugar;

class SqlSugarDatabase : Database
{
    private readonly SqlSugarScope sugar;

    public SqlSugarDatabase()
    {
        sugar = SqlSugarFactory.CreateSugar();
        var config = sugar.CurrentConnectionConfig;
        DatabaseType = SqlSugarHelper.GetDatabaseType(config.DbType);
        ConnectionString = config.ConnectionString;
    }

    public override Task OpenAsync()
    {
        sugar.Open();
        return Task.CompletedTask;
    }

    public override Task CloseAsync()
    {
        sugar.Close();
        return Task.CompletedTask;
    }

    public override async Task<Result> TransactionAsync(string name, Func<Database, Task> action, object data = null)
    {
        using (var db = new SqlSugarDatabase())
        {
            db.Context = Context;
            db.User = User;
            try
            {
                await sugar.BeginTranAsync();
                await action.Invoke(db);
                await sugar.CommitTranAsync();
                return Result.Success(Context?.Language.Success(name), data);
            }
            catch (Exception ex)
            {
                await sugar.RollbackTranAsync();
                Logger.Exception(ex);
                if (ex is SystemException)
                    return Result.Error(ex.Message);
                else
                    return Result.Error(Context?.Language["Tip.TransError"]);
            }
        }
    }

    public override async Task<PagingResult<T>> QueryPageAsync<T>(string sql, PagingCriteria criteria)
    {
        try
        {
            DBUtils.SetAutoQuery<T>(this, ref sql, criteria);
            if (criteria.ExportMode != ExportMode.None && criteria.ExportMode != ExportMode.Page)
                criteria.PageIndex = -1;

            await OpenAsync();

            byte[] exportData = null;
            Dictionary<string, object> sums = null;
            var watch = Stopwatcher.Start<T>();
            var pageData = new List<T>();
            var parameters = SqlSugarHelper.GetSugarParameters(sql, criteria, User);
            var countSql = $"select count(*) from ({sql}) t";
            var total = await sugar.Ado.GetIntAsync(countSql, parameters);
            watch.Watch("Total");
            if (total > 0)
            {
                watch.Watch("Paging");
                var query = sugar.SqlQueryable<T>(sql).QueryBuilder;
                query.Take = criteria.PageSize;
                query.Skip = (criteria.PageIndex - 1) * criteria.PageSize;
                var pageSql = query.ToSqlString();
                pageData = await sugar.QueryListAsync<T>(pageSql, parameters);
                watch.Watch("Convert");
                if (criteria.ExportMode == ExportMode.None)
                {
                    if (criteria.SumColumns != null && criteria.SumColumns.Count > 0)
                    {
                        watch.Watch("Suming");
                        var columns = string.Join(",", criteria.SumColumns.Select(c => $"sum({c}) as {c}"));
                        var sumSql = $"select {columns} from ({sql}) t";
                        sums = await sugar.QueryAsync<Dictionary<string, object>>(sumSql, parameters);
                        watch.Watch("Sum");
                    }
                }
            }

            await CloseAsync();

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

    public override Task<PagingResult<T>> QueryPageAsync<T>(PagingCriteria criteria)
    {
        var columns = new Dictionary<string, object> { [nameof(EntityBase.CompNo)] = User.CompNo };
        var sql = sugar.Queryable<T>().WhereColumns(columns).ToSqlString();
        return QueryPageAsync<T>(sql, criteria);
    }

    public override Task<T> QueryAsync<T>(string sql, object param = null)
    {
        return sugar.QueryAsync<T>(sql, param);
    }

    public override Task<T> QueryAsync<T>(Expression<Func<T, bool>> expression)
    {
        var sql = sugar.Queryable<T>().Where(expression).ToSql();
        return sugar.QueryAsync<T>(sql.Key, sql.Value);
    }

    public override Task<List<T>> QueryListAsync<T>()
    {
        var sql = sugar.Queryable<T>().ToSql();
        return sugar.QueryListAsync<T>(sql.Key, sql.Value);
    }

    public override Task<List<T>> QueryListAsync<T>(string sql, object param = null)
    {
        return sugar.QueryListAsync<T>(sql, param);
    }

    public override Task<List<T>> QueryListAsync<T>(Expression<Func<T, bool>> expression)
    {
        var sql = sugar.Queryable<T>().Where(expression).ToSql();
        return sugar.QueryListAsync<T>(sql.Key, sql.Value);
    }

    public override Task<List<T>> QueryListByIdAsync<T>(string[] ids)
    {
        var sql = sugar.Queryable<T>().In(ids).ToSql();
        return sugar.QueryListAsync<T>(sql.Key, sql.Value);
    }

    public override Task<System.Data.DataTable> QueryTableAsync(string sql, object param = null)
    {
        return sugar.Ado.GetDataTableAsync(sql, param);
    }

    public override Task<int> ExecuteAsync(string sql, object param = null)
    {
        return sugar.Ado.ExecuteCommandAsync(sql, param);
    }

    public override async Task<T> ScalarAsync<T>(string sql, object param = null)
    {
        var scalar = await sugar.Ado.GetScalarAsync(sql, param);
        return Utils.ConvertTo<T>(scalar);
    }

    public override async Task<List<T>> ScalarsAsync<T>(string sql, object param = null)
    {
        var data = new List<T>();
        using (var reader = await sugar.Ado.GetDataReaderAsync(sql, param))
        {
            while (reader.Read())
            {
                var obj = Utils.ConvertTo<T>(reader[0]);
                data.Add(obj);
            }
        }
        return data;
    }

    public override Task<int> CountAsync<T>()
    {
        return sugar.Queryable<T>().CountAsync();
    }

    public override Task<int> CountAsync<T>(Expression<Func<T, bool>> expression)
    {
        return sugar.Queryable<T>().Where(expression).CountAsync();
    }

    public override Task<int> DeleteAsync<T>(Expression<Func<T, bool>> expression)
    {
        return sugar.Deleteable<T>().Where(expression).ExecuteCommandAsync();
    }

    public override Task<int> DeleteAllAsync<T>()
    {
        return sugar.Deleteable<T>().ExecuteCommandAsync();
    }

    public override Task<int> InsertAsync<T>(T data)
    {
        return sugar.Insertable(data).ExecuteCommandAsync();
    }

    public override Task<int> InsertListAsync<T>(List<T> datas)
    {
        return sugar.Insertable(datas).ExecuteCommandAsync();
    }

    public override async Task<bool> ExistsAsync(string tableName, string id)
    {
        var lists = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object> { { nameof(EntityBase.Id), id } }
        };
        var count = await sugar.Queryable<object>().AS(tableName).WhereColumns(lists).CountAsync();
        return count > 0;
    }

    public override Task<int> DeleteAsync(string tableName, string id)
    {
        var lists = new List<Dictionary<string, object>>
        {
            new Dictionary<string, object> { { nameof(EntityBase.Id), id } }
        };
        return sugar.Deleteable<object>().AS(tableName).WhereColumns(lists).ExecuteCommandAsync();
    }

    public override Task<int> InsertAsync(string tableName, Dictionary<string, object> data)
    {
        return sugar.Insertable(data).AS(tableName).ExecuteCommandAsync();
    }

    public override Task<int> UpdateAsync(string tableName, string keyField, Dictionary<string, object> data)
    {
        return sugar.Updateable(data).AS(tableName).WhereColumns(keyField.Split(',')).ExecuteCommandAsync();
    }

    protected override Task<int> SaveDataAsync<T>(T entity)
    {
        if (entity.IsNew)
            return sugar.Insertable(entity).ExecuteCommandAsync();
        else
            return sugar.Updateable(entity).ExecuteCommandAsync();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        sugar.Dispose();
    }
}