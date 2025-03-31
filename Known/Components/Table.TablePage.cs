namespace Known.Components;

/// <summary>
/// 泛型页面表格组件类。
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
        container?.ReloadPage();
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
            DropdownModel model = null;
            if (actions == null || actions.Count == 0)
                model = new DropdownModel { Icon = "menu", TriggerType = "Click", Overlay = BuildOverlay };
            builder.Component<PluginPanel>()
                   .Set(c => c.Class, "table")
                   .Set(c => c.Name, "低代码表格")
                   .Set(c => c.Dropdown, model)
                   .Set(c => c.Actions, actions)
                   .Set(c => c.ChildContent, BuildContent)
                   .Build();
        }
        else
        {
            BuildContent(builder);
        }
    }

    private void BuildOverlay(RenderTreeBuilder builder)
    {
        var data = Menu.TablePage.Page ?? new PageInfo();
        var form = new FormModel<PageInfo>(this)
        {
            SmallLabel = true,
            Data = data,
            OnFieldChanged = async v =>
            {
                Menu.TablePage.Page = data;
                await SaveSettingAsync(Menu.TablePage);
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
               .Set(c => c.ChildContent, BuildTablePage)
               .Build(value => container = value);
    }

    private void BuildTablePage(RenderTreeBuilder builder) => builder.PageTable(Model);
}