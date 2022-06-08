using System.Collections.Generic;
using System.Linq;

namespace Known.Core
{
    partial class PlatformService
    {
        public Dictionary<string, List<CodeInfo>> GetCodes(string appId, string compNo)
        {
            var datas = Repository.GetCodes(Database, appId, compNo);
            if (datas != null && datas.Count > 0)
            {
                var cateList = datas.Select(c => c.Category).Distinct();
                foreach (var item in cateList)
                {
                    Cache.AttachCodes(appId, datas.Where(d => d.Category == item).ToList());
                }
            }

            var codes = Cache.GetCodes(appId);
            var dics = new Dictionary<string, List<CodeInfo>>();
            var cates = codes.Select(c => c.Category).Distinct();
            foreach (var item in cates)
            {
                dics[item] = codes.Where(c => c.Category == item).ToList();
            }

            var dictionaries = PlatformAction.GetDictionaries(Database, compNo, appId);
            if (dictionaries != null && dictionaries.Count > 0)
            {
                var cateList = dictionaries.Select(c => c.Category).Distinct();
                foreach (var item in cateList)
                {
                    dics[item] = dictionaries.Where(d => d.Category == item).ToList();
                }
            }

            return dics;
        }
    }

    partial interface IPlatformRepository
    {
        List<CodeInfo> GetCodes(Database db, string appId, string compNo);
    }

    partial class PlatformRepository
    {
        public List<CodeInfo> GetCodes(Database db, string appId, string compNo)
        {
            var sql = "select * from SysDictionary where Enabled=1 and Category<>'KnownDict' and AppId=@appId and CompNo=@compNo order by Category,Sort";
            return db.QueryList<CodeInfo>(sql, new { appId, compNo });
        }
    }
}
