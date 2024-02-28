using AntDesign;
using Known.Blazor;
using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

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
        if (ShowAction && !Form.IsView)
        {
            builder.FormAction(() =>
            {
                builder.Button(new ActionInfo { Name = "确定", Style = ButtonType.Primary, OnClick = this.Callback<MouseEventArgs>(OnSaveAsync) });
                builder.Button(new ActionInfo { Name = "取消", OnClick = this.Callback<MouseEventArgs>(OnCloseAsync) });
            });
        }
    }

    private async void OnSaveAsync(MouseEventArgs args) => await Form.SaveAsync();
    private async void OnCloseAsync(MouseEventArgs args) => await Form.CloseAsync();
}