using System.Collections.Generic;
using Known.Entities;

namespace Known.Core
{
    partial class SystemService
    {
        public PagingResult<SysLog> QueryLogs(PagingCriteria criteria)
        {
            SetCriteriaAppId(criteria);
            return Repository.QueryLogs(Database, criteria);
        }

        public Result DeleteLogs(string data)
        {
            var ids = Utils.FromJson<string[]>(data);
            var entities = Database.QueryListById<SysLog>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            return Database.Transaction(Language.Delete, db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                }
            });
        }

        public List<SysLog> GetLogs(string bizId)
        {
            return Repository.GetLogs(Database, bizId);
        }
    }

    partial interface ISystemRepository
    {
        PagingResult<SysLog> QueryLogs(Database db, PagingCriteria criteria);
        List<SysLog> GetLogs(Database db, string bizId);
    }

    partial class SystemRepository
    {
        public PagingResult<SysLog> QueryLogs(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysLog where AppId=@AppId and CompNo=@CompNo";
            db.SetQuery(ref sql, criteria, QueryType.Contain, "CreateBy");
            db.SetQuery(ref sql, criteria, QueryType.Contain, "Content");
            return db.QueryPage<SysLog>(sql, criteria);
        }

        public List<SysLog> GetLogs(Database db, string bizId)
        {
            var sql = "select * from SysLog where Target=@bizId order by CreateTime";
            return db.QueryList<SysLog>(sql, new { bizId });
        }
    }
}