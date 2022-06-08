using Known.Entities;

namespace Known.Core
{
    partial class SystemService
    {
        public PagingResult<SysUserMessage> QueryUserMessages(PagingCriteria criteria)
        {
            criteria.Parameter["UserId"] = CurrentUser.UserName;
            return Repository.QueryUserMessages(Database, criteria);
        }

        public Result DeleteUserMessages(string data)
        {
            var ids = Utils.FromJson<string[]>(data);
            var entities = Database.QueryListById<SysUserMessage>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            return Database.Transaction(Language.Delete, db =>
            {
                foreach (var item in entities)
                {
                    if (item.Type != Constants.UMTypeDelete)
                    {
                        item.Type = Constants.UMTypeDelete;
                        db.Save(item);
                    }
                    else
                    {
                        db.Delete(item);
                    }
                }
            });
        }

        public Result MarkUserMessages(string status, string data)
        {
            var ids = Utils.FromJson<string[]>(data);
            var user = CurrentUser;
            if (status == "全部已读")
            {
                Repository.UpdateUserMessageStatus(Database, user.UserName, Constants.UMStatusRead);
                return Result.Success("标记成功！");
            }

            var entities = Database.QueryListById<SysUserMessage>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            return Database.Transaction("标记", db =>
            {
                foreach (var item in entities)
                {
                    item.Status = status;
                    db.Save(item);
                }
            });
        }

        public Result SaveUserMessage(SysUserMessage model)
        {
            var entity = Database.QueryById<SysUserMessage>(model.Id);
            if (entity == null)
            {
                var user = CurrentUser;
                entity = new SysUserMessage { UserId = user.UserName };
            }

            entity.Type = model.Type;
            entity.MsgBy = model.MsgBy;
            entity.MsgByName = model.MsgByName;
            entity.MsgLevel = model.MsgLevel ?? "一般";
            entity.Category = model.Category;
            entity.Subject = model.Subject;
            entity.Content = model.Content;
            entity.FilePath = model.FilePath;
            entity.IsHtml = model.IsHtml ?? "否";
            entity.Status = model.Status ?? Constants.UMStatusUnread;
            entity.BizId = model.BizId;

            var vr = entity.Validate();
            if (!vr.IsValid)
                return vr;

            Database.Save(entity);
            return Result.Success(Language.SaveSuccess, entity.Id);
        }
    }

    partial interface ISystemRepository
    {
        PagingResult<SysUserMessage> QueryUserMessages(Database db, PagingCriteria criteria);
        void UpdateUserMessageStatus(Database db, string userId, string status);
    }

    partial class SystemRepository
    {
        public PagingResult<SysUserMessage> QueryUserMessages(Database db, PagingCriteria criteria)
        {
            var sql = "select * from SysUserMessage where UserId=@UserId and Type=@Type";
            return db.QueryPage<SysUserMessage>(sql, criteria);
        }

        public void UpdateUserMessageStatus(Database db, string userId, string status)
        {
            var sql = "update SysUserMessage set Status=@status where UserId=@userId";
            db.Execute(sql, new { status, userId });
        }
    }
}