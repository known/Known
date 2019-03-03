using System.Collections.Generic;
using Known.Core.Entities;
using Known.Core.Repositories;
using Known.Extensions;
using Known.Mapping;

namespace Known.Core.Services
{
    public class ModuleService : CoreServiceBase<IModuleRepository>
    {
        public ModuleService(Context context) : base(context)
        {
        }

        public PagingResult QueryModules(PagingCriteria criteria)
        {
            return Repository.QueryModules(criteria);
        }

        public List<Module> GetModules(string[] ids)
        {
            return Repository.QueryListById<Module>(ids);
        }

        public List<Module> GetModules(bool isTree = false)
        {
            return Repository.QueryList<Module>();
        }

        public Module GetModule(string id)
        {
            return Repository.QueryById<Module>(id);
        }

        public Result<Module> SaveModule(dynamic model)
        {
            if (model == null)
                return Result.Error<Module>("不能提交空数据！");

            var id = (string)model.Id;
            var entity = Repository.QueryById<Module>(id);
            if (entity == null)
            {
                entity = new Module();
                entity.AppId = Setting.Instance.AppId;
            }
            EntityHelper.FillModel(entity, model);

            var vr = EntityHelper.Validate(entity);
            if (vr.HasError)
                return Result.Error<Module>(vr.ErrorMessage);

            Repository.Save(entity);
            return Result.Success("保存成功！", entity);
        }

        public Result DeleteModules(string data)
        {
            var ids = data.FromJson<string[]>();
            var modules = Repository.QueryListById<Module>(ids);
            if (modules == null || modules.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Repository.Transaction(rep =>
            {
                modules.ForEach(e => rep.Delete(e));
            });
        }

        public Result DropModule(string id, string pid)
        {
            var module = Repository.QueryById<Module>(id);
            if (module == null)
                return Result.Error("模块不存在！");

            var parent = Repository.QueryById<Module>(pid);
            if (parent == null)
                return Result.Error("父模块不存在！");

            module.ParentId = parent.Id;
            Repository.Save(module);
            return Result.Success("保存成功！");
        }
    }
}
