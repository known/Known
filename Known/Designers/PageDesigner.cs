using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class PageDesigner : BaseDesigner<PageInfo>
{
    private PageProperty property;
    private PageColumnInfo current;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Fields = Model.Columns?.Select(c => new FieldInfo { Id = c.Id, Name = c.Name }).ToList();
        current = Model.Columns.FirstOrDefault();
    }

    protected override void BuildDesigner(RenderTreeBuilder builder)
    {
        builder.Div("panel-view", () =>
        {
            builder.Component<PageView>().Set(c => c.Model, Model).Build(value => view = value);
        });
        builder.Div("panel-property", () =>
        {
            builder.Component<PageProperty>()
                   .Set(c => c.ReadOnly, ReadOnly)
                   .Set(c => c.Model, current)
                   .Set(c => c.OnChanged, OnPropertyChanged)
                   .Build(value => property = value);
        });
    }

    protected override void OnFieldCheck()
    {
        var columns = new List<PageColumnInfo>();
        foreach (var item in Fields)
        {
            var column = new PageColumnInfo { Id = item.Id, Name = item.Name };
            var info = Model.Columns.FirstOrDefault(c => c.Id == item.Id);
            SetPageColumn(column, info);
            columns.Add(column);
        }
        Model.Columns = columns;
        ChangeView();
    }

    protected override void OnFieldClick(FieldInfo field)
    {
        current = Model.Columns.FirstOrDefault(c => c.Id == field.Id);
        property?.SetModel(current);
    }

    private void OnPropertyChanged(PageColumnInfo info)
    {
        SetPageColumn(current, info);
        ChangeView();
    }

    private static void SetPageColumn(PageColumnInfo column, PageColumnInfo info)
    {
        if (info != null)
        {
            column.DefaultSort = info.DefaultSort;
            column.IsViewLink = info.IsViewLink;
            column.IsQuery = info.IsQuery;
            column.IsQueryAll = info.IsQueryAll;
        }
    }

    private void ChangeView()
    {
        view?.SetModel(Model);
        OnChanged?.Invoke(Model);
    }
}