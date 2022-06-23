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
        public PagingResult<SysTask> QueryTasks(PagingCriteria criteria)
        {
            SetCriteriaAppId(criteria);
            return Repository.QueryTasks(Database, criteria);
        }

        public Result PostTaskStatus(string id, string status, string note)
        {
            return Platform.SaveTaskStatus(Database, id, status, note);
        }

        public List<TaskInfo> GetPendingTasks(string appId, string compNo)
        {
            return Platform.GetPendingTasks(compNo);
        }

        public Result SaveTaskStatus(string data)
        {
            var model = Utils.FromJson<TaskStatusModel>(data);
            if (model == null)
                return Result.Error(Language.NotValidData);

            return Platform.SaveTaskStatus(Database, model.Id, model.Status, model.Note);
        }

        class TaskStatusModel
        {
            public string Id { get; set; }
            public string Status { get; set; }
            public string Note { get; set; }
        }
    }

    partial class SystemRepository
    {
        public PagingResult<SysTask> QueryTasks(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysTask where AppId=@AppId and CompNo=@CompNo";
            db.SetQuery(ref sql, criteria, QueryType.Contain, "Type");
            db.SetQuery(ref sql, criteria, QueryType.Contain, "Name");
            return db.QueryPage<SysTask>(sql, criteria);
        }
    }
}