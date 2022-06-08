using System.Collections.Generic;
using Known.Entities;

namespace Known.Core
{
    partial class SystemService
    {
        public List<SysFlow> GetFlowTodos()
        {
            var user = CurrentUser;
            return Repository.GetFlowTodos(Database, user.AppId, user.UserName);
        }

        public List<SysFlowLog> GetFlowLogs(string bizId)
        {
            return Repository.GetFlowLogs(Database, bizId);
        }

        public List<SysFlowStep> GetFlowSteps(string flowCode)
        {
            var user = CurrentUser;
            return Repository.GetFlowSteps(Database, user.AppId, user.CompNo, flowCode);
        }

        public SysFlowStep GetFlowStep(string flowCode, string stepCode)
        {
            var user = CurrentUser;
            return Repository.GetFlowStep(Database, user.AppId, user.CompNo, flowCode, stepCode);
        }

        public Result SaveFlowStep(string data)
        {
            var model = Utils.ToDynamic(data);
            var entity = Database.QueryById<SysFlowStep>((string)model.Id);
            if (entity == null)
                entity = new SysFlowStep();

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return vr;

            Database.Save(entity);
            return Result.Success(Language.SaveSuccess, entity.Id);
        }

        public Result AssignFlow(string bizId, string userName, string note)
        {
            var user = Platform.GetUser(userName);
            if (user == null)
                return Result.Error($"账号[{userName}]不存在！");

            return Database.Transaction("分配", db =>
            {
                Platform.AssignFlow(db, bizId, user, note);
            });
        }

        public Result SubmitFlow(string bizId, string userName, string note)
        {
            var user = Platform.GetUser(userName);
            if (user == null)
                return Result.Error($"账号[{userName}]不存在！");

            return Database.Transaction(Language.Submit, db =>
            {
                Platform.SubmitFlow(db, bizId, "已提交", "提交流程", user, note);
            });
        }

        public Result RevokeFlow(string bizId, string reason)
        {
            if (string.IsNullOrEmpty(reason))
                return Result.Error("撤回原因不能为空！");

            return Database.Transaction(Language.Revoke, db =>
            {
                Platform.RevokeFlow(db, bizId, "已撤回", reason);
            });
        }

        public Result ReturnFlow(string bizId, string bizStatus, string reason)
        {
            if (string.IsNullOrEmpty(reason))
                return Result.Error("退回原因不能为空！");

            return Database.Transaction(Language.Return, db =>
            {
                Platform.ReturnFlow(db, bizId, bizStatus, reason);
            });
        }

        public Result StopFlow(string bizId, string reason)
        {
            if (string.IsNullOrEmpty(reason))
                return Result.Error("终止原因不能为空！");

            return Database.Transaction("终止", db =>
            {
                Platform.StopFlow(db, bizId, reason);
            });
        }
    }

    partial interface ISystemRepository
    {
        List<SysFlow> GetFlowTodos(Database db, string appId, string userName);
        List<SysFlowLog> GetFlowLogs(Database db, string bizId);
        List<SysFlowStep> GetFlowSteps(Database db, string appId, string compNo, string flowCode);
        SysFlowStep GetFlowStep(Database db, string appId, string compNo, string flowCode, string stepCode);
    }

    partial class SystemRepository
    {
        public List<SysFlow> GetFlowTodos(Database db, string appId, string userName)
        {
            var sql = $"select * from SysFlow where FlowStatus='{FlowActionInfo.Open}' and AppId=@appId and CurrById=@userName order by CreateTime";
            return db.QueryList<SysFlow>(sql, new { appId, userName });
        }

        public List<SysFlowLog> GetFlowLogs(Database db, string bizId)
        {
            var sql = "select * from SysFlowLog where BizId=@bizId order by ExecuteTime";
            return db.QueryList<SysFlowLog>(sql, new { bizId });
        }

        public List<SysFlowStep> GetFlowSteps(Database db, string appId, string compNo, string flowCode)
        {
            var sql = "select * from SysFlowStep where AppId=@appId and CompNo=@compNo and FlowCode=@flowCode";
            return db.QueryList<SysFlowStep>(sql, new { appId, compNo, flowCode });
        }

        public SysFlowStep GetFlowStep(Database db, string appId, string compNo, string flowCode, string stepCode)
        {
            var sql = "select * from SysFlowStep where AppId=@appId and CompNo=@compNo and FlowCode=@flowCode and StepCode=@stepCode";
            return db.Query<SysFlowStep>(sql, new { appId, compNo, flowCode, stepCode });
        }
    }
}