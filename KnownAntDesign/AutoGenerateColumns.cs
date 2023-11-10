using AntDesign;
using Known.Razor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace KnownAntDesign;

public class AutoGenerateColumns<TItem> : ComponentBase
{
    [Parameter] public TableModel<TItem> Table { get; set; }
    [Parameter] public TItem Item { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var i = 0;
        foreach (var item in Table.Columns)
        {
            if (!item.IsGrid)
                continue;

            var property = item.Property;
            var columnType = typeof(Column<>).MakeGenericType(property.PropertyType.GetUnderlyingType());
            //var instance = Activator.CreateInstance(columnType);

            RenderFragment<TItem> template = null;
            Table.Templates?.TryGetValue(property.Name, out template);

            //var attributes = columnType.GetProperties()
            //    .Where(x => x.GetCustomAttribute<ParameterAttribute>() != null)
            //    .Where(x => !x.Name.IsIn("DataIndex"))
            //    .ToDictionary(x => x.Name, x => x.GetValue(instance))
            //    .Where(x => x.Value != null);

            builder.OpenComponent(++i, columnType);
            builder.AddAttribute(++i, "Title", item.Description);
            builder.AddAttribute(++i, "DataIndex", property.Name);
            builder.AddAttribute(++i, "Sortable", true);
            //builder.AddAttribute(++i, "Filterable", true);
            //builder.AddMultipleAttributes(++i, attributes);
            if (template != null)
            {
                builder.AddAttribute(++i, "ChildContent", (RenderFragment)delegate (RenderTreeBuilder builder1)
                {
                    builder1.AddContent(++i, template(Item));
                });
            }
            //else if (property.PropertyType == typeof(bool))
            //{
            //    builder.AddAttribute(++i, "ChildContent", (RenderFragment)delegate (RenderTreeBuilder builder1)
            //    {
            //        builder1.AddContent(++i, template(Item));
            //    });
            //}
            builder.CloseComponent();
        }
    }
}