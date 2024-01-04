using System.Linq.Expressions;
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

            if (isDictionary)
                BuildPropertyColumn(builder, item);
            else
                BuildColumn(builder, item);
        }
    }

    private void BuildPropertyColumn(RenderTreeBuilder builder, ColumnInfo item)
    {
        var data = Item as Dictionary<string, object>;
        data.TryGetValue(item.Id, out var value);
        builder.OpenComponent<PropertyColumn<Dictionary<string, object>, object>>(0);
        AddPropertyColumn(builder, (Dictionary<string, object> c) => c[item.Id]);
        AddAttributes(builder, item, value);
        builder.CloseComponent();
    }

    private void BuildColumn(RenderTreeBuilder builder, ColumnInfo item)
    {
        var propertyType = typeof(string);
        if (item.Property != null)
            propertyType = item.Property.PropertyType.UnderlyingSystemType;
        var columnType = typeof(Column<>).MakeGenericType(propertyType);
        var value = TypeHelper.GetPropertyValue(Item, item.Id);
        builder.OpenComponent(0, columnType);
        builder.AddAttribute(1, nameof(Column<TItem>.DataIndex), item.Id);
        AddAttributes(builder, item, value);
        builder.CloseComponent();
    }

    private static void AddPropertyColumn(RenderTreeBuilder builder, Expression<Func<Dictionary<string, object>, object>> property)
    {
        builder.AddAttribute(1, nameof(PropertyColumn<TItem, object>.Property), property);
    }

    private void AddAttributes(RenderTreeBuilder builder, ColumnInfo item, object value)
    {
        builder.AddAttribute(1, nameof(Column<TItem>.Title), Language[item.Id]);
        builder.AddAttribute(1, nameof(Column<TItem>.Sortable), item.IsSort);
        builder.AddAttribute(1, nameof(Column<TItem>.Fixed), item.Fixed);
        builder.AddAttribute(1, nameof(Column<TItem>.Width), item.Width);
        if (!string.IsNullOrWhiteSpace(item.DefaultSort))
        {
            var sortName = item.DefaultSort == "Descend" ? "descend" : "ascend";
            builder.AddAttribute(1, nameof(Column<TItem>.DefaultSortOrder), SortDirection.Parse(sortName));
        }
        //builder.AddAttribute(1, nameof(Column<TItem>.Filterable), true);

        RenderFragment<TItem> template = null;
        Table.Templates?.TryGetValue(item.Id, out template);

        if (template != null)
        {
            builder.AddAttribute(1, nameof(Column<TItem>.ChildContent), this.BuildTree(b => b.AddContent(1, template(Item))));
        }
        else if (value?.GetType() == typeof(bool))
        {
            builder.AddAttribute(1, nameof(Column<TItem>.ChildContent), this.BuildTree(b =>
            {
                var isChecked = Utils.ConvertTo<bool>(value);
                b.Component<Switch>().Set(c => c.Checked, isChecked)
                                     .Set(c => c.Disabled, true)
                                     .Build();
            }));
        }
        else if (item.IsViewLink)
        {
            builder.AddAttribute(1, nameof(Column<TItem>.ChildContent), this.BuildTree(b =>
            {
                b.Link($"{value}", this.Callback(() => Table.ViewForm(Item)));
            }));
        }
    }
}