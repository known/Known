# 快速开始

### 1. 安装项目模板并创建新项目

- 打开命令行输入如下命令安装和创建。

```bash
-- 安装模板
dotnet new install KnownTemplate
-- 创建项目
dotnet new known --name=KIMS
```

> 说明：KIMS为测试项目名称

- 安装和创建成功后，项目文件夹自动生成如下内容

```plaintext
├─KIMS          ->项目前后端共用库，客户端和实体类等。
├─KIMS.Client   ->Web前端，Blazor WebAssembly。
├─KIMS.Core     ->项目后端库，控制器、服务、数据访问等。
├─KIMS.Razor    ->项目前端库，模块页面和表单。
├─KIMS.Server   ->Web后端。
├─KIMS.WinForm  ->WinForm窗体及Razor页面。
├─KIMSAlone     ->桌面exe程序。
├─KIMS.sln      ->VS解决方案文件。
```

### 2. 打开解决方案配置应用

- 使用 VS2022 打开 KIMS.sln 文件，打开 KIMS 项目下 AppConfig.cs 文件，配置App名称，示例代码如下：

```csharp
public class AppConfig
{
    public static void Initialize()
    {
        Config.AppId = "KIMS";                             //系统ID，自动生成，默认项目名称
        Config.AppName = "Known管理系统";                   //在此配置你的系统名称
        Config.SetAppAssembly(typeof(AppConfig).Assembly); //App程序集，自动获取版本，反射实体模型用于模块管理配置列表字段

        PagingCriteria.DefaultPageSize = 20;               //默认分页大小
        DicCategory.AddCategories<AppDictionary>();        //自动加载数据字典类别，在AppDictionary中增加类别
        Cache.AttachCodes(typeof(AppConfig).Assembly);     //自动加载CodeTable特性类常量进入缓存
        //在此配置你的系统其他全局配置
    }
}
```

### 3. 配置后端数据库连接

- 打开 KIMS.Server 项目文件，添加你的系统使用的数据库访问包，常用数据库包如下

```xml
-- SQLite
<PackageReference Include="Microsoft.Data.Sqlite" Version="7.0.8" />
-- Access
<PackageReference Include="System.Data.OleDb" Version="7.0.0" />
-- MySQL
<PackageReference Include="MySqlConnector" Version="2.2.5" />
```

- 打开项目下 AppServer.cs 文件修改数据库连接，示例代码如下：

```csharp
class AppServer
{
    internal static void Initialize(WebApplicationBuilder builder)
    {
        //配置环境目录
        KCConfig.WebRoot = builder.Environment.WebRootPath;
        KCConfig.ContentRoot = builder.Environment.ContentRootPath;
        //读取appsettings.json配置
        var configuration = builder.Configuration;
        var dbFile = configuration.GetSection("DBFile").Get<string>();//数据库配置
        var uploadPath = configuration.GetSection("UploadPath").Get<string>();//上传文件存储路径
        Initialize(dbFile, uploadPath);
    }

    internal static void Initialize(string? dbFile, string? uploadPath)
    {
        //初始化配置
        AppConfig.Initialize();
        AppCore.Initialize();
        //转换绝对路径
        var path = KCConfig.ContentRoot;
        dbFile = Path.GetFullPath(Path.Combine(path, dbFile));
        uploadPath = Path.GetFullPath(Path.Combine(path, uploadPath));
        //注册数据访问提供者和初始化数据库连接
        Database.RegisterProviders(new Dictionary<string, Type>
        {
            ["SQLite"] = typeof(Microsoft.Data.Sqlite.SqliteFactory)
        });
        var connInfo = new Known.Core.ConnectionInfo
        {
            Name = "Default",
            ProviderName = "SQLite",
            ConnectionString = $"Data Source={dbFile};"
        };
        KCConfig.App = new AppInfo
        {
            Connections = new List<Known.Core.ConnectionInfo> { connInfo },
            UploadPath = uploadPath
        };
    }
}
```

### 4. 创建数据库表

- 数据库表脚本详见框架源码文件夹Document\Scripts
- 默认有Access、MySql、Oracle、SQLite、SqlServer脚本

### 5. 配置完成运行项目

- 到此简单配置已完成，现在可以点击VS运行 KIMS.Server 项目啦，运行效果如下：

![输入图片说明](https://foruda.gitee.com/images/1684208404409711237/6154486a_14334.png "屏幕截图")
