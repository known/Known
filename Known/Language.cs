namespace Known;

public static class Language
{
    public static List<ActionInfo> Items =
    [
        new ActionInfo { Id = "zh-CN", Name = "简体中文" },
        new ActionInfo { Id = "zh-TW", Name = "繁体中文" },
        new ActionInfo { Id = "en-US", Name = "English" }
    ];

    public static string Format(this string format, params object[] args) => string.Format(format, args);

    //Respose
    public const string XXSuccess = "{0}成功！";
    public const string TransError = "事务执行出错！";
    public const string SelectOne = "请选择一条记录进行操作！";
    public const string SelectOneAtLeast = "请至少选择一条记录进行操作！";
    public const string ImportOneAtLeast = "请至少导入一条记录！";
    public const string OperateRecord = "只能操作{0}的记录！";
    public const string Save = "保存";
    public const string SaveSuccess = "保存成功！";
    public const string Update = "修改";
    public const string UpdateSuccess = "修改成功！";
    public const string Copy = "复制";
    public const string CopySuccess = "复制成功！";
    public const string Delete = "删除";
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
    public const string OK = "确定";
    public const string Cancel = "取消";
    public const string Close = "关闭";
    public const string Send = "发送";
    public const string Search = "搜索";
    public const string Query = "查询";
    public const string Detail = "查看";
    public const string Add = "添加";
    public const string New = "新增";
    public const string Edit = "编辑";
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

    //Fields
    public const string CreateBy = "创建人";
    public const string CreateTime = "创建时间";
    public const string ModifyBy = "修改人";
    public const string ModifyTime = "修改时间";
    public const string Version = "版本号";
    public const string CompNo = "公司编码";

    //Biz
    //public const string LocalUser = "本地用户";
    //public const string ProdActiveFail = "激活失败！";
    //public const string ProdServerError = "激活服务器出错！";
    //public const string ProdNotNetwork = "计算机未联网，无法激活！";
    
    //public const string Error403Title = "无权限访问！";
    //public const string Error403Content = "抱歉，您无权限访问该页面~";
    //public const string Error404Title = "页面未找到！";
    //public const string Error404Content = "抱歉，页面好像去火星了~";
    //public const string Error500Title = "服务器内部错误！";
    //public const string Error500Content = "服务器好像出错了...";

    //public const string DataByList = "列表显示";
    //public const string DataBySquared = "宫格显示";

    //public const string PagerTotalText = "共{0}条";
    //public const string PagerRefresh = "刷新";
    //public const string PagerFirst = "第一页";
    //public const string PagerPrevious = "上一页";
    //public const string PagerNext = "下一页";
    //public const string PagerLast = "最后一页";
}