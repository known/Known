namespace Known;

public partial class Language
{
    internal const string TipAIEntry = "有问题，问小K。";

    internal const string TipNotSaveWithoutDev = "非开发环境，不能保存代码！";
    internal const string TipPathRequired = "路径不能为空！";
    internal const string TipFileExisted = "文件[{file}]已存在！";
    internal const string TipTableRequired = "实体表名不能为空！";
    internal const string TipTableHasData = "数据表存在数据，不能执行！";
    internal const string ExecuteSuccess = "执行成功！";

    internal const string TipFormRouteIsNull = "表单类型或路由不存在！";
    internal const string TipLanguageFetch = "提取系统语言常量、枚举、信息类、实体类字段名称。";
    internal const string TipLanguageSetting = "配置系统语言选项。";
    internal const string TipLanguageFetchConfirm = "确定要提取语言信息吗？";
    internal const string TipLanguageSettingConfirm = "确定要重置语言设置吗？";
    internal const string TipCodeAndNameRequired = "请输入代码和名称！";
    internal const string TipNoConfigOnSelectModel = "未配置 UIConfig.OnSelectModel 委托！";
    internal const string TipNoConfigOnFastAddField = "未配置 UIConfig.OnFastAddField 委托！";
    internal const string TipSelectDataTable = "请选择数据表！";
    internal const string TipMenuNotExists = "菜单不存在！";
    /// <summary>
    /// 不能操作代码配置的模块。
    /// </summary>
    public const string TipCodeModuleNotOperate = "不能操作代码配置的模块！";

    internal const string TipSystemNotInstall = "系统未安装！";
    internal const string TipAdminNoMigrate = "Admin插件不支持迁移！";
    internal const string TipModuleMigrated = "已经迁移过，无需再次迁移！";
    internal const string TipModuleNotExists = "模块不存在！";
    internal const string TipModuleDeleteExistsChild = "存在子模块，不能删除！";

    internal const string OverMaxTabCount = "超过最大标签页数！";
    internal const string MoreTableSetting = "更多表格属性设置";
    internal const string MoreFormSetting = "更多表单属性设置";

    internal const string ImportTable = "导入表";
    internal const string DataTable = "数据表";
    
    internal const string CloseAdvSearchForm = "关闭高级搜索框";
    internal const string TableColumnSetting = "列展示设置";
    internal const string TableColumnShow = "列展示";
    internal const string NotFixed = "不固定";
    internal const string FixLeft = "固定在左侧";
    internal const string FixRight = "固定在右侧";
    internal const string FixToLeft = "固定到左侧";
    internal const string FixToRight = "固定到右侧";

    internal const string AddNavbar = "添加导航";
    internal const string Component = "组件";
    internal const string NavFontSize = "字体大小";
    internal const string NavLanguage = "多语言";
    internal const string NavTheme = "主题";
    internal const string NavFullScreen = "全屏";
    internal const string NavLink = "连接";

    ///// <summary>
    ///// 获取默认多语言信息列表。
    ///// </summary>
    ///// <returns>多语言信息列表。</returns>
    //public static List<LanguageInfo> GetDefaultLanguages()
    //{
    //    var infos = new List<LanguageInfo>();
    //    foreach (var item in Config.Modules)
    //    {
    //        infos.Add(item.Name);
    //    }
    //    foreach (var item in Config.Menus)
    //    {
    //        infos.Add(item.Name);
    //    }
    //    foreach (var item in Config.AppMenus)
    //    {
    //        infos.Add(item.Name);
    //    }
    //    foreach (var item in PluginConfig.Plugins)
    //    {
    //        infos.Add(item.Attribute?.Name);
    //    }
    //    foreach (var assembly in Config.Assemblies)
    //    {
    //        foreach (var item in assembly.GetTypes())
    //        {
    //            if (item.IsEnum)
    //                infos.AddEnum(item);
    //            else if (item.IsAssignableTo(typeof(EntityBase)) || item.Name.EndsWith("Info"))
    //                infos.AddAttribute(item);
    //            else if (item.Name.Contains(nameof(Language)))
    //                infos.AddConstant(item);
    //        }
    //    }
    //    return infos;
    //}
}