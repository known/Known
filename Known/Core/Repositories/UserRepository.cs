using Known.Core.Entities;
using Known.Data;

namespace Known.Core
{
    /// <summary>
    /// 系统用户数据仓库接口。
    /// </summary>
    public interface IUserRepository : IRepository
    {
        /// <summary>
        /// 查询用户分页数据对象。
        /// </summary>
        /// <param name="criteria">查询条件对象。</param>
        /// <returns>分页数据对象。</returns>
        PagingResult QueryUsers(PagingCriteria criteria);
    }

    internal class UserRepository : DbRepository, IUserRepository
    {
        public UserRepository(Database database) : base(database)
        {
        }

        public PagingResult QueryUsers(PagingCriteria criteria)
        {
            var sql = "select * from t_plt_users where parent_id=@pid";

            var key = (string)criteria.Parameter.key;
            if (!string.IsNullOrWhiteSpace(key))
            {
                sql += " and (code like @key or name like @key)";
                criteria.Parameter.key = $"%{key}%";
            }

            return Database.QueryPage<User>(sql, criteria);
        }
    }
}
