# 附录C 源码索引

本附录用于帮助你快速定位 Known 的关键源码目录和文件。

---

## C.1 核心装配与配置

| 文件 | 说明 |
|------|------|
| `Known/Extension.cs` | 框架核心装配入口 |
| `Known/Config.cs` | 全局配置、程序集和模块注册 |
| `Known/CoreConfig.cs` | 后端配置与扩展钩子 |
| `Known/Blazor/UIConfig.cs` | UI 配置 |
| `Plugins/Known.Core/CoreExtension.cs` | Web 宿主增强、动态 API、认证、UseKnown |

---

## C.2 实体与数据访问

| 文件 | 说明 |
|------|------|
| `Known/Entity.cs` | 实体基类 |
| `Known/Entities/*.cs` | 系统实体 |
| `Known/Data/Database*.cs` | 数据访问实现 |
| `Known/Data/QueryBuilder.cs` | 表达式查询构造 |
| `Known/Data/DbProvider*.cs` | 多数据库适配 |

---

## C.3 服务与页面

| 文件 | 说明 |
|------|------|
| `Known/Service.cs` | `IService`、`ServiceBase`、`ClientBase` |
| `Known/Blazor/Base.Page.cs` | 页面基类 |
| `Known/Blazor/Base.TablePage.cs` | 列表页基类 |
| `Known/Blazor/Base.Form.cs` | 表单基类 |
| `Known/Pages/*.cs` | 系统内置页面 |

---

## C.4 插件与低代码

| 文件 | 说明 |
|------|------|
| `Known/Plugins/Attributes.cs` | 插件特性定义 |
| `Known/Plugins/PluginConfig.cs` | 插件注册表 |
| `Known/Plugins/PluginBase.cs` | 插件基类 |
| `Known/Plugins/PluginPage.cs` | 页面插件容器 |
| `Known/Pages/AutoPage.cs` | 低代码统一页面入口 |

---

## C.5 平台能力

| 文件 | 说明 |
|------|------|
| `Known/Task.cs` | 后台任务 |
| `Known/Import.cs` | 导入体系 |
| `Known/WorkFlows/*` | 工作流引擎 |
| `Known/AI/*` | AI 能力 |
| `Known/Weixins/*` | 微信能力 |
| `Known/Services/FileService.cs` | 文件服务 |
| `Known/Services/LanguageService.cs` | 多语言服务 |

---

## C.6 插件项目

| 项目 | 说明 |
|------|------|
| `Plugins/Known.Core` | Web 后端增强、Redis、Processer、动态 API |
| `Plugins/Known.Admin` | 开发中心、模块管理、设计器 |
| `Plugins/Known.Cells` | Excel/PDF 输出实现 |
| `Plugins/Known.Sample` | 示例业务、报表、大屏、打印、工作流 |

---

## C.7 宿主项目

| 项目 | 说明 |
|------|------|
| `Known.Server` | Server 模式宿主 |
| `Known.Wasm` | Wasm 客户端宿主 |
| `Known.WasmHost` | Wasm 后端宿主 |
| `Known.WinForm` | WinForm 宿主 |
| `Known.Maui` | MAUI 宿主 |
| `Known.Photino` | Photino 桌面宿主示例 |
