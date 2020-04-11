using System.Collections.Generic;
using Known.Core.Datas;
using Known.Core.Entities;

namespace Known.Core.Services
{
    class SystemService : ServiceBase
    {
        private ISystemRepository Repository
        {
            get { return Container.Resolve<ISystemRepository>(); }
        }

        #region Module
        public List<SysModule> GetModules()
        {
            return Repository.GetModules(Database);
        }

        public PagingResult<SysModule> QueryModules(PagingCriteria criteria)
        {
            return Repository.QueryModules(Database, criteria);
        }

        public Result DeleteModules(string[] ids)
        {
            var entities = Database.QueryListById<SysModule>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Database.Transaction("删除", db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                }
            });
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
                return Result.Error(vr.Message);

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }
        #endregion

        #region Role
        #endregion

        #region User
        #endregion
    }
}
