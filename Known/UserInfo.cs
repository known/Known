namespace Known;

/// <summary>
/// 登录用户信息类。
/// </summary>
public class UserInfo
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    public UserInfo()
    {
        Id = Utils.GetNextId();
        IsNew = true;
        Enabled = true;
        Gender = "Female";
    }

    /// <summary>
    /// 取得或设置用户ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置是否是新增实体。
    /// </summary>
    public bool IsNew { get; set; }

    /// <summary>
    /// 取得或设置用户登录名。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column(IsQuery = true, IsViewLink = true)]
    [Form(Row = 1, Column = 1)]
    public string UserName { get; set; }

    /// <summary>
    /// 取得或设置用户姓名。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column]
    [Form(Row = 1, Column = 2)]
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置用户英文名。
    /// </summary>
    [MaxLength(50)]
    [Form(Row = 2, Column = 1)]
    public string EnglishName { get; set; }

    /// <summary>
    /// 取得或设置用户性别。
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Category(nameof(GenderType))]
    [Column]
    [Form(Row = 2, Column = 2, Type = nameof(FieldType.RadioList))]
    public string Gender { get; set; }

    /// <summary>
    /// 取得或设置用户固定电话。
    /// </summary>
    [MaxLength(50)]
    [RegularExpression(RegexPattern.Phone, ErrorMessage = "固定电话格式不正确！")]
    [Form(Row = 3, Column = 1)]
    public string Phone { get; set; }

    /// <summary>
    /// 取得或设置用户移动电话。
    /// </summary>
    [MaxLength(50)]
    [RegularExpression(RegexPattern.Mobile, ErrorMessage = "移动电话格式不正确！")]
    [Column]
    [Form(Row = 3, Column = 2)]
    public string Mobile { get; set; }

    /// <summary>
    /// 取得或设置用户Email。
    /// </summary>
    [MaxLength(50)]
    [RegularExpression(RegexPattern.Email, ErrorMessage = "电子邮件格式不正确！")]
    [Column]
    [Form(Row = 4, Column = 1)]
    public string Email { get; set; }

    /// <summary>
    /// 取得或设置状态。
    /// </summary>
    [Required]
    [Column]
    [Form(Row = 4, Column = 2, Type = nameof(FieldType.Switch))]
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
    [MaxLength(50)]
    public string FirstLoginIP { get; set; }

    /// <summary>
    /// 取得或设置用户最近登录时间。
    /// </summary>
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 取得或设置用户登录IP地址。
    /// </summary>
    [MaxLength(50)]
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
    [MaxLength(50)]
    public string OrgNo { get; set; }

    /// <summary>
    /// 取得或设置用户所属组织名称。
    /// </summary>
    public string OrgName { get; set; }

    /// <summary>
    /// 取得或设置用户角色。
    /// </summary>
    [MaxLength(500)]
    [Column]
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
    /// 取得或设置用户部门名称。
    /// </summary>
    public string Department { get; set; }

    /// <summary>
    /// 取得或设置默认密码。
    /// </summary>
    public string DefaultPassword { get; set; }

    /// <summary>
    /// 取得或设置用户关联的角色ID集合。
    /// </summary>
    [Category("Roles")]
    public string[] RoleIds { get; set; }

    // 取得或设置用户关联的数据权限ID集合。
    //public string[] DataIds { get; set; }

    /// <summary>
    /// 取得或设置系统角色代码表列表。
    /// </summary>
    public List<CodeInfo> Roles { get; set; }
    //public List<CodeInfo> Datas { get; set; }

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
        if (!string.IsNullOrWhiteSpace(Id))
            identity.AddClaim(new(ClaimTypes.NameIdentifier, Id));
        if (!string.IsNullOrWhiteSpace(UserName))
            identity.AddClaim(new(ClaimTypes.Name, UserName));
        if (!string.IsNullOrWhiteSpace(Role))
            identity.AddClaim(new(ClaimTypes.Role, Role));
        return new ClaimsPrincipal(identity);
    }
}