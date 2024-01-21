using Known.Extensions;
using Microsoft.AspNetCore.Components.Rendering;

namespace Known.Designers;

class PageDesigner : BaseViewDesigner<PageInfo>
{
    private PageProperty property;
    private PageColumnInfo current;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Fields = Model.Columns?.Select(c => new FieldInfo { Id = c.Id, Name = c.Name }).ToList();
        current = Model.Columns.FirstOrDefault();
    }

    protected override void BuildView(RenderTreeBuilder builder)
    {
        builder.Component<PageView>()
               .Set(c => c.ReadOnly, ReadOnly)
               .Set(c => c.Model, Model)
               .Set(c => c.Entity, Entity)
               .Set(c => c.OnChanged, OnChanged)
               .Build(value => view = value);
    }

    protected override void BuildProperty(RenderTreeBuilder builder)
    {
        builder.Component<PageProperty>()
               .Set(c => c.ReadOnly, ReadOnly)
               .Set(c => c.Model, current)
               .Set(c => c.OnChanged, OnPropertyChanged)
               .Build(value => property = value);
    }

    protected override void OnFieldCheck(FieldInfo field)
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
        OnFieldClick(field);
        ChangeView();
    }

    protected override void OnFieldClick(FieldInfo field)
    {
        current = Model.Columns.FirstOrDefault(c => c.Id == field.Id);
        property?.SetModel(current);
    }

    private void OnPropertyChanged(PageColumnInfo info)
    {
        current = Model.Columns.FirstOrDefault(c => c.Id == info.Id);
        SetPageColumn(current, info);
        ChangeView();
    }

    private static void SetPageColumn(PageColumnInfo column, PageColumnInfo info)
    {
        if (column == null || info == null)
            return;

        column.IsViewLink = info.IsViewLink;
        column.IsQuery = info.IsQuery;
        column.IsQueryAll = info.IsQueryAll;
        column.IsSort = info.IsSort;
        column.DefaultSort = info.DefaultSort;
        column.Fixed = info.Fixed;
        column.Width = info.Width;
        column.Align = info.Align;
    }

    private void ChangeView()
    {
        view?.SetModel(Model);
        OnChanged?.Invoke(Model);
    }
}