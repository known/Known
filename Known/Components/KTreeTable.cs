namespace Known.Components;

/// <summary>
/// 左侧树和右侧表格关联组件类。
/// </summary>
/// <typeparam name="TItem">表格数据类型。</typeparam>
public class KTreeTable<TItem> : BaseComponent where TItem : class, new()
{
    /// <summary>
    /// 取得或设置树配置模型。
    /// </summary>
    [Parameter] public TreeModel Tree { get; set; }

    /// <summary>
    /// 取得或设置表格配置模型。
    /// </summary>
    [Parameter] public TableModel<TItem> Table { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        builder.Div("kui-row-28", () =>
        {
            builder.Div("kui-card kui-p10", () => builder.Tree(Tree));
            builder.TablePage(Table);
        });
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
            await Tree.RefreshAsync();
    }
}