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

using Known.Entities;

namespace Known.Core
{
    partial class SystemService
    {
        public PagingResult<SysModule> QueryApps(PagingCriteria criteria)
        {
            return Repository.QueryApps(Database, criteria);
        }

        public Result DeleteApps(string data)
        {
            var ids = Utils.FromJson<string[]>(data);
            var entities = Database.QueryListById<SysModule>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            foreach (var item in entities)
            {
                if (Repository.ExistsSubModule(Database, item.Id))
                    return Result.Error($"{item.Name}存在模块，不能删除！");
            }

            return Database.Transaction(Language.Delete, db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                }
            });
        }

        public Result SaveApp(string data)
        {
            var model = Utils.ToDynamic(data);
            var entity = Database.QueryById<SysModule>((string)model.Id);
            if (entity == null)
            {
                entity = new SysModule
                {
                    ParentId = "0",
                    Type = "menu",
                    Target = "tab"
                };
            }

            entity.FillModel(model);
            var vr = entity.Validate();
            if (vr.IsValid)
            {
                if (Repository.ExistsModuleCode(Database, entity.Id, entity.ParentId, entity.Code))
                    vr.AddError("编码不能重复！");
            }

            if (!vr.IsValid)
                return vr;

            entity.AppId = entity.Code;
            Database.Save(entity);
            Config.Apps = null;
            return Result.Success(Language.SaveSuccess, entity.Id);
        }
    }
}