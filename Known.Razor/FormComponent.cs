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

public class FormComponent : PageComponent
{
    private Form form;

    [Parameter] public bool IsDialog { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public object Model { get; set; }
    [Parameter] public Action<Result> OnSuccess { get; set; }

    protected virtual bool IsTable { get; } = true;
    protected virtual string Style { get; }

    protected FieldContext FieldContext
    {
        get { return form.FieldContext; }
    }

    protected override void BuildPage(RenderTreeBuilder builder)
    {
        BuildPage(builder, Model);
    }

    protected void BuildPage(RenderTreeBuilder builder, object model)
    {
        builder.Component<Form>(attr =>
        {
            attr.Add(nameof(Form.IsTable), IsTable)
                .Add(nameof(Form.ReadOnly), ReadOnly)
                .Add(nameof(Form.Style), Style)
                .Add(nameof(Form.Model), model)
                .Add(nameof(Form.ChildContent), BuildTree(b => BuildFields(b)));
            builder.Reference<Form>(value => form = value);
        });
        builder.Div("form-button", attr => BuildButtons(builder));
    }

    protected virtual void BuildFields(RenderTreeBuilder builder)
    {
    }

    protected virtual void BuildButtons(RenderTreeBuilder builder)
    {
    }

    public bool CheckIsNew<TModel>() where TModel : EntityBase
    {
        if (Model == null)
            return true;

        var model = (TModel)Model;
        if (model == null)
            return true;

        return model.CheckIsNew();
    }

    public virtual void OnOK()
    {
        form.Submit(data =>
        {
            var result = Result.Success("", data);
            OnSuccess?.Invoke(result);
        });
    }

    public virtual void OnCancel()
    {
        UI.CloseDialog();
    }

    public void Submit(Func<dynamic, Result> action, Action<Result> onSuccess)
    {
        form.Submit(data => action(data), onSuccess);
    }

    public void Submit(Func<dynamic, Result> action, bool clear = false)
    {
        Submit(action, result =>
        {
            if (clear)
                form.Clear();
            else
                OnSuccess?.Invoke(result);
        });
    }
}
