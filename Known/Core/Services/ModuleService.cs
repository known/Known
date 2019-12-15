using System.Collections.Generic;
using Known.Core.Datas;
using Known.Core.Entities;
using Known.Mapping;

namespace Known.Core.Services
{
    class ModuleService : CoreServiceBase
    {
        public ModuleService(Context context) : base(context)
        {
        }

        #region View
        public PagingResult QueryModules(PagingCriteria criteria)
        {
            return Database.QueryModules(criteria);
        }

        public Result DeleteModules(string[] ids)
        {
            var message = CheckEntities(ids, out List<TModule> modules, (e, errs) =>
            {
                if (Database.ExistsChildren(e.Id))
                    errs.Add($"{e.Name}存在子模块，不能删除！");
            });

            if (!string.IsNullOrWhiteSpace(message))
                return Result.Error(message);

            var info = Database.Transaction("删除", db =>
            {
                foreach (var item in modules)
                {
                    //db.DeleteModuleFunctions(item.Id);
                    db.Delete(item);
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
            var module = Database.QueryById<TModule>(id);
            if (module == null)
                return Result.Error("模块不存在！");

            var sort = direct == "up" ? module.Sort - 1 : module.Sort + 1;
            var module1 = Database.GetModule(module.ParentId, sort);
            if (module1 == null)
                return Result.Error("不能移动！");
            
            return Database.Transaction("移动", db =>
            {
                var moduleSort = module.Sort;
                module.Sort = module1.Sort;
                db.Save(module);

                module1.Sort = moduleSort;
                db.Save(module1);
            });
        }
        #endregion

        #region Form
        public TModule GetModule(string id)
        {
            return Database.QueryById<TModule>(id);
        }

        public Result SaveModule(dynamic model)
        {
            if (model == null)
                return Result.Error("不能提交空数据！");

            var entity = GetEntityById((string)model.Id, new TModule());
            EntityHelper.FillModel(entity, model);

            if (string.IsNullOrWhiteSpace(entity.AppId))
                entity.AppId = Setting.Instance.AppId;

            var vr = EntityHelper.Validate(entity);
            if (vr.HasError)
                return Result.Error(vr.ErrorMessage);

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }
        #endregion

        #region Private
        private void ResortModules(string parentId)
        {
            var modules = Database.GetModules(parentId);
            if (modules == null || modules.Count == 0)
                return;

            Database.Transaction("排序", db =>
            {
                var index = 0;
                foreach (var item in modules)
                {
                    item.Sort = ++index;
                    db.Save(item);
                }
            });
        }
        #endregion
    }
}
