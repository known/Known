using Known.Entities;
using Known.Repositories;

namespace Known.Services;

class ModuleService : BaseService
{
    public Task<List<SysModule>> GetModulesAsync() => Database.QueryListAsync<SysModule>();

    public async Task<Result> DeleteModulesAsync(List<SysModule> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        foreach (var model in models)
        {
            if (await ModuleRepository.ExistsChildAsync(Database, model.Id))
                return Result.Error("存在子模块，不能删除！");
        }

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteAsync(item);
            }
        });
    }

    public async Task<Result> CopyModulesAsync(List<SysModule> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Copy, async db =>
        {
            foreach (var item in models)
            {
                await db.InsertAsync(item);
            }
        });
    }

    public async Task<Result> MoveModulesAsync(List<SysModule> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            foreach (var item in models)
            {
                await db.SaveAsync(item);
            }
        });
    }

    public async Task<Result> MoveModuleAsync(SysModule model)
    {
        if (model == null)
            return Result.Error(Language.SelectOne);

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            var sort = model.IsMoveUp ? model.Sort - 1 : model.Sort + 1;
            var module = await ModuleRepository.GetModuleAsync(db, model.ParentId, sort);
            if (module != null)
            {
                module.Sort = model.Sort;
                await db.SaveAsync(module);

                if (model.IsMoveUp)
                    model.Sort--;
                else
                    model.Sort++;
                await db.SaveAsync(model);
            }
        });
    }

    public async Task<Result> SaveModuleAsync(dynamic model)
    {
        var entity = await Database.QueryByIdAsync<SysModule>((string)model.Id);
        entity ??= new SysModule();
        entity.FillModel(model);
        var vr = entity.Validate();
        if (!vr.IsValid)
            return vr;

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(entity);
        }, entity);
    }
}