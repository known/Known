using BootstrapBlazor.Components;
using Known.Blazor;
using Known.Extensions;
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
            var propertyType = property.PropertyType.UnderlyingSystemType;
            var columnType = typeof(TableColumn<,>).MakeGenericType(typeof(TItem), propertyType);

            RenderFragment<TItem> template = null;
            Table.Templates?.TryGetValue(property.Name, out template);
            
            builder.OpenComponent(0, columnType);
            builder.AddAttribute(1, "Text", property.DisplayName());
            builder.AddAttribute(1, "FieldName", property.Name);
            builder.AddAttribute(1, "Sortable", true);
            if (!string.IsNullOrWhiteSpace(item.DefaultSort))
            {
                var sortName = item.DefaultSort == "desc" ? "Desc" : "Asc";
                builder.AddAttribute(1, "DefaultSort", true);
                builder.AddAttribute(1, "DefaultSortOrder", Utils.ConvertTo<SortOrder>(sortName));
            }
            //builder.AddAttribute(1, "Filterable", true);
            if (template != null)
            {
                if (propertyType == typeof(bool))
                    AddTemplate<bool>(builder, template);
                else if (propertyType == typeof(short))
                    AddTemplate<short>(builder, template);
                else if (propertyType == typeof(int))
                    AddTemplate<int>(builder, template);
                else if (propertyType == typeof(long))
                    AddTemplate<long>(builder, template);
                else if (propertyType == typeof(float))
                    AddTemplate<float>(builder, template);
                else if (propertyType == typeof(double))
                    AddTemplate<double>(builder, template);
                else if (propertyType == typeof(decimal))
                    AddTemplate<decimal>(builder, template);
                else if (propertyType == typeof(DateTime))
                    AddTemplate<DateTime>(builder, template);
                else if (propertyType == typeof(DateTime?))
                    AddTemplate<DateTime?>(builder, template);
                else if (propertyType == typeof(DateTimeOffset))
                    AddTemplate<DateTimeOffset>(builder, template);
                else if (propertyType == typeof(DateTimeOffset?))
                    AddTemplate<DateTimeOffset?>(builder, template);
                else if (propertyType == typeof(string[]))
                    AddTemplate<string[]>(builder, template);
                else if (propertyType == typeof(string))
                    AddTemplate<string>(builder, template);
            }
            else if (item.IsViewLink)
            {
                builder.AddAttribute(1, "Template", this.BuildTree<TableColumnContext<TItem, string>>((b, c) =>
                {
                    b.Link(c.Value, this.Callback(() => Table.ViewForm(Item)));
                }));
            }
            builder.CloseComponent();
        }
    }

    private void AddTemplate<TValue>(RenderTreeBuilder builder, RenderFragment<TItem> template)
    {
        builder.AddAttribute(1, "Template", this.BuildTree<TableColumnContext<TItem, TValue>>((b, c) =>
        {
            b.AddContent(1, template(c.Row));
        }));
    }
}