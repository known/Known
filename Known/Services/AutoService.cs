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
}