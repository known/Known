namespace Known.Helpers;

class MigrateHelper
{
    internal static async Task MigrateModulesAsync(Database db)
    {
        var exists = await db.ExistsAsync<SysConfig>(true);
        if (!exists)
            return;

        var key = "Known";
        var version = await db.GetConfigAsync(key);
        if (!string.IsNullOrWhiteSpace(version))
            return;

        var modules = await db.QueryListAsync<SysModule>();
        if (modules == null || modules.Count == 0)
            return;

        await db.BackupTableAsync<SysModule>($"{DateTime.Now:yyyyMMdd}");
        foreach (var item in modules)
        {
            if (!string.IsNullOrWhiteSpace(item.PageData)) // 升级2.x配置
            {
                var page = Utils.FromJson<Page2Info>(item.PageData);
                if (page != null)
                {
                    var info = page.ToPageInfo();
                    item.PageData = Utils.ToJson(info);
                }
            }
            if (!string.IsNullOrWhiteSpace(item.PluginData)) // 升级3.x配置
            {
                var plugins = ZipHelper.UnZipDataAsString<List<PluginInfo>>(item.PluginData);
                var plugin = plugins?.FirstOrDefault();
                if (plugin != null)
                {
                    var param = Utils.FromJson<AutoPage2Info>(plugin.Setting);
                    if (param != null)
                    {
                        var info = param.ToPageInfo();
                        plugin.Setting = Utils.ToJson(info);
                        item.Plugins.Clear();
                        item.Plugins.Add(plugin);
                        item.PluginData = ZipHelper.ZipDataAsString(item.Plugins);
                    }
                }
            }
            await db.SaveAsync(item);
        }

        await db.SaveConfigAsync(key, "模块已升级！");
    }

    class AutoPage2Info
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string Namespace { get; set; } = Config.App.Id;

        public string Type
        {
            get { return PageType.ToString(); }
            set { PageType = Utils.ConvertTo<AutoPageType>(value); }
        }
        public AutoPageType PageType { get; set; }
        public string Database { get; set; }
        public string Script { get; set; }
        public string IdField { get; set; } = nameof(EntityBase.Id);
        public string EntityData { get; set; }
        public string FlowData { get; set; }
        public Page2Info Page { get; set; } = new();
        public FormInfo Form { get; set; } = new();

        internal AutoPageInfo ToPageInfo()
        {
            var info = new AutoPageInfo
            {
                Id = Id,
                Name = Name,
                Prefix = Prefix,
                Namespace = Namespace,
                Type = Type,
                Database = Database,
                Script = Script,
                IdField = IdField,
                EntityData = EntityData,
                FlowData = FlowData,
                Form = Form,
                Page = Page?.ToPageInfo()
            };
            return info;
        }
    }

    class Page2Info
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool ShowAdvSearch { get; set; } = true;
        public bool ShowPager { get; set; } = true;
        public bool ShowSetting { get; set; } = true;
        public int? PageSize { get; set; }
        public int? ToolSize { get; set; }
        public int? ActionSize { get; set; }
        public List<string> Tools { get; set; } = [];
        public List<string> Actions { get; set; } = [];
        public List<PageColumnInfo> Columns { get; set; } = [];

        internal PageInfo ToPageInfo()
        {
            return new PageInfo
            {
                Name = Name,
                Type = Type,
                ShowAdvSearch = ShowAdvSearch,
                ShowPager = ShowPager,
                ShowSetting = ShowSetting,
                PageSize = PageSize,
                ToolSize = ToolSize,
                ActionSize = ActionSize,
                Tools = Tools?.Select(d => new ActionInfo(d)).ToList(),
                Actions = Actions?.Select(d => new ActionInfo(d)).ToList(),
                Columns = Columns
            };
        }
    }
}