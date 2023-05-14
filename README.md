# Known

Known是一个基于C#和Blazor开发的前后端分离快速开发框架，开箱即用，可跨平台运行。

[![star](https://gitee.com/known/Known/badge/star.svg?theme=dark)](https://gitee.com/known/Known/stargazers)
[![fork](https://gitee.com/known/Known/badge/fork.svg?theme=dark)](https://gitee.com/known/Known/members)
[![GitHub license](https://img.shields.io/badge/license-Apache2-yellow)](https://gitee.com/known/Known/blob/master/LICENSE)

### 概述
* 基于.NET 7和Blazor实现的快速开发框架，前后端分离，开箱即用。
* 跨平台，生成混合桌面应用，Web和桌面共享一份代码。
* 多租户、缓存、数据校验、鉴权、远程请求、任务调度等。
* 包含模块、用户、角色、字典、日志、定时任务等功能。
* 代码简洁、易扩展，让开发更简单、更快捷！

> 如果对您有帮助，点击右上角⭐Star⭐关注 ，感谢支持开源！


### 效果图

![Web效果图](Document/images/Web.png)
![桌面效果图](Document/images/WinForm.png)

### 框架
```
├─Known         ->框架前后端共用库，前后端数据交互模型。
├─Known.Core    ->框架后端库，通用业务逻辑。
├─Known.Razor   ->框架前端库，Blazor组件。
├─Known.Studio  ->框架代码生成工具。
```

### 示例框架
```
├─Test          ->项目前后端共用库。
├─Test.Core     ->项目后端库。
├─Test.Razor    ->项目前端库。
├─Test.Client   ->Web前端BlazorWebAssembly。
├─Test.Server   ->Web后端。
├─Test.WinForm  ->WinForm桌面。
├─TestAlone     ->桌面程序。
```

### 详细功能


