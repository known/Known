using Known.Data;
using Known.Web.Entities;

namespace Known.Web.Datas
{
    internal static class RoleRepository
    {
        public static PagingResult QueryRoles(this Database database, PagingCriteria criteria)
        {
            var sql = "select * from t_plt_roles where comp_no=@compNo";
            criteria.Parameter.compNo = "";

            if (!string.IsNullOrWhiteSpace(criteria.Key))
            {
                sql += " and (code like @key or name like @key)";
                criteria.Parameter.key = $"%{criteria.Key}%";
            }

            return database.QueryPage<TUser>(sql, criteria);
        }

        public static void DeleteRoleUsers(this Database database, string roleId)
        {
        }

        public static void DeleteRoleFunctions(this Database database, string roleId)
        {
        }
    }
}
