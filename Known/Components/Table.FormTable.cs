namespace Known.Components;

/// <summary>
/// 表单表格组件类。
/// </summary>
/// <typeparam name="TItem">数据类型。</typeparam>
public class FormTable<TItem> : BaseComponent where TItem : class, new()
{
    /// <summary>
    /// 取得或设置表格页面组件模型。
    /// </summary>
    [Parameter] public TableModel<TItem> Model { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Model == null)
            return;

        builder.BuildTable(Model.FixedWidth, true, () =>
        {
            if (ShowToolbar())
            {
                builder.Toolbar(() =>
                {
                    builder.Div(() =>
                    {
                        if (Model.ShowName)
                            builder.FormTitle(Model.Name);
                        if (Model.QueryColumns.Count > 0)
                            builder.Query(Model);
                    });
                    if (Model.ShowToolbar)
                        builder.Toolbar(Model.Toolbar);
                });
            }
            builder.Table(Model);
        });
    }

    private bool ShowToolbar()
    {
        return !string.IsNullOrWhiteSpace(Model.Name) ||
               Model.QueryColumns.Count > 0 ||
               Model.Toolbar.HasItem;
    }
}