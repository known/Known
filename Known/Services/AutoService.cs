using Known.Extensions;

namespace Known.Services;

class AutoService : ServiceBase
{
    public Task<PagingResult<Dictionary<string, object>>> QueryModelsAsync(string tableName, PagingCriteria criteria)
    {
        var sql = $"select * from {tableName} where CompNo=@CompNo";
        return Database.QueryPageAsync<Dictionary<string, object>>(sql, criteria);
    }

    public async Task<Result> DeleteModelsAsync(string tableName, List<Dictionary<string, object>> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteAsync(tableName, item.GetValue<string>("Id"));
            }
        });
    }

    public async Task<Result> SaveModelAsync(string tableName, Dictionary<string, object> model)
    {
        return await Database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(tableName, model);
        }, model);
    }

    public async Task<Result> CreateTableAsync(string tableName, string script)
    {
        try
        {
            try
            {
                var sql = $"select count(*) from {tableName}";
                var count = await Database.ScalarAsync<int>(sql);
                if (count > 0)
                    return Result.Error(Language["Tip.TableHasData"]);

                sql = $"drop table {tableName}";
                await Database.ExecuteAsync(sql);
            }
            catch
            {
            }

            await Database.ExecuteAsync(script);
            return Result.Success(Language["Tip.ExecuteSuccess"]);
        }
        catch (Exception ex)
        {
            Logger.Error(ex.ToString());
            return Result.Error(ex.Message);
        }
    }
}