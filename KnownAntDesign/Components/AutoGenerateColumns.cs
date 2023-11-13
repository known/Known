using AntDesign;
using Known.Extensions;
using Known.Helpers;
using Known.Razor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace KnownAntDesign.Components;

public class AutoGenerateColumns<TItem> : BaseComponent where TItem : class, new()
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

            builder.OpenComponent(i++, columnType);
            builder.AddAttribute(i++, "Title", property.DisplayName());
            builder.AddAttribute(i++, "DataIndex", property.Name);
            builder.AddAttribute(i++, "Sortable", true);
            //builder.AddAttribute(i++, "Filterable", true);
            //builder.AddMultipleAttributes(i++, attributes);
            if (template != null)
            {
                builder.AddAttribute(i++, "ChildContent", (RenderFragment)delegate (RenderTreeBuilder builder1)
                {
                    builder1.AddContent(i++, template(Item));
                });
            }
            else if (property.PropertyType == typeof(bool))
            {
                builder.AddAttribute(i++, "ChildContent", (RenderFragment)delegate (RenderTreeBuilder builder1)
                {
                    var value = TypeHelper.GetPropertyValue<bool>(Item, property.Name);
                    builder1.Component<Switch>()
                            .Set(c => c.Checked, value)
                            .Set(c => c.Disabled, true)
                            .Set(c => c.CheckedChildren, "是")
                            .Set(c => c.UnCheckedChildren, "否")
                            .Build();
                });
            }
            else if (item.IsViewLink)
            {
                builder.AddAttribute(i++, "ChildContent", (RenderFragment)delegate (RenderTreeBuilder builder1)
                {
                    var value = TypeHelper.GetPropertyValue<string>(Item, property.Name);
                    builder1.Link(value, Callback(() => Table.ViewForm(Item)));
                });
            }
            builder.CloseComponent();
        }
    }
}