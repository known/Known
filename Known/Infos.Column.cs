﻿namespace Known;

public partial class ColumnInfo
{
    internal ColumnInfo(string id, RenderFragment template)
    {
        Id = id;
        Template = template;
    }

    /// <summary>
    /// 取得或设置表单字段呈现模板。
    /// </summary>
    public RenderFragment Template { get; set; }

    internal ColumnInfo(PageColumnInfo info) => SetPageColumnInfo(info);
    private void SetPageColumnInfo(PageColumnInfo info)
    {
        if (info == null)
            return;

        Id = info.Id;
        Name = info.Name ?? info.Id;
        IsViewLink = info.IsViewLink;
        IsQuery = info.IsQuery;
        IsQueryAll = info.IsQueryAll;
        Type = info.Type;
        Category = info.Category;
        IsSum = info.IsSum;
        IsSort = info.IsSort;
        DefaultSort = info.DefaultSort;
        Fixed = info.Fixed;
        Width = info.Width;
        Align = info.Align;
        Position = info.Position;

        if (info.Id == nameof(EntityBase.CreateTime) || info.Id == nameof(EntityBase.ModifyTime))
            Type = FieldType.Date;
    }

    internal ColumnInfo(FormFieldInfo info) => SetFormFieldInfo(info);
    internal void SetFormFieldInfo(FormFieldInfo info)
    {
        if (info == null)
            return;

        Id = info.Id;
        Name = info.Name ?? info.Id;
        Row = info.Row;
        Column = info.Column;
        Span = info.Span;
        Type = info.Type;
        CustomField = info.CustomField;
        MultiFile = info.MultiFile;
        ReadOnly = info.ReadOnly;
        Required = info.Required;
        Placeholder = info.Placeholder;
        Rows = info.Rows;
        Category = info.Category;
    }
}