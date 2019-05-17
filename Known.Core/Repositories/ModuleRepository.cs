﻿using Known.Core.Entities;
using Known.Data;

namespace Known.Core.Repositories
{
    public interface IModuleRepository : IRepository
    {
        PagingResult QueryModules(PagingCriteria criteria);
        bool ExistsChildren(string id);
    }

    internal class ModuleRepository : DbRepository, IModuleRepository
    {
        public ModuleRepository(Database database) : base(database)
        {
        }

        public PagingResult QueryModules(PagingCriteria criteria)
        {
            var sql = "select * from t_plt_modules where parent_id=@pid";

            var key = (string)criteria.Parameter.key;
            if (!string.IsNullOrWhiteSpace(key))
            {
                sql += " and (code like @key or name like @key)";
                criteria.Parameter.key = $"%{key}%";
            }

            return Database.QueryPage<Module>(sql, criteria);
        }

        public bool ExistsChildren(string id)
        {
            var sql = "select count(*) from t_plt_modules where parent_id=@id";
            return Database.Scalar<int>(sql, new { id }) > 0;
        }
    }
}
