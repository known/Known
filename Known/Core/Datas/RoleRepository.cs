using Known.Data;

namespace Known.Core
{
    internal static class RoleRepository
    {
        public static PagingResult QueryRoles(this Database database, PagingCriteria criteria)
        {
            var sql = "select * from t_plt_roles where parent_id=@pid";

            var key = (string)criteria.Parameter.key;
            if (!string.IsNullOrWhiteSpace(key))
            {
                sql += " and (code like @key or name like @key)";
                criteria.Parameter.key = $"%{key}%";
            }

            return database.QueryPage<User>(sql, criteria);
        }

        public static void DeleteRoleUsers(this Database database, string roleId)
        {

        }

        public static void DeleteRoleFunctions(this Database database, string roleId)
        {

        }
    }
}
