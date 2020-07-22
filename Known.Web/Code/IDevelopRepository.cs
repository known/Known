using System.Collections.Generic;
using Known.Web.Entities;

namespace Known.Web
{
    public interface IDevelopRepository
    {
        #region Module
        PagingResult<SysModule> QueryModules(Database db, PagingCriteria criteria);
        List<SysModule> GetModules(Database db);
        bool ExistsSubModule(Database db, string parentId);
        void DeleteModuleRights(Database db, string moduleId);
        #endregion
    }

    class DevelopRepository : IDevelopRepository
    {
        #region Module
        public PagingResult<SysModule> QueryModules(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysModule";
            return db.QueryPage<SysModule>(sql, criteria);
        }

        public List<SysModule> GetModules(Database db)
        {
            var sql = "select * from SysModule order by Sort";
            return db.QueryList<SysModule>(sql);
        }

        public bool ExistsSubModule(Database db, string parentId)
        {
            var sql = "select count(*) from SysModule where ParentId=@parentId";
            return db.Scalar<int>(sql, new { parentId }) > 0;
        }

        public void DeleteModuleRights(Database db, string moduleId)
        {
            var sql = "delete from SysRoleModule where ModuleId=@moduleId";
            db.Execute(sql, new { moduleId });

            sql = "delete from SysUserModule where ModuleId=@moduleId";
            db.Execute(sql, new { moduleId });
        }
        #endregion
    }
}
