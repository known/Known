namespace Known;

/// <summary>
/// 性别类型枚举。
/// </summary>
public enum GenderType
{
    /// <summary>
    /// 男。
    /// </summary>
    Male,
    /// <summary>
    /// 女。
    /// </summary>
    Female
}

/// <summary>
/// 登录用户信息类。
/// </summary>
public class UserInfo
{
    /// <summary>
    /// 取得或设置用户ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置用户登录名。
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 取得或设置用户姓名。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置用户英文名。
    /// </summary>
    public string EnglishName { get; set; }

    /// <summary>
    /// 取得或设置用户性别。
    /// </summary>
    public string Gender { get; set; }

    /// <summary>
    /// 取得或设置用户固定电话。
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// 取得或设置用户移动电话。
    /// </summary>
    public string Mobile { get; set; }

    /// <summary>
    /// 取得或设置用户Email。
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 取得或设置用户备注信息。
    /// </summary>
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
    /// 取得或设置用户微信OpenId。
    /// </summary>
    public string OpenId { get; set; }

    /// <summary>
    /// 取得或设置是否是租户。
    /// </summary>
    public bool IsTenant { get; set; }

    /// <summary>
    /// 获取用户是否是系统或租户管理员。
    /// </summary>
    /// <returns></returns>
    public bool IsAdmin() => IsSystemAdmin() || IsTenantAdmin();

    /// <summary>
    /// 获取用户是否是租户管理员。
    /// </summary>
    /// <returns></returns>
    public bool IsTenantAdmin() => CompNo == UserName;

    /// <summary>
    /// 获取用户是否是系统超级管理员。
    /// </summary>
    /// <returns></returns>
    public bool IsSystemAdmin() => UserName.Equals(Constants.SysUserName, StringComparison.CurrentCultureIgnoreCase);

    /// <summary>
    /// 判断用户是否是该角色。
    /// </summary>
    /// <param name="role">角色名。</param>
    /// <returns>返回是否是该角色。</returns>
    public bool IsRole(string role)
    {
        if (string.IsNullOrWhiteSpace(Role))
            return false;

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

        return Role.Contains(role);
    }

    /// <summary>
    /// 将登录用户转换成Claims认证对象。
    /// </summary>
    /// <param name="authType">认证类型。</param>
    /// <returns>Claims认证对象。</returns>
    public ClaimsPrincipal ToPrincipal(string authType)
    {
        var identity = new ClaimsIdentity(authType);
        identity.AddClaim(new(ClaimTypes.NameIdentifier, Id));
        identity.AddClaim(new(ClaimTypes.Name, UserName));
        if (!string.IsNullOrWhiteSpace(Role))
            identity.AddClaim(new(ClaimTypes.Role, Role));
        return new ClaimsPrincipal(identity);
    }
}

/// <summary>
/// 用户头像信息类。
/// </summary>
public class AvatarInfo
{
    /// <summary>
    /// 取得或设置用户ID。
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 取得或设置用户头像文件信息。
    /// </summary>
    public FileDataInfo File { get; set; }
}