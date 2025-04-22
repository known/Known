namespace Known.Services;

[WebApi, Service]
class CodeService(Context context) : ServiceBase(context), ICodeService
{
    public async Task<List<CodeInfo>> GetDbTablesAsync()
    {
        var tables = await Database.GetTableNamesAsync();
        return tables?.Select(t => new CodeInfo(t, t)).ToList();
    }

    public async Task<List<CodeFieldInfo>> GetDbFieldsAsync(string tableName)
    {
        var baseFields = TypeHelper.GetBaseFields();
        var fields = await Database.GetTableFieldsAsync(tableName);
        return fields?.Where(d => !baseFields.Exists(f => f.Id == d.Id))
                      .Select(CodeFieldInfo.FromField)
                      .ToList();
    }

    public async Task<List<CodeModelInfo>> GetModelsAsync()
    {
        var infos = new List<CodeModelInfo>();
        var database = Database;
        var codes = await database.QueryListAsync<SysCode>();
        if (codes == null || codes.Count == 0)
        {
            var models = AppData.LoadCodeModels();
            if (models != null && models.Count > 0)
            {
                infos.AddRange(models);
                codes = models.Select(c => new SysCode
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    Data = ZipHelper.ZipDataAsString(c)
                }).ToList();
                await database.InsertAsync(codes);
            }
        }
        else
        {
            foreach (var item in codes)
            {
                var info = ZipHelper.UnZipDataFromString<CodeModelInfo>(item.Data);
                infos.Add(info);
            }
        }
        return infos;
    }

    public async Task<Result> DeleteModelsAsync(List<CodeInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteAsync<SysCode>(item.Code);
            }
        });
    }

    public async Task<Result> SaveModelAsync(CodeModelInfo info)
    {
        var db = Database;
        var model = await db.QueryByIdAsync<SysCode>(info.Id);
        model ??= new SysCode();
        model.Id = info.Id;
        model.Code = info.Code;
        model.Name = info.Name;
        model.Data = ZipHelper.ZipDataAsString(info);
        await db.SaveAsync(model);
        return Result.Success("保存成功！", info);
    }

    public async Task<Result> SaveCodeAsync(AutoInfo<string> info)
    {
        if (!Config.IsDebug)
            return Result.Error("非开发环境，不能保存代码！");

        if (string.IsNullOrWhiteSpace(info.PageId))
            return Result.Error("路径不能为空！");

        var path = GetCodePath(info.PluginId, info.PageId);
        if (File.Exists(path))
            return Result.Error($"文件[{info.PageId}]已存在！");

        await Utils.SaveFileAsync(path, info.Data);
        return Result.Success("保存成功！");
    }

    public async Task<Result> CreateTableAsync(AutoInfo<string> info)
    {
        var database = Database;
        var autoPage = await database.GetAutoPageAsync(info.PageId, info.PluginId);
        var database1 = await database.GetDatabaseAsync(autoPage);
        // autoPage为空时，为Admin插件创建表，表名取info.PageId
        return await database1.CreateTableAsync(autoPage?.Script ?? info.PageId, info.Data);
    }

    private static string GetCodePath(string name, string path)
    {
        var projectPath = Config.App.ContentRoot?.Replace(".Web", "");
        var code = CoreOption.Instance.Code ?? new CodeConfigInfo();
        if (string.IsNullOrWhiteSpace(code.ModelPath))
            code.ModelPath = projectPath;
        if (string.IsNullOrWhiteSpace(code.PagePath))
            code.PagePath = projectPath;
        if (string.IsNullOrWhiteSpace(code.FormPath))
            code.FormPath = projectPath;
        if (string.IsNullOrWhiteSpace(code.ServiceIPath))
            code.ServiceIPath = projectPath;
        if (string.IsNullOrWhiteSpace(code.EntityPath))
            code.EntityPath = Config.App.ContentRoot;
        if (string.IsNullOrWhiteSpace(code.ServicePath))
            code.ServicePath = Config.App.ContentRoot;

        return name switch
        {
            CodeTab.Info => Path.Combine(code.ModelPath, path),
            CodeTab.Entity => Path.Combine(code.EntityPath, path),
            CodeTab.Page => Path.Combine(code.PagePath, path),
            CodeTab.Form => Path.Combine(code.FormPath, path),
            CodeTab.ServiceI => Path.Combine(code.ServiceIPath, path),
            CodeTab.Service => Path.Combine(code.ServicePath, path),
            _ => ""
        };
    }
}