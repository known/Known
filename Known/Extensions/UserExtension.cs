namespace Known.Extensions;

public static class UserExtension
{
    public static Task SendMessageAsync(this UserInfo user, Database db, string toUser, string subject, string content, string filePath = null, string bizId = null)
    {
        return user.SendMessageAsync(db, Constants.UMLGeneral, toUser, subject, content, filePath, bizId);
    }

    public static Task SendUrgentMessageAsync(this UserInfo user, Database db, string toUser, string subject, string content, string filePath = null, string bizId = null)
    {
        return user.SendMessageAsync(db, Constants.UMLUrgent, toUser, subject, content, filePath, bizId);
    }

    private static Task SendMessageAsync(this UserInfo user, Database db, string level, string toUser, string subject, string content, string filePath = null, string bizId = null)
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
        return db.SaveAsync(model);
    }
}