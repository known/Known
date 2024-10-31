![Logo](https://foruda.gitee.com/images/1703494572944391289/24f26ae0_14334.png "屏幕截图")

Known是基于Blazor的企业级快速开发框架，低代码，跨平台，开箱即用，一处代码，多处运行。

[![star](https://gitee.com/known/Known/badge/star.svg?theme=dark)](https://gitee.com/known/Known/stargazers)
[![stars](https://img.shields.io/github/stars/known/known?color=%231890FF)](https://github.com/known/Known)
[![License](https://img.shields.io/badge/license-Apache2-yellow)](https://gitee.com/known/Known/blob/master/LICENSE)
[![Nuget](https://img.shields.io/nuget/v/Known.svg?color=red&logo=nuget&logoColor=green)](https://www.nuget.org/packages/Known)
[![Nuget](https://img.shields.io/nuget/dt/Known.svg?logo=nuget&logoColor=green)](https://www.nuget.org/packages/Known)

![.NET](https://img.shields.io/badge/.NET-8.0-green)
![DEV](https://img.shields.io/badge/DEV-VS2022-brightgreen)
![QQ群](https://img.shields.io/badge/QQ群-865982686-blue)

- 官网：[http://known.org.cn](http://known.org.cn)
- 源码：[https://gitee.com/known/Known](https://gitee.com/known/Known)
- 源码：[https://github.com/known/Known](https://github.com/known/Known)

### 概述

- 基于`C#`和`Blazor`的快速开发框架，开箱即用，跨平台。
- 模块化，Web应用，混合桌面应用，Web和桌面共享一处代码。
- `UI`默认支持`AntDesign`，可扩展其他UI组件库。
- 包含模块、字典、组织、角色、用户、日志、消息、工作流、定时任务等功能。
- 低代码、简洁、易扩展，让开发更简单、更快捷！

### 特性

- 快速开发：基于`Blazor`实现，在线表单设计，实现无代码开发增删改查导功能。
- 通用权限：内置通用权限模块，基于角色鉴权，`URL`检查，只需专注业务功能。
- 最佳实践：低代码开发，一人多角色，沟通高效，进度快，无前后端沟通成本。
- 国际化：提供完备的国际化多语言解决方案，默认支持简体中文、繁体中文、英语。
- 抽象`UI`：抽象`UI`展现层，易扩展，默认支持`Ant Design Blazor`组件库。
- `C#`语言：全量使用C#语言进行全栈开发，从前端到后端只需学习一门语言。

### 快速安装

``` bash
--安装模板
dotnet new install KnownTemplate
--创建项目
dotnet new known --name=MyApp
```

### 项目结构

```
├─Known             -> 框架类库，包含通用前后端，内置ORM、组件、模块。
├─Known.Core        -> 基于AspNetCore的服务端类库。
├─Plugins           -> 插件项目
| ├─Known.AntBlazor -> 基于AntDesign Blazor的界面库。
| ├─Known.Cells     -> 基于Aspose.Cells实现的Excel操作类库。
├─Sample            -> 示例项目
  ├─Sample          -> 包含配置、常量、枚举、实体、模型、服务接口、路由、页面，动态代理访问后端WebApi。
  ├─Sample.Web      -> 包含业务逻辑、数据访问、定时任务，根据服务接口动态生成WebApi。
  ├─Sample.WebApi   -> 框架示例WebApi。
  ├─Sample.WinForm  -> 框架示例WinForm App。
```

### 主要功能

- 模块管理：配置系统功能模块，在线设计模型、页面和表单，自动生成代码。
- 数据字典：维护系统各模块下拉框数据源。
- 组织架构：维护企业组织架构信息，树形结构。
- 角色管理：维护系统角色及权限信息，权限可控制菜单，按钮，列表栏位。
- 用户管理：维护系统登录用户信息。
- 系统日志：查询系统用户登录和访问菜单等日志，可用于统计用户常用功能。
- 消息管理：系统内消息提醒，工作流消息通知。
- 流程管理：系统内置工作流引擎，提供提交、撤回、分配、审核、重启操作。
- 定时任务：导入和计算耗时的功能采用定时任务异步执行。

### 项目连接

- 项目模板：[https://gitee.com/known/known-template](https://gitee.com/known/known-template)
- KnownDB：[https://gitee.com/known/known-db](https://gitee.com/known/known-db)
- JxcLite：[https://gitee.com/known/JxcLite](https://gitee.com/known/JxcLite)

### 界面截图

效果图|效果图
:--:|:--:
![登录页面](https://foruda.gitee.com/images/1704862471614256238/bcd00189_14334.png "屏幕截图")|![系统主页](https://foruda.gitee.com/images/1704862533488666485/5c79f459_14334.png "屏幕截图")
![数据字典](https://foruda.gitee.com/images/1704862600410677167/ed1bb520_14334.png "屏幕截图")|![模块管理](https://foruda.gitee.com/images/1704862643924749072/d877454b_14334.png "屏幕截图")
![模型设置](https://foruda.gitee.com/images/1704862710807573057/3d5d3a2b_14334.png "屏幕截图")|![页面设置](https://foruda.gitee.com/images/1704862788614790653/58c83e0d_14334.png "屏幕截图")
![暗黑模式](https://foruda.gitee.com/images/1704862844381870249/2172fd58_14334.png "屏幕截图")|![系统主页](https://foruda.gitee.com/images/1700054395179186493/6c574df9_14334.png "屏幕截图")
![数据字典](https://foruda.gitee.com/images/1700054455264217536/4c154259_14334.png "屏幕截图")|![模块管理](https://foruda.gitee.com/images/1700054506626636592/98b9add3_14334.png "屏幕截图")
![角色管理](https://foruda.gitee.com/images/1700054617363123970/48133586_14334.png "屏幕截图")|![用户管理](https://foruda.gitee.com/images/1700054722192459256/2308879c_14334.png "屏幕截图")
![模块管理](https://foruda.gitee.com/images/1703494369039793921/74a4b867_14334.png "屏幕截图")|![模型设置](https://foruda.gitee.com/images/1703494151446430428/2e136a4e_14334.png "屏幕截图")
![页面设置](https://foruda.gitee.com/images/1703494262522668999/941de354_14334.png "屏幕截图")|![表单设置](https://foruda.gitee.com/images/1703494306696925357/beeba7dc_14334.png "屏幕截图")

### 捐赠支持

> 如果你觉得这个框架对你有帮助，你可以请作者喝杯咖啡表示鼓励 ☕️

![捐赠支持](https://foruda.gitee.com/images/1726452783813098766/71768ec0_14334.png "屏幕截图")