# 安装激活

本章介绍系统安装与激活及其自定义功能。

## 概述

- 框架内置简单的系统安装功能。
- 录入企业编码、名称、系统名称、产品密钥、管理员密码信息完成安装。
- 可自定义高级安装功能，如安装数据库等您产品所需的安装信息。
- 框架默认无需注册产品密钥，若产品需要安装产品密钥进行激活，可进行自定义。
- 若产品密钥到期，所有模块页面自动显示授权激活组件。

## 自定义安装

若需自定义系统安装功能，则完成如下步骤即可。

### 1. 构建安装模块

```csharp
//自定义安装页面类
class MyInstall : Form
{
    private InstallInfo? info;
    private readonly List<MenuItem> items = new()
    {
        new MenuItem{Icon="fa fa-home",Name="步骤一"},
        new MenuItem{Icon="fa fa-home",Name="步骤二"},
        new MenuItem{Icon="fa fa-home",Name="步骤三"}
    };
    //安装成功后回调
    [Parameter] public Action<CheckInfo>? OnInstall { get; set; }
    
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //使用内置Steps组件构造分步表单
        builder.Component<Steps>()
               .Set(c => c.Items, items)
               .Set(c => c.OnChanged, OnChanged)
               .Set(c => c.OnFinished, OnFinished)
               .Set(c => c.Body, BuildStep)
               .Build();
    }
    //建造步骤内容
    private void BuildStep(RenderTreeBuilder builder, MenuItem item)
    {
        builder.Span(item.Name);
    }
    //每一步保存
    private void OnChanged(MenuItem item)
    {
        //在此异步请求各步骤的后端逻辑
        UI.Tips($"保存{item.Name}");
    }
    //最后一步完成
    private void OnFinished()
    {
        //调用内置保存安装信息方法
        SubmitAsync(data => Platform.System.SaveInstallAsync(info), result =>
        {
            if (result.IsValid)
                OnInstall?.Invoke(result.DataAs<CheckInfo>());
        });
    }
}
```

### 2. 重写Index的BuildInstall

```csharp
public class Index : Known.Razor.Pages.Index
{
    protected override void BuildInstall(RenderTreeBuilder builder)
    {
        //Build自定义Install
        builder.Component<MyInstall>().Set(c => c.OnInstall, OnInstall).Build();
    }
}
```

## 自定义产品ID

- 产品ID默认识别主机的MAC地址。
- 若需自定义ID格式及识别方式，在后端初始化方法中注册自定义方法即可。

```csharp
public class AppCore
{
    public static void Initialize()
    {
        //注册自定义产品ID，需在KCConfig.AddWebPlatform()之后添加
        KCConfig.ProductId = MyCheck.GetProductId;
        ...
    }
}
```

## 自定义产品密钥

- 框架默认不验证产品ID和密钥。
- 若产品需要安装密钥才能使用，在后端初始化方法中注册验证方法即可。

```csharp
public class AppCore
{
    public static void Initialize()
    {
        //注册安装页面自动刷新产品密钥
        PlatformHelper.UpdateKey = MyCheck.UpdateKey;
        //注册产品密钥验证
        PlatformHelper.CheckSystem = MyCheck.CheckSystem;
        //注册用户数限制
        PlatformHelper.CheckUser = MyCheck.CheckUser;
        ...
    }
}
```

## 效果图

![安装页面](https://foruda.gitee.com/images/1688431150542136719/c40dc9b4_14334.png "屏幕截图")
