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

public class DataGrid<TItem> : DataComponent<TItem>
{
    private bool checkAll = false;
    private readonly List<TItem> selectedItems = new();

    [Parameter] public bool ShowIndex { get; set; } = true;
    [Parameter] public bool ShowCheckBox { get; set; }

    public List<TItem> SelectedItems
    {
        get { return selectedItems; }
    }

    protected override string ContainerStyle => "grid-view";
    protected override string ContentStyle => "grid";

    public void CheckSelectedItems(Action<List<TItem>> action)
    {
        var selected = SelectedItems;
        if (!selected.Any())
        {
            UI.Tips(Language.SelectOneAtLeast);
            return;
        }

        action.Invoke(selected);
    }

    protected override void BuildContent(RenderTreeBuilder builder)
    {
        if (Data == null || Data.Count == 0)
        {
            BuildTable(builder);
            BuildEmpty(builder);
        }
        else
        {
            BuildTable(builder, () =>
            {
                var index = 1;
                foreach (TItem item in Data)
                {
                    builder.Tr(attr =>
                    {
                        BuildRowIndex(builder, index++);
                        BuildRowCheckBox(builder, item);
                        builder.Fragment(ItemTemplate(item));
                    });
                }
            });
        }
    }

    private void BuildTable(RenderTreeBuilder builder, Action action = null)
    {
        builder.Table("table", attr =>
        {
            BuildHead(builder);
            action?.Invoke();
        });
    }

    private void BuildHead(RenderTreeBuilder builder)
    {
        builder.Tr(attr =>
        {
            if (ShowIndex)
                builder.Th(attr => attr.Class("index"));

            BuildHeadCheckBox(builder);
            builder.Fragment(HeadTemplate);
        });
    }

    private void BuildHeadCheckBox(RenderTreeBuilder builder)
    {
        if (!ShowCheckBox)
            return;

        builder.Th("check", attr =>
        {
            builder.Check(attr => attr.Checked(checkAll).OnClick(Callback(e =>
            {
                checkAll = !checkAll;
                selectedItems.Clear();
                if (checkAll)
                {
                    selectedItems.AddRange(Data);
                }
            })));
        });
    }

    private void BuildRowCheckBox(RenderTreeBuilder builder, TItem item)
    {
        if (!ShowCheckBox)
            return;

        builder.Td("check", attr =>
        {
            builder.Check(attr =>
            {
                attr.Checked(selectedItems.Contains(item)).OnClick(Callback(e =>
                {
                    if (selectedItems.Contains(item))
                        selectedItems.Remove(item);
                    else
                        selectedItems.Add(item);
                    checkAll = selectedItems.Count == TotalCount ||
                               selectedItems.Count == PageSize;
                }));
            });
        });
    }

    private void BuildRowIndex(RenderTreeBuilder builder, int index)
    {
        if (!ShowIndex)
            return;

        builder.Td("txt-center", attr =>
        {
            builder.Text($"{index}");
        });
    }
}
