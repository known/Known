# 第22章 Wasm 模式开发

Known 的 Wasm 模式不是单工程结构，而是由 `Known.Wasm` 客户端和 `Known.WasmHost` 后端宿主共同组成。它适合更强调前端本地运行和远程 API 调用的场景。

---

## 22.1 双工程结构

### 22.1.1 `Known.Wasm`

客户端工程，负责：

- 启动 WebAssembly 应用
- 调用 `AddApplicationClient(...)`
- 注册 `AddKnownClient(...)`
- 通过 `ClientBase` 访问后端 WebApi

### 22.1.2 `Known.WasmHost`

后端工程，负责：

- 提供 API
- 提供 Razor Components 宿主
- 提供数据库访问和平台能力
- 注册 `AddKnownWeb(...)`

---

## 22.2 客户端启动

`Known.Wasm/Program.cs` 的关键代码是：

```csharp
builder.Services.AddApplicationClient(option =>
{
    option.BaseAddress = builder.HostEnvironment.BaseAddress + "api";
});
```

这会先调用应用级装配，再注册 `ClientBase` 所需的 HTTP 客户端和客户端代理类。

---

## 22.3 Host 启动

`Known.WasmHost/AppServer.cs` 会先复用 `Known.Wasm` 的基础应用装配，再补上 `AddKnownWeb(...)`，形成完整后端宿主。

这说明 Wasm 模式下，业务模块本身仍然是同一套，只是客户端调用路径变成了 HTTP。

---

## 22.4 Wasm 模式下的服务开发

在 Wasm 模式下，页面最常依赖的是 `[Client]` 标记的代理服务，而这些代理最终调用的是后端 `[WebApi]` 服务。

因此服务开发时更应遵循：

1. 定义接口
2. 定义客户端代理
3. 定义服务端实现

---

## 22.5 优势与限制

### 优势

- 前端可在浏览器本地运行
- 服务契约清晰
- 更适合对前后端分离边界有要求的场景

### 限制

- 对网络依赖更明显
- 复杂实时交互场景不如 Server 模式直接
- 调试链路比 Server 模式更长

---

## 相关源码文件

- `Known.Wasm/Program.cs`：Wasm 客户端启动入口。
- `Known.Wasm/AppConfig.cs`：Wasm 客户端装配。
- `Known.WasmHost/Program.cs`：WasmHost 宿主入口。
- `Known.WasmHost/AppServer.cs`：WasmHost 装配封装。

## 继续阅读

- 上一章：[第21章 Server 模式开发](ch21-Server%E6%A8%A1%E5%BC%8F%E5%BC%80%E5%8F%91.md)
- 下一章：[第23章 WinForm 开发](ch23-WinForm%E5%BC%80%E5%8F%91.md)
- 推荐配套阅读：[第7章 服务模型](ch07-%E6%9C%8D%E5%8A%A1%E6%A8%A1%E5%9E%8B.md)

## 本章小结

Known 的 Wasm 模式本质是“同一套业务模块 + 客户端代理 + 后端宿主”的双工程协作模式。
