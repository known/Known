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
            return db.QueryPage<SysDictionary>(sql, criteria);
        }
    }
}
