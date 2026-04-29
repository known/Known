# Known 框架文档导读

Known 是一个基于 Blazor 的插件化全栈业务开发框架，核心特点是：统一 C# 开发、内置后台基座、支持低代码扩展、支持多宿主运行。

如果你第一次接触 Known，建议先按通用路径通读一遍，再按自己的角色深入。

## 通用阅读路径

1. 先看 [第1章 框架概述](ch01-%E6%A1%86%E6%9E%B6%E6%A6%82%E8%BF%B0.md)
2. 再看 [第2章 快速开始](ch02-%E5%BF%AB%E9%80%9F%E5%BC%80%E5%A7%8B.md)
3. 然后看 [第3章 架构概述](ch03-%E6%9E%B6%E6%9E%84%E6%A6%82%E8%BF%B0.md) 和 [第4章 内置模块](ch04-%E5%86%85%E7%BD%AE%E6%A8%A1%E5%9D%97.md)
4. 进入核心开发：实体、数据访问、服务、页面、权限
5. 最后再读插件、开发中心、工作流、AI、微信、多端宿主

## 按角色阅读

### 初学者

1. [第1章 框架概述](ch01-%E6%A1%86%E6%9E%B6%E6%A6%82%E8%BF%B0.md)
2. [第2章 快速开始](ch02-%E5%BF%AB%E9%80%9F%E5%BC%80%E5%A7%8B.md)
3. [第3章 架构概述](ch03-%E6%9E%B6%E6%9E%84%E6%A6%82%E8%BF%B0.md)
4. [第4章 内置模块](ch04-%E5%86%85%E7%BD%AE%E6%A8%A1%E5%9D%97.md)
5. [第28章 从 0 到 1 开发一个插件模块](ch28-%E6%8F%92%E4%BB%B6%E6%A8%A1%E5%9D%97%E5%AE%9E%E6%88%98.md)

### 业务开发者

1. [第5章 实体模型](ch05-%E5%AE%9E%E4%BD%93%E6%A8%A1%E5%9E%8B.md)
2. [第6章 数据访问](ch06-%E6%95%B0%E6%8D%AE%E8%AE%BF%E9%97%AE.md)
3. [第7章 服务模型](ch07-%E6%9C%8D%E5%8A%A1%E6%A8%A1%E5%9E%8B.md)
4. [第8章 页面开发](ch08-%E9%A1%B5%E9%9D%A2%E5%BC%80%E5%8F%91.md)
5. [第9章 权限与菜单](ch09-%E6%9D%83%E9%99%90%E4%B8%8E%E8%8F%9C%E5%8D%95.md)
6. [第28章 从 0 到 1 开发一个插件模块](ch28-%E6%8F%92%E4%BB%B6%E6%A8%A1%E5%9D%97%E5%AE%9E%E6%88%98.md)

### 平台开发者

1. [第10章 插件体系](ch10-%E6%8F%92%E4%BB%B6%E4%BD%93%E7%B3%BB.md)
2. [第11章 开发中心](ch11-%E5%BC%80%E5%8F%91%E4%B8%AD%E5%BF%83.md)
3. [第12章 工作流引擎](ch12-%E5%B7%A5%E4%BD%9C%E6%B5%81%E5%BC%95%E6%93%8E.md)
4. [第16章 配置体系](ch16-%E9%85%8D%E7%BD%AE%E4%BD%93%E7%B3%BB.md)
5. [第17章 缓存与性能](ch17-%E7%BC%93%E5%AD%98%E4%B8%8E%E6%80%A7%E8%83%BD.md)
6. [第18章 安全机制](ch18-%E5%AE%89%E5%85%A8%E6%9C%BA%E5%88%B6.md)

### 二次开发与低代码

1. [第4章 内置模块](ch04-%E5%86%85%E7%BD%AE%E6%A8%A1%E5%9D%97.md)
2. [第10章 插件体系](ch10-%E6%8F%92%E4%BB%B6%E4%BD%93%E7%B3%BB.md)
3. [第11章 开发中心](ch11-%E5%BC%80%E5%8F%91%E4%B8%AD%E5%BF%83.md)
4. [第12章 工作流引擎](ch12-%E5%B7%A5%E4%BD%9C%E6%B5%81%E5%BC%95%E6%93%8E.md)
5. [第28章 从 0 到 1 开发一个插件模块](ch28-%E6%8F%92%E4%BB%B6%E6%A8%A1%E5%9D%97%E5%AE%9E%E6%88%98.md)

## 你能从 Known 获得什么

- 一套可直接运行的系统后台基座
- 一套统一的实体、服务、页面开发模型
- 一套可扩展的插件体系和开发中心
- 一套覆盖 Server、Wasm、WinForm、MAUI 的宿主方案

## 重点源码入口

- 核心装配：`Known/Extension.cs`
- Web 宿主增强：`Plugins/Known.Core/CoreExtension.cs`
- 开发中心：`Plugins/Known.Admin`
- 示例业务：`Plugins/Known.Sample`
- Server 启动入口：`Known.Server/Program.cs`

## 推荐边看边跑

建议优先运行 `Known.Server`，并结合以下源码一起阅读：

- `Known.Server/AppConfig.cs`
- `Plugins/Known.Sample/AppModule.cs`
- `Plugins/Known.Sample/Pages/Produce/MaterialList.cs`
- `Plugins/Known.Sample/Services/ProduceService.cs`

这样最容易把文档内容和实际运行效果对应起来。
