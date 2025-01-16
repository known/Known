﻿namespace Known.Pages;

/// <summary>
/// 表格页面组件类。
/// </summary>
/// <typeparam name="TItem"></typeparam>
public class TablePage<TItem> : BaseComponent where TItem : class, new()
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
                container?.Reload();
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
               .Set(c => c.ChildContent, b =>
               {
                   if (Model.IsForm)
                       BuildFormList(b);
                   else
                       BuildPageList(b);
               })
               .Build(v => container = v);
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
            BuildDataTable(builder);
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
            BuildDataTable(builder);
        });
    }

    private void BuildRight(RenderTreeBuilder builder)
    {
        if (Model.Toolbar.HasItem)
            builder.Toolbar(Model.Toolbar);
        if (Model.ShowSetting)
            builder.Component<TableSetting<TItem>>().Set(c => c.Table, Model).Build();
    }

    private void BuildDataTable(RenderTreeBuilder builder)
    {
        builder.Component<KTable<TItem>>().Set(c => c.Model, Model).Build();
    }
}