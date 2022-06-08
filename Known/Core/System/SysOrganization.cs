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
        public PagingResult<SysOrganization> QueryOrganizations(PagingCriteria criteria)
        {
            SetCriteriaAppId(criteria);
            return Repository.QueryOrganizations(Database, criteria);
        }

        public Result DeleteOrganizations(string data)
        {
            var ids = Utils.FromJson<string[]>(data);
            var entities = Database.QueryListById<SysOrganization>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            foreach (var item in entities)
            {
                if (Repository.ExistsSubOrganization(Database, item.Id))
                    return Result.Error($"{item.Name}存在子部门，不能删除！");
            }

            return Database.Transaction(Language.Delete, db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                }
            });
        }

        public IEnumerable<object> GetOrganizations()
        {
            var user = CurrentUser;
            var datas = Repository.GetOrganizations(Database, user.AppId, user.CompNo);
            if (datas == null)
            {
                datas = new List<SysOrganization>();
            }

            datas.Insert(0, new SysOrganization
            {
                Id = user.CompNo,
                ParentId = "0",
                Code = user.CompNo,
                Name = user.CompName
            });

            return datas.Select(d => d.ToTree());
        }

        public Result SaveOrganization(string data)
        {
            var user = CurrentUser;
            var model = Utils.ToDynamic(data);
            var entity = Database.QueryById<SysOrganization>((string)model.Id);
            if (entity == null)
            {
                entity = new SysOrganization
                {
                    CompNo = user.CompNo,
                    AppId = user.AppId
                };
            }

            entity.FillModel(model);
            var vr = ValidateOrganization(Database, entity);
            if (!vr.IsValid)
                return vr;

            return Database.Transaction(Language.Save, db =>
            {
                db.Save(entity);
                PlatformAction.SetBizOrganization(db, user, entity);
            }, entity.Id);
        }

        private static Result ValidateOrganization(Database db, SysOrganization entity)
        {
            var vr = entity.Validate();
            if (vr.IsValid)
            {
                if (Repository.ExistsOrganization(db, entity))
                    vr.AddError("组织编码已存在！");
            }

            return vr;
        }
    }

    partial interface ISystemRepository
    {
        PagingResult<SysOrganization> QueryOrganizations(Database db, PagingCriteria criteria);
        List<SysOrganization> GetOrganizations(Database db, string appId, string compNo);
        bool ExistsOrganization(Database db, SysOrganization entity);
        bool ExistsSubOrganization(Database db, string parentId);
    }

    partial class SystemRepository
    {
        public PagingResult<SysOrganization> QueryOrganizations(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysOrganization where AppId=@AppId and CompNo=@CompNo";
            db.SetQuery(ref sql, criteria, QueryType.Contain, "Name");
            return db.QueryPage<SysOrganization>(sql, criteria);
        }

        public List<SysOrganization> GetOrganizations(Database db, string appId, string compNo)
        {
            var sql = "select * from SysOrganization where AppId=@appId and CompNo=@compNo order by ParentId,Code";
            return db.QueryList<SysOrganization>(sql, new { appId, compNo });
        }

        public bool ExistsOrganization(Database db, SysOrganization entity)
        {
            var sql = "select count(*) from SysOrganization where Id<>@Id and AppId=@AppId and CompNo=@CompNo and Code=@Code";
            return db.Scalar<int>(sql, new { entity.Id, entity.AppId, entity.CompNo, entity.Code }) > 0;
        }

        public bool ExistsSubOrganization(Database db, string parentId)
        {
            var sql = "select count(*) from SysOrganization where ParentId=@parentId";
            return db.Scalar<int>(sql, new { parentId }) > 0;
        }
    }
}