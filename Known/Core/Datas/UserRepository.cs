using Known.Data;

namespace Known.Core
{
    internal static class UserRepository
    {
        public static PagingResult QueryUsers(this Database database, PagingCriteria criteria)
        {
            var sql = "select * from t_plt_users where parent_id=@pid";

            var key = (string)criteria.Parameter.key;
            if (!string.IsNullOrWhiteSpace(key))
            {
                sql += " and (code like @key or name like @key)";
                criteria.Parameter.key = $"%{key}%";
            }

            return database.QueryPage<User>(sql, criteria);
        }
    }
}
