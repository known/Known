﻿namespace Known.Blazor;

partial class TableModel<TItem>
{
    /// <summary>
    /// 取得或设置表格关联的表单配置信息。
    /// </summary>
    public FormInfo Form { get; set; }

    /// <summary>
    /// 取得或设置表格关联的自定义表单组件类型。
    /// </summary>
    public Type FormType { get; set; }

    /// <summary>
    /// 取得或设置表格关联的表单标题委托。
    /// </summary>
    public Func<TItem, string> FormTitle { get; set; }

    /// <summary>
    /// 取得或设置表格关联的表单内容委托。
    /// </summary>
    public Action<FormModel<TItem>> OnForm { get; set; }

    private bool ShowForm(FormModel<TItem> model)
    {
        model.SmallLabel = Form?.SmallLabel == true;
        model.Info ??= Form;
        model.Info ??= new FormInfo();

        if (model.Info.Width == null)
            model.Info.Width = Form?.Width;

        OnForm?.Invoke(model);
        return UI.ShowForm(model);
    }
}