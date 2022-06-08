/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

using System.Collections.Generic;
using System.Linq;
using Known.Entities;

namespace Known.Core
{
    partial class SystemService
    {
        public IEnumerable<CodeInfo> GetDicCategories(string appId)
        {
            var user = CurrentUser;
            var lists = new List<SysDictionary>();
            if (Platform.CheckDevUser(user) && appId == DevId)
            {
                lists.Add(Constants.FrameDict);
            }

            var cates = Repository.GetDicCategories(Database, appId);
            if (cates != null && cates.Count > 0)
            {
                lists.AddRange(cates);
            }

            return lists.Select(d => new CodeInfo(d.Code, d.Name, d));
        }

        public PagingResult<SysDictionary> QueryDictionarys(PagingCriteria criteria)
        {
            if (criteria.Parameter["Category"] == Constants.FrameDictCode)
                return Repository.QueryFrameDictionarys(Database, criteria);

            SetCriteriaAppId(criteria);
            return Repository.QueryDictionarys(Database, criteria);
        }

        public Result DeleteDictionarys(string data)
        {
            var ids = Utils.FromJson<string[]>(data);
            var entities = Database.QueryListById<SysDictionary>(ids);
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

        public Result SaveDictionary(string data)
        {
            var model = Utils.ToDynamic(data);
            var entity = Database.QueryById<SysDictionary>((string)model.Id);
            if (entity == null)
                entity = new SysDictionary();

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return vr;

            Database.Save(entity);
            return Result.Success(Language.SaveSuccess, entity.Id);
        }
    }

    partial interface ISystemRepository
    {
        List<SysDictionary> GetDicCategories(Database db, string appId);
        PagingResult<SysDictionary> QueryDictionarys(Database db, PagingCriteria criteria);
        PagingResult<SysDictionary> QueryFrameDictionarys(Database db, PagingCriteria criteria);
    }

    partial class SystemRepository
    {
        public List<SysDictionary> GetDicCategories(Database db, string appId)
        {
            var sql = $"select * from SysDictionary where Category='{Constants.FrameDictCode}' and AppId=@appId order by Sort";
            return db.QueryList<SysDictionary>(sql, new { appId });
        }

        public PagingResult<SysDictionary> QueryDictionarys(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysDictionary where AppId=@AppId and CompNo=@CompNo and Category=@Category";
            db.SetQuery(ref sql, criteria, QueryType.Contain, "Code");
            db.SetQuery(ref sql, criteria, QueryType.Contain, "Name");
            return db.QueryPage<SysDictionary>(sql, criteria);
        }

        public PagingResult<SysDictionary> QueryFrameDictionarys(Database db, PagingCriteria criteria)
        {
            var sql = $"select * from SysDictionary where CompNo=@CompNo and Category='{Constants.FrameDictCode}'";
            db.SetQuery(ref sql, criteria, QueryType.Equal, "AppId");
            db.SetQuery(ref sql, criteria, QueryType.Contain, "Code");
            db.SetQuery(ref sql, criteria, QueryType.Contain, "Name");
            return db.QueryPage<SysDictionary>(sql, criteria);
        }
    }
}