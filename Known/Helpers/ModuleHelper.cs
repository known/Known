namespace Known.Helpers;

class ModuleHelper
{
    internal static async Task ImportModulesAsync(Database db, FileDataInfo info)
    {
        using (var stream = new MemoryStream(info.Bytes))
        using (var reader = new MemoryStream())
        using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
        {
            await gzip.CopyToAsync(reader);
            var json = Encoding.UTF8.GetString(reader.ToArray());
            var modules = Utils.FromJson<List<SysModule>>(json);
            if (modules != null && modules.Count > 0)
            {
                await db.DeleteAllAsync<SysModule>();
                await db.InsertListAsync(modules);
            }
        }
    }

    internal static async Task<byte[]> ExportModulesAsync(Database db)
    {
        var modules = await db.QueryListAsync<SysModule>();
        var json = Utils.ToJson(modules);
        var bytes = Encoding.UTF8.GetBytes(json);
        using (var stream = new MemoryStream())
        using (var gzip = new GZipStream(stream, CompressionMode.Compress, true))
        {
            await gzip.WriteAsync(bytes, 0, bytes.Length);
            await gzip.FlushAsync();
            stream.Position = 0;

            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }

    internal static List<SysModule> GetModules()
    {
        var modules = new List<SysModule>();
        var baseData = GetModule("BaseData", "基础数据", "database", ModuleType.Menu.ToString(), 1);
        modules.Add(baseData);
        modules.Add(GetSysDictionary(baseData.Id));
        modules.Add(GetSysOrganization(baseData.Id));

        Config.OnAddModule?.Invoke(modules);

        var roots = modules.Count(m => m.ParentId == "0");
        var system = GetModule("System", "系统管理", "setting", ModuleType.Menu.ToString(), roots + 1);
        modules.Add(system);
        modules.Add(GetSysSystem(system.Id));
        modules.Add(GetSysRole(system.Id));
        modules.Add(GetSysUser(system.Id));
        modules.Add(GetSysTask(system.Id));
        modules.Add(GetSysFile(system.Id));
        modules.Add(GetSysLog(system.Id));
        //modules.Add(GetSysModule(system.Id));
        return modules;
    }

    private static SysModule GetModule(string code, string name, string icon, string target, int sort)
    {
        return new SysModule { ParentId = "0", Code = code, Name = name, Icon = icon, Target = target, Sort = sort, Enabled = true };
    }

    private static SysModule GetSysDictionary(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysDictionaryList",
            Name = "数据字典",
            Icon = "unordered-list",
            Description = "维护系统所需的下拉框数据源。",
            Target = ModuleType.Page.ToString(),
            Url = "/sys/dictionaries",
            Sort = 1,
            Enabled = true,
            EntityData = @"数据字典|SysDictionary
类别|Category|Text|50|Y
类别名称|CategoryName|Text|50
代码|Code|Text|100|Y
名称|Name|Text|150
顺序|Sort|Number
状态|Enabled|Switch
备注|Note|Text|500
子字典|Child|TextArea",
            PageData = "{\"Type\":null,\"ShowPager\":true,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":[\"New\",\"DeleteM\",\"Import\",\"AddCategory\"],\"Actions\":[\"Edit\",\"Delete\"],\"Columns\":[{\"Id\":\"Category\",\"Name\":\"类别\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":null},{\"Id\":\"Code\",\"Name\":\"代码\",\"IsViewLink\":true,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":120,\"Align\":null},{\"Id\":\"Name\",\"Name\":\"名称\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":120,\"Align\":null},{\"Id\":\"Sort\",\"Name\":\"顺序\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":\"升序\",\"Fixed\":null,\"Width\":60,\"Align\":\"center\"},{\"Id\":\"Enabled\",\"Name\":\"状态\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":60,\"Align\":\"center\"},{\"Id\":\"Note\",\"Name\":\"备注\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":200,\"Align\":null}]}",
            FormData = "{\"LabelSpan\":null,\"WrapperSpan\":null,\"Fields\":[{\"Row\":1,\"Column\":1,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Code\",\"Name\":\"代码\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":1,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Name\",\"Name\":\"名称\",\"Type\":0,\"Length\":null,\"Required\":false},{\"Row\":1,\"Column\":1,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Sort\",\"Name\":\"顺序\",\"Type\":3,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":1,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Enabled\",\"Name\":\"状态\",\"Type\":4,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":1,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Note\",\"Name\":\"备注\",\"Type\":1,\"Length\":null,\"Required\":false}]}"
        };
    }

    private static SysModule GetSysOrganization(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysOrganizationList",
            Name = "组织架构",
            Icon = "partition",
            Description = "维护企业组织架构信息。",
            Target = ModuleType.Page.ToString(),
            Url = "/sys/organizations",
            Sort = 2,
            Enabled = true,
            EntityData = @"组织架构|SysOrganization
上级组织|ParentId|Text|50
编码|Code|Text|50|Y
名称|Name|Text|50|Y
管理者|ManagerId|Text|50
备注|Note|Text|500",
            PageData = "{\"Type\":null,\"ShowPager\":false,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":[\"New\",\"DeleteM\"],\"Actions\":[\"Edit\",\"Delete\"],\"Columns\":[{\"Id\":\"Code\",\"Name\":\"编码\",\"IsViewLink\":true,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"Name\",\"Name\":\"名称\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":null},{\"Id\":\"Note\",\"Name\":\"备注\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":400,\"Align\":null}]}",
            FormData = "{\"LabelSpan\":null,\"WrapperSpan\":null,\"Fields\":[{\"Row\":1,\"Column\":1,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Code\",\"Name\":\"编码\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":2,\"Column\":1,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Name\",\"Name\":\"名称\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":3,\"Column\":1,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Note\",\"Name\":\"备注\",\"Type\":1,\"Length\":null,\"Required\":false}]}"
        };
    }

    private static SysModule GetSysSystem(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysSystem",
            Name = "关于系统",
            Icon = "info-circle",
            Description = "显示系统版本及产品授权信息。",
            Target = ModuleType.Custom.ToString(),
            Url = "/sys/info",
            Sort = 1,
            Enabled = true
        };
    }

    private static SysModule GetSysRole(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysRoleList",
            Name = "角色管理",
            Icon = "team",
            Description = "维护系统用户角色及其菜单权限信息。",
            Target = ModuleType.Page.ToString(),
            Url = "/sys/roles",
            Sort = 2,
            Enabled = true,
            EntityData = @"系统角色|SysRole
名称|Name|Text|50|Y
状态|Enabled|Switch
备注|Note|TextArea",
            PageData = "{\"Type\":null,\"ShowPager\":true,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":[\"New\",\"DeleteM\"],\"Actions\":[\"Edit\",\"Delete\"],\"Columns\":[{\"Id\":\"Name\",\"Name\":\"名称\",\"IsViewLink\":true,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":120,\"Align\":null},{\"Id\":\"Enabled\",\"Name\":\"状态\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":60,\"Align\":\"center\"},{\"Id\":\"Note\",\"Name\":\"备注\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":500,\"Align\":null}]}",
            FormData = "{\"Width\":1000,\"Maximizable\":false,\"DefaultMaximized\":false,\"LabelSpan\":null,\"WrapperSpan\":null,\"Fields\":[{\"Row\":1,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Name\",\"Name\":\"名称\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Enabled\",\"Name\":\"状态\",\"Type\":4,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Note\",\"Name\":\"备注\",\"Type\":1,\"Length\":null,\"Required\":false}]}"
        };
    }

    private static SysModule GetSysUser(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysUserList",
            Name = "用户管理",
            Icon = "user",
            Description = "维护系统用户账号及角色信息。",
            Target = ModuleType.Page.ToString(),
            Url = "/sys/users",
            Sort = 3,
            Enabled = true,
            EntityData = @"系统用户|SysUser
组织编码|OrgNo|Text|50
用户名|UserName|Text|50|Y
密码|Password|Text|50|Y
姓名|Name|Text|50
英文名|EnglishName|Text|50
性别|Gender|Text|50
固定电话|Phone|Text|50
移动电话|Mobile|Text|50
电子邮件|Email|Text|50
状态|Enabled|Switch
简介|Note|TextArea|500
首次登录时间|FirstLoginTime|Date
首次登录IP|FirstLoginIP|Text|50
最近登录时间|LastLoginTime|Date
最近登录IP|LastLoginIP|Text|50
类型|Type|Text|500
角色|Role|Text|500
数据|Data|Text",
            PageData = "{\"Type\":null,\"ShowPager\":true,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":[\"New\",\"DeleteM\",\"Enable\",\"Disable\",\"ResetPassword\",\"ChangeDepartment\"],\"Actions\":[\"Edit\",\"Delete\"],\"Columns\":[{\"Id\":\"UserName\",\"Name\":\"用户名\",\"IsViewLink\":true,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":120,\"Align\":null},{\"Id\":\"Name\",\"Name\":\"姓名\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"Gender\",\"Name\":\"性别\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":60,\"Align\":null},{\"Id\":\"Mobile\",\"Name\":\"移动电话\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":120,\"Align\":null},{\"Id\":\"Email\",\"Name\":\"电子邮件\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":120,\"Align\":null},{\"Id\":\"Enabled\",\"Name\":\"状态\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":60,\"Align\":\"center\"},{\"Id\":\"Role\",\"Name\":\"角色\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":200,\"Align\":null}]}",
            FormData = "{\"Width\":800,\"Maximizable\":false,\"DefaultMaximized\":false,\"LabelSpan\":null,\"WrapperSpan\":null,\"Fields\":[{\"Row\":1,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"UserName\",\"Name\":\"用户名\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":2,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Name\",\"Name\":\"姓名\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":2,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"EnglishName\",\"Name\":\"英文名\",\"Type\":0,\"Length\":null,\"Required\":false},{\"Row\":2,\"Column\":2,\"CategoryType\":null,\"Category\":\"GenderType\",\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Gender\",\"Name\":\"性别\",\"Type\":7,\"Length\":null,\"Required\":false},{\"Row\":3,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Phone\",\"Name\":\"固定电话\",\"Type\":0,\"Length\":null,\"Required\":false},{\"Row\":3,\"Column\":2,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Mobile\",\"Name\":\"移动电话\",\"Type\":0,\"Length\":null,\"Required\":false},{\"Row\":4,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Email\",\"Name\":\"电子邮件\",\"Type\":0,\"Length\":null,\"Required\":false},{\"Row\":4,\"Column\":2,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Enabled\",\"Name\":\"状态\",\"Type\":4,\"Length\":null,\"Required\":true}]}"
        };
    }

    private static SysModule GetSysTask(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysTaskList",
            Name = "后台任务",
            Icon = "control",
            Description = "查询系统所有定时任务运行情况。",
            Target = ModuleType.Page.ToString(),
            Url = "/sys/tasks",
            Sort = 4,
            Enabled = true,
            EntityData = @"系统任务|SysTask
业务ID|BizId|Text|50|Y
类型|Type|Text|50
名称|Name|Text|50
执行目标|Target|Text|200
执行状态|Status|Text|50|Y
开始时间|BeginTime|Date
结束时间|EndTime|Date
备注|Note|Text",
            PageData = "{\"Type\":null,\"ShowPager\":true,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":[\"Reset\",\"DeleteM\",\"Export\"],\"Actions\":[\"Delete\"],\"Columns\":[{\"Id\":\"Type\",\"Name\":\"类型\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"Name\",\"Name\":\"名称\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":null},{\"Id\":\"Target\",\"Name\":\"执行目标\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":180,\"Align\":null},{\"Id\":\"Status\",\"Name\":\"执行状态\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"BeginTime\",\"Name\":\"开始时间\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":\"Descend\",\"Fixed\":null,\"Width\":150,\"Align\":\"center\"},{\"Id\":\"EndTime\",\"Name\":\"结束时间\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":\"center\"},{\"Id\":\"Note\",\"Name\":\"备注\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":200,\"Align\":null}]}"
        };
    }

    private static SysModule GetSysFile(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysFileList",
            Name = "系统附件",
            Icon = "file",
            Description = "查询系统所有附件信息。",
            Target = ModuleType.Page.ToString(),
            Url = "/sys/files",
            Sort = 5,
            Enabled = true,
            EntityData = @"系统文件|SysFile
一级分类|Category1|Text|50|Y
二级分类|Category2|Text|50
文件名称|Name|Text|250|Y
文件类型|Type|Text|50
文件路径|Path|Text|500
文件大小|Size|Number
原文件名|SourceName|Text|250|Y
扩展名|ExtName|Text|50|Y
备注|Note|Text|500
业务ID|BizId|Text|50
文件缩略图路径|ThumbPath|Text|500",
            PageData = "{\"Type\":null,\"ShowPager\":true,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":null,\"Actions\":null,\"Columns\":[{\"Id\":\"Category1\",\"Name\":\"一级分类\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"Category2\",\"Name\":\"二级分类\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"Name\",\"Name\":\"文件名称\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":180,\"Align\":null},{\"Id\":\"Type\",\"Name\":\"文件类型\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":null},{\"Id\":\"Size\",\"Name\":\"文件大小\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":80,\"Align\":null},{\"Id\":\"SourceName\",\"Name\":\"原文件名\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":180,\"Align\":null},{\"Id\":\"ExtName\",\"Name\":\"扩展名\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":80,\"Align\":null},{\"Id\":\"Note\",\"Name\":\"备注\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":200,\"Align\":null}]}"
        };
    }

    private static SysModule GetSysLog(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysLogList",
            Name = "系统日志",
            Icon = "clock-circle",
            Description = "查询系统用户操作日志信息。",
            Target = ModuleType.Page.ToString(),
            Url = "/sys/logs",
            Sort = 6,
            Enabled = true,
            EntityData = @"系统日志|SysLog
操作类型|Type|Text|50|Y
操作对象|Target|Text|50|Y
操作内容|Content|Text",
            PageData = "{\"Type\":null,\"ShowPager\":true,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":[\"Export\"],\"Actions\":null,\"Columns\":[{\"Id\":\"Type\",\"Name\":\"操作类型\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":true,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"Target\",\"Name\":\"操作对象\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":null},{\"Id\":\"Content\",\"Name\":\"操作内容\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":null,\"Align\":null},{\"Id\":\"CreateBy\",\"Name\":\"创建人\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"CreateTime\",\"Name\":\"创建时间\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":\"Descend\",\"Fixed\":null,\"Width\":150,\"Align\":null}]}"
        };
    }

    private static SysModule GetSysModule(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysModuleList",
            Name = "模块管理",
            Icon = "appstore-add",
            Description = "维护系统菜单按钮及列表栏位信息。",
            Target = ModuleType.Page.ToString(),
            Url = "/sys/modules",
            Sort = 7,
            Enabled = true,
            EntityData = @"系统模块|SysModule
上级|ParentId|Text|50
代码|Code|Text|50|Y
名称|Name|Text|50|Y
图标|Icon|Text|50
描述|Description|Text|200
类型|Target|Text|50
URL|Url|Text|200
顺序|Sort|Number
可用|Enabled|Switch
实体设置|EntityData|Text
页面设置|PageData|Text
表单设置|FormData|Text
备注|Note|Text|500",
            PageData = "{\"Type\":null,\"ShowPager\":false,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":[\"New\",\"DeleteM\",\"Copy\",\"Move\"],\"Actions\":[\"Edit\",\"Delete\",\"MoveUp\",\"MoveDown\"],\"Columns\":[{\"Id\":\"Code\",\"Name\":\"代码\",\"IsViewLink\":true,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":130,\"Align\":null},{\"Id\":\"Name\",\"Name\":\"名称\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":120,\"Align\":null},{\"Id\":\"Description\",\"Name\":\"描述\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":180,\"Align\":null},{\"Id\":\"Target\",\"Name\":\"类型\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":80,\"Align\":\"center\"},{\"Id\":\"Url\",\"Name\":\"Url\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":null},{\"Id\":\"Sort\",\"Name\":\"顺序\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":60,\"Align\":\"center\"},{\"Id\":\"Enabled\",\"Name\":\"可用\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":60,\"Align\":\"center\"},{\"Id\":\"Note\",\"Name\":\"备注\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":null}]}",
            FormData = "{\"Width\":1200,\"Maximizable\":true,\"DefaultMaximized\":false,\"LabelSpan\":null,\"WrapperSpan\":null,\"Fields\":[{\"Row\":1,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Code\",\"Name\":\"代码\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":2,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Name\",\"Name\":\"名称\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":2,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Icon\",\"Name\":\"图标\",\"Type\":0,\"Length\":null,\"Required\":false},{\"Row\":4,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Description\",\"Name\":\"描述\",\"Type\":0,\"Length\":null,\"Required\":false},{\"Row\":2,\"Column\":2,\"CategoryType\":\"Custom\",\"Category\":\"ModuleType\",\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Target\",\"Name\":\"类型\",\"Type\":7,\"Length\":null,\"Required\":true},{\"Row\":3,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Url\",\"Name\":\"URL\",\"Type\":0,\"Length\":null,\"Required\":false},{\"Row\":3,\"Column\":2,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Enabled\",\"Name\":\"可用\",\"Type\":4,\"Length\":null,\"Required\":true},{\"Row\":5,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Note\",\"Name\":\"备注\",\"Type\":1,\"Length\":null,\"Required\":false}]}"
        };
    }
}