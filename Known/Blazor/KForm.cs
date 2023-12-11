using Known.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Blazor;

class KForm<TItem> : EditForm where TItem : class, new()
{
    [Parameter] public FormModel<TItem> Form { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Form.Initialize();
        Model = Form.Data;
        ChildContent = this.BuildTree((Action<RenderTreeBuilder, EditContext>)((b, c) =>
        {
            if (Form.Rows.Count == 0)
                return;

            b.Div("kui-form", () =>
            {
                foreach (var row in Form.Rows)
                {
                    BuildFormRow(b, row);
                }
            });
        }));
    }

    private static void BuildFormRow(RenderTreeBuilder builder, FormRow<TItem> row)
    {
        var colSpan = 24 / row.Fields.Count;
        builder.Div("kui-row", () =>
        {
            foreach (var field in row.Fields)
            {
                builder.Div($"kui-col kui-col-{colSpan}", () => BuildFormField(builder, field));
            }
        });
    }

    private static void BuildFormField(RenderTreeBuilder builder, FieldModel<TItem> field)
    {
        builder.Div("kui-field", () =>
        {
            var property = field.Column.GetProperty();
            if (property != null && property.PropertyType == typeof(bool))
            {
                builder.Label(() =>
                {
                    builder.Fragment(field.InputTemplate);
                    builder.Text(field.Column.Name);
                });
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(field.Column.Name))
                    builder.Label(field.Column.Name);
                builder.Fragment(field.InputTemplate);
            }
        });
    }
}