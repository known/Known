
namespace Known.Pages;

/// <summary>
/// 表格页面组件类。
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class TablePage<TItem> : BaseComponent where TItem : class, new()
{
    /// <summary>
    /// 取得或设置表格页面组件模型。
    /// </summary>
    [Parameter] public TableModel<TItem> Model { get; set; }

    /// <summary>
    /// 异步初始化组件。
    /// </summary>
    /// <returns></returns>
    protected override async Task OnInitAsync()
    {
        await base.OnInitAsync();
        Model?.SetQueryColumns();
    }

    /// <summary>
    /// 呈现表格页面组件内容。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Model == null)
            return;

        if (Model.IsForm)
            BuildFormList(builder);
        else
            BuildPageList(builder);
    }

    private void BuildFormList(RenderTreeBuilder builder)
    {
        builder.Div("kui-table form-list", () =>
        {
            if (!string.IsNullOrWhiteSpace(Model.Name) ||
                 Model.QueryColumns.Count > 0 ||
                 Model.ShowToolbar && Model.Toolbar.HasItem)
            {
                builder.Toolbar(() =>
                {
                    builder.Div(() =>
                    {
                        builder.FormTitle(Model.Name);
                        if (Model.QueryColumns.Count > 0)
                            builder.Query(Model);
                    });
                    if (Model.ShowToolbar && Model.Toolbar.HasItem)
                        builder.Toolbar(Model.Toolbar);
                });
            }
            builder.Component<KTable<TItem>>().Set(c => c.Model, Model).Build();
        });
    }

    private void BuildPageList(RenderTreeBuilder builder)
    {
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
                Model.Tab.Left = b => b.FormTitle(Model.Name);
                if (Model.Toolbar.HasItem)
                    Model.Tab.Right = BuildRight;
                builder.Tabs(Model.Tab);
            }
            else
            {
                builder.Toolbar(() =>
                {
                    builder.Div(() =>
                    {
                        builder.FormTitle(Model.Name);
                        if (Model.TopStatis != null)
                            builder.Component<ToolbarSlot<TItem>>().Set(c => c.Table, Model).Build();
                    });
                    builder.Div(() => BuildRight(builder));
                });
            }
            builder.Component<KTable<TItem>>().Set(c => c.Model, Model).Build();
        });
    }

    private void BuildRight(RenderTreeBuilder builder)
    {
        if (Model.Toolbar.HasItem)
            builder.Toolbar(Model.Toolbar);
        if (Model.ShowSetting)
            builder.Component<TableSetting<TItem>>().Set(c => c.Table, Model).Build();
    }
}