# 第21章 Server 模式开发

Server 模式是 Known 最完整、最推荐的宿主模式。它的特点是页面交互、服务注入、数据库访问和平台能力都运行在同一 ASP.NET Core 进程中，最适合后台管理系统和平台型系统。

---

## 21.1 启动入口

`Known.Server/Program.cs` 是标准入口，关键步骤如下：

1. 注册 Razor Components 和 Server 交互模式。
2. 调用 `builder.Services.AddApplication(...)`。
3. 配置应用数据库、程序集和路径。
4. 构建后调用 `app.UseApplication()`。

---

## 21.2 `AddApplication` 做了什么

`Known.Server/AppConfig.cs` 中的 `AddApplication` 实际完成了：

- 设置 `Config.IsDevelopment`、`Config.IsDebug`
- 配置 `CoreConfig.OnInitial`
- 调用 `services.AddKnown(...)`
- 注册示例插件 `services.AddSample()`
- 注册 UI 资源
- 调用 `services.AddKnownWeb(...)`
- 注册后台工作线程 `TestWorker`

这意味着 Server 宿主是“框架核心 + 业务插件 + Web 增强”的完整组合。

---

## 21.3 `UseApplication` 做了什么

`UseApplication()` 内部调用 `app.UseKnown()`，因此会自动：

- 注册静态文件
- 映射上传文件目录
- 开启控制器和 Razor Pages
- 映射通知 Hub
- 初始化数据库基础数据

随后还会执行版本更新逻辑，这是宿主自己的附加行为。

---

## 21.4 Server 模式的优势

1. 页面可以直接调用服务和数据库能力。
2. 平台功能最完整，调试最方便。
3. SignalR 通知、在线用户、开发中心等能力体验最佳。
4. 适合大型后台和中台应用。

---

## 21.5 适合场景

- 企业内部后台
- 管理系统
- 中台系统
- 平台管理门户

---

## 相关源码文件

- `Known.Server/Program.cs`：Server 宿主启动入口。
- `Known.Server/AppConfig.cs`：Server 装配封装。
- `Plugins/Known.Core/CoreExtension.cs`：`AddKnownWeb` 与 `UseKnown`。

## 继续阅读

- 上一章：[第20章 微信集成](ch20-%E5%BE%AE%E4%BF%A1%E9%9B%86%E6%88%90.md)
- 下一章：[第22章 Wasm 模式开发](ch22-Wasm%E6%A8%A1%E5%BC%8F%E5%BC%80%E5%8F%91.md)
- 推荐配套阅读：[第2章 快速开始](ch02-%E5%BF%AB%E9%80%9F%E5%BC%80%E5%A7%8B.md)

## 本章小结

如果你只打算选择一种 Known 宿主模式进行正式开发，Server 模式通常是首选。
