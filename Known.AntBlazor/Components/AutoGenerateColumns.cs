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

        var isDictionary = typeof(TItem) == typeof(Dictionary<string, object>);
        foreach (var item in Table.Columns)
        {
            if (!item.IsVisible)
                continue;

            var propertyType = typeof(string);
            var property = item.GetProperty();
            if (property != null)
                propertyType = property.PropertyType.UnderlyingSystemType;
            var columnType = typeof(Column<>).MakeGenericType(propertyType);

            RenderFragment<TItem> template = null;
            Table.Templates?.TryGetValue(item.Id, out template);

            builder.OpenComponent(0, columnType);
            builder.AddAttribute(1, "Title", item.Name);
            if (!isDictionary)
                builder.AddAttribute(1, "DataIndex", item.Id);
            builder.AddAttribute(1, "Sortable", item.IsSort);
            if (!string.IsNullOrWhiteSpace(item.DefaultSort))
            {
                var sortName = item.DefaultSort == "desc" ? "descend" : "ascend";
                builder.AddAttribute(1, "DefaultSortOrder", SortDirection.Parse(sortName));
            }
            //builder.AddAttribute(1, "Filterable", true);
            if (template != null)
            {
                builder.AddAttribute(1, "ChildContent", this.BuildTree(b => b.AddContent(1, template(Item))));
            }
            else if (propertyType == typeof(bool))
            {
                builder.AddAttribute(1, "ChildContent", this.BuildTree(b =>
                {
                    var value = TypeHelper.GetPropertyValue<bool>(Item, item.Id);
                    b.Component<Switch>().Set(c => c.Checked, value)
                                         .Set(c => c.Disabled, true)
                                         .Set(c => c.CheckedChildren, "是")
                                         .Set(c => c.UnCheckedChildren, "否")
                                         .Build();
                }));
            }
            else if (item.IsViewLink)
            {
                builder.AddAttribute(1, "ChildContent", this.BuildTree(b =>
                {
                    var value = TypeHelper.GetPropertyValue<string>(Item, item.Id);
                    b.Link(value, this.Callback(() => Table.ViewForm(Item)));
                }));
            }
            builder.CloseComponent();
        }
    }
}