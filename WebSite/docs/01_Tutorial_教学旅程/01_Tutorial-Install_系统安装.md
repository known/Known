# 系统安装

本章介绍系统安装功能及其自定义功能。

## 概述

- 框架内置简单的系统安装功能。
- 录入企业编码、名称、系统名称、产品密钥、管理员密码信息完成安装。
- 可自定义高级安装功能，如安装数据库等您产品所需的安装信息。


## 自定义

若需自定义系统安装功能，则完成如下步骤即可。

### 1. 构建安装模块

```csharp
class MyInstall : Form
{
    private InstallInfo? info;
    
    [Parameter] public Action<CheckInfo>? OnInstall { get; set; }
    
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        //分布表单
        //第一步
        //第二步
        //最后一步需要条约OnStart完成安装
    }
    
    private void OnStart()
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

## 效果图

![登录页面](https://foruda.gitee.com/images/1688121567850878119/aeb0ba7c_14334.png "屏幕截图")
![Captcha](https://foruda.gitee.com/images/1688197097755819765/80038310_14334.png "屏幕截图")
