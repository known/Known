namespace Known.Core.Extensions;

public static class UserExtension
{
    public static void SendMessage(this UserInfo user, Database db, string toUser, string level, string subject, string content, string filePath = null, string bizId = null)
    {
        var model = new SysMessage
        {
            UserId = toUser,
            Type = Constants.UMTypeReceive,
            MsgBy = user.Name,
            MsgLevel = level,
            Subject = subject,
            Content = content,
            FilePath = filePath,
            IsHtml = true,
            Status = Constants.UMStatusUnread,
            BizId = bizId
        };
        db.Save(model);
    }
}