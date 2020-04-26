using System.Collections.Generic;
using Known.Core.Entities;
using Known.Core.Repositories;

namespace Known.Core.Services
{
    public class ModuleService : ServiceBase
    {
        private IModuleRepository Repository { get; } = Container.Resolve<IModuleRepository>();

        #region View
        public PagingResult<SysModule> QueryModules(PagingCriteria criteria)
        {
            return Repository.QueryModules(Database, criteria);
        }

        public Result DeleteModules(string[] ids)
        {
            var entities = Database.QueryListById<SysModule>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            foreach (var item in entities)
            {
                if (Repository.ExistsSubModule(Database, item.Id))
                    return Result.Error($"{item.Name}存在子模块，不能删除！");
            }

            return Database.Transaction("删除", db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                    Repository.DeleteModuleRights(db, item.Id);
                }
            });
        }

        public Result CopyModules(string[] ids, string mid)
        {
            var module = Database.QueryById<SysModule>(mid);
            if (module == null)
                return Result.Error("所选模块不存在！");

            var entities = Database.QueryListById<SysModule>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Database.Transaction("复制", db =>
            {
                foreach (var item in entities)
                {
                    item.ParentId = module.Id;
                    db.Insert(item);
                }
            });
        }
        #endregion

        #region Form
        public List<SysModule> GetModules()
        {
            return Repository.GetModules(Database);
        }

        public SysModule GetModule(string id)
        {
            return Database.QueryById<SysModule>(id);
        }

        public Result SaveModule(dynamic model)
        {
            var entity = Database.QueryById<SysModule>((string)model.Id);
            if (entity == null)
                entity = new SysModule();

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return vr;

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }
        #endregion
    }
}
