using System.Collections.Generic;
using Known.Core.Entities;
using Known.Core.Repositories;

namespace Known.Core.Services
{
    public class UserService : ServiceBase
    {
        private const string DEFALUT_PWD = "123456";
        private IUserRepository Repository { get; } = Container.Resolve<IUserRepository>();

        #region View
        public PagingResult<SysUser> QueryUsers(PagingCriteria criteria)
        {
            return Repository.QueryUsers(Database, criteria);
        }

        public Result DeleteUsers(string[] ids)
        {
            var entities = Database.QueryListById<SysUser>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Database.Transaction("删除", db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                    Repository.DeleteUserRoles(db, item.Id);
                    Repository.DeleteUserModules(db, item.Id);
                }
            });
        }

        public Result SetUserPwds(string[] ids)
        {
            var entities = Database.QueryListById<SysUser>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Database.Transaction("重置", db =>
            {
                foreach (var item in entities)
                {
                    item.Password = Utils.ToMd5(DEFALUT_PWD);
                    db.Save(item);
                }
            });
        }

        public Result EnableUsers(string[] ids, int enable)
        {
            var entities = Database.QueryListById<SysUser>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            var name = enable == 1 ? "启用" : "停用";
            return Database.Transaction(name, db =>
            {
                foreach (var item in entities)
                {
                    item.Enabled = enable;
                    db.Save(item);
                }
            });
        }
        #endregion

        #region Form
        public SysUser GetUser(string id)
        {
            return Database.QueryById<SysUser>(id);
        }

        public Result SaveUser(dynamic model)
        {
            var entity = Database.QueryById<SysUser>((string)model.Id);
            if (entity == null)
            {
                entity = new SysUser();
                entity.Password = Utils.ToMd5(DEFALUT_PWD);
            }

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return Result.Error(vr.Message);

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }
        #endregion

        #region RoleRole
        public List<SysRole> GetRoles()
        {
            var roles = Repository.GetRoles(Database, "");
            if (roles == null)
                roles = new List<SysRole>();

            return roles;
        }

        public List<string> GetUserRoles(string userId)
        {
            return Repository.GetUserRoles(Database, userId);
        }

        public Result SaveUserRoles(string userId, List<string> roleIds)
        {
            return Database.Transaction("保存", db =>
            {
                Repository.DeleteUserRoles(db, userId);
                if (roleIds != null && roleIds.Count > 0)
                {
                    foreach (var item in roleIds)
                    {
                        Repository.AddUserRole(db, userId, item);
                    }
                }
            });
        }
        #endregion

        #region UserModule
        public List<string> GetUserModules(string userId)
        {
            return Repository.GetUserModules(Database, userId);
        }

        public Result SaveUserModules(string userId, List<string> moduleIds)
        {
            return Database.Transaction("保存", db =>
            {
                Repository.DeleteUserModules(db, userId);
                if (moduleIds != null && moduleIds.Count > 0)
                {
                    foreach (var item in moduleIds)
                    {
                        Repository.AddUserModule(db, userId, item);
                    }
                }
            });
        }
        #endregion
    }
}
