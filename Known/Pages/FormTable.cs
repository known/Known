namespace Known.Pages;

/// <summary>
/// 表单表格组件类。
/// </summary>
/// <typeparam name="TItem">数据类型。</typeparam>
public class FormTable<TItem> : BaseComponent where TItem : class, new()
{
    private ReloadContainer container = null;

    /// <summary>
    /// 取得或设置表格页面组件模型。
    /// </summary>
    [Parameter] public TableModel<TItem> Model { get; set; }

    /// <inheritdoc />
    protected override void BuildRender(RenderTreeBuilder builder)
    {
        if (Model == null)
            return;

        if (UIConfig.EnableEdit && Model.EnableEdit)
        {
            var model = new DropdownModel
            {
                Icon = "menu",
                TriggerType = "Click",
                Overlay = BuildTableOverlay
            };
            builder.Component<PluginPanel>()
                   .Set(c => c.Class, "table")
                   .Set(c => c.Dropdown, model)
                   .Set(c => c.ChildContent, BuildContent)
                   .Build();
        }
        else
        {
            BuildContent(builder);
        }
    }

    private void BuildTableOverlay(RenderTreeBuilder builder)
    {
        var menu = Context.Current;
        var data = menu.TablePage.Page ?? new PageInfo();
        var form = new FormModel<PageInfo>(this)
        {
            SmallLabel = true,
            Data = data,
            OnFieldChanged = async v =>
            {
                menu.TablePage.Page = data;
                menu.Plugins.AddPlugin(menu.TablePage);
                await Platform.SaveMenuAsync(menu);
                Model.Initialize(menu.TablePage);
                container?.ReloadPage();
            }
        };
        form.AddRow().AddColumn(c => c.ShowPager);
        form.AddRow().AddColumn(c => c.ShowSetting);
        form.AddRow().AddColumn(c => c.PageSize);
        form.AddRow().AddColumn(c => c.ToolSize);
        builder.Overlay(() =>
        {
            builder.FormTitle("表格设置");
            builder.Form(form);
        });
    }

    private void BuildContent(RenderTreeBuilder builder)
    {
        builder.Component<ReloadContainer>()
               .Set(c => c.ChildContent, BuildTable)
               .Build(v => container = v);
    }

    private void BuildTable(RenderTreeBuilder builder)
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
}