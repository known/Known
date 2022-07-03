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

public abstract class PageGridView<TModel> : PageComponent
{
    protected DataGridView<TModel> grid;

    protected virtual bool AutoLoad { get; } = true;
    protected virtual bool ShowPager { get; } = true;
    protected virtual bool ShowTools { get; } = true;
    protected virtual bool ShowCheckBox { get; }

    [Parameter] public List<TModel> Data { get; set; }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        builder.Component<DataGridView<TModel>>(attr =>
        {
            if (Data != null)
                attr.Add(nameof(DataGridView<TModel>.Data), Data);
            else
                attr.Add(nameof(DataGridView<TModel>.OnQuery), new Func<PagingCriteria, PagingResult<TModel>>(OnQueryData));
            
            attr.Add(nameof(DataGridView<TModel>.AutoLoad), AutoLoad)
                .Add(nameof(DataGridView<TModel>.ShowPager), ShowPager)
                .Add(nameof(DataGridView<TModel>.ShowCheckBox), ShowCheckBox)
                .Add(nameof(DataGridView<TModel>.Fields), BuildTree(b => BuildFields(b)));
            
            if (ShowTools)
                attr.Add(nameof(DataGridView<TModel>.Tools), BuildTree(b => BuildTools(b)));
            
            builder.Reference<DataGridView<TModel>>(value => grid = value);
        });
    }

    protected virtual PagingResult<TModel> OnQueryData(PagingCriteria criteria)
    {
        return null;
    }

    protected virtual void BuildTools(RenderTreeBuilder builder)
    {
    }

    protected virtual void BuildFields(RenderTreeBuilder builder)
    {
    }
}
