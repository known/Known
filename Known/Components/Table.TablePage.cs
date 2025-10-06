namespace Known.Components;

/// <summary>
/// 泛型表格页面组件类。
/// </summary>
/// <typeparam name="TItem">数据类型。</typeparam>
public class TablePage<TItem> : BaseTablePage where TItem : class, new()
{
    private ReloadContainer container = null;

    /// <summary>
    /// 取得或设置表格页面组件模型。
    /// </summary>
    [Parameter] public TableModel<TItem> Model { get; set; }

    /// <inheritdoc />
    public override async Task<Result> SaveSettingAsync(AutoPageInfo info)
    {
        var result = await base.SaveSettingAsync(info);
        Model.Initialize(info);
        container?.Reload();
        return result;
    }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Model == null)
            return;

        if (UIConfig.EnableEdit && Model.EnableEdit)
        {
            var actions = Plugin?.GetTableActions(this);
            builder.Component<PluginPanel>()
                   .Set(c => c.Class, "table")
                   .Set(c => c.Name, Language.LowCodeTable)
                   .Set(c => c.Actions, actions)
                   .Set(c => c.ChildContent, BuildContent)
                   .Build();
        }
        else
        {
            BuildContent(builder);
        }
    }

    private void BuildContent(RenderTreeBuilder builder)
    {
        builder.Component<ReloadContainer>()
               .Set(c => c.ChildContent, BuildTablePage)
               .Build(value => container = value);
    }

    private void BuildTablePage(RenderTreeBuilder builder) => builder.PageTable(Model);
}