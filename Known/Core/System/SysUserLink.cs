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
using Known.Entities;

namespace Known.Core
{
    partial class SystemService
    {
        public List<SysUserLink> GetUserLinks(string type)
        {
            var user = CurrentUser;
            return Repository.GetUserLinks(Database, user.AppId, type, user.UserName);
        }

        public Result DeleteUserLink(string id)
        {
            Database.Delete<SysUserLink>(id);
            return Result.Success(Language.XXSuccess.Format(Language.Delete));
        }

        public Result AddUserLink(string data)
        {
            var entity = Utils.FromJson<SysUserLink>(data);
            if (entity == null)
                return Result.Error(Language.NotPostData);

            var user = CurrentUser;
            entity.AppId = user.AppId;
            entity.CompNo = user.CompNo;
            entity.UserName = user.UserName;

            var vr = entity.Validate();
            if (!vr.IsValid)
                return vr;

            Database.Insert(entity);
            return Result.Success(Language.XXSuccess.Format(Language.Save), entity.Id);
        }

        public Result AddShortcuts(string data)
        {
            var entities = Utils.FromJson<List<SysUserLink>>(data);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            var user = CurrentUser;
            return Database.Transaction(Language.Save, db =>
            {
                Repository.DeleteUserLinks(db, user.AppId, "shortcut", user.UserName);
                foreach (var item in entities)
                {
                    item.AppId = user.AppId;
                    item.CompNo = user.CompNo;
                    item.UserName = user.UserName;
                    db.Insert(item);
                }
            });
        }
    }

    partial class SystemRepository
    {
        public List<SysUserLink> GetUserLinks(Database db, string appId, string type, string userName)
        {
            var sql = "select * from SysUserLink where AppId=@appId and Type=@type and UserName=@userName";
            return db.QueryList<SysUserLink>(sql, new { appId, type, userName });
        }

        public void DeleteUserLinks(Database db, string appId, string type, string userName)
        {
            var sql = "delete from SysUserLink where AppId=@appId and Type=@type and UserName=@userName";
            db.Execute(sql, new { appId, type, userName });
        }
    }
}