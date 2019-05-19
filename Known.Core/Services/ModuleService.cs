using System.Collections.Generic;
using Known.Core.Entities;
using Known.Core.Repositories;
using Known.Mapping;

namespace Known.Core.Services
{
    public class ModuleService : CoreServiceBase<IModuleRepository>
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
            var modules = Repository.QueryListById<Module>(ids);
            if (modules == null || modules.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            foreach (var item in modules)
            {
                if (Repository.ExistsChildren(item.Id))
                    return Result.Error($"{item.Name}存在子模块，不能删除！");
            }

            var info = Repository.Transaction("删除", rep =>
            {
                foreach (var item in modules)
                {
                    rep.Delete(item);
                }
            });

            if (info.IsValid)
            {
                var modules1 = Repository.GetModules(modules[0].ParentId);
                if (modules1 != null && modules1.Count > 0)
                {
                    Repository.Transaction("排序", rep =>
                    {
                        var index = 0;
                        foreach (var item in modules1)
                        {
                            item.Sort = ++index;
                            rep.Save(item);
                        }
                    });
                }
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

            var id = (string)model.Id;
            var entity = Repository.QueryById<Module>(id);
            if (entity == null)
                entity = new Module();

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
    }
}
