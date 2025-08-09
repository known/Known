namespace Known.Blazor;

partial class TableModel<TItem>
{
    /// <summary>
    /// 取得或设置表格关联的表单配置信息。
    /// </summary>
    public FormInfo Form { get; set; }

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
        if (FormTitle != null)
            model.Title = FormTitle.Invoke(model.Data);
        SetFormModel(model);
        OnForm?.Invoke(model);
        return UI.ShowForm(model);
    }

    private bool ShowForm<T>(FormModel<T> model) where T : class, new()
    {
        SetFormModel(model);
        return UI.ShowForm(model);
    }

    private void SetFormModel<T>(FormModel<T> model) where T : class, new()
    {
        model.Info ??= new FormInfo();
        if (Form != null)
        {
            model.WrapClass = Form.WrapClass;
            model.SmallLabel = Form.SmallLabel;
            model.Info.NoFooter = Form.NoFooter;
            model.Info.ShowFooter = Form.ShowFooter;
            model.Info.Maximizable = Form.Maximizable;
            model.Info.DefaultMaximized = Form.DefaultMaximized;
            model.Info.Width = Form.Width;
            if (Form.OpenType != FormOpenType.None)
                model.Info.OpenType = Form.OpenType;
        }
    }
}