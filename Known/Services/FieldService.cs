namespace Known.Services;

/// <summary>
/// 字段管理服务接口。
/// </summary>
public interface IFieldService : IService
{
    /// <summary>
    /// 异步分页查询字段信息列表。
    /// </summary>
    /// <param name="criteria">查询条件。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<FieldDataInfo>> QueryFieldsAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取字段信息列表。
    /// </summary>
    /// <returns></returns>
    Task<List<FieldDataInfo>> GetFieldsAsync();

    /// <summary>
    /// 异步从实体类中提取公共字段信息。
    /// </summary>
    /// <returns>提取结果。</returns>
    Task<Result> FetchFieldsAsync();

    /// <summary>
    /// 异步删除字段信息列表。
    /// </summary>
    /// <param name="infos">字段信息列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteFieldsAsync(List<FieldDataInfo> infos);

    /// <summary>
    /// 异步保存字段信息列表。
    /// </summary>
    /// <param name="info">字段信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveFieldAsync(FieldDataInfo info);
}

[Client]
class FieldClient(HttpClient http) : ClientBase(http), IFieldService
{
    public Task<PagingResult<FieldDataInfo>> QueryFieldsAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<FieldDataInfo>("/Field/QueryFields", criteria);
    }

    public Task<List<FieldDataInfo>> GetFieldsAsync()
    {
        return Http.GetAsync<List<FieldDataInfo>>("/Field/GetFields");
    }

    public Task<Result> FetchFieldsAsync()
    {
        return Http.PostAsync("/Field/FetchFields");
    }

    public Task<Result> DeleteFieldsAsync(List<FieldDataInfo> infos)
    {
        return Http.PostAsync("/Field/DeleteFields", infos);
    }

    public Task<Result> SaveFieldAsync(FieldDataInfo info)
    {
        return Http.PostAsync("/Field/SaveField", info);
    }
}

[WebApi, Service]
class FieldService(Context context) : SysServiceBase(context), IFieldService
{
    public Task<PagingResult<FieldDataInfo>> QueryFieldsAsync(PagingCriteria criteria)
    {
        return Database.Query<SysField>(criteria).ToPageAsync<FieldDataInfo>();
    }

    public Task<List<FieldDataInfo>> GetFieldsAsync()
    {
        return Database.Query<SysField>().ToListAsync<FieldDataInfo>();
    }

    public async Task<Result> FetchFieldsAsync()
    {
        await InitializeDataAsync(Database);
        return Result.Success("提取成功！");
    }

    public async Task<Result> DeleteFieldsAsync(List<FieldDataInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteAsync<SysField>(item.Id);
            }
        });
    }

    public async Task<Result> SaveFieldAsync(FieldDataInfo info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<SysField>(info.Id);
        model ??= new SysField();
        model.FillModel(info);

        var vr = model.Validate(Context);
        if (vr.IsValid)
        {
            if (await database.ExistsAsync<SysField>(d => d.Id != model.Id && d.Code == model.Code))
                vr.AddError($"字段[{model.Code}]已存在！");
        }
        if (!vr.IsValid)
            return vr;

        return await database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
            info.Id = model.Id;
        }, info);
    }

    internal static async Task InitializeDataAsync(Database db)
    {
        var fields = await db.QueryListAsync<SysField>();
        var newFields = new List<SysField>();
        var commonFields = GetCommonFields();
        foreach (var field in commonFields)
        {
            if (string.IsNullOrWhiteSpace(field.Name))
                continue;

            if (fields.All(f => f.Code != field.Id))
            {
                newFields.Add(new SysField
                {
                    Code = field.Id,
                    Name = field.Name,
                    Type = field.Type.ToString(),
                    Length = field.Length,
                    Required = field.Required
                });
            }
        }

        if (newFields.Count > 0)
            await db.InsertAsync(newFields);
    }

    private static List<FieldInfo> GetCommonFields()
    {
        var baseFields = TypeHelper.GetBaseFields().Select(f => f.Id).ToList();
        var fields = new List<FieldInfo>();
        DbConfig.Models.ToList().ForEach(m => fields.AddRange(m.Fields));
        var items = fields.Where(f => !baseFields.Contains(f.Id) && !string.IsNullOrWhiteSpace(f.Name))
                          .GroupBy(c => new { c.Id, c.Name })
                          .Select(g => new { g.Key, Count = g.Count() })
                          .ToList();
        var commons = items.Where(d => d.Count > 1).Select(d =>
        {
            var first = fields.FirstOrDefault(f => f.Id == d.Key.Id);
            return new FieldInfo
            {
                Id = d.Key.Id,
                Name = d.Key.Name,
                Type = first.Type,
                Length = first.Length,
                Required = first.Required
            };
        }).ToList();
        return commons;
    }
}