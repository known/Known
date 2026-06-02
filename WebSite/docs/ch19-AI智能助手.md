# 第19章 AI 智能助手

Known 已经内置 AI 能力，不需要额外集成整套聊天框架。其核心实现位于 `Known/AI`，支持云端模型、本地模型和自定义扩展模型三种接入方式。

---

## 19.1 核心组成

AI 相关关键文件包括：

- `ChatService.cs`
- `OpenAIClient.cs`
- `OllamaClient.cs`
- `ExtendService.cs`
- `ChatView.razor`
- `ChatList.cs`

---

## 19.2 服务模型

AI 服务接口 `IChatService` 提供的核心能力包括：

- 发送聊天消息
- 获取会话记录
- 清理会话
- 分页查询聊天记录
- 删除聊天记录
- 保存聊天记录

这说明 Known 的 AI 能力不仅是即时聊天，还包含会话持久化和后台管理。

---

## 19.3 支持的模型类型

`ChatService` 中可以看到框架支持以下模型类型：

- `OpenAI`
- `Ollama`
- `Extend`
- 默认模拟模式

对应说明：

- `OpenAI`：适合接入云端大模型服务
- `Ollama`：适合本地部署模型
- `Extend`：适合接入任意第三方或企业内部模型

---

## 19.4 流式输出

聊天发送接口使用 `IAsyncEnumerable<string>` 返回结果，说明 Known 已支持流式输出体验。对话页面可以逐步显示模型返回内容，而不是必须等待完整响应。

---

## 19.5 会话持久化

AI 会话并不是只存在浏览器内存中。`ChatService` 会把会话落到数据库中，便于：

- 查看历史对话
- 删除旧记录
- 按用户和助理区分会话
- 构建上下文消息列表

这对企业内部知识问答或工作助手很重要。

---

## 19.6 扩展方式

如果内置 OpenAI 和 Ollama 不能满足需求，推荐实现 `IExtendService`，把模型接入逻辑集中放在扩展服务中。

这样可以避免把第三方 API 调用散落到业务页面中。

---

## 19.7 典型应用场景

Known 的 AI 模块适合：

- 系统内置聊天助手
- 业务知识问答
- 开发辅助或代码说明
- 表单填写建议
- 流程辅助决策

如果要进一步做企业智能体，也可以在现有会话与模型接入层上继续扩展。

---

## 相关源码文件

- `Known/AI/ChatService.cs`：AI 聊天服务。
- `Known/AI/OpenAIClient.cs`：OpenAI 客户端。
- `Known/AI/OllamaClient.cs`：Ollama 客户端。
- `Known/AI/ExtendService.cs`：扩展模型接入点。
- `Known/AI/ChatView.razor`、`Known/AI/ChatList.cs`：前端对话与记录页。

## 继续阅读

- 上一章：[第18章 安全机制](ch18-%E5%AE%89%E5%85%A8%E6%9C%BA%E5%88%B6.md)
- 下一章：[第20章 微信集成](ch20-%E5%BE%AE%E4%BF%A1%E9%9B%86%E6%88%90.md)
- 推荐配套阅读：[第10章 插件体系](ch10-%E6%8F%92%E4%BB%B6%E4%BD%93%E7%B3%BB.md)

## 本章小结

Known 的 AI 能力已经覆盖模型接入、流式输出、会话存储和后台管理，足以作为业务系统中的 AI 底座。
