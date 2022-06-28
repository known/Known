/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-04-01     KnownChen
 * ------------------------------------------------------------------------------- */

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Razor;

public class DataGridView<TItem> : DataGrid<TItem>
{
    private bool isSetTemplate = false;

    public DataGridView()
    {
        FieldContext = new FieldContext { IsGridView = true };
    }

    [Parameter] public RenderFragment Tools { get; set; }
    [Parameter] public RenderFragment Fields { get; set; }

    public FieldContext FieldContext { get; }

    public void Show<TForm>(string title, int width, int height, object model) where TForm : FormComponent
    {
        UI.Show<TForm>(title, width, height, model, data => QueryData());
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (isSetTemplate)
        {
            base.BuildRenderTree(builder);
        }
        else
        {
            builder.Component<CascadingValue<FieldContext>>(attr =>
            {
                attr.Add(nameof(CascadingValue<FieldContext>.IsFixed), true)
                    .Add(nameof(CascadingValue<FieldContext>.Value), FieldContext)
                    .Add(nameof(CascadingValue<FieldContext>.ChildContent), Fields);
            });
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (FieldContext != null && FieldContext.Fields.Count > 0 && !isSetTemplate)
        {
            SetQueryTemplate(FieldContext.QueryFields);
            SetHeadTemplate(FieldContext.DataFields);
            SetItemTemplate(FieldContext.DataFields);
            isSetTemplate = true;
            StateHasChanged();
        }
    }

    private void SetQueryTemplate(List<Field> queries)
    {
        if ((queries == null || queries.Count == 0) && Tools == null)
            return;

        QueryTemplate = builder =>
        {
            BuildQuery(builder, queries);
            if (Tools != null)
            {
                builder.Fragment(Tools);
            }
        };
    }

    private void BuildQuery(RenderTreeBuilder builder, List<Field> queries)
    {
        if (queries == null || queries.Count == 0)
            return;

        foreach (var item in queries)
        {
            item.BuildQuery(builder);
        }
        builder.ButtonQuery(Callback(e => QueryData()));
    }

    private void SetHeadTemplate(List<Field> columns)
    {
        HeadTemplate = builder =>
        {
            foreach (var item in columns)
            {
                builder.Th(attr =>
                {
                    if (item.Width.HasValue)
                    {
                        attr.Style($"width:{item.Width}px");
                    }
                    builder.Text(item.Label);
                });
            }
        };
    }

    private void SetItemTemplate(List<Field> columns)
    {
        ItemTemplate = (TItem row) => delegate (RenderTreeBuilder builder)
        {
            var data = Utils.MapTo<Dictionary<string, object>>(row);
            foreach (var item in columns)
            {
                builder.Td(item.GridCellStyle, attr =>
                {
                    if (item.ChildContent != null)
                        builder.Fragment(item.ChildContent(row));
                    else
                        BuildGridCell(builder, item, data);
                });
            }
        };
    }

    private static void BuildGridCell(RenderTreeBuilder builder, Field item, Dictionary<string, object> data)
    {
        if (data != null && data.ContainsKey(item.Id))
        {
            var value = data[item.Id];
            item.BuildGridCell(builder, value);
        }
    }
}
