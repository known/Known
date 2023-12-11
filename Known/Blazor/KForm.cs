using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Known.Blazor;

class KForm<TItem> : EditForm where TItem : class, new()
{
    [Parameter] public FormModel<TItem> Form { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Form.Initialize();
        ChildContent = this.BuildTree<EditContext>((b, c) =>
        {
            if (Form.Rows.Count > 0)
            {
                foreach (var row in Form.Rows)
                {
                    //        var colSpan = 24 / row.Fields.Count;
                    //        var lblSpan = Model.LabelSpan ?? GetLabelSpan(colSpan);
                    //< GridRow >
                    //    @foreach(var field in row.Fields)
                    //            {
                    //        < GridCol Span = "colSpan" >
                    //            < FormItem LabelColSpan = "lblSpan" Label = "@field.Column.Name" >
                    //                @field.InputTemplate
                    //            </ FormItem >
                    //        </ GridCol >
                    //    }
                    //</ GridRow >
                }
            }
        });
    }
}