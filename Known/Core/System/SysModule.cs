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
        public List<SysModule> GetSystems()
        {
            return Repository.GetSystems(Database);
        }

        public PagingResult<SysModule> QueryModules(PagingCriteria criteria)
        {
            return Repository.QueryModules(Database, criteria);
        }

        public Result CopyModules(string data, string mid)
        {
            var ids = Utils.FromJson<string[]>(data);
            var module = Database.QueryById<SysModule>(mid);
            if (module == null)
                return Result.Error("所选模块不存在！");

            var entities = Database.QueryListById<SysModule>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            return Database.Transaction(Language.Copy, db =>
            {
                foreach (var item in entities)
                {
                    item.AppId = module.AppId;
                    item.ParentId = module.Id;
                    db.Insert(item);
                }
            });
        }

        public Result MoveModules(string data, string mid)
        {
            var ids = Utils.FromJson<string[]>(data);
            var module = Database.QueryById<SysModule>(mid);
            if (module == null)
                return Result.Error("所选模块不存在！");

            var entities = Database.QueryListById<SysModule>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            return Database.Transaction("移动", db =>
            {
                foreach (var item in entities)
                {
                    item.AppId = module.AppId;
                    item.ParentId = module.Id;
                    db.Save(item);
                }
            });
        }

        public Result DeleteModules(string data)
        {
            var ids = Utils.FromJson<string[]>(data);
            var entities = Database.QueryListById<SysModule>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            foreach (var item in entities)
            {
                if (Repository.ExistsSubModule(Database, item.Id))
                    return Result.Error($"{item.Name}存在子模块，不能删除！");
            }

            return Database.Transaction(Language.Delete, db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                    Repository.DeleteModuleRights(db, item.Id);
                }
            });
        }

        public Result MoveModule(string id, string direct)
        {
            var entity = Database.QueryById<SysModule>(id);
            if (entity == null)
                return Result.Error("模块不存在！");

            var modules = Repository.GetSubModules(Database, entity.ParentId);
            if (modules == null || modules.Count == 0)
                return Result.Error("暂无子模块，不能移动！");

            for (int i = 0; i < modules.Count; i++)
            {
                if (modules[i].Id == entity.Id)
                {
                    if (direct == "up")
                    {
                        modules[i - 1].Sort = entity.Sort;
                        modules[i].Sort = entity.Sort - 1;
                    }
                    else
                    {
                        modules[i + 1].Sort = entity.Sort;
                        modules[i].Sort = entity.Sort + 1;
                    }
                }
            }

            return Database.Transaction("移动", db =>
            {
                var index = 1;
                foreach (var item in modules.OrderBy(m => m.Sort))
                {
                    item.Sort = index++;
                    db.Save(item);
                }
            });
        }

        public object GetModuleTree(string appId)
        {
            var modules = Repository.GetModules(Database, appId);
            if (!string.IsNullOrEmpty(appId))
            {
                modules = modules.Where(m => m.ParentId != "0").ToList();
            }

            return modules.Select(m => new
            {
                id = m.Id,
                pid = m.ParentId,
                type = m.Type,
                code = m.Code,
                name = m.Name,
                title = m.Name,
                icon = m.Icon,
                open = string.IsNullOrEmpty(m.ParentId),
                hid = string.IsNullOrEmpty(m.Url) ? 0 : m.Url.GetHashCode(),
                module = m
            });
        }

        public SysModule GetModule(string id)
        {
            return Database.QueryById<SysModule>(id);
        }

        public Result SaveModule(string data)
        {
            var model = Utils.ToDynamic(data);
            var entity = Database.QueryById<SysModule>((string)model.Id);
            if (entity == null)
                entity = new SysModule();

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return vr;

            if (entity.ParentId == "0")
                entity.AppId = entity.Code;

            Database.Save(entity);
            return Result.Success(Language.SaveSuccess, entity.Id);
        }

        public Result AddModules(string data)
        {
            var modules = Utils.FromJson<List<SysModule>>(data);
            if (modules == null || modules.Count == 0)
                return Result.Error(Language.NotPostData);

            var compNo = CurrentUser.CompNo;
            return Database.Transaction(Language.Save, db =>
            {
                foreach (var item in modules)
                {
                    item.CompNo = compNo;
                    db.Insert(item);
                }
            });
        }

        public Result EnableModules(string data, int enable)
        {
            var ids = Utils.FromJson<string[]>(data);
            var entities = Database.QueryListById<SysModule>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            var name = enable == 1 ? "启用" : "禁用";
            return Database.Transaction(name, db =>
            {
                foreach (var item in entities)
                {
                    item.Enabled = enable;
                    db.Save(item);
                }
            });
        }
    }

    partial interface ISystemRepository
    {
        PagingResult<SysModule> QueryApps(Database db, PagingCriteria criteria);
        PagingResult<SysModule> QueryModules(Database db, PagingCriteria criteria);
        List<SysModule> GetSystems(Database db);
        List<SysModule> GetModules(Database db, string appId);
        List<SysModule> GetSubModules(Database db, string parentId);
        bool ExistsModuleCode(Database db, string id, string parentId, string code);
        bool ExistsSubModule(Database db, string parentId);
        void DeleteModuleRights(Database db, string moduleId);
    }

    partial class SystemRepository
    {
        public PagingResult<SysModule> QueryApps(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysModule where ParentId='0'";
            db.SetQuery(ref sql, criteria, QueryType.Contain, "Name");
            return db.QueryPage<SysModule>(sql, criteria);
        }

        public PagingResult<SysModule> QueryModules(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysModule";
            return db.QueryPage<SysModule>(sql, criteria);
        }

        public List<SysModule> GetSystems(Database db)
        {
            var sql = "select * from SysModule where ParentId='0' and Enabled=1 order by Sort";
            return db.QueryList<SysModule>(sql);
        }

        public List<SysModule> GetModules(Database db, string appId)
        {
            if (string.IsNullOrEmpty(appId))
            {
                var sql = "select * from SysModule order by Sort";
                return db.QueryList<SysModule>(sql);
            }
            else
            {
                var sql = "select * from SysModule where AppId=@appId order by Sort";
                return db.QueryList<SysModule>(sql, new { appId });
            }
        }

        public List<SysModule> GetSubModules(Database db, string parentId)
        {
            var sql = "select * from SysModule where ParentId=@parentId order by Sort";
            return db.QueryList<SysModule>(sql, new { parentId });
        }

        public bool ExistsModuleCode(Database db, string id, string parentId, string code)
        {
            var sql = "select count(*) from SysModule where Id<>@id and ParentId=@parentId and Code=@code";
            return db.Scalar<int>(sql, new { id, parentId, code }) > 0;
        }

        public bool ExistsSubModule(Database db, string parentId)
        {
            var sql = "select count(*) from SysModule where ParentId=@parentId";
            return db.Scalar<int>(sql, new { parentId }) > 0;
        }

        public void DeleteModuleRights(Database db, string moduleId)
        {
            var sql = "delete from SysRoleModule where ModuleId=@moduleId";
            db.Execute(sql, new { moduleId });

            sql = "delete from SysUserModule where ModuleId=@moduleId";
            db.Execute(sql, new { moduleId });
        }
    }
}