<center>

![Logo](https://foruda.gitee.com/images/1703494572944391289/24f26ae0_14334.png "屏幕截图")

`Known`是基于`Blazor`轻量级、跨平台、极易扩展的插件开发框架。

[![star](https://gitee.com/known/Known/badge/star.svg?theme=dark)](https://gitee.com/known/Known/stargazers)
[![star](https://img.shields.io/github/stars/known/known?color=%231890FF)](https://github.com/known/Known)
[![License](https://img.shields.io/badge/license-Apache2-yellow)](https://gitee.com/known/Known/blob/master/LICENSE)
[![Nuget](https://img.shields.io/nuget/v/Known.svg?color=red&logo=nuget&logoColor=green)](https://www.nuget.org/packages/Known)
[![Nuget](https://img.shields.io/nuget/dt/Known.svg?logo=nuget&logoColor=green)](https://www.nuget.org/packages/Known)

![.NET](https://img.shields.io/badge/.NET-8.0-green)
![.NET](https://img.shields.io/badge/.NET-9.0-green)
![.DEV](https://img.shields.io/badge/DEV-VS2022-brightgreen)
![QQ群](https://img.shields.io/badge/QQ群-865982686-blue)

</center>

- 官网：[https://known.org.cn](https://known.org.cn)
- 源码：[https://gitee.com/known/Known](https://gitee.com/known/Known)
- 源码：[https://github.com/known/Known](https://github.com/known/Known)

### 项目特性

- 插件开发：基于`Blazor`实现，轻量级、跨平台、极易扩展的插件开发。
- 通用权限：内置通用权限模块，基于角色鉴权，`URL`检查，只需专注业务功能。
- 最佳实践：低代码开发，一人多角色，沟通高效，进度快，无前后端沟通成本。
- 国际化：提供完备的国际化多语言解决方案，默认支持简体中文。
- 现代`UI`：基于`Ant Design`设计风格的现代`UI`展现层，易扩展。
- `C#`语言：全量使用C#语言进行全栈开发，从前端到后端只需学习一门语言。


### 项目结构

```
├─Shared            -> Known与Known.Pure的代码共享项目，包含Excel接口、Database、微信、工作流等公用代码。
├─Known             -> 框架核心类库，定义统一的对象模型、接口、组件和扩展，内置权限、微信、工作流。
├─Known.Pure        -> 无Blazor功能的纯净版Known类库。
├─Plugins           -> 插件项目
| ├─Known.Core      -> 基于Asp.Net Core的服务端插件。
| ├─Known.Cells     -> 基于Aspose.Cells实现的Excel插件。
├─Sample            -> 示例项目
  ├─Sample.Wasm     -> 框架WebAssembly示例。
  ├─Sample.Web      -> 框架WebApp示例。
  ├─Sample.WinForm  -> 框架WinForm示例。
  ├─Sample.Maui     -> 框架MAUI示例。
  ├─Sample.Photino  -> 框架Photino.NET示例。
```

### 项目链接

- `KnownDB`：[https://gitee.com/known/KnownDB](https://gitee.com/known/KnownDB)
- `JxcLite`：[https://gitee.com/known/JxcLite](https://gitee.com/known/JxcLite)
- `KnownCMS`：[https://gitee.com/known/KnownCMS](https://gitee.com/known/KnownCMS)

### 界面截图

![亮色](Document/home1.png)
![暗色](Document/home2.png)

### 捐赠支持

> 如果你觉得这个框架对你有帮助，你可以请作者喝杯咖啡表示鼓励 ☕️

![捐赠支持](https://foruda.gitee.com/images/1726452783813098766/71768ec0_14334.png "屏幕截图")