using System.Collections.Generic;
using Known.Core.Entities;
using Known.Core.Repositories;

namespace Known.Core.Services
{
    public class OrganizationService : ServiceBase
    {
        private IOrganizationRepository Repository { get; } = Container.Resolve<IOrganizationRepository>();

        #region View
        public PagingResult<SysOrganization> QueryOrganizations(PagingCriteria criteria)
        {
            return Repository.QueryOrganizations(Database, criteria);
        }

        public Result DeleteOrganizations(string[] ids)
        {
            var entities = Database.QueryListById<SysOrganization>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            foreach (var item in entities)
            {
                if (Repository.ExistsSubOrganization(Database, item.Id))
                    return Result.Error($"{item.Name}存在子模块，不能删除！");
            }

            return Database.Transaction("删除", db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                }
            });
        }
        #endregion

        #region Form
        public List<SysOrganization> GetOrganizations()
        {
            var datas = Repository.GetOrganizations(Database);
            if (datas == null)
                datas = new List<SysOrganization>();
            datas.Insert(0, new SysOrganization
            {
                Id = "1",
                ParentId = "",
                Code = "",
                Name = "总公司"
            });
            return datas;
        }

        public SysOrganization GetOrganization(string id)
        {
            return Database.QueryById<SysOrganization>(id);
        }

        public Result SaveOrganization(dynamic model)
        {
            var entity = Database.QueryById<SysOrganization>((string)model.Id);
            if (entity == null)
                entity = new SysOrganization();

            entity.FillModel(model);
            var vr = ValidateOrganization(Database, entity);
            if (!vr.IsValid)
                return vr;

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }

        private Result ValidateOrganization(Database db, SysOrganization entity)
        {
            var vr = entity.Validate();
            if (vr.IsValid)
            {
                if (Repository.ExistsOrganization(db, entity))
                    vr.AddError("组织编码已存在！");
            }

            return vr;
        }
        #endregion
    }
}
