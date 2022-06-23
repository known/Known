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
        //View
        public List<CodeInfo> GetNoRules()
        {
            var user = CurrentUser;
            var isDev = Platform.CheckDevUser(user) && user.AppId == DevId;
            var rules = isDev
                      ? Database.QueryList<SysNoRule>().OrderBy(n => n.AppId).ThenBy(n => n.Code).ToList()
                      : Repository.GetNoRules(Database, user.AppId, user.CompNo);

            return rules.Select(r =>
            {
                var name = isDev ? $"{r.AppId}-{r.Name}" : r.Name;
                return new CodeInfo(r.Code, name, r);
            }).ToList();
        }

        public Result DeleteNoRules(string data)
        {
            var ids = Utils.FromJson<string[]>(data);
            var entities = Database.QueryListById<SysNoRule>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            foreach (var item in entities)
            {
                if (Repository.ExistsNoRuleData(Database, item.Id))
                    return Result.Error($"{item.Name}已生成过编号，不能删除！");
            }

            return Database.Transaction(Language.Delete, db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                }
            });
        }

        //Form
        public Result SaveNoRule(string data)
        {
            var model = Utils.ToDynamic(data);
            var entity = Database.QueryById<SysNoRule>((string)model.Id);
            if (entity == null)
                entity = new SysNoRule { CompNo = CurrentUser.CompNo };

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return vr;

            Database.Save(entity);
            return Result.Success(Language.SaveSuccess, entity.Id);
        }
    }

    partial class SystemRepository
    {
        public List<SysNoRule> GetNoRules(Database db, string appId, string compNo)
        {
            var sql = "select * from SysNoRule where AppId=@appId and CompNo=@compNo order by Code";
            return db.QueryList<SysNoRule>(sql, new { appId, compNo });
        }

        public bool ExistsNoRuleData(Database db, string ruleId)
        {
            var sql = "select count(*) from SysNoRuleData where RuleId=@ruleId";
            return db.Scalar<int>(sql, new { ruleId }) > 0;
        }
    }
}