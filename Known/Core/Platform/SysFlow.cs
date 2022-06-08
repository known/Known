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
using Known.Entities;

namespace Known.Core
{
    partial class PlatformService
    {
        public SysFlow GetFlowInfo(Database db, string bizId)
        {
            return Repository.GetFlowInfo(db, bizId);
        }

        public UserInfo GetFlowStepUser(Database db, string flowCode, string stepCode)
        {
            var user = CurrentUser;
            return Repository.GetFlowStepUser(db, user.CompNo, user.AppId, flowCode, stepCode);
        }

        public void DeleteFlow(Database db, string bizId)
        {
            Repository.DeleteFlowLogs(db, bizId);
            Repository.DeleteFlow(db, bizId);
        }

        public void CreateFlow(Database db, FlowBizInfo biz)
        {
            var user = CurrentUser;
            var stepName = "创建流程";
            if (biz.CurrUser == null)
            {
                biz.CurrUser = user;
            }

            var info = new SysFlow
            {
                Id = Utils.GetGuid(),
                CompNo = user.CompNo,
                AppId = user.AppId,
                FlowCode = biz.FlowCode,
                FlowName = biz.FlowName,
                FlowStatus = FlowActionInfo.Open,
                BizId = biz.BizId,
                BizName = biz.BizName,
                BizUrl = biz.BizUrl,
                BizStatus = biz.BizStatus,
                CurrStep = stepName,
                CurrById = biz.CurrUser.UserName,
                CurrBy = biz.CurrUser.Name
            };

            Repository.AddFlowInfo(db, info);
            AddFlowLog(db, user, info, stepName, "创建", $"{biz.FlowName}流程创建成功，{biz.BizName}");
        }

        public void AssignFlow(Database db, string bizId, UserInfo next, string note = null)
        {
            if (next == null)
                Check.Throw("下一步执行人不能为空！");

            var info = Repository.GetFlowInfo(db, bizId);
            if (info == null)
                Check.Throw("流程未创建，无法执行！");

            var user = CurrentUser;
            if (info.CurrById != user.UserName)
                Check.Throw($"无权分配[{info.CurrBy}]的单据！");

            var stepName = info.CurrStep;
            SetCurrStep(info, stepName, next);

            var noteText = $"指派给[{info.CurrBy}]";
            if (!string.IsNullOrEmpty(note))
                noteText += $"，{note}";

            Repository.UpdateFlowInfo(db, info);
            AddFlowLog(db, user, info, "任务分配", "分配", noteText);
        }

        public void SubmitFlow(Database db, string bizId, string bizStatus, string stepName, UserInfo next = null, string note = null)
        {
            var info = Repository.GetFlowInfo(db, bizId);
            if (info == null)
                Check.Throw("流程未创建，无法执行！");

            var user = CurrentUser;
            if (info.CurrById != user.UserName)
                Check.Throw($"无权操作[{info.CurrBy}]的单据！");

            SetCurrToPrevStep(info);

            if (!string.IsNullOrEmpty(info.NextById))
            {
                SetNextToCurrStep(info);
            }
            else
            {
                if (next == null)
                    Check.Throw("下一步执行人不能为空！");

                SetCurrStep(info, stepName, next);
            }

            info.BizStatus = bizStatus;
            var noteText = $"提交给[{info.CurrBy}]";
            if (!string.IsNullOrEmpty(note))
                noteText += $"，{note}";

            Repository.UpdateFlowInfo(db, info);
            AddFlowLog(db, user, info, stepName, Language.Submit, noteText);
        }

        public void RevokeFlow(Database db, string bizId, string bizStatus, string note)
        {
            if (string.IsNullOrEmpty(note))
                Check.Throw("撤回原因不能为空！");

            var info = Repository.GetFlowInfo(db, bizId);
            if (info == null)
                Check.Throw("流程未创建，无法执行！");

            var user = CurrentUser;
            if (info.PrevById != user.UserName)
                Check.Throw($"无权撤回[{info.PrevBy}]的单据！");

            SetCurrToNextStep(info);
            SetPrevToCurrStep(info);
            info.BizStatus = bizStatus;

            PlatformAction.SetBizFlowStatus(db, info, note);
            Repository.UpdateFlowInfo(db, info);
            AddFlowLog(db, user, info, "撤回流程", Language.Revoke, note);
        }

        public void ReturnFlow(Database db, string bizId, string bizStatus, string note)
        {
            if (string.IsNullOrEmpty(note))
                Check.Throw("退回原因不能为空！");

            var info = Repository.GetFlowInfo(db, bizId);
            if (info == null)
                Check.Throw("流程未创建，无法执行！");

            var user = CurrentUser;
            if (info.CurrById != user.UserName)
                Check.Throw($"无权操作[{info.CurrBy}]的单据！");

            SetCurrToNextStep(info);
            SetPrevToCurrStep(info);

            info.BizStatus = bizStatus;
            var noteText = $"退回给[{info.CurrBy}]";
            if (!string.IsNullOrEmpty(note))
                noteText += $"，{note}";

            PlatformAction.SetBizFlowStatus(db, info, note);
            Repository.UpdateFlowInfo(db, info);
            AddFlowLog(db, user, info, "退回流程", Language.Return, noteText);
        }

        public void OverFlow(Database db, string bizId, string bizStatus, string note = null)
        {
            var info = Repository.GetFlowInfo(db, bizId);
            if (info == null)
                Check.Throw("流程未创建，无法执行！");

            var user = CurrentUser;
            if (info.CurrById != user.UserName)
                Check.Throw($"无权操作[{info.CurrBy}]的单据！");

            info.BizStatus = bizStatus;
            info.FlowStatus = FlowActionInfo.Over;

            PlatformAction.SetBizFlowStatus(db, info, note);
            Repository.UpdateFlowInfo(db, info);
            AddFlowLog(db, user, info, "结束流程", "结束", note);
        }

        public void StopFlow(Database db, string bizId, string note)
        {
            if (string.IsNullOrEmpty(note))
                Check.Throw("终止原因不能为空！");

            var info = Repository.GetFlowInfo(db, bizId);
            if (info == null)
                Check.Throw("流程未创建，无法执行！");

            var user = CurrentUser;
            if (info.CurrById != user.UserName)
                Check.Throw($"无权分配[{info.CurrBy}]的单据！");

            info.BizStatus = "已终止";
            info.FlowStatus = FlowActionInfo.Stop;

            PlatformAction.SetBizFlowStatus(db, info, note);
            Repository.UpdateFlowInfo(db, info);
            AddFlowLog(db, user, info, "终止流程", "终止", note);
        }

        public void AddFlowLog(Database db, string bizId, string stepName, string status, string note)
        {
            var info = Repository.GetFlowInfo(db, bizId);
            if (info == null)
                Check.Throw("流程未创建，无法执行！");

            var user = CurrentUser;
            AddFlowLog(db, user, info, stepName, status, note);
        }

        private static void SetPrevToCurrStep(SysFlow info)
        {
            info.CurrStep = info.PrevStep;
            info.CurrById = info.PrevById;
            info.CurrBy = info.PrevBy;
        }

        private static void SetNextToCurrStep(SysFlow info)
        {
            info.CurrStep = info.NextStep;
            info.CurrById = info.NextById;
            info.CurrBy = info.NextBy;
        }

        private static void SetCurrToPrevStep(SysFlow info)
        {
            info.PrevStep = info.CurrStep;
            info.PrevById = info.CurrById;
            info.PrevBy = info.CurrBy;
        }

        private static void SetCurrToNextStep(SysFlow info)
        {
            info.NextStep = info.CurrStep;
            info.NextById = info.CurrById;
            info.NextBy = info.CurrBy;
        }

        private static void SetCurrStep(SysFlow info, string stepName, UserInfo user)
        {
            info.CurrStep = stepName;
            info.CurrById = user.UserName;
            info.CurrBy = user.Name;
        }

        private static void AddFlowLog(Database db, UserInfo user, SysFlow info, string stepName, string result, string note)
        {
            Repository.AddFlowLog(db, new SysFlowLog
            {
                Id = Utils.GetGuid(),
                CompNo = info.CompNo,
                AppId = info.AppId,
                BizId = info.BizId,
                StepName = stepName,
                ExecuteById = user.UserName,
                ExecuteBy = user.Name,
                ExecuteTime = DateTime.Now,
                Result = result,
                Note = note
            });
        }
    }

    partial interface IPlatformRepository
    {
        SysFlow GetFlowInfo(Database db, string bizId);
        UserInfo GetFlowStepUser(Database db, string appId, string compNo, string flowCode, string stepCode);
        void AddFlowInfo(Database db, SysFlow info);
        void UpdateFlowInfo(Database db, SysFlow info);
        void AddFlowLog(Database db, SysFlowLog info);
        void DeleteFlow(Database db, string bizId);
        void DeleteFlowLogs(Database db, string bizId);
    }

    partial class PlatformRepository
    {
        public SysFlow GetFlowInfo(Database db, string bizId)
        {
            var sql = "select * from SysFlow where BizId=@bizId";
            return db.Query<SysFlow>(sql, new { bizId });
        }

        public UserInfo GetFlowStepUser(Database db, string appId, string compNo, string flowCode, string stepCode)
        {
            var sql = @"
select u.* from SysFlowStep s,SysUser u 
where s.OperateBy=u.UserName and s.AppId=@appId and s.CompNo=@compNo 
  and s.FlowCode=@flowCode and s.StepCode=@stepCode";
            return db.Query<UserInfo>(sql, new { appId, compNo, flowCode, stepCode });
        }

        public void AddFlowInfo(Database db, SysFlow info)
        {
            db.Insert(info);
        }

        public void UpdateFlowInfo(Database db, SysFlow info)
        {
            db.Save(info);
        }

        public void AddFlowLog(Database db, SysFlowLog info)
        {
            db.Insert(info);
        }

        public void DeleteFlow(Database db, string bizId)
        {
            var sql = "delete from SysFlow where BizId=@bizId";
            db.Execute(sql, new { bizId });
        }

        public void DeleteFlowLogs(Database db, string bizId)
        {
            var sql = "delete from SysFlowLog where BizId=@bizId";
            db.Execute(sql, new { bizId });
        }
    }
}
