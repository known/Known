# 第24章 MAUI 开发

Known 的 MAUI 宿主用于把统一业务平台能力带到移动和跨平台客户端中。当前仓库中的 `Known.Maui` 更偏示例性质，但已经展示了宿主装配方式。

---

## 24.1 启动入口

`Known.Maui/MauiProgram.cs` 中，应用通过：

- `builder.Services.AddApplication()`
- `builder.Services.AddMauiBlazorWebView()`

完成 Known 与 MAUI WebView 的结合。

---

## 24.2 应用装配

`Known.Maui/AppConfig.cs` 与 WinForm 类似，核心步骤包括：

- `AddKnown(...)`
- `AddSample()`
- `AddKnownDesktop(...)`
- 使用本地 SQLite

这说明 MAUI 版本当前同样采用桌面/本地模式的思路，而不是独立的远程 API 宿主结构。

---

## 24.3 平台说明

从项目配置和代码结构看，当前 MAUI 宿主主要展示了移动/跨平台承载能力，但文档描述时应避免夸大为“所有平台全量支持一致能力”。

在实际项目中，应重点验证：

- 本地文件路径
- WebView 资源加载
- 权限与系统 API 调用
- 网络与离线策略

---

## 24.4 适合场景

- 移动管理端
- 巡检、PDA、轻量业务操作端
- 需要共享后台逻辑的跨平台客户端

---

## 相关源码文件

- `Known.Maui/MauiProgram.cs`：MAUI 宿主启动入口。
- `Known.Maui/AppConfig.cs`：MAUI 应用装配。

## 继续阅读

- 上一章：[第23章 WinForm 开发](ch23-WinForm%E5%BC%80%E5%8F%91.md)
- 下一章：[第25章 代码规范](ch25-%E4%BB%A3%E7%A0%81%E8%A7%84%E8%8C%83.md)
- 推荐配套阅读：[第21章 Server 模式开发](ch21-Server%E6%A8%A1%E5%BC%8F%E5%BC%80%E5%8F%91.md)

## 本章小结

MAUI 宿主表明 Known 的业务能力可以延伸到移动端，但项目落地时仍要结合具体终端能力做适配验证。
