﻿using AntDesign;

namespace Known.Components;

/// <summary>
/// 扩展Ant多选框列表组件类。
/// </summary>
public class AntCheckboxGroup : CheckboxGroup<string>
{
    [CascadingParameter] private DataItem Item { get; set; }

    /// <summary>
    /// 取得或设置表单容器对象。
    /// </summary>
    [CascadingParameter] protected IComContainer AntForm { get; set; }

    /// <summary>
    /// 取得或设置UI上下文对象级联值实例。
    /// </summary>
    [CascadingParameter] public UIContext Context { get; set; }

    /// <summary>
    /// 取得或设置列表组件关联的数据字典类别名或可数项目（用逗号分割，如：项目1,项目2）。
    /// </summary>
    [Parameter] public string Category { get; set; }

    /// <summary>
    /// 取得或设置列表组件关联的代码表列表集合。
    /// </summary>
    [Parameter] public List<CodeInfo> Codes { get; set; }

    /// <summary>
    /// 取得或设置是否每个元素单独一行显示。
    /// </summary>
    [Parameter] public bool Block { get; set; }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (AntForm != null)
            Disabled = AntForm.IsView;
        if (Item != null)
            Item.Type = typeof(string);
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        if (!string.IsNullOrWhiteSpace(Category))
            Codes = Cache.GetCodes(Category);
        Options = Codes.ToCheckboxOptions(Context?.Language, o => o.Checked = Value != null && Value.Contains(o.Value));
        Class = CssBuilder.Default().AddClass("kui-block", Block).BuildClass();
        await base.OnParametersSetAsync();
    }
}