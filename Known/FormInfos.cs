namespace Known;

/// <summary>
/// 登录表单信息类。
/// </summary>
public class LoginFormInfo
{
    /// <summary>
    /// 取得或设置用户名。
    /// </summary>
    [Required] public string UserName { get; set; }

    /// <summary>
    /// 取得或设置密码。
    /// </summary>
    [Required] public string Password { get; set; }

    /// <summary>
    /// 取得或设置手机号，用于手机验证码登录。
    /// </summary>
    [Required] public string PhoneNo { get; set; }

    /// <summary>
    /// 取得或设置手机验证码。
    /// </summary>
    [Required] public string PhoneCode { get; set; }

    /// <summary>
    /// 取得或设置图片验证码。
    /// </summary>
    [Required] public string Captcha { get; set; }

    /// <summary>
    /// 取得或设置当前登录的站别，用于多站别系统。
    /// </summary>
    [Required] public string Station { get; set; }

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