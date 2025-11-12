namespace Known;

/// <summary>
/// 登录用户信息类。
/// </summary>
[Table("SysUser")]
public class UserInfo
{
    /// <summary>
    /// 取得或设置用户ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置用户登录名。
    /// </summary>
    [DisplayName("用户名")]
    public string UserName { get; set; }

    /// <summary>
    /// 取得或设置用户姓名。
    /// </summary>
    [DisplayName("姓名")]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置用户英文名。
    /// </summary>
    [DisplayName("英文名")]
    public string EnglishName { get; set; }

    /// <summary>
    /// 取得或设置用户性别。
    /// </summary>
    [DisplayName("性别")]
    public string Gender { get; set; }

    /// <summary>
    /// 取得或设置用户固定电话。
    /// </summary>
    [DisplayName("固定电话")]
    public string Phone { get; set; }

    /// <summary>
    /// 取得或设置用户移动电话。
    /// </summary>
    [DisplayName("移动电话")]
    public string Mobile { get; set; }

    /// <summary>
    /// 取得或设置用户Email。
    /// </summary>
    [DisplayName("电子邮箱")]
    public string Email { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [DisplayName("状态")]
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置用户备注信息。
    /// </summary>
    [DisplayName("备注")]
    public string Note { get; set; }

    /// <summary>
    /// 取得或设置首次登录时间。
    /// </summary>
    public DateTime? FirstLoginTime { get; set; }

    /// <summary>
    /// 取得或设置首次登录IP。
    /// </summary>
    public string FirstLoginIP { get; set; }

    /// <summary>
    /// 取得或设置用户最近登录时间。
    /// </summary>
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 取得或设置用户登录IP地址。
    /// </summary>
    public string LastLoginIP { get; set; }

    /// <summary>
    /// 取得或设置用户身份Token。
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// 取得或设置用户头像URL。
    /// </summary>
    public string AvatarUrl { get; set; }

    /// <summary>
    /// 取得或设置系统ID。
    /// </summary>
    public string AppId { get; set; }

    /// <summary>
    /// 取得或设置系统名称。
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// 取得或设置用户所属企业编码。
    /// </summary>
    public string CompNo { get; set; }

    /// <summary>
    /// 取得或设置用户所属企业名称。
    /// </summary>
    public string CompName { get; set; }

    /// <summary>
    /// 取得或设置用户所属组织编码。
    /// </summary>
    public string OrgNo { get; set; }

    /// <summary>
    /// 取得或设置用户所属组织名称。
    /// </summary>
    public string OrgName { get; set; }

    /// <summary>
    /// 取得或设置用户角色。
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// 取得或设置用户登录站别。
    /// </summary>
    public string Station { get; set; }

    /// <summary>
    /// 取得或设置用户会话Id。
    /// </summary>
    [DisplayName("会话ID")]
    public string SessionId { get; set; }

    /// <summary>
    /// 取得或设置客户端唯一标识。
    /// </summary>
    [DisplayName("客户端ID")]
    public string ClientId { get; set; }

    /// <summary>
    /// 取得或设置用户微信OpenId。
    /// </summary>
    public string OpenId { get; set; }

    /// <summary>
    /// 取得或设置是否是租户。
    /// </summary>
    public bool IsTenant { get; set; }

    /// <summary>
    /// 取得或设置用户部门名称。
    /// </summary>
    public string Department { get; set; }

    /// <summary>
    /// 取得或设置用户水印文字。
    /// </summary>
    public string Watermark { get; set; }

    /// <summary>
    /// 取得或设置用户活动状态，5分钟之内没活动显示离开。
    /// </summary>
    [DisplayName("状态")]
    public string Status => (DateTime.Now - LastTime).TotalMinutes < 5 ? "在线" : "离开";

    /// <summary>
    /// 取得或设置开始时间。
    /// </summary>
    [DisplayName("开始时间")]
    public DateTime StartTime { get; set; }

    /// <summary>
    /// 取得或设置最近活动时间。
    /// </summary>
    [DisplayName("最近活动时间")]
    public DateTime LastTime { get; set; }

    /// <summary>
    /// 取得或设置最近活动页面。
    /// </summary>
    [DisplayName("最近活动页面")]
    public string LastPage { get; set; }

    /// <summary>
    /// 取得在线持续时间。
    /// </summary>
    [DisplayName("持续时间")]
    public string Duration => (LastTime - StartTime).ToString(@"dd\.hh\:mm\:ss");

    /// <summary>
    /// 取得或设置IP地址。
    /// </summary>
    [DisplayName("IP地址")]
    public string IPAddress { get; set; }

    /// <summary>
    /// 取得或设置IP所在地。
    /// </summary>
    [DisplayName("IP所在地")]
    public string IPLocal { get; set; }

    /// <summary>
    /// 取得或设置客户端代理信息。
    /// </summary>
    [DisplayName("代理信息")]
    public string Agent { get; set; }

    /// <summary>
    /// 取得或设置操作系统。
    /// </summary>
    [DisplayName("操作系统")]
    public string OSName { get; set; }

    /// <summary>
    /// 取得或设置设备。
    /// </summary>
    [DisplayName("设备")]
    public string Device { get; set; }

    /// <summary>
    /// 取得或设置浏览器。
    /// </summary>
    [DisplayName("浏览器")]
    public string Browser { get; set; }

    /// <summary>
    /// 获取用户是否是系统或租户管理员。
    /// </summary>
    /// <returns></returns>
    public bool IsAdmin() => IsSystemAdmin() || IsTenantAdmin();

    /// <summary>
    /// 获取用户是否是系统超级管理员。
    /// </summary>
    /// <returns></returns>
    public bool IsSystemAdmin() => UserName.Equals(Constants.SysUserName, StringComparison.CurrentCultureIgnoreCase);

    /// <summary>
    /// 获取用户是否是租户管理员。
    /// </summary>
    /// <returns></returns>
    public bool IsTenantAdmin() => CompNo == UserName;

    /// <summary>
    /// 判断用户是否是该角色。
    /// </summary>
    /// <param name="role">角色名。</param>
    /// <returns>返回是否是该角色。</returns>
    public bool IsRole(string role)
    {
        if (string.IsNullOrWhiteSpace(Role))
            return false;

        if (IsAdmin())
            return true;

        return Role == role;
    }

    /// <summary>
    /// 判断用户是否有该角色。
    /// </summary>
    /// <param name="role">角色名。</param>
    /// <returns>返回是否有该角色。</returns>
    public bool HasRole(string role)
    {
        if (string.IsNullOrWhiteSpace(Role))
            return false;

        if (IsAdmin())
            return true;

        return Role.Split(',').Contains(role);
    }

    /// <summary>
    /// 判断用户是否在角色中。
    /// </summary>
    /// <param name="roles"></param>
    /// <returns>返回是否在角色中。</returns>
    public bool InRole(string[] roles)
    {
        if (roles == null || roles.Length == 0)
            return true;

        foreach (string role in roles)
        {
            if (HasRole(role))
                return true;
        }
        return false;
    }

    /// <summary>
    /// 将登录用户转换成Claims认证对象。
    /// </summary>
    /// <param name="authType">认证类型。</param>
    /// <returns>Claims认证对象。</returns>
    public ClaimsPrincipal ToPrincipal(string authType)
    {
        var identity = new ClaimsIdentity(authType);
        if (!string.IsNullOrWhiteSpace(Id))
            identity.AddClaim(new(ClaimTypes.NameIdentifier, Id));
        if (!string.IsNullOrWhiteSpace(UserName))
            identity.AddClaim(new(ClaimTypes.Name, UserName));
        if (!string.IsNullOrWhiteSpace(Role))
            identity.AddClaim(new(ClaimTypes.Role, Role));
        return new ClaimsPrincipal(identity);
    }
}