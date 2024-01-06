using Known.Entities;

namespace Known;

public enum GenderType { Female, Male }

public class UserInfo
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string EnglishName { get; set; }
    public string Gender { get; set; }
    public string Phone { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
    public string Note { get; set; }
    public DateTime? LastLoginTime { get; set; }
    public string LastLoginIP { get; set; }
    public string Token { get; set; }
    public string AvatarUrl { get; set; }
    public string AppId { get; set; }
    public string AppName { get; set; }
    public string CompNo { get; set; }
    public string CompName { get; set; }
    public string OrgNo { get; set; }
    public string OrgName { get; set; }
    public string Role { get; set; }
    internal bool IsTenant { get; set; }
    internal bool IsAdmin => IsSystemAdmin() || IsTenantAdmin();
    private bool IsSystemAdmin() => UserName.Equals(Constants.SysUserName, StringComparison.CurrentCultureIgnoreCase);
    internal bool IsTenantAdmin() => CompNo == UserName;

    public Task SendMessageAsync(Database db, string toUser, string subject, string content, bool isUrgent = false, string filePath = null, string bizId = null)
    {
        var level = isUrgent ? Constants.UMLUrgent : Constants.UMLGeneral;
        return SendMessageAsync(db, level, toUser, subject, content, filePath, bizId);
    }

    private Task SendMessageAsync(Database db, string level, string toUser, string subject, string content, string filePath = null, string bizId = null)
    {
        var model = new SysMessage
        {
            UserId = toUser,
            Type = Constants.UMTypeReceive,
            MsgBy = Name,
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