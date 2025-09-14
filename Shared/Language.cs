namespace Known;

/// <summary>
/// 多语言类。
/// </summary>
public partial class Language
{
    private static readonly ConcurrentDictionary<string, Dictionary<string, object>> caches = new();

    internal Language(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            name = CultureInfo.CurrentCulture.Name;

        Name = name;
    }

    internal string Name { get; }

    /// <summary>
    /// 取得或设置多语言项目列表（简体中文/繁体中文/English等）。
    /// </summary>
    public static List<LanguageSettingInfo> Settings { get; set; } = [];

    /// <summary>
    /// 取得或设置多语言数据列表。
    /// </summary>
    public static List<LanguageInfo> Datas { get; set; } = [];

    /// <summary>
    /// 根据ID取得当前语言字符串。
    /// </summary>
    /// <param name="id">语言标识ID。</param>
    /// <returns></returns>
    public string this[string id]
    {
        get
        {
            var text = GetString(id);
            if (string.IsNullOrEmpty(text))
                return id;

            return text;
        }
    }

    /// <summary>
    /// 取得首页。
    /// </summary>
    public const string Home = "首页";

    /// <summary>
    /// 系统。
    /// </summary>
    public const string System = "系统";

    /// <summary>
    /// 选择一条数据。
    /// </summary>
    public const string SelectOne = "请选择一条记录进行操作！";

    /// <summary>
    /// 至少选择一条数据。
    /// </summary>
    public const string SelectOneAtLeast = "请至少选择一条记录进行操作！";

    /// <summary>
    /// 确定。
    /// </summary>
    public const string OK = "确定";

    /// <summary>
    /// 取消。
    /// </summary>
    public const string Cancel = "取消";

    /// <summary>
    /// 保存继续。
    /// </summary>
    public const string SaveContinue = "确定继续";

    /// <summary>
    /// 保存关闭。
    /// </summary>
    public const string SaveClose = "确定关闭";

    /// <summary>
    /// 关闭。
    /// </summary>
    public const string Close = "关闭";

    /// <summary>
    /// 新增。
    /// </summary>
    public const string New = "新增";

    /// <summary>
    /// 编辑。
    /// </summary>
    public const string Edit = "编辑";

    /// <summary>
    /// 删除。
    /// </summary>
    public const string Delete = "删除";

    /// <summary>
    /// 删除成功。
    /// </summary>
    public const string DeleteSuccess = "删除成功！";

    /// <summary>
    /// 保存。
    /// </summary>
    public const string Save = "保存";

    /// <summary>
    /// 保存成功。
    /// </summary>
    public const string SaveSuccess = "保存成功！";

    /// <summary>
    /// 搜索。
    /// </summary>
    public const string Search = "搜索";

    /// <summary>
    /// 高级搜索。
    /// </summary>
    public const string AdvSearch = "高级搜索";

    /// <summary>
    /// 重置。
    /// </summary>
    public const string Reset = "重置";

    /// <summary>
    /// 启用。
    /// </summary>
    public const string Enable = "启用";

    /// <summary>
    /// 禁用。
    /// </summary>
    public const string Disable = "禁用";

    /// <summary>
    /// 导入。
    /// </summary>
    public const string Import = "导入";

    /// <summary>
    /// 导入成功。
    /// </summary>
    public const string ImportSuccess = "导入成功！";

    /// <summary>
    /// 导入失败。
    /// </summary>
    public const string ImportFailed = "导入失败！";

    /// <summary>
    /// 导出。
    /// </summary>
    public const string Export = "导出";

    /// <summary>
    /// 上传。
    /// </summary>
    public const string Upload = "上传";

    /// <summary>
    /// 下载。
    /// </summary>
    public const string Download = "下载";

    /// <summary>
    /// 复制。
    /// </summary>
    public const string Copy = "复制";

    /// <summary>
    /// 复制成功。
    /// </summary>
    public const string CopySuccess = "复制成功！";

    /// <summary>
    /// 提交。
    /// </summary>
    public const string Submit = "提交";

    /// <summary>
    /// 撤回。
    /// </summary>
    public const string Revoke = "撤回";

    /// <summary>
    /// 审核。
    /// </summary>
    public const string Verify = "审核";

    /// <summary>
    /// 授权。
    /// </summary>
    public string Authorize = "授权";

    /// <summary>
    /// 登录。
    /// </summary>
    public const string Login = "登录";

    /// <summary>
    /// 注册。
    /// </summary>
    public const string Register = "注册";

    internal const string ConfirmUpdate = "确定修改";

    /// <summary>
    /// 安全退出。
    /// </summary>
    public const string Exit = "安全退出";

    /// <summary>
    /// 退出成功。
    /// </summary>
    public const string ExitSuccess = "退出成功！";

    /// <summary>
    /// 修改。
    /// </summary>
    public const string Update = "修改";

    /// <summary>
    /// 添加。
    /// </summary>
    public const string Add = "添加";

    /// <summary>
    /// 添加成功。
    /// </summary>
    public const string AddSuccess = "添加成功！";

    /// <summary>
    /// 提取。
    /// </summary>
    public const string Fetch = "提取";

    /// <summary>
    /// 提取成功。
    /// </summary>
    public const string FetchSuccess = "提取成功！";

    /// <summary>
    /// 清空。
    /// </summary>
    public const string Clear = "清空";

    /// <summary>
    /// 清空成功。
    /// </summary>
    public const string ClearSuccess = "清空成功！";

    /// <summary>
    /// 查看。
    /// </summary>
    public const string View = "查看";

    /// <summary>
    /// 上一步。
    /// </summary>
    public const string Previous = "上一步";

    /// <summary>
    /// 下一步。
    /// </summary>
    public const string Next = "下一步";

    /// <summary>
    /// 完成。
    /// </summary>
    public const string Finish = "完成";

    /// <summary>
    /// 更多。
    /// </summary>
    public const string More = "更多";

    /// <summary>
    /// 批量删除。
    /// </summary>
    public const string DeleteM = "批量删除";

    /// <summary>
    /// 打印。
    /// </summary>
    public const string Print = "打印";

    /// <summary>
    /// 移动。
    /// </summary>
    public const string Move = "移动";

    /// <summary>
    /// 上移。
    /// </summary>
    public const string MoveUp = "上移";

    /// <summary>
    /// 下移。
    /// </summary>
    public const string MoveDown = "下移";

    /// <summary>
    /// 恢复。
    /// </summary>
    public const string Restore = "恢复";

    /// <summary>
    /// 测试。
    /// </summary>
    public const string Test = "测试";

    /// <summary>
    /// 安装。
    /// </summary>
    public const string Install = "安装";

    /// <summary>
    /// 迁移。
    /// </summary>
    public const string Migrate = "迁移";

    /// <summary>
    /// 执行。
    /// </summary>
    public const string Execute = "执行";

    /// <summary>
    /// 返回首页。
    /// </summary>
    public const string BackHome = "返回首页";

    /// <summary>
    /// 数据库。
    /// </summary>
    public const string Database = "数据库";

    /// <summary>
    /// 基本信息。
    /// </summary>
    public const string BasicInfo = "基本信息";

    /// <summary>
    /// 系统信息。
    /// </summary>
    public const string SystemInfo = "系统信息";

    /// <summary>
    /// 账号信息。
    /// </summary>
    public const string AccountInfo = "账号信息";

    /// <summary>
    /// 功能待加。
    /// </summary>
    public const string FuncPending = "功能待加";

    /// <summary>
    /// 复制代码。
    /// </summary>
    public const string CopyCode = "复制代码";

    /// <summary>
    /// 下载代码。
    /// </summary>
    public const string DownloadCode = "下载代码";

    /// <summary>
    /// 提示。
    /// </summary>
    public const string Prompt = "提示";

    /// <summary>
    /// 询问。
    /// </summary>
    public const string Question = "询问";

    /// <summary>
    /// 错误。
    /// </summary>
    public const string Error = "错误";

    /// <summary>
    /// 警告。
    /// </summary>
    public const string Warning = "警告";

    /// <summary>
    /// 操作。
    /// </summary>
    public const string Action = "操作";

    /// <summary>
    /// 设置成功。
    /// </summary>
    public const string SetSuccess = "设置成功！";

    /// <summary>
    /// 预览。
    /// </summary>
    public const string Preview = "预览";
    internal const string PreviewFile = "预览附件";
    internal const string SelectIcon = "选择图标";
    internal const string SelectUser = "选择用户";
    internal const string SystemSetting = "系统设置";
    /// <summary>
    /// 安全设置。
    /// </summary>
    public const string SecuritySetting = "安全设置";
    internal const string FlowAction = "{action}流程";
    internal const string FlowLog = "流程记录";
    internal const string TodoList = "待办事项";
    internal const string MyMessage = "我的消息";
    internal const string MyProfile = "我的信息";
    internal const string CopyTo = "复制到";
    internal const string MoveTo = "移动到";
    internal const string Icon = "图标";
    internal const string Default = "默认";
    internal const string Field = "字段";
    internal const string Template = "模板";
    /// <summary>
    /// 类型。
    /// </summary>
    public const string Type = "类型";
    internal const string ParentMenu = "上级菜单";
    internal const string SelectParentMenu = "选择上级菜单";

    internal const string ValidRequired = "{label}不能为空！";
    internal const string ValidMinLength = "{label}至少{length}个字符！";
    internal const string ValidMaxLength = "{label}不能超过{length}个字符！";
    internal const string ValidMustInteger = "{label}必须是整数！";
    internal const string ValidMustNumber = "{label}必须是数值型！";
    internal const string ValidMustDateTime = "{label}必须是日期时间型！";
    internal const string ValidMustMinLength = "{label}必须大于等于{length}！";
    internal const string ValidMustMaxLength = "{label}必须小于等于{length}！";
    internal const string ValidMustDateFormat = "{label}必须是{format}格式的日期时间型！";

    internal const string ActionTitle = "{action}{title}";
    internal const string ImportTitle = "导入{name}";
    internal const string PageTotal = "共 {total} 条记录";
    /// <summary>
    /// 确定要退出系统。
    /// </summary>
    public const string TipExits = "确定要退出系统？";
    public const string TipNoUser = "用户不存在！";
    public const string TipUserNameExists = "用户名已存在，请使用其他字符创建用户！";
    internal const string TipPage404 = "页面不存在！";
    internal const string TipNoMethod = "{method}方法不存在！";
    internal const string TipXXSuccess = "{action}成功！";
    internal const string TipXXFailed = "{action}失败！";
    internal const string TipTransError = "事务执行出错！";
    internal const string TipConfirmDelete = "确定要删除{name}？";
    internal const string TipConfirmDeleteRecord = "确定要删除该记录？";
    internal const string TipConfirmRecordName = "确定要{text}选中的记录？";
    internal const string TipSelectOneAtLeast = "请至少选择{count}条记录进行操作！";
    internal const string TipKeyRequired = "配置键不能为空！";
    internal const string TipNotDeleteMenu = "存在子菜单，不能删除！";
    internal const string TipSelectModule = "请选择模块！";
    internal const string TipSelectParentModule = "请先选择上级模块！";
    internal const string TipImportModules = "确定要导入替换系统模块？";
    internal const string TipWebLogSaveDay = "该日志为内存日志，默认保留{LogDays}天。";
    internal const string TipConfirmClearLog = "确定要清空所有日志？";
    internal const string RequestHeaders = "请求头";
    internal const string RequestParameters = "请求参数";
    internal const string ResponseResults = "响应结果";

    internal const string ImportTips = "提示: 请上传单个txt或Excel格式附件！";
    internal const string ImportIsAsync = "异步导入";
    internal const string ImportDownload = "模板下载";
    internal const string ImportError = "错误信息";
    internal const string TipFormatInvalid = "{label}格式不正确！";

    internal const string TipEnterKeyword = "输入关键字";
    internal const string TipNotPreview = "不支持在线预览";
    internal const string TipSystemUpdating = "系统正在更新，请稍候...";
    internal const string TipSystemDisconnect = "系统连接失败，请确认网络，尝试重新连接！";
    internal const string TipSystemConnected = "系统连接成功，请重新加载！";
    internal const string TipSystemError = "抱歉，系统出错了，请重新加载！";
    internal const string ReConnect = "重新连接";
    internal const string ReLoad = "重新加载";
    internal const string MenuManage = "菜单管理";
    internal const string AddMenu = "添加菜单";
    internal const string AddPage = "添加页面";
    internal const string AddAction = "添加操作按钮";
    internal const string AddColumn = "添加表格列";
    internal const string AddData = "添加数据";
    internal const string AddLink = "添加连接";
    internal const string AddDesktop = "添加到主屏幕";
    internal const string ClickShareButton = "点击分享按钮";
    internal const string SelectInstallDesktop = "，选择“添加到主屏幕”安装。";
    internal const string SumPage = "本页合计";
    internal const string SumQuery = "查询总计";
    internal const string EditToolbar = "编辑工具条按钮";
    internal const string DragFileUpload = "单击或拖动文件到此区域进行上传";
    internal const string RenderMode = "呈现模式";
    internal const string AutoMode = "自动模式";
    internal const string SSRMode = "SSR模式";
    internal const string NotSupportAutoMode = "当前程序不支持切换为自动模式。";

    internal const string Downloading = "数据下载中...";
    internal const string NoDataDownload = "无数据可下载！";
    internal const string DataExporting = "数据导出中...";
    internal const string NoDataExport = "无数据可导出！";
    internal const string DataQuering = "数据查询中...";
    internal const string QueryByEnter = "按回车查询";

    /// <summary>
    /// 请选择导入文件。
    /// </summary>
    public const string ImportSelectFile = "请选择导入文件！";

    internal const string Loading = "加载中...";
    internal const string ImportImporting = "正在导入中...";
    internal const string ImportFile = "导入文件";
    internal const string ImportTemplate = "导入模板";
    internal const string ImportSize = "大小：";
    internal const string TipUploadMaxSize = "附件允许最大上传{size}M。";
    
    /// <summary>
    /// 导入文件不存在。
    /// </summary>
    public const string ImportFileNotExists = "导入文件不存在！";

    internal const string SysLanguage = "系统语言";
    internal const string SysModule = "系统模块";
    internal const string TipNewModule = "点此安装新模块。";
    internal const string MigrateModule = "可将 AppData.kmd 和 Admin 插件配置数据迁移至新框架配置库。";
    internal const string NoInstallModule = "未安装模块";
    internal const string InstallTo = "安装到";
    internal const string InstallNewModule = "安装新模块";
    internal const string ConfirmMigrate = "确定迁移AppData或Admin插件配置数据？";

    internal const string PleaseSelect = "请选择";
    internal const string PleaseSelectInput = "请选择或输入";
    internal const string All = "全部";
    internal const string Yes = "是";
    internal const string No = "否";
    internal const string Have = "有";
    internal const string NotHave = "无";
    internal const string Back = "返回";
    internal const string Index = "序号";
    internal const string Expand = "展开";
    internal const string Collapse = "折叠";
    internal const string EditMode = "编辑模式";
    internal const string HomePage = "系统首页";
    internal const string Setting = "设置";
    internal const string FullScreen = "全屏显示";
    internal const string ExitScreen = "退出全屏";
    internal const string RefreshPage = "刷新页面";
    internal const string ClearCache = "清空缓存";
    internal const string SizeDefault = "默认";
    internal const string SizeCompact = "紧凑";
    internal const string Profile = "个人中心";
    internal const string Development = "开发中心";

    internal const string ThemeSetting = "主题设置";
    internal const string Menu = "菜单";
    internal const string LayoutSetting = "布局设置";
    internal const string Layout = "布局";
    internal const string Tabs = "标签";
    internal const string TopTab = "顶行标签";
    internal const string MaxTabCount = "最大标签数";
    internal const string PreferenceSetting = "偏好设置";
    internal const string Accordion = "手风琴";
    internal const string RegionalSetting = "区域设置";
    internal const string Footer = "底部";
    internal const string Dark = "暗色";
    internal const string Light = "亮色";
    internal const string Vertical = "纵向";
    internal const string Horizontal = "横向";
    internal const string Float = "浮动";
    internal const string LowCodeTable = "低代码表格";
    internal const string TableSetting = "表格设置";
    internal const string TipNavDelete = "将导航按钮拖到此处删除";
    internal const string TipReset = "重置为默认查询条件";
    internal const string ViewDetail = "查看详情";
    internal const string Info = "信息";
    internal const string Content = "内容";
    internal const string CopyError = "复制错误";

    /// <summary>
    /// 用户注册。
    /// </summary>
    public const string UserRegister = "用户注册";
    /// <summary>
    /// 用户名登录。
    /// </summary>
    public const string LoginWithAccount = "用户名登录";
    /// <summary>
    /// 手机号登录。
    /// </summary>
    public const string LoginWithPhone = "手机号登录";
    /// <summary>
    /// 扫码登录。
    /// </summary>
    public const string LoginWithQRCode = "扫码登录";
    /// <summary>
    /// 请使用手机浏览器扫码登录。
    /// </summary>
    public const string ScanCodeWithMobile = "请使用手机浏览器扫码登录";
    internal const string PhoneNo = "手机号";
    internal const string PhoneCode = "手机验证码";
    internal const string Captcha = "验证码";
    internal const string RememberPhone = "记住手机号";
    internal const string UserName = "用户名";
    internal const string Password = "密码";
    internal const string Password1 = "确认密码";
    internal const string Station = "站点";
    internal const string RememberUser = "记住用户名";
    internal const string UserOrPhone = "用户名或手机号";
    internal const string RegisterLogin = "注册并登录";
    internal const string CaptchaRefresh = "点击图片刷新";
    internal const string CaptchaFetch = "获取验证码";
    internal const string CaptchaCountdown = "{SmsCount}秒后重新获取";
    internal const string CaptchaNotValid = "验证码不正确！";

    /// <summary>
    /// 官网。
    /// </summary>
    public const string WebSite = "官网";
    /// <summary>
    /// 文档。
    /// </summary>
    public const string Document = "文档";
    /// <summary>
    /// Known标题。
    /// </summary>
    public const string KnownTitle = "Known是基于Blazor轻量级、跨平台、极易扩展的插件开发框架。";
    /// <summary>
    /// Known说明1。
    /// </summary>
    public const string KnownNote1 = "包含模块、字典、组织、角色、用户、日志、消息、工作流、定时任务等功能。";
    /// <summary>
    /// Known说明2。
    /// </summary>
    public const string KnownNote2 = "低代码、简洁、易扩展，让开发更简单、更快捷！";
    /// <summary>
    /// 官网网站。
    /// </summary>
    public const string KnownWebSite = "官网网站：";
    /// <summary>
    /// 源码下载。
    /// </summary>
    public const string KnownSource = "源码下载：";
    /// <summary>
    /// 交流群号。
    /// </summary>
    public const string KnownQQNo = "交流群号：";

    internal const string Greeting0 = "您好！{name}，您貌似不在我们的时空中！";
    internal const string Greeting5 = "早安！{name}，开始您一天的工作吧！";
    internal const string Greeting9 = "上午好！{name}，加油！";
    internal const string Greeting11 = "午安！{name}，别忘了准备午饭！";
    internal const string Greeting13 = "下午好！{name}，继续加油！";
    internal const string Greeting15 = "下午好！{name}，工作一天累了吧，泡杯下午茶解解乏！";
    internal const string Greeting18 = "晚上好！{name}，晚饭吃了吗？";
    internal const string Greeting22 = "晚安！{name}，该睡觉了，明天还会天亮的！";
    internal const string Greeting23 = "{name}，还没休息啊，身体是革命的本钱！";

    /// <summary>
    /// 访问量。
    /// </summary>
    public const string HomeVisitCount = "访问量";

    internal const string HomeToday = "今天是：{date}";
    internal const string HomeTotal = "总";

    /// <summary>
    /// 用户数量。
    /// </summary>
    public const string HomeUserCount = "用户数量";

    /// <summary>
    /// 日志数量。
    /// </summary>
    public const string HomeLogCount = "日志数量";

    /// <summary>
    /// 日志统计。
    /// </summary>
    public const string HomeLogStatistic = "日志统计";

    internal const string HomeCommonFunc = "常用功能";

    /// <summary>
    /// 月系统访问量统计。
    /// </summary>
    public const string HomeVisitTitle = "{month}月系统访问量统计";

    internal const string HomeDate = "日期";
    internal const string HomeCount = "数量";

    /// <summary>
    /// 开源。
    /// </summary>
    public const string OpenSource = "开源";

    /// <summary>
    /// 低代码。
    /// </summary>
    public const string LowCode = "低代码";

    /// <summary>
    /// 现代UI。
    /// </summary>
    public const string ModernUI = "现代UI";

    /// <summary>
    /// 根据语言标识获取语言项目。
    /// </summary>
    /// <param name="name">语言标识</param>
    /// <returns>语言项目对象。</returns>
    public static LanguageSettingInfo GetLanguage(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            name = CultureInfo.CurrentCulture.Name;

        var info = Settings?.FirstOrDefault(l => l.Code == name);
        info ??= Settings?.FirstOrDefault();
        return info;
    }

    /// <summary>
    /// 获取默认语言设置信息列表。
    /// </summary>
    /// <returns>语言设置信息列表。</returns>
    public static List<LanguageSettingInfo> GetDefaultSettings()
    {
        var infos = new List<LanguageSettingInfo>();
        var properties = TypeHelper.Properties<LanguageInfo>();
        foreach (var item in properties)
        {
            var attr = item.GetCustomAttribute<LanguageAttribute>();
            if (attr == null)
                continue;

            infos.Add(new LanguageSettingInfo
            {
                Id = item.Name,
                Code = attr.Code,
                Name = item.DisplayName(),
                Icon = attr.Icon,
                Default = attr.Default,
                Enabled = attr.Enabled
            });
        }
        return infos;
    }

    /// <summary>
    /// 根据ID获取语言。
    /// </summary>
    /// <param name="id">ID。</param>
    /// <returns>语言字符串。</returns>
    public string GetString(string id)
    {
        if (string.IsNullOrEmpty(id))
            return "";

        var data = Datas.FirstOrDefault(d => d.Chinese == id);
        if (data != null)
        {
            foreach (var item in Settings)
            {
                if (item.Code == Name)
                    return TypeHelper.GetPropertyValue<string>(data, item.Id);
            }
        }

        if (!caches.TryGetValue(Name, out Dictionary<string, object> langs))
            return "";

        if (langs == null || !langs.TryGetValue(id, out object value))
            return "";

        return value?.ToString();
    }

    /// <summary>
    /// 获取代码表语言。
    /// </summary>
    /// <param name="info">代码表对象。</param>
    /// <returns>代码表语言。</returns>
    public string GetString(CodeInfo info) => GetText("Code", info?.Code, info?.Name);

    /// <summary>
    /// 获取替换{label}的验证信息的语言。
    /// </summary>
    /// <param name="id">语言ID。</param>
    /// <param name="label">替换的字段名。</param>
    /// <returns>验证信息。</returns>
    public string GetString(string id, string label) => this[id].Replace("{label}", this[label]);

    /// <summary>
    /// 获取替换{label}和{length}的验证信息的语言。
    /// </summary>
    /// <param name="id">语言ID。</param>
    /// <param name="label">替换的字段名。</param>
    /// <param name="length">替换的长度。</param>
    /// <returns>验证信息。</returns>
    public string GetString(string id, string label, int? length) => GetString(id, label).Replace("{length}", $"{length}");

    /// <summary>
    /// 获取替换{label}和{format}的验证信息的语言。
    /// </summary>
    /// <param name="id">语言ID。</param>
    /// <param name="label">替换的字段名。</param>
    /// <param name="format">替换的格式。</param>
    /// <returns>验证信息。</returns>
    public string GetString(string id, string label, string format) => GetString(id, label).Replace("{format}", format);

    /// <summary>
    /// 获取标题语言。
    /// </summary>
    /// <param name="title">标题ID。</param>
    /// <returns>标题语言。</returns>
    public string GetTitle(string title) => GetText("Title", title);

    /// <summary>
    /// 获取代码表语言。
    /// </summary>
    /// <param name="code">代码。</param>
    /// <returns>代码表语言。</returns>
    public string GetCode(string code) => GetText("Code", code);

    /// <summary>
    /// 获取必填验证信息语言。
    /// </summary>
    /// <param name="label">替换的字段名。</param>
    /// <returns>必填验证信息。</returns>
    public string Required(string label) => GetString(ValidRequired, label);

    /// <summary>
    /// 获取操作成功提示语言。
    /// </summary>
    /// <param name="action">操作动作名。</param>
    /// <returns>操作成功提示语言。</returns>
    public string Success(string action) => this[TipXXSuccess].Replace("{action}", action);

    /// <summary>
    /// 获取操作失败提示语言。
    /// </summary>
    /// <param name="action">操作动作名。</param>
    /// <returns>操作失败提示语言。</returns>
    public string Failed(string action) => this[TipXXFailed].Replace("{action}", action);

    /// <summary>
    /// 获取表单的标题语言。
    /// </summary>
    /// <param name="action">表单操作名（新增/编辑）。</param>
    /// <param name="title">表单标题。</param>
    /// <returns>表单的标题语言。</returns>
    public string GetFormTitle(string action, string title)
    {
        return this[ActionTitle]?.Replace("{action}", this[action]).Replace("{title}", title);
    }

    /// <summary>
    /// 获取导入表单的标题语言。
    /// </summary>
    /// <param name="name">模块名称。</param>
    /// <returns>导入表单的标题语言。</returns>
    public string GetImportTitle(string name) => this[ImportTitle].Replace("{name}", name);

    public string GetText(string prefix, string code, string name = null)
    {
        var text = GetString($"{prefix}.{code}");
        if (string.IsNullOrWhiteSpace(text))
            text = GetString($"Flow.{code}");
        if (string.IsNullOrWhiteSpace(text))
            text = GetString(code);
        if (string.IsNullOrWhiteSpace(text))
            text = name;
        if (string.IsNullOrWhiteSpace(text))
            text = code;
        return text;
    }
}