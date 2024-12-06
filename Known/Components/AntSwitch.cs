﻿using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant开关组件类。
/// </summary>
public class AntSwitch : Switch
{
    [CascadingParameter] private IAntForm AntForm { get; set; }
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 初始化组件。
    /// </summary>
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(bool);
        base.OnInitialized();
    }
}