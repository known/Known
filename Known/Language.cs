using System.Collections.Concurrent;
using System.Globalization;

namespace Known;

public class Language
{
    private readonly string lang;
    private static readonly ConcurrentDictionary<string, Dictionary<string, object>> caches = new();

    internal Language(string lang)
    {
        if (string.IsNullOrWhiteSpace(lang))
            lang = CultureInfo.CurrentCulture.Name;

        this.lang = lang;
    }

    public string this[string name]
    {
        get
        {
            if (string.IsNullOrEmpty(name))
                return "";

            if (!caches.TryGetValue(lang, out Dictionary<string, object> langs))
                return name;

            if (langs == null || !langs.TryGetValue(name, out object value))
                return name;

            return value?.ToString();
        }
    }

    public static List<ActionInfo> Items { get; } = [];

    public static ActionInfo GetLanguage(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            name = CultureInfo.CurrentCulture.Name;

        var info = Items?.FirstOrDefault(l => l.Id == name);
        info ??= Items?.FirstOrDefault();
        return info;
    }

    internal static void Initialize()
    {
        var path = Path.GetFullPath("Locales");
        if (!Directory.Exists(path))
            return;

        var files = Directory.GetFiles(path);
        foreach (var file in files)
        {
            var name = new FileInfo(file).Name.Split('.')[0];
            var json = File.ReadAllText(file);
            var lang = Utils.FromJson<Dictionary<string, object>>(json);
            caches[name] = lang;
            Items.Add(new ActionInfo
            {
                Id = name,
                Name = lang["localeName"].ToString(),
                Icon = lang["localeIcon"].ToString()
            });
        }
    }

    public string Home => this["Menu.Home"];

    public string BasicInfo => this["Title.BasicInfo"];

    public string OK => this["Button.OK"];
    public string Cancel => this["Button.Cancel"];
    public string New => this["Button.New"];
    public string Edit => this["Button.Edit"];
    public string Delete => this["Button.Delete"];
    public string Save => this["Button.Save"];
    public string Search => this["Button.Search"];
    public string Reset => this["Button.Reset"];
    public string Enable => this["Button.Enable"];
    public string Disable => this["Button.Disable"];

    //Respose
    public const string XXSuccess = "{0}成功！";
    public const string TransError = "事务执行出错！";
    public const string SelectOne = "请选择一条记录进行操作！";
    public const string SelectOneAtLeast = "请至少选择一条记录进行操作！";
    public const string ImportOneAtLeast = "请至少导入一条记录！";
    public const string OperateRecord = "只能操作{0}的记录！";
    public const string SaveSuccess = "保存成功！";
    public const string Update = "修改";
    public const string UpdateSuccess = "修改成功！";
    public const string Copy = "复制";
    public const string CopySuccess = "复制成功！";
    public const string DeleteSuccess = "删除成功！";
    public const string Import = "导入";
    public const string ImportSuccess = "导入成功！";
    public const string Export = "导出";
    public const string Upload = "上传";
    public const string UploadSuccess = "上传成功！";
    public const string Download = "下载";
    public const string Submit = "提交";
    public const string Revoke = "撤回";
    public const string Return = "退回";
    public const string Pass = "通过";
    public const string Fail = "退回";
    public const string Print = "打印";
    public const string FailDataCheck = "数据校验失败！";
    public const string NoDataFound = "未查到任何数据！";
    public const string NoLogin = "用户未登录！";
    public const string NoUser = "用户不存在！";
    public const string ReLogin = "重登录";
    public const string Login = "登录";
    public const string LoginOK = "登录成功！";
    public const string LogoutOK = "成功退出！";
    public const string LoginNoNamePwd = "用户名或密码不正确！";
    public const string LoginDisabled = "用户已禁用！";
    public const string LoginOneAccount = "该账号已在其他地方登录，确定踢掉？<br/>IP:{0}";
    public const string LoginOffline = "您登录的账号已被踢出！";
    public const string Close = "关闭";
    public const string Send = "发送";
    public const string Query = "查询";
    public const string Detail = "查看";
    public const string Add = "添加";
    public const string DeleteM = "批量删除";
    public const string Remove = "移除";
    public const string MoveUp = "上移";
    public const string MoveDown = "下移";
    public const string BackStep = "上一步";
    public const string NextStep = "下一步";
    public const string Finish = "完成";
    public const string Select = "选择";
    public const string Set = "设置";
    public const string More = "更多";
    public const string Error = "错误";
    public const string Refresh = "刷新";

    //Alert
    public const string AlertTips = "提示";
    public const string AlertConfirm = "确认";
    public const string AlertConfirmText = "确定要{0}吗？";

    //Valid
    public const string NotEmpty = "{0}不能为空！";
    public const string NotExists = "{0}不存在！";
    public const string MinLength = "{0}最少为{1}位字符！";
    public const string MaxLength = "{0}最多为{1}位字符！";
    public const string MustInteger = "{0}必须是整数！";
    public const string MustNumber = "{0}必须是数值型！";
    public const string MustDateTime = "{0}必须是日期时间型！";
    public const string MustMinLength = "{0}必须大于等于{1}！";
    public const string MustMaxLength = "{0}必须小于等于{1}！";
    public const string MustDateFormat = "{0}必须是{1}格式的日期时间型！";
    public const string NotPostData = "不能提交空数据！";
    public const string NotValidData = "提交数据不正确！";
    public const string NotUploadData = "不能上传空数据！";
    public const string NotImportData = "不能导入空数据！";
    public const string NotCaptcha = "验证码不正确！";
}