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
using System.Linq;
using Known.Entities;

namespace Known.Core
{
    partial class SystemService
    {
        public PagingResult<SysUser> QueryUsers(PagingCriteria criteria)
        {
            SetCriteriaAppId(criteria);
            return Repository.QueryUsers(Database, criteria);
        }

        public Result DeleteUsers(string data)
        {
            var ids = Utils.FromJson<string[]>(data);
            var entities = Database.QueryListById<SysUser>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            return Database.Transaction(Language.Delete, db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                    Repository.DeleteUserRoles(db, item.Id);
                    Repository.DeleteUserModules(db, item.Id);
                }
            });
        }

        public Result SetUserPwds(string data)
        {
            var ids = Utils.FromJson<string[]>(data);
            var entities = Database.QueryListById<SysUser>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            var info = Platform.GetSystem();
            if (info == null || string.IsNullOrEmpty(info.UserDefaultPwd))
                return Result.Error("用户默认密码未配置！");

            return Database.Transaction("重置", db =>
            {
                foreach (var item in entities)
                {
                    item.Password = Utils.ToMd5(info.UserDefaultPwd);
                    db.Save(item);
                }
            });
        }

        public Result EnableUsers(string data, int enable)
        {
            var ids = Utils.FromJson<string[]>(data);
            var entities = Database.QueryListById<SysUser>(ids);
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

        public Result SaveUser(string data, string role)
        {
            var model = Utils.ToDynamic(data);
            var roles = Utils.FromJson<List<CodeInfo>>(role);
            var entity = Database.QueryById<SysUser>((string)model.Id);
            if (entity == null)
            {
                var user = CurrentUser;
                entity = new SysUser
                {
                    FirstLoginTime = DateTime.Now,
                    LastLoginTime = DateTime.Now
                };

                var info = Platform.GetSystem();
                if (info == null || string.IsNullOrEmpty(info.UserDefaultPwd))
                    return Result.Error("用户默认密码未配置！");

                entity.Password = Utils.ToMd5(info.UserDefaultPwd);
            }

            entity.FillModel(model);
            var vr = entity.Validate();
            if (vr.IsValid)
            {
                if (Repository.ExistsUserName(Database, entity.Id, entity.UserName))
                {
                    vr.AddError("用户名已存在，请使用其他字符创建用户！");
                }
            }

            if (!vr.IsValid)
                return vr;

            return Database.Transaction(Language.Save, db =>
            {
                entity.Role = string.Empty;
                Repository.DeleteUserRoles(db, entity.Id);
                if (roles != null && roles.Count > 0)
                {
                    entity.Role = string.Join(",", roles.Select(r => r.Name).ToArray());
                    foreach (var item in roles)
                    {
                        Repository.AddUserRole(db, entity.Id, item.Code);
                    }
                }
                db.Save(entity);
                PlatformAction.SetBizUser(db, entity);
            }, entity.Id);
        }

        private List<SysRole> GetRoles()
        {
            var user = CurrentUser;
            var roles = Repository.GetRoles(Database, user.AppId, user.CompNo);
            if (roles == null)
                roles = new List<SysRole>();

            return roles;
        }

        public object GetUserRoles(string userId)
        {
            var roles = GetRoles().Select(r => new CodeInfo(r.Id, r.Name));
            var roleIds = Repository.GetUserRoles(Database, userId);
            var value = roleIds == null || roleIds.Count == 0
                      ? string.Empty
                      : string.Join(",", roleIds.ToArray());
            return new { roles, value };
        }
    }

    partial interface ISystemRepository
    {
        PagingResult<SysUser> QueryUsers(Database db, PagingCriteria criteria);
        bool ExistsUserName(Database db, string id, string userName);
        List<SysRole> GetRoles(Database db, string appId, string compNo);
        List<string> GetUserRoles(Database db, string userId);
        void DeleteUserRoles(Database db, string userId);
        void AddUserRole(Database db, string userId, string roleId);
        List<string> GetUserModules(Database db, string userId);
        void DeleteUserModules(Database db, string userId);
        void AddUserModule(Database db, string userId, string moduleId);
    }

    partial class SystemRepository
    {
        public PagingResult<SysUser> QueryUsers(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysUser where CompNo=@CompNo";
            if (db.UserName != Constants.SysUserName)
                sql += $" and AppId=@AppId and UserName<>'{Constants.SysUserName}' and Id not in (select Id from SysUser where CompNo=OrgNo and CompNo=UserName)";
            else
                sql += $" and (AppId=@AppId or AppId='{SystemService.DevId}')";

            if (criteria.HasParameter("OrgNo"))
            {
                if (criteria.Parameter["OrgNo"] != criteria.Parameter["CompNo"])
                    sql += " and OrgNo=@OrgNo";
            }

            db.SetQuery(ref sql, criteria, QueryType.Contain, "UserName");
            db.SetQuery(ref sql, criteria, QueryType.Contain, "Name");
            return db.QueryPage<SysUser>(sql, criteria);
        }

        public bool ExistsUserName(Database db, string id, string userName)
        {
            var sql = "select count(*) from SysUser where Id<>@id and UserName=@userName";
            return db.Scalar<int>(sql, new { id, userName }) > 0;
        }

        public List<SysRole> GetRoles(Database db, string appId, string compNo)
        {
            var sql = "select * from SysRole where AppId=@appId and CompNo=@compNo order by CreateTime";
            return db.QueryList<SysRole>(sql, new { appId, compNo });
        }

        public List<string> GetUserRoles(Database db, string userId)
        {
            var sql = "select RoleId from SysUserRole where UserId=@userId";
            return db.Scalars<string>(sql, new { userId });
        }

        public void DeleteUserRoles(Database db, string userId)
        {
            var sql = "delete from SysUserRole where UserId=@userId";
            db.Execute(sql, new { userId });
        }

        public void AddUserRole(Database db, string userId, string roleId)
        {
            var sql = "insert into SysUserRole(UserId,RoleId) values(@userId,@roleId)";
            db.Execute(sql, new { userId, roleId });
        }

        public List<string> GetUserModules(Database db, string userId)
        {
            var sql = "select ModuleId from SysUserModule where UserId=@userId";
            return db.Scalars<string>(sql, new { userId });
        }

        public void DeleteUserModules(Database db, string userId)
        {
            var sql = "delete from SysUserModule where UserId=@userId";
            db.Execute(sql, new { userId });
        }

        public void AddUserModule(Database db, string userId, string moduleId)
        {
            var sql = "insert into SysUserModule(UserId,ModuleId) values(@userId,@moduleId)";
            db.Execute(sql, new { userId, moduleId });
        }
    }
}