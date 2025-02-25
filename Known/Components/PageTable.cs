namespace Known.Components;

/// <summary>
/// 页面表格组件类。
/// </summary>
/// <typeparam name="TItem">数据类型。</typeparam>
public class PageTable<TItem> : BaseComponent where TItem : class, new()
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

        if (Model.QueryColumns.Count > 0)
        {
            builder.Div("kui-table-page", () =>
            {
                builder.Div("kui-query", () => builder.Query(Model));
                BuildTable(builder);
            });
        }
        else
        {
            BuildTable(builder);
        }
    }

    private void BuildTable(RenderTreeBuilder builder)
    {
        builder.Div("kui-table", () =>
        {
            if (Model.Tab.HasItem)
            {
                Model.Tab.Left = BuildTitle;
                Model.Tab.Right = BuildRight;
                builder.Tabs(Model.Tab);
            }
            else
            {
                builder.Toolbar(() =>
                {
                    builder.Div("left", () =>
                    {
                        BuildTitle(builder);
                        if (Model.TopStatis != null)
                            builder.Component<ToolbarSlot<TItem>>().Set(c => c.Table, Model).Build();
                        if (Model.ShowSetting && Context.IsMobile)
                            builder.Component<TableSetting<TItem>>().Set(c => c.Table, Model).Build();
                    });
                    builder.Div("right", () => BuildRight(builder));
                });
            }
            builder.Table(Model);
        });
    }

    private void BuildTitle(RenderTreeBuilder builder)
    {
        builder.FormTitle(Model.Name);
        if (Context.IsEditTable)
        {
            builder.Div().Class("kui-edit").Style("margin-left:10px;")
                   .Child(() => builder.IconName("plus", "配置", this.Callback<MouseEventArgs>(e => OnConfig())));
        }
    }

    private void OnConfig()
    {
        UIConfig.OnEditTable?.Invoke();
    }

    private void BuildRight(RenderTreeBuilder builder)
    {
        builder.Toolbar(Model.Toolbar);
        if (Model.ShowSetting && !Context.IsMobile)
            builder.Component<TableSetting<TItem>>().Set(c => c.Table, Model).Build();
    }
}