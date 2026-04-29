# 附录B API参考

本附录列出阅读和二开时最常用的核心类型。

---

## B.1 核心装配

| 类型 | 作用 |
|------|------|
| `Extension` | `AddKnown`、`AddKnownCore`、`AddKnownClient` 等装配入口 |
| `CoreExtension` | `AddKnownWeb`、`UseKnown`、动态 WebApi、认证 |
| `Config` | 全局配置和程序集/模块注册 |
| `CoreConfig` | 后端扩展点 |
| `UIConfig` | UI 扩展点 |

---

## B.2 模型与数据访问

| 类型 | 作用 |
|------|------|
| `EntityBase` | 通用业务实体基类 |
| `Database` | 数据访问主入口 |
| `QueryBuilder<T>` | 表达式查询构造器 |
| `DbProvider` | 数据库适配层 |
| `PagingCriteria` | 分页查询条件 |
| `PagingResult<T>` | 分页结果 |

---

## B.3 服务与页面

| 类型 | 作用 |
|------|------|
| `IService` | 服务统一接口 |
| `ServiceBase` | 服务端业务基类 |
| `ClientBase` | 客户端代理基类 |
| `BasePage` | 页面基类 |
| `BaseTablePage<T>` | 列表页基类 |
| `BaseForm<T>` | 表单页基类 |
| `BaseTabPage` | 标签页组合基类 |

---

## B.4 插件与扩展

| 类型 | 作用 |
|------|------|
| `PluginAttribute` | 普通插件 |
| `DevPluginAttribute` | 开发中心插件 |
| `NavPluginAttribute` | 顶部导航插件 |
| `PagePluginAttribute` | 页面区块插件 |
| `PluginConfig` | 插件注册表 |
| `PluginBase<T>` | 区块插件基类 |
| `PluginPage` | 页面插件容器 |

---

## B.5 平台能力

| 类型 | 作用 |
|------|------|
| `TaskBase` | 后台任务基类 |
| `ImportBase` | 导入基类 |
| `FlowBase` | 工作流回调基类 |
| `ChatService` | AI 对话服务 |
| `WeixinService` | 微信服务 |
| `FileService` | 文件服务 |
| `LanguageService` | 多语言服务 |

---

## B.6 高频特性

| 特性 | 作用 |
|------|------|
| `[Table]` | 实体表映射 |
| `[Column]` | 表格列/查询元数据 |
| `[Form]` | 表单元数据 |
| `[Menu]` | 菜单 |
| `[Action]` | 页面动作 |
| `[Service]` | 服务注入 |
| `[Client]` | 客户端注入 |
| `[WebApi]` | 自动 API 暴露 |
| `[Task]` | 后台任务注册 |
| `[Import]` | 导入器注册 |
