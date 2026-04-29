# 附录A 配置参考

本附录汇总 Known 开发中最常用的配置入口。

---

## A.1 应用级配置

主要入口：`Config.App`

常见字段：

- `Id`
- `Name`
- `Assembly`
- `WebRoot`
- `ContentRoot`
- `Database`
- `IsPlatform`
- `IsTopMenu`

---

## A.2 后端配置

主要入口：`CoreConfig`

常用扩展点：

- `OnInitial`
- `OnAdmin`
- `OnInstall`
- `OnRegistering`
- `OnRegistered`
- `OnLoging`
- `OnLoged`
- `OnDatabase`
- `CheckSystem`

---

## A.3 UI 配置

主要入口：`UIConfig`

常用配置：

- `HomeUrl`
- `SideWidth`
- `IgnoreRoutes`
- `UserTabs`
- `SystemTabs`
- `ModuleTabs`
- `CompanyTabs`

---

## A.4 宿主中的数据库配置

常见写法：

```csharp
option.App.Database = db =>
{
    db.AddSQLite<Microsoft.Data.Sqlite.SqliteFactory>(connString);
};
```

---

## A.5 文件与上传路径

相关方法：

- `Config.GetUploadDirectory(...)`
- `Config.GetUploadPath(...)`
- `Config.GetFileUrl(...)`

用于统一管理上传目录和文件访问 URL。
