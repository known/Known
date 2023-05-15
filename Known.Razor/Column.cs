﻿namespace Known.Razor;

public class Column<T> : ColumnInfo
{
    private readonly List<string> classNames = new();

    public Column() { }
    internal Column(string name, string id) : base(name, id) { }

    internal Column(ColumnInfo info) : this(info.Name, info.Id)
    {
        Type = info.Type;
        Align = info.Align;
        IsQuery = info.IsQuery;
        IsSum = info.IsSum;
        IsSort = info.IsSort;
        IsVisible = info.IsVisible;
        IsFixed = info.IsFixed;
        Sort = info.Sort;
        if (info.Width > 0)
            Width = info.Width;
        SetColumnStyle();
    }

    internal Column(PropertyInfo pi, ColumnAttribute attr)
    {
        Id = pi.Name;
        Name = attr?.Description;
        SetColumnType(pi.PropertyType);
        SetColumnStyle();
    }

    public string ClassName => string.Join(" ", classNames.Distinct());
    public bool ReadOnly { get; set; }
    public bool IsEdit { get; set; }
    public Type Control { get; set; }
    public SelectOption Select { get; set; }
    public EditOption<T> Edit { get; set; }
    public Action<RenderTreeBuilder, T> Template { get; set; }

    internal void Class(string className)
    {
        classNames.Clear();
        classNames.Add(className);
    }

    internal void AddClass(string className) => classNames.Add(className);

    internal void SetColumnStyle()
    {
        if (Type == ColumnType.Boolean)
            classNames.Add("check-box");
        else if (Type == ColumnType.Date || Type == ColumnType.DateTime)
            classNames.Add("txt-center inline2");
        else if (Align == AlignType.Center)
            classNames.Add("txt-center");
        else if (Type == ColumnType.Number || Align == AlignType.Right)
            classNames.Add("txt-right");
    }

    internal ColumnInfo ToColumn()
    {
        return new ColumnInfo
        {
            Id = Id,
            Name = Name,
            Align = Align,
            Width = Width,
            IsVisible = IsVisible,
            IsQuery = IsQuery,
            IsSort = IsSort
        };
    }

    internal void BuildQuery(RenderTreeBuilder builder, BaseComponent grid, bool more)
    {
        var style = more ? "more" : "";
        if (Control != null)
        {
            builder.Component(Control, attr =>
            {
                attr.Add(nameof(Field.Id), Id)
                    .Add(nameof(Field.Label), Name)
                    .Add(nameof(Field.Style), style);
            });
            return;
        }

        if (Type == ColumnType.Date || Type == ColumnType.DateTime)
        {
            builder.Field<DateRange>(Name, Id).Set(f => f.Style, style).Build();
        }
        else
        {
            if (Select != null)
                Select.BuildQuery(builder, this, style, grid.Refresh);
            else
                builder.Field<Text>(Name, Id).Style(style).Build();
        }
    }

    internal void BuildCell(RenderTreeBuilder builder, T row, object value, bool readOnly)
    {
        if (IsEdit && !ReadOnly && !readOnly)
            BuildEditCell(builder, row, value);
        else if (Template != null)
            Template.Invoke(builder, row);
        else if (Type == ColumnType.Boolean)
            builder.Field<CheckBox>(Id).Value(value?.ToString()).Enabled(false).Build();
        else
            builder.Text(Format(value));
    }

    private void BuildEditCell(RenderTreeBuilder builder, T row, object value)
    {
        if (Control != null)
        {
            builder.Component(Control, attr =>
            {
                attr.Add(nameof(Field.Id), Id)
                    .Add(nameof(Field.Enabled), true)
                    .Add(nameof(Field.IsInput), true)
                    .Add(nameof(Field.Value), value?.ToString())
                    .Add(nameof(Field.ValueChanged), delegate (string value) { OnValueChanged(row, value); });
            });
            return;
        }

        if (Type == ColumnType.Boolean)
        {
            builder.Field<CheckBox>(Id).IsInput(true).Value(value?.ToString())
                   .ValueChanged(value => OnValueChanged(row, value)).Build();
        }
        else if (Type == ColumnType.Date || Type == ColumnType.DateTime)
        {
            builder.Field<DateRange>(Id).IsInput(true).Value(value?.ToString())
                   .ValueChanged(value => OnValueChanged(row, value))
                   .Set(f => f.Split, "")
                   .Build();
        }
        else
        {
            if (Select != null)
            {
                Select.BuildCell(builder, Id, value?.ToString());
            }
            else
            {
                builder.Field<Text>(Id).IsInput(true).Value(value?.ToString())
                       .ValueChanged(value => OnValueChanged(row, value)).Build();
            }
        }
    }

    private void OnValueChanged(T row, string value)
    {
        TypeHelper.SetValue(row, Id, value);
        Edit?.ValueChanged?.Invoke(row, value);
    }

    private string Format(object value)
    {
        if (Type == ColumnType.Date)
        {
            var date = Utils.ConvertTo<DateTime?>(value);
            return date?.ToString(Config.DateFormat);
        }
        else if (Type == ColumnType.DateTime)
        {
            var date = Utils.ConvertTo<DateTime?>(value);
            return date?.ToString(Config.DateTimeFormat);
        }
        else if (Select != null)
        {
            return Select.Format(value);
        }
        else
        {
            return $"{value}";
        }
    }
}

public class SelectOption
{
    public SelectOption() { }

    public SelectOption(Type enumType)
    {
        ShowEmpty = false;
        Items = TypeHelper.GetEnumCodes(enumType).ToArray();
    }

    public bool ShowEmpty { get; set; } = true;
    public string Codes { get; set; }
    public CodeInfo[] Items { get; set; }
    public Action<string> ValueChanged { get; set; }

    internal string Format(object value)
    {
        var codes = Items;
        if (codes == null || codes.Length == 0)
            codes = CodeInfo.GetCodes(Codes).ToArray();

        var str = value?.ToString();
        return codes.FirstOrDefault(i => i.Code == str)?.Name;
    }

    internal void BuildQuery(RenderTreeBuilder builder, ColumnInfo column, string style, Action refresh)
    {
        var emptyText = ShowEmpty ? "全部" : "";
        builder.Field<Select>(column.Name, column.Id).Style(style)
               .ValueChanged(value =>
               {
                   ValueChanged?.Invoke(value);
                   refresh();
               })
               .Set(c => c.EmptyText, emptyText)
               .Set(f => f.Codes, Codes)
               .Set(f => f.Items, Items)
               .Build();
    }

    internal void BuildCell(RenderTreeBuilder builder, string id, string value)
    {
        var emptyText = ShowEmpty ? "请选择" : "";
        builder.Field<Select>(id).Value(value)
               .ValueChanged(value => ValueChanged?.Invoke(value))
               .Set(c => c.EmptyText, emptyText)
               .Set(f => f.Codes, Codes)
               .Set(f => f.Items, Items)
               .Build();
    }
}

public class EditOption<T>
{
    public Action<T, string> ValueChanged { get; set; }
}