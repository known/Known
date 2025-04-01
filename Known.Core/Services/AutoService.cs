namespace Known.Services;

[WebApi, Service]
class AutoService(Context context) : ServiceBase(context), IAutoService
{
    public async Task<PagingResult<Dictionary<string, object>>> QueryModelsAsync(PagingCriteria criteria)
    {
        var database = Database;
        var tableName = criteria.GetParameter<string>("TableName");
        if (string.IsNullOrWhiteSpace(tableName))
        {
            var pageId = criteria.GetParameter<string>(nameof(AutoInfo<object>.PageId));
            var pluginId = criteria.GetParameter<string>(nameof(AutoInfo<object>.PluginId));
            var entity = await GetEntityByModuleIdAsync(database, pageId, pluginId);
            tableName = entity?.Id;
        }

        if (string.IsNullOrWhiteSpace(tableName))
            return new PagingResult<Dictionary<string, object>>();

        criteria.SetQuery(nameof(EntityBase.CompNo), QueryType.Equal, CurrentUser?.CompNo);
        return await database.QueryPageAsync(tableName, criteria);
    }

    public async Task<Dictionary<string, object>> GetModelAsync(string pageId, string id)
    {
        var database = Database;
        var entity = await GetEntityByModuleIdAsync(database, pageId, "");
        var tableName = entity?.Id;
        if (string.IsNullOrWhiteSpace(tableName))
            return [];

        return await database.QueryByIdAsync(tableName, id);
    }

    public async Task<Result> DeleteModelsAsync(AutoInfo<List<Dictionary<string, object>>> info)
    {
        var database = Database;
        var entity = await GetEntityByModuleIdAsync(database, info.PageId, info.PluginId);
        var tableName = entity?.Id;
        if (string.IsNullOrWhiteSpace(tableName))
            return Result.Error(Language.Required("tableName"));

        if (info.Data == null || info.Data.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var oldFiles = new List<string>();
        var result = await database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in info.Data)
            {
                var id = item.GetValue<string>(nameof(EntityBase.Id));
                await db.DeleteFlowAsync(id);
                await db.DeleteFilesAsync(id, oldFiles);
                await db.DeleteAsync(tableName, id);
            }
        });
        if (result.IsValid)
            AttachFile.DeleteFiles(oldFiles);
        return result;
    }

    public async Task<Result> SaveModelAsync(UploadInfo<Dictionary<string, object>> info)
    {
        var database = Database;
        var entity = await GetEntityByModuleIdAsync(database, info.PageId, info.PluginId);
        var tableName = entity?.Id;
        if (entity == null || string.IsNullOrWhiteSpace(tableName))
            return Result.Error(Language.Required("tableName"));

        var model = info.Model;
        var vr = DataHelper.Validate(Context, entity, model);
        if (!vr.IsValid)
            return vr;

        return await database.TransactionAsync(Language.Save, async db =>
        {
            var id = model.GetValue<string>(nameof(EntityBase.Id));
            if (string.IsNullOrWhiteSpace(id))
                id = Utils.GetNextId();
            if (info.Files != null && info.Files.Count > 0)
            {
                foreach (var file in info.Files)
                {
                    var bizType = $"{tableName}.{file.Key}";
                    var files = info.Files.GetAttachFiles(CurrentUser, file.Key, tableName);
                    await db.AddFilesAsync(files, id, bizType);
                    model[file.Key] = $"{id}_{bizType}";
                }
            }
            model.SetValue(nameof(EntityBase.Id), id);
            await db.SaveAsync(tableName, model);
        }, model);
    }

    public Task<CodeConfigInfo> GetCodeConfigAsync()
    {
        var info = new CodeConfigInfo();
        if (Config.IsDebug)
        {
            var clientPath = Config.App.ContentRoot?.Replace(".Web", "");
            var serverPath = Config.App.ContentRoot;
            info.EntityPath = CoreOption.Instance.Code?.EntityPath ?? clientPath;
            info.PagePath = CoreOption.Instance.Code?.PagePath ?? clientPath;
            info.ServicePath = CoreOption.Instance.Code?.ServicePath ?? serverPath;
        }
        return Task.FromResult(info);
    }

    public async Task<Result> SaveCodeAsync(AutoInfo<string> info)
    {
        if (!Config.IsDebug)
            return Result.Error("非开发环境，不能保存代码！");

        if (string.IsNullOrWhiteSpace(info.PageId))
            return Result.Error("路径不能为空！");

        if (File.Exists(info.PageId))
            return Result.Error($"文件[{info.PageId}]已存在！");

        await Utils.SaveFileAsync(info.PageId, info.Data);
        return Result.Success("保存成功！");
    }

    public Task<Result> CreateTableAsync(AutoInfo<string> info)
    {
        var tableName = info.PageId;
        var script = info.Data;
        return Database.CreateTableAsync(tableName, script);
    }

    private static async Task<EntityInfo> GetEntityByModuleIdAsync(Database db, string moduleId, string pluginId)
    {
        var param = await db.GetAutoPageParameterAsync(moduleId, pluginId);
        if (param == null)
            return null;

        if (!string.IsNullOrWhiteSpace(param.EntityData))
            return DataHelper.ToEntity(param.EntityData);

        return new EntityInfo
        {
            Id = param.Script,
            Name = param.Name,
            Fields = [.. param.Form.Fields.Select(f => f.ToField())]
        };
    }
}