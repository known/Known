using System.Collections.Generic;
using Known.Core.Entities;
using Known.Mapping;

namespace Known.Core
{
    class ModuleService : CoreServiceBase<IModuleRepository>
    {
        public ModuleService(Context context) : base(context)
        {
        }

        #region View
        public PagingResult QueryModules(PagingCriteria criteria)
        {
            return Repository.QueryModules(criteria);
        }

        public Result DeleteModules(string[] ids)
        {
            var message = CheckEntities(ids, out List<Module> modules, (e, errs) =>
            {
                if (Repository.ExistsChildren(e.Id))
                    errs.Add($"{e.Name}存在子模块，不能删除！");
            });

            if (!string.IsNullOrWhiteSpace(message))
                return Result.Error(message);

            var info = Repository.Transaction("删除", rep =>
            {
                foreach (var item in modules)
                {
                    //rep.DeleteModuleFunctions(item.Id);
                    rep.Delete(item);
                }
            });

            if (info.IsValid)
            {
                ResortModules(modules[0].ParentId);
            }

            return info;
        }

        public Result MoveModule(string id, string direct)
        {
            var module = Repository.QueryById<Module>(id);
            if (module == null)
                return Result.Error("模块不存在！");

            var sort = direct == "up" ? module.Sort - 1 : module.Sort + 1;
            var module1 = Repository.GetModule(module.ParentId, sort);
            if (module1 == null)
                return Result.Error("不能移动！");
            
            return Repository.Transaction("移动", rep =>
            {
                var moduleSort = module.Sort;
                module.Sort = module1.Sort;
                rep.Save(module);

                module1.Sort = moduleSort;
                rep.Save(module1);
            });
        }
        #endregion

        #region Form
        public Module GetModule(string id)
        {
            return Repository.QueryById<Module>(id);
        }

        public Result SaveModule(dynamic model)
        {
            if (model == null)
                return Result.Error("不能提交空数据！");

            var entity = GetEntityById((string)model.Id, new Module());
            EntityHelper.FillModel(entity, model);

            if (string.IsNullOrWhiteSpace(entity.AppId))
                entity.AppId = Setting.Instance.AppId;

            var vr = EntityHelper.Validate(entity);
            if (vr.HasError)
                return Result.Error(vr.ErrorMessage);

            Repository.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }
        #endregion

        #region Private
        private void ResortModules(string parentId)
        {
            var modules = Repository.GetModules(parentId);
            if (modules == null || modules.Count == 0)
                return;

            Repository.Transaction("排序", rep =>
            {
                var index = 0;
                foreach (var item in modules)
                {
                    item.Sort = ++index;
                    rep.Save(item);
                }
            });
        }
        #endregion
    }
}
