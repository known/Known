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
        public string GetMaxFormNo(Database db, string appId, string compNo, string ruleCode)
        {
            var rules = Repository.GetNoRuleInfos(db, appId, compNo, ruleCode);
            if (rules == null || rules.Count == 0)
                Check.Throw($"{ruleCode}编号规则未配置！");

            var prefix = string.Empty;
            var length = 0;
            foreach (var item in rules)
            {
                switch (item.Type)
                {
                    case "固定值":
                        prefix += item.Value;
                        break;
                    case "日期值":
                        prefix += DateTime.Now.ToString(item.Value);
                        break;
                    case "流水号":
                        length = Utils.ConvertTo<int>(item.Value);
                        break;
                }
            }

            var ruleNo = Repository.GetMaxRuleNo(db, appId, compNo, prefix);
            return GetMaxFormNo(ruleNo, prefix, length);
        }

        public string GetMaxFormNo(string maxFormNo, string prefix, int length)
        {
            var maxNo = 1;
            if (!string.IsNullOrEmpty(maxFormNo))
            {
                maxNo = int.Parse(maxFormNo.Replace(prefix, "").TrimStart('0')) + 1;
            }

            return string.Format("{0}{1:D" + length + "}", prefix, maxNo);
        }

        public string GenerateFormNo(Database db, string appId, string compNo, string ruleCode)
        {
            var formNo = GetMaxFormNo(db, appId, compNo, ruleCode);
            Repository.SaveRuleNoData(db, appId, compNo, ruleCode, formNo);
            return formNo;
        }

        public void DeleteRuleNo(Database db, string ruleId, string ruleNo)
        {
            Repository.DeleteRuleNoData(db, ruleId, ruleNo);
        }
    }

    partial interface IPlatformRepository
    {
        List<NoRuleInfo> GetNoRuleInfos(Database db, string appId, string compNo, string ruleCode);
        string GetMaxRuleNo(Database db, string appId, string compNo, string prefix);
        void SaveRuleNoData(Database db, string appId, string compNo, string ruleId, string ruleNo);
        void DeleteRuleNoData(Database db, string ruleId, string ruleNo);
    }

    partial class PlatformRepository
    {
        public List<NoRuleInfo> GetNoRuleInfos(Database db, string appId, string compNo, string ruleCode)
        {
            var sql = "select RuleData from SysNoRule where AppId=@appId and CompNo=@compNo and Code=@ruleCode";
            var data = db.Scalar<string>(sql, new { appId, compNo, ruleCode });
            return Utils.FromJson<List<NoRuleInfo>>(data);
        }

        public string GetMaxRuleNo(Database db, string appId, string compNo, string prefix)
        {
            var sql = "select max(RuleNo) from SysNoRuleData where AppId=@appId and CompNo=@compNo and RuleNo like @prefix";
            return db.Scalar<string>(sql, new { appId, compNo, prefix = $"{prefix}%" });
        }

        public void SaveRuleNoData(Database db, string appId, string compNo, string ruleId, string ruleNo)
        {
            var sql = "insert into SysNoRuleData(AppId,CompNo,RuleId,RuleNo) values(@appId,@compNo,@ruleId,@ruleNo)";
            db.Execute(sql, new { appId, compNo, ruleId, ruleNo });
        }

        public void DeleteRuleNoData(Database db, string ruleId, string ruleNo)
        {
            var sql = "delete from SysNoRuleData where RuleId=@ruleId and RuleNo=@ruleNo";
            db.Execute(sql, new { ruleId, ruleNo });
        }
    }
}
