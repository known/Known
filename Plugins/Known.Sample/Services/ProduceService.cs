namespace Known.Sample.Services;

public interface IProduceService : IService
{
    Task<PagingResult<TbMaterial>> QueryMaterialsAsync(PagingCriteria criteria);
    Task<TbMaterial> GetMaterialAsync(string id);
    Task<Result> DeleteMaterialsAsync(List<TbMaterial> infos);
    Task<Result> SaveMaterialAsync(TbMaterial info);

    Task<PagingResult<TbWork>> QueryWorksAsync(PagingCriteria criteria);
    Task<TbWork> GetWorkAsync(string id);
    Task<Result> DeleteWorksAsync(List<TbWork> infos);
    Task<Result> SaveWorkAsync(TbWork info);
}

[Client]
class ProduceClient(HttpClient http) : ClientBase(http), IProduceService
{
    public Task<PagingResult<TbMaterial>> QueryMaterialsAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<TbMaterial>("/Produce/QueryMaterials", criteria);
    }

    public Task<TbMaterial> GetMaterialAsync(string id)
    {
        return Http.GetAsync<TbMaterial>($"/Produce/GetMaterial?id={id}");
    }

    public Task<Result> DeleteMaterialsAsync(List<TbMaterial> infos)
    {
        return Http.PostAsync("/Produce/DeleteMaterials", infos);
    }

    public Task<Result> SaveMaterialAsync(TbMaterial info)
    {
        return Http.PostAsync("/Produce/SaveMaterial", info);
    }

    public Task<PagingResult<TbWork>> QueryWorksAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<TbWork>("/Produce/QueryWorks", criteria);
    }

    public Task<TbWork> GetWorkAsync(string id)
    {
        return Http.GetAsync<TbWork>($"/Produce/GetWork?id={id}");
    }

    public Task<Result> DeleteWorksAsync(List<TbWork> infos)
    {
        return Http.PostAsync("/Produce/DeleteWorks", infos);
    }

    public Task<Result> SaveWorkAsync(TbWork info)
    {
        return Http.PostAsync("/Produce/SaveWork", info);
    }
}

[WebApi, Service]
class ProduceService(Context context) : ServiceBase(context), IProduceService
{
    public Task<PagingResult<TbMaterial>> QueryMaterialsAsync(PagingCriteria criteria)
    {
        return Database.QueryPageAsync<TbMaterial>(criteria);
    }

    public async Task<TbMaterial> GetMaterialAsync(string id)
    {
        var info = await Database.QueryByIdAsync<TbMaterial>(id);
        info ??= new TbMaterial();
        return info;
    }

    public async Task<Result> DeleteMaterialsAsync(List<TbMaterial> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        foreach (var model in infos)
        {
            if (await ProduceRepository.ExistsWorkMaterialAsync(database, model.CustGNo))
                return Result.Error($"{model.CustGNo}存在工单信息，不能删除！");
        }

        return await database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteAsync<TbMaterial>(item.Id);
            }
        });
    }

    public async Task<Result> SaveMaterialAsync(TbMaterial info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<TbMaterial>(info.Id);
        model ??= new TbMaterial();
        model.FillModel(info);

        var vr = model.Validate(Context);
        if (vr.IsValid)
        {
            if (await database.ExistsAsync<TbMaterial>(d => d.Id != model.Id && d.CustGNo == model.CustGNo))
                vr.AddError($"客户料号[{model.CustGNo}]已存在！");
        }
        if (!vr.IsValid)
            return vr;

        return await database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
            info.Id = model.Id;
        }, info);
    }

    public Task<PagingResult<TbWork>> QueryWorksAsync(PagingCriteria criteria)
    {
        return Database.QueryPageAsync<TbWork>(criteria);
    }

    public async Task<TbWork> GetWorkAsync(string id)
    {
        TbWork info = null;
        await Database.QueryActionAsync(async db =>
        {
            info = await ProduceRepository.GetWorkAsync(db, id);
            info ??= new TbWork
            {
                WorkNo = await db.GetMaxWorkNoAsync(),
                Status = WorkStatus.Pending
            };
            if (info.PackInfo == null || info.PackInfo.Count == 0)
                info.SetPackInfo();
        });
        return info;
    }

    public async Task<Result> DeleteWorksAsync(List<TbWork> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        foreach (var item in infos)
        {
            if (item.Status != WorkStatus.Pending)
                return Result.Error($"{item.WorkNo}已开始生产，不能删除！");
        }

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteFlowAsync(item.Id);
                await db.DeleteAsync<TbWork>(item.Id);
            }
        });
    }

    public async Task<Result> SaveWorkAsync(TbWork info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<TbWork>(info.Id);
        model ??= new TbWork();
        model.FillModel(info);

        if (model.Status != WorkStatus.Pending)
            return Result.Error($"{model.WorkNo}已开始生产，不能编辑！");

        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        return await database.TransactionAsync(Language.Save, async db =>
        {
            if (model.IsNew)
            {
                model.WorkNo = await db.GetMaxWorkNoAsync();
                await WorkFlow.CreateAsync(db, model);
            }

            await db.SaveAsync(model);
            info.Id = model.Id;
        }, info);
    }
}