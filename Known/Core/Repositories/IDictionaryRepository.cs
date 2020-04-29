using System.Collections.Generic;
using Known.Core.Entities;

namespace Known.Core.Repositories
{
    public interface IDictionaryRepository
    {
        List<SysDictionary> GetCategories(Database db);
        PagingResult<SysDictionary> QueryDictionarys(Database db, PagingCriteria criteria);
    }

    class DictionaryRepository : IDictionaryRepository
    {
        public List<SysDictionary> GetCategories(Database db)
        {
            var sql = "select * from SysDictionary where Category=''";
            return db.QueryList<SysDictionary>(sql);
        }

        public PagingResult<SysDictionary> QueryDictionarys(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysDictionary where Category=@Category";
            if (!string.IsNullOrWhiteSpace((string)criteria.Parameter.key))
            {
                var key = criteria.Parameter.key;
                sql += " and (Code like @key or Name like @key)";
                criteria.Parameter.key = $"%{key}%";
            }

            return db.QueryPage<SysDictionary>(sql, criteria);
        }
    }
}
