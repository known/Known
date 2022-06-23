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

using System;
using System.Collections.Generic;
using Known.Entities;

namespace Known.Core
{
    partial class SystemService
    {
        public PagingResult<SysNotice> QueryNotices(PagingCriteria criteria)
        {
            SetCriteriaAppId(criteria);
            return Repository.QueryNotices(Database, criteria);
        }

        public Result DeleteNotices(string data)
        {
            var ids = Utils.FromJson<string[]>(data);
            var entities = Database.QueryListById<SysNotice>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            if (entities.Exists(e => e.Status != "暂存"))
                return Result.Error("只能删除暂存的记录！");

            return Database.Transaction(Language.Delete, db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                }
            });
        }

        public List<SysNotice> GetNewestNotices()
        {
            var user = CurrentUser;
            return Repository.GetNewestNotices(Database, user.AppId, user.CompNo, 5);
        }

        public SysNotice GetNotice(string id)
        {
            return Database.QueryById<SysNotice>(id);
        }

        public Result SaveNotice(string data)
        {
            return SaveNoticeData(data, false);
        }

        public Result PublishNotice(string data)
        {
            return SaveNoticeData(data, true);
        }

        private Result SaveNoticeData(string data, bool isPublish)
        {
            var model = Utils.ToDynamic(data);
            var entity = Database.QueryById<SysNotice>((string)model.Id);
            if (entity == null)
            {
                entity = new SysNotice { Status = "暂存" };
            }

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return vr;

            if (isPublish)
            {
                entity.Status = "已发布";
                if (!entity.PublishTime.HasValue)
                    entity.PublishTime = DateTime.Now;
            }

            Database.Save(entity);
            var opName = isPublish ? "发布" : "保存";
            return Result.Success($"{opName}成功！", entity.Id);
        }
    }

    partial class SystemRepository
    {
        public PagingResult<SysNotice> QueryNotices(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysNotice where AppId=@AppId and CompNo=@CompNo";
            if (criteria.HasParameter("type"))
            {
                if (criteria.Parameter["type"] == "more")
                    sql += $" and Status='{Constants.NSPublished}'";
            }
            db.SetQuery(ref sql, criteria, QueryType.Contain, "Title");
            return db.QueryPage<SysNotice>(sql, criteria);
        }

        public List<SysNotice> GetNewestNotices(Database db, string appId, string compNo, int size)
        {
            var sql = $"select * from SysNotice where AppId=@appId and CompNo=@compNo and Status='{Constants.NSPublished}' order by PublishTime desc";
            return db.QueryList<SysNotice>(size, sql, new { appId, compNo });
        }
    }
}