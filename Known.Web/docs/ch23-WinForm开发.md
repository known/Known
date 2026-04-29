# 第23章 WinForm 开发

Known 的 WinForm 宿主用于把同一套业务平台能力放到桌面应用中运行。它并不是传统 WinForms 控件式开发，而是通过桌面宿主承载 Known 页面和平台逻辑。

---

## 23.1 启动方式

`Known.WinForm/AppConfig.cs` 中，桌面宿主通过以下方式装配：

- `services.AddKnown(...)`
- `services.AddSample()`
- `services.AddKnownCells()`
- `services.AddKnownDesktop(...)`

其中 `AddKnownDesktop(...)` 会把应用模式切换为 `AppType.Desktop`，并关闭通知 Hub。

---

## 23.2 数据库与资源

WinForm 示例默认使用本地 SQLite：

```csharp
db.AddSQLite<Microsoft.Data.Sqlite.SqliteFactory>(@"Data Source=.\Sample.db");
```

同时将 `WebRoot` 和 `ContentRoot` 指向应用目录，适合单机桌面应用部署。

---

## 23.3 适用场景

- 内网单机管理工具
- 需要本地数据库的轻量业务系统
- 不方便部署 Web 服务器的桌面业务工具

---

## 23.4 开发建议

1. 页面和服务尽量继续复用已有 Known 业务模块。
2. 优先使用 SQLite 等本地数据库。
3. 需要打印、Excel、文件操作时，桌面模式通常更方便。

---

## 相关源码文件

- `Known.WinForm/Program.cs`：WinForm 程序入口。
- `Known.WinForm/AppConfig.cs`：WinForm 宿主装配。
- `Known.WinForm/MainForm.cs`：桌面主窗口宿主。

## 继续阅读

- 上一章：[第22章 Wasm 模式开发](ch22-Wasm%E6%A8%A1%E5%BC%8F%E5%BC%80%E5%8F%91.md)
- 下一章：[第24章 MAUI 开发](ch24-MAUI%E5%BC%80%E5%8F%91.md)
- 推荐配套阅读：[第21章 Server 模式开发](ch21-Server%E6%A8%A1%E5%BC%8F%E5%BC%80%E5%8F%91.md)

## 本章小结

WinForm 模式的价值在于复用 Known 的平台能力，而不是重新做一套桌面业务系统架构。
