/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-04-01     KnownChen
 * ------------------------------------------------------------------------------- */

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public class Input : Field
{
    // button         定义可点击的按钮（大多与 JavaScript 使用来启动脚本）
    // checkbox       定义复选框。
    // color          定义拾色器。
    // date           定义日期字段（带有 calendar 控件）
    // datetime       定义日期字段（带有 calendar 和 time 控件）
    // datetime-local 定义日期字段（带有 calendar 和 time 控件）
    // month          定义日期字段的月（带有 calendar 控件）
    // week           定义日期字段的周（带有 calendar 控件）
    // time           定义日期字段的时、分、秒（带有 time 控件）
    // email          定义用于 e-mail 地址的文本字段
    // file           定义输入字段和 "浏览..." 按钮，供文件上传
    // hidden         定义隐藏输入字段
    // image          定义图像作为提交按钮
    // number         定义带有 spinner 控件的数字字段 max,min,step
    // password       定义密码字段。字段中的字符会被遮蔽。
    // radio          定义单选按钮。
    // range          定义带有 slider 控件的数字字段。max,min,step
    // reset          定义重置按钮。重置按钮会将所有表单字段重置为初始值。
    // search         定义用于搜索的文本字段。
    // submit         定义提交按钮。提交按钮向服务器发送数据。
    // tel            定义用于电话号码的文本字段。
    // text           默认。定义单行输入字段，用户可在其中输入文本。默认是 20 个字符。
    // url            定义用于 URL 的文本字段。
    [Parameter] public string Type { get; set; }

    protected override void BuidChildContent(RenderTreeBuilder builder)
    {
        BuildInput(builder, Type);
    }

    internal static void BuildIcon(RenderTreeBuilder builder, string icon)
    {
        if (!string.IsNullOrWhiteSpace(icon))
        {
            builder.Icon(icon);
        }
    }

    protected void BuildInput(RenderTreeBuilder builder, string type, string placeholder = null)
    {
        builder.Input(attr =>
        {
            //var value = BindConverter.FormatValue(Value);
            //var hasChanged = !EqualityComparer<string>.Default.Equals(value, Value);
            attr.Type(type).Id(Id).Name(Id).Disabled(!Enabled)
                .Value(Value).Required(Required)
                .Placeholder(placeholder)
                .Add("autocomplete", "off")
                .OnChange(CreateBinder());
            //builder.SetUpdatesAttributeName("value");
        });
    }

    protected void BuidDate(RenderTreeBuilder builder, string id, string value, Action<DateTime> action, string type = "date")
    {
        builder.Input(attr =>
        {
            attr.Type(type).Id(id).Name(id).Disabled(!Enabled)
                .Value(value).Required(Required)
                .OnChange(CreateBinder(action));
        });
    }
}
