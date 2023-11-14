# Known

[Known](https://gitee.com/known/Known)是基于C#和Blazor的快速开发框架，开箱即用，跨平台，一处代码，多处运行。

[![star](https://gitee.com/known/Known/badge/star.svg?theme=dark)](https://gitee.com/known/Known/stargazers)
[![fork](https://gitee.com/known/Known/badge/fork.svg?theme=dark)](https://gitee.com/known/Known/members)
[![stars](https://img.shields.io/github/stars/known/known?color=%231890FF)](https://github.com/known/Known)
[![License](https://img.shields.io/badge/license-Apache2-yellow)](https://gitee.com/known/Known/blob/master/LICENSE)
![.NET](https://img.shields.io/badge/.NET-8.0-green)
![DEV](https://img.shields.io/badge/DEV-VS2022-brightgreen)

![QQ群](https://img.shields.io/badge/QQ群-865982686-blue)

- Gitee： [https://gitee.com/known/Known](https://gitee.com/known/Known)
- Github：[https://github.com/known/Known](https://github.com/known/Known)

### 概述

- 基于C#和Blazor的快速开发框架，模块化，跨平台，开箱即用。
- 单页应用，混合桌面应用，Web和桌面共享一处代码。
- UI默认支持AntDesign，可扩展其他UI组件库。
- 包含模块、字典、组织、角色、用户、日志、消息、工作流、定时任务等功能。
- 代码简洁、易扩展，让开发更简单、更快捷！

> 如果对您有帮助，点击[⭐Star⭐](https://gitee.com/known/Known)关注 ，感谢支持开源！

### 项目结构

```
├─Known          -> 框架类库，包含通用后端、内置组件、内置模块。
├─Known.Cells    -> 基于Aspose.Cells实现的Excel操作类库。
├─Known.Demo     -> 框架示例模块。
├─Known.Studio   -> 框架代码生成工具。
├─Known.Web      -> 框架Blazor Web App。
├─KnownAntDesign -> 基于AntDesign Blazor的界面。
├─WebSite        -> 框架网站及在线文档。
```

### 主要功能

- 模块管理：配置系统功能模块、操作按钮、列表栏位，供开发者使用。
- 数据字典：维护系统各模块下拉框数据源。
- 组织架构：维护企业组织架构信息，树形结构。
- 角色管理：维护系统角色及权限信息，权限可控制菜单，按钮，列表栏位。
- 用户管理：维护系统登录用户信息。
- 系统日志：查询系统用户登录和访问菜单等日志，可用于统计用户常用功能。
- 消息管理：系统内消息提醒，工作流消息通知。
- 流程管理：系统内置工作流引擎，提供提交、撤回、分配、审核、重启操作。
- 定时任务：导入和计算耗时的功能采用定时任务异步执行。

### 主要组件

- 输入类：Form、Button、Input、Hidden、Password、Select、Text、TextArea、CheckBox、Switch、Captcha、Date、DateRange、Number、CheckList、RadioList、Picker、Upload、SearchBox
- 导航类：Menu、Breadcrumb、Pager、Steps、Tabs、Tree
- 展示类：Badge、Tag、Card、Carousel、Empty、Dropdown、GroupBox、ImageBox、Dialog、Chart、QuickView、Icon、Timeline
- 反馈类：Toast、Notify、Banner、Progress
- 数据类：DataList、DataGrid、EditGrid

### 在线体验

- 官网地址：[http://known.pumantech.com](http://known.pumantech.com)
- 演示地址：[http://demo.pumantech.com](http://demo.pumantech.com)
- 登录信息：Admin/888888

### 快速安装

```bash
--安装模板
dotnet new install KnownTemplate
--创建项目
--host参数：full(默认)/hosted/server/winform
dotnet new known --name=你的项目 --host=server
```

### 生态系统

- [项目模板](https://gitee.com/known/known-template)
- [开发示例](https://gitee.com/known/known-sample)

### 系统美图

![登录页面](https://foruda.gitee.com/images/1688121567850878119/aeb0ba7c_14334.png "屏幕截图")
![桌面主页](https://foruda.gitee.com/images/1688092817417883098/53a1da51_14334.png "屏幕截图")
![数据字典](https://foruda.gitee.com/images/1688121245593898303/e45b1e89_14334.png "屏幕截图")
![模块管理](https://foruda.gitee.com/images/1688121372620870803/ca564f91_14334.png "屏幕截图")
![角色管理](https://foruda.gitee.com/images/1688121430233035965/c6e8df7f_14334.png "屏幕截图")
![用户管理](https://foruda.gitee.com/images/1688121486294777387/218d0eb9_14334.png "屏幕截图")
![列表页面](https://foruda.gitee.com/images/1688093103502236712/7ad4f573_14334.png "屏幕截图")
![表单页面](https://foruda.gitee.com/images/1688093130502934536/ee69a56f_14334.png "屏幕截图")
