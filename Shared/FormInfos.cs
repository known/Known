namespace Known;

/// <summary>
/// 系统安装信息类。
/// </summary>
public class InstallInfo
{
    /// <summary>
    /// 取得或设置系统是否已经安装。
    /// </summary>
    public bool IsInstalled { get; set; }

    /// <summary>
    /// 取得或设置是否需要配置数据库连接。
    /// </summary>
    public bool IsDatabase { get; set; }

    /// <summary>
    /// 取得或设置企业编码。
    /// </summary>
    [DisplayName("企业编码")]
    public string CompNo { get; set; }

    /// <summary>
    /// 取得或设置企业名称。
    /// </summary>
    [DisplayName("企业名称")]
    public string CompName { get; set; }

    /// <summary>
    /// 取得或设置系统名称。
    /// </summary>
    [DisplayName("系统名称")]
    public string AppName { get; set; }

    /// <summary>
    /// 取得或设置产品ID。
    /// </summary>
    [DisplayName("产品ID")]
    public string ProductId { get; set; }

    /// <summary>
    /// 取得或设置产品Key。
    /// </summary>
    [DisplayName("产品Key")]
    public string ProductKey { get; set; }

    /// <summary>
    /// 取得或设置管理员用户名。
    /// </summary>
    [DisplayName("管理员账号")]
    public string AdminName { get; set; }

    /// <summary>
    /// 取得或设置管理员密码。
    /// </summary>
    [DisplayName("管理员密码")]
    public string AdminPassword { get; set; }

    /// <summary>
    /// 取得或设置管理员确认密码。
    /// </summary>
    [DisplayName("确认密码")]
    public string Password1 { get; set; }

    /// <summary>
    /// 取得或设置数据库信息列表。
    /// </summary>
    public List<ConnectionInfo> Connections { get; set; }
}

/// <summary>
/// 登录表单信息类。
/// </summary>
public class LoginFormInfo
{
    /// <summary>
    /// 取得或设置用户名。
    /// </summary>
    [Required]
    [DisplayName("用户名")]
    public string UserName { get; set; }

    /// <summary>
    /// 取得或设置密码。
    /// </summary>
    [Required]
    [DisplayName("密码")]
    public string Password { get; set; }

    /// <summary>
    /// 取得或设置手机号，用于手机验证码登录。
    /// </summary>
    [Required]
    [DisplayName("手机号")]
    public string PhoneNo { get; set; }

    /// <summary>
    /// 取得或设置手机验证码。
    /// </summary>
    [Required]
    [DisplayName("手机验证码")]
    public string PhoneCode { get; set; }

    /// <summary>
    /// 取得或设置图片验证码。
    /// </summary>
    [Required]
    [DisplayName("验证码")]
    public string Captcha { get; set; }

    /// <summary>
    /// 取得或设置当前登录的站别，用于多站别系统。
    /// </summary>
    [Required]
    [DisplayName("站别")]
    public string Station { get; set; }

    /// <summary>
    /// 取得或设置是否记住用户名。
    /// </summary>
    public bool Remember { get; set; }

    /// <summary>
    /// 取得或设置是否移动端登录。
    /// </summary>
    public bool IsMobile { get; set; }

    /// <summary>
    /// 取得或设置登录IP地址。
    /// </summary>
    public string IPAddress { get; set; }

    /// <summary>
    /// 取得或设置登录窗体标签键。
    /// </summary>
    public string TabKey { get; set; }
}

/// <summary>
/// 注册表单信息类。
/// </summary>
public class RegisterFormInfo
{
    /// <summary>
    /// 取得或设置用户名。
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 取得或设置密码。
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 取得或设置确认密码。
    /// </summary>
    public string Password1 { get; set; }

    /// <summary>
    /// 取得或设置图片验证码。
    /// </summary>
    public string Captcha { get; set; }

    /// <summary>
    /// 取得或设置登录IP地址。
    /// </summary>
    public string IPAddress { get; set; }
}

/// <summary>
/// 用户个性化设置表单信息类，用于记忆高级查询条件、栏位设置等。
/// </summary>
public class SettingFormInfo
{
    /// <summary>
    /// 取得或设置设置业务类型，如高级查询、表格设置等。
    /// </summary>
    public string BizType { get; set; }

    /// <summary>
    /// 取得或设置个性化设置的数据对象。
    /// </summary>
    public object BizData { get; set; }
}

/// <summary>
/// 上传附件表单信息类。
/// </summary>
/// <typeparam name="TModel">表单数据对象类型。</typeparam>
public class UploadInfo<TModel>
{
    /// <summary>
    /// 构造函数，创建一个上传附件表单信息类的实例。
    /// </summary>
    public UploadInfo() { }

    /// <summary>
    /// 构造函数，创建一个上传附件表单信息类的实例。
    /// </summary>
    /// <param name="model">表单数据对象。</param>
    public UploadInfo(TModel model)
    {
        Model = model;
        Files = [];
    }

    /// <summary>
    /// 取得或设置页面模块ID。
    /// </summary>
    public string PageId { get; set; }

    /// <summary>
    /// 取得或设置页面插件ID。
    /// </summary>
    public string PluginId { get; set; }

    /// <summary>
    /// 取得或设置表单数据对象。
    /// </summary>
    public TModel Model { get; set; }

    /// <summary>
    /// 取得或设置表单挂载的附件数据字典。
    /// </summary>
    public Dictionary<string, List<FileDataInfo>> Files { get; set; }

    /// <summary>
    /// 判断表单是否有指定Key的附件。
    /// </summary>
    /// <param name="key">附件键。</param>
    /// <returns></returns>
    public bool HasFile(string key)
    {
        if (Files == null)
            return false;

        if (!Files.TryGetValue(key, out List<FileDataInfo> value))
            return false;

        return value.Count > 0;
    }
}

/// <summary>
/// 附件表单信息类。
/// </summary>
public class FileFormInfo
{
    /// <summary>
    /// 取得或设置附件类别。
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置附件关联的业务数据ID。
    /// </summary>
    public string BizId { get; set; }

    /// <summary>
    /// 取得或设置附件关联的业务名称。
    /// </summary>
    public string BizName { get; set; }

    /// <summary>
    /// 取得或设置附件关联的业务类型。
    /// </summary>
    public string BizType { get; set; }

    /// <summary>
    /// 取得或设置附件关联的业务路径。
    /// </summary>
    public string BizPath { get; set; }
}

/// <summary>
/// 导入信息类。
/// </summary>
public class ImportInfo
{
    /// <summary>
    /// 取得或设置页面ID。
    /// </summary>
    public string PageId { get; set; }

    /// <summary>
    /// 取得或设置插件ID。
    /// </summary>
    public string PluginId { get; set; }

    /// <summary>
    /// 取得或设置页面名称。
    /// </summary>
    public string PageName { get; set; }

    /// <summary>
    /// 取得或设置导入实体类型。
    /// </summary>
    public Type EntityType { get; set; }

    /// <summary>
    /// 取得或设置实体数据是否是字典类型。
    /// </summary>
    public bool IsDictionary { get; set; }

    /// <summary>
    /// 取得或设置导入参数。
    /// </summary>
    public string Param { get; set; }

    /// <summary>
    /// 取得或设置导入成功回调。
    /// </summary>
    public Action OnSuccess { get; set; }
}

/// <summary>
/// 数据导入表单信息类。
/// </summary>
public class ImportFormInfo : FileFormInfo
{
    /// <summary>
    /// 取得或设置导入名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置是否是异步导入。
    /// </summary>
    public bool IsAsync { get; set; }

    /// <summary>
    /// 取得或设置导入是否已完成，默认True。
    /// </summary>
    public bool IsFinished { get; set; } = true;

    /// <summary>
    /// 取得或设置异步导入反馈的提示信息。
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 取得或设置导入校验的错误信息。
    /// </summary>
    public string Error { get; set; }

    ///// <summary>
    ///// 根据模型类型获取导入栏位名称列表，适用于自动生成导入规范（暂未使用）。
    ///// </summary>
    ///// <param name="modelType">模型类型。</param>
    ///// <returns>导入栏位名称列表。</returns>
    //public static List<string> GetImportColumns(string modelType)
    //{
    //    var columns = new List<string>();
    //    var baseProperties = TypeHelper.Properties<EntityBase>();
    //    var type = Type.GetType(modelType);
    //    var properties = TypeHelper.Properties(type);
    //    foreach (var item in properties)
    //    {
    //        if (item.GetGetMethod().IsVirtual || baseProperties.Any(p => p.Name == item.Name))
    //            continue;

    //        var name = item.DisplayName();
    //        if (!string.IsNullOrWhiteSpace(name))
    //            columns.Add(name);
    //    }
    //    return columns;
    //}
}

/// <summary>
/// 修改密码表单信息类。
/// </summary>
public class PwdFormInfo
{
    /// <summary>
    /// 取得或设置用户ID。
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 取得或设置原始密码。
    /// </summary>
    [Required]
    [Form(Type = nameof(FieldType.Password))]
    [DisplayName("原密码")]
    public string OldPwd { get; set; }

    /// <summary>
    /// 取得或设置新密码。
    /// </summary>
    [Required]
    [Form(Type = nameof(FieldType.Password))]
    [DisplayName("新密码")]
    public string NewPwd { get; set; }

    /// <summary>
    /// 取得或设置确认新密码。
    /// </summary>
    [Required]
    [Form(Type = nameof(FieldType.Password))]
    [DisplayName("确认密码")]
    public string NewPwd1 { get; set; }
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

/// <summary>
/// 自动页面数据交互信息类。
/// </summary>
/// <typeparam name="TData">交互数据泛型类型。</typeparam>
public class AutoInfo<TData>
{
    /// <summary>
    /// 取得或设置页面模块ID/建表实体名/生成代码路径。
    /// </summary>
    public string PageId { get; set; }

    /// <summary>
    /// 取得或设置页面插件ID/生成代码类型。
    /// </summary>
    public string PluginId { get; set; }

    /// <summary>
    /// 取得或设置交互数据泛型对象/建表脚本/生成代码。
    /// </summary>
    public TData Data { get; set; }
}