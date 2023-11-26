using AntDesign;
using Known.Blazor;
using Known.Extensions;
using Known.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.AntBlazor.Components;

public class AutoGenerateColumns<TItem> : BaseComponent where TItem : class, new()
{
    [Parameter] public TableModel<TItem> Table { get; set; }
    [Parameter] public TItem Item { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Table == null || Table.Columns == null)
            return;

        var i = 0;
        foreach (var item in Table.Columns)
        {
            if (!item.IsGrid)
                continue;

            var property = item.GetProperty();
            var columnType = typeof(Column<>).MakeGenericType(property.PropertyType.UnderlyingSystemType);

            RenderFragment<TItem> template = null;
            Table.Templates?.TryGetValue(property.Name, out template);

            builder.OpenComponent(i++, columnType);
            builder.AddAttribute(i++, "Title", property.DisplayName());
            builder.AddAttribute(i++, "DataIndex", property.Name);
            builder.AddAttribute(i++, "Sortable", true);
            if (!string.IsNullOrWhiteSpace(item.DefaultSort))
            {
                var sortName = item.DefaultSort == "desc" ? "descend" : "ascend";
                builder.AddAttribute(i++, "DefaultSortOrder", SortDirection.Parse(sortName));
            }
            //builder.AddAttribute(i++, "Filterable", true);
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
                    builder1.Link(value, Callback(() => Table.Page.ViewForm(Item)));
                });
            }
            builder.CloseComponent();
        }
    }
}