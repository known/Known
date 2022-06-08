using System;

namespace Known.Core
{
    partial class PlatformService
    {
        public void AddLog(Database db, string appId, string compNo, string userName, string type, string target, string content)
        {
            Repository.AddLog(db, appId, compNo, userName, type, target, content);
        }
    }

    partial interface IPlatformRepository
    {
        void AddLog(Database db, string appId, string compNo, string userName, string type, string target, string content);
    }

    partial class PlatformRepository
    {
        public void AddLog(Database db, string appId, string compNo, string userName, string type, string target, string content)
        {
            var sql = @"
insert into SysLog(Id,CreateBy,CreateTime,Version,AppId,CompNo,Type,Target,Content) 
values(@Id,@CreateBy,@CreateTime,1,@appId,@compNo,@type,@target,@content)";
            db.Execute(sql, new
            {
                Id = Utils.GetGuid(),
                CreateBy = userName,
                CreateTime = DateTime.Now,
                appId,
                compNo,
                type,
                target,
                content
            });
        }
    }
}
