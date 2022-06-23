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

namespace Known.Core
{
    partial class PlatformService
    {
        public List<TaskInfo> GetPendingTasks(string compNo)
        {
            return Repository.GetPendingTasks(Database, compNo);
        }

        public Result AddTask(Database db, TaskInfo task)
        {
            var info = Repository.GetLastTask(db, task.BizId);
            if (info != null && info.Status != TaskInfo.Success && info.Status != TaskInfo.Failed)
                return Result.Error("当前任务正在执行，无需重复添加！");

            var user = CurrentUser;
            task.Id = Utils.GetGuid();
            task.AppId = user.AppId;
            task.CreateBy = user.UserName;
            task.CreateTime = DateTime.Now;
            Repository.AddTask(db, task);
            return Result.Success(Language.XXSuccess.Format(Language.Save));
        }

        public Result SaveTaskStatus(Database db, string id, string status, string note)
        {
            if (status != TaskInfo.Running &&
                status != TaskInfo.Success &&
                status != TaskInfo.Failed)
                return Result.Error("状态参数不正确！");

            var task = new TaskInfo { Id = id, Status = status, Note = note };
            if (status == TaskInfo.Running)
                task.BeginTime = DateTime.Now;
            else
                task.EndTime = DateTime.Now;
            Repository.UpdateTask(db, task);
            return Result.Success(Language.XXSuccess.Format(Language.Save));
        }
    }

    partial class PlatformRepository
    {
        public List<TaskInfo> GetPendingTasks(Database db, string compNo)
        {
            var sql = $"select * from SysTask where CompNo=@compNo and Status='{TaskInfo.Pending}' order by CreateTime";
            return db.QueryList<TaskInfo>(sql, new { compNo });
        }

        public TaskInfo GetLastTask(Database db, string bizId)
        {
            var sql = "select * from SysTask where BizId=@bizId order by CreateTime desc";
            return db.Query<TaskInfo>(sql, new { bizId });
        }

        public void AddTask(Database db, TaskInfo task)
        {
            var sql = $@"
insert into SysTask(Id,CreateBy,CreateTime,Version,AppId,CompNo,BizId,Type,Name,Target,Status) 
values(@Id,@CreateBy,@CreateTime,1,@AppId,@CompNo,@BizId,@Type,@Name,@Target,'{TaskInfo.Pending}')";
            db.Execute(sql, task);
        }

        public void UpdateTask(Database db, TaskInfo task)
        {
            var beginTime = task.BeginTime.HasValue ? ",BeginTime=@BeginTime" : "";
            var endTime = task.EndTime.HasValue ? ",EndTime=@EndTime" : "";
            var sql = $"update SysTask set Status=@Status,Note=@Note{beginTime}{endTime} where Id=@Id";
            db.Execute(sql, task);
        }
    }
}
