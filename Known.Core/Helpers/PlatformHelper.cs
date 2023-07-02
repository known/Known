namespace Known.Core.Helpers;

public sealed class PlatformHelper
{
    private PlatformHelper() { }

    public static Action<Database, SysOrganization> Organization { get; set; }
    public static Action<Database, UserInfo> User { get; set; }
    public static Func<Database, List<CodeInfo>> UserDatas { get; set; }
    public static Func<Database, List<CodeInfo>> Dictionary { get; set; }
    public static Func<Database, SystemInfo, Result> CheckSystem { get; set; }
    public static Func<Database, SysUser, Result> CheckUser { get; set; }
    public static Func<string> ProductId { get; set; }
    public static Func<InstallInfo, Result> UpdateKey { get; set; }

    internal static string GetProductId()
    {
        if (ProductId != null)
            return ProductId.Invoke();

        var mac = Platform.GetMacAddress();
        var id = mac.Split(':').Select(m => Convert.ToInt32(m, 16)).Sum();
        return $"PM-{Config.AppId}-{id:000000}";
    }

    internal static void SetBizOrganization(Database db, SysOrganization entity)
    {
        Organization?.Invoke(db, new SysOrganization
        {
            CompNo = db.User.CompNo,
            Code = entity.Code,
            Name = entity.Name,
            Note = entity.Note
        });
    }

    internal static void SetBizUser(Database db, SysUser entity)
    {
        var info = Utils.MapTo<UserInfo>(entity);
        User?.Invoke(db, info);
    }
}