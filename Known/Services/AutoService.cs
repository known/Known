using Known.Designers;
using Known.Extensions;

namespace Known.Services;

class AutoService(Context context) : ServiceBase(context)
{
    public Task<PagingResult<Dictionary<string, object>>> QueryModelsAsync(string tableName, PagingCriteria criteria)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            return Task.FromResult(new PagingResult<Dictionary<string, object>>());

        var sql = $"select * from {tableName} where CompNo=@CompNo";
        return Database.QueryPageAsync<Dictionary<string, object>>(sql, criteria);
    }

    public async Task<Result> DeleteModelsAsync(string tableName, List<Dictionary<string, object>> models)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            return Result.Error(Language.Required("tableName"));

        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var oldFiles = new List<string>();
        var result = await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                var id = item.GetValue<string>("Id");
                await Platform.DeleteFlowAsync(db, id);
                await Platform.DeleteFilesAsync(db, id, oldFiles);
                await db.DeleteAsync(tableName, id);
            }
        });
        if (result.IsValid)
            Platform.DeleteFiles(oldFiles);
        return result;
    }

    public async Task<Result> SaveModelAsync(string tableName, UploadInfo<Dictionary<string, object>> info)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            return Result.Error(Language.Required("tableName"));

        var model = info.Model;
        var vr = DataHelper.Validate(Context, tableName, model);
        if (!vr.IsValid)
            return vr;

        var user = CurrentUser;
        return await Database.TransactionAsync(Language.Save, async db =>
        {
            var id = model.GetValue<string>("Id");
            if (string.IsNullOrWhiteSpace(id))
                id = Utils.GetGuid();
            if (info.Files != null && info.Files.Count > 0)
            {
                foreach (var file in info.Files)
                {
                    var bizType = $"{tableName}.{file.Key}";
                    var files = info.Files.GetAttachFiles(user, file.Key, tableName);
                    await Platform.AddFilesAsync(db, files, id, bizType);
                    model[file.Key] = $"{id}_{bizType}";
                }
            }
            model["Id"] = id;
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