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

public class Form : BaseComponent
{
    public Form()
    {
        FieldContext = new FieldContext();
    }

    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool IsTable { get; set; }
    [Parameter] public string Style { get; set; }
    [Parameter] public object Model { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }

    public FieldContext FieldContext { get; }

    public dynamic Data
    {
        get { return FieldContext.Data; }
    }

    public bool Validate()
    {
        return FieldContext.Validate();
    }

    public void Clear()
    {
        FieldContext.Clear();
    }

    public void SetData(object data)
    {
        FieldContext.SetData(data);
    }

    public void Submit(Action<dynamic> action)
    {
        if (!Validate())
            return;

        action.Invoke(Data);
    }

    public void Submit(Func<dynamic, Result> action, Action<Result> onSuccess = null)
    {
        if (!Validate())
            return;

        Result result = action.Invoke(Data);
        UI.Result(result, () => onSuccess?.Invoke(result));
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        FieldContext.IsTableForm = IsTable;
        FieldContext.ReadOnly = ReadOnly;
        FieldContext.Model = Utils.MapTo<Dictionary<string, object>>(Model);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.Div($"form {Style}", attr =>
        {
            builder.Component<CascadingValue<FieldContext>>(attr =>
            {
                attr.Add(nameof(CascadingValue<FieldContext>.IsFixed), false)
                    .Add(nameof(CascadingValue<FieldContext>.Value), FieldContext)
                    .Add(nameof(CascadingValue<FieldContext>.ChildContent), ChildContent);
            });
        });
    }
}
