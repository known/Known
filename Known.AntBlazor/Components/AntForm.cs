using AntDesign;
using Known.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.AntBlazor.Components;

public class AntForm<TItem> : Form<TItem> where TItem : class, new()
{
    [Parameter] public bool ShowAction { get; set; }
    [Parameter] public FormModel<TItem> Form { get; set; }

    protected override void OnInitialized()
    {
        ValidateOnChange = true;
        ValidateMode = FormValidateMode.Rules;
        LabelColSpan = Form?.LabelSpan ?? 0;
        WrapperColSpan = Form?.WrapperSpan ?? 0;
        Model = Form?.Data;
        Form?.Initialize();
        base.OnInitialized();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
            Form.OnValidate = Validate;
        base.OnAfterRender(firstRender);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
    }
}