using Known.Core.Entities;

namespace Known.Core.Repositories
{
    public interface IDictionaryRepository
    {
        PagingResult<SysDictionary> QueryDictionarys(Database db, PagingCriteria criteria);
    }

    class DictionaryRepository : IDictionaryRepository
    {
        public PagingResult<SysDictionary> QueryDictionarys(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysDictionary";
            return db.QueryPage<SysDictionary>(sql, criteria);
        }
    }
}
