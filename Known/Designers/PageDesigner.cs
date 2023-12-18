﻿using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class PageDesigner : BaseDesigner
{
    private BaseView view;
    private BaseProperty property;

    [Parameter] public PageInfo Model { get; set; }
    [Parameter] public Action<PageInfo> OnChanged { get; set; }

    protected override void BuildDesigner(RenderTreeBuilder builder)
    {
        builder.Div("panel-view", () =>
        {
            builder.Component<PageView>().Build(value => view = value);
        });
        builder.Div("panel-property", () =>
        {
            builder.Component<PageProperty>().Build(value => property = value);
        });
    }

    protected override void FieldChanged(FieldInfo field)
    {
        view?.StateChanged();
        property?.StateChanged();
    }
}