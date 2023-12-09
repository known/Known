using BootstrapBlazor.Components;
using Known.Blazor;
using Known.Extensions;
using Known.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.BootBlazor.Components;

public class AutoGenerateColumns<TItem> : BaseComponent where TItem : class, new()
{
    [Parameter] public TableModel<TItem> Table { get; set; }
    [Parameter] public TItem Item { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Table == null || Table.Columns == null)
            return;

        foreach (var item in Table.Columns)
        {
            if (!item.IsGrid || !item.IsVisible)
                continue;

            var property = item.GetProperty();
            var columnType = typeof(TableColumn<,>).MakeGenericType(typeof(TItem), property.PropertyType.UnderlyingSystemType);

            RenderFragment<TItem> template = null;
            Table.Templates?.TryGetValue(property.Name, out template);

            builder.OpenComponent(0, columnType);
            builder.AddAttribute(1, "Text", property.DisplayName());
            builder.AddAttribute(1, "FieldName", property.Name);
            builder.AddAttribute(1, "Sortable", true);
            //if (!string.IsNullOrWhiteSpace(item.DefaultSort))
            //{
            //    var sortName = item.DefaultSort == "desc" ? "descend" : "ascend";
            //    builder.AddAttribute(1, "DefaultSortOrder", SortDirection.Parse(sortName));
            //}
            //builder.AddAttribute(1, "Filterable", true);
            if (template != null)
            {
                //builder.AddAttribute(1, "Template", (RenderFragment)delegate (RenderTreeBuilder builder1)
                //{
                //    builder1.AddContent(1, template(Item));
                //});
            }
            else if (item.IsViewLink)
            {
                //builder.AddAttribute(1, "Template", (RenderFragment)delegate (RenderTreeBuilder builder1)
                //{
                //    var value = TypeHelper.GetPropertyValue<string>(Item, property.Name);
                //    builder1.Link(value, Callback(() => Table.ViewForm(Item)));
                //});
            }
            builder.CloseComponent();
        }
    }
}