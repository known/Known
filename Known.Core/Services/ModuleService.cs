namespace Known.Core.Services;

class ModuleService : BaseService
{
    internal ModuleService(Context context) : base(context) { }

    internal List<SysModule> GetModules() => Database.QueryList<SysModule>();

    internal Result DeleteModules(List<SysModule> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        foreach (var model in models)
        {
            if (ModuleRepository.ExistsChild(Database, model.Id))
                return Result.Error("存在子模块，不能删除！");
        }

        return Database.Transaction(Language.Delete, db =>
        {
            foreach (var item in models)
            {
                db.Delete(item);
            }
        });
    }

    internal Result CopyModules(List<SysModule> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return Database.Transaction(Language.Copy, db =>
        {
            foreach (var item in models)
            {
                db.Insert(item);
            }
        });
    }

    internal Result MoveModules(List<SysModule> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return Database.Transaction(Language.Save, db =>
        {
            foreach (var item in models)
            {
                db.Save(item);
            }
        });
    }

    internal Result MoveModule(SysModule model)
    {
        if (model == null)
            return Result.Error(Language.SelectOne);

        return Database.Transaction(Language.Save, db =>
        {
            var sort = model.IsMoveUp ? model.Sort - 1 : model.Sort + 1;
            var module = ModuleRepository.GetModule(db, model.ParentId, sort);
            if (module != null)
            {
                module.Sort = model.Sort;
                db.Save(module);

                if (model.IsMoveUp)
                    model.Sort--;
                else
                    model.Sort++;
                db.Save(model);
            }
        });
    }

    internal Result SaveModule(dynamic model)
    {
        var entity = Database.QueryById<SysModule>((string)model.Id);
        entity ??= new SysModule();
        entity.FillModel(model);
        var vr = entity.Validate();
        if (!vr.IsValid)
            return vr;

        return Database.Transaction(Language.Save, db =>
        {
            db.Save(entity);
        }, entity);
    }
}