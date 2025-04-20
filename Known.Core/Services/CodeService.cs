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
        return fields?.Where(d => !baseFields.Exists(f => f.Id == d.Id)).Select(d => new CodeFieldInfo
        {
            Id = d.Id,
            Name = d.Name,
            Type = d.Type,
            Length = d.Length,
            Required = d.Required,
            IsKey = d.IsKey
        }).ToList();
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
}