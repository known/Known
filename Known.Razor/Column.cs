namespace Known.Razor;

public class Column<T> : ColumnInfo
{
    private readonly List<string> classNames = new();
    private readonly CodeInfo[] QueryTypes = TypeHelper.GetEnumCodes<QueryType>().ToArray();

    public Column() { }
    internal Column(string name, string id) : base(name, id) { }

    internal Column(ColumnInfo info) : this(info.Name, info.Id)
    {
        Type = info.Type;
        Align = info.Align;
        IsQuery = info.IsQuery;
        IsAdvQuery = info.IsAdvQuery;
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
    public IPicker Pick { get; set; }
    public SelectOption Select { get; set; }
    public Action<T, object> ValueChanged { get; set; }
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
            IsAdvQuery = IsAdvQuery,
            IsSort = IsSort
        };
    }

    internal void BuildQuery(RenderTreeBuilder builder, BaseComponent grid = null)
    {
        BuildQuery(builder, Name, "", null, grid);
    }

    internal void BuildAdvQuery(RenderTreeBuilder builder, QueryInfo info)
    {
        builder.Div("item", attr =>
        {
            builder.Label("form-label", Name);
            builder.Field<Select>("", "QueryType").IsInput(true).Value($"{(int)info.Type}")
                   .Set(f => f.Items, QueryTypes)
                   .Set(f => f.ValueChanged, v => info.Type = (QueryType)int.Parse(v))
                   .Build();
            BuildQuery(builder, "", info.Value, v => info.Value = v);
        });
    }

    internal void BuildCell(RenderTreeBuilder builder, T row, object value, bool readOnly)
    {
        if (IsEdit && !ReadOnly && !readOnly)
            BuildEditCell(builder, row, value);
        else if (Template != null)
            Template.Invoke(builder, row);
        else if (Type == ColumnType.Boolean)
            builder.Field<CheckBox>(Id).Value(value?.ToString()).Enabled(false).Set(f => f.Switch, true).Build();
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
                    .Add(nameof(Field.ValueChanged), delegate (string val) { OnValueChanged(row, val); });
            });
            return;
        }

        if (Type == ColumnType.Boolean)
        {
            builder.Field<CheckBox>(Id).IsInput(true).Value(value?.ToString())
                   .ValueChanged(val => OnValueChanged(row, val)).Build();
        }
        else if (Type == ColumnType.Date)
        {
            builder.Field<Date>(Id).IsInput(true).Value(value?.ToString())
                   .ValueChanged(val => OnValueChanged(row, val))
                   .Set(f => f.DateType, DateType.Date).Build();
        }
        else if (Type == ColumnType.DateTime)
        {
            builder.Field<Date>(Id).IsInput(true).Value(value?.ToString())
                   .ValueChanged(val => OnValueChanged(row, val))
                   .Set(f => f.DateType, DateType.DateTime).Build();
        }
        else
        {
            if (Pick != null)
                builder.Field<Picker>(Id).IsInput(true).Value(value?.ToString())
                       .Set(f => f.Pick, Pick)
                       .Set(f => f.OnPicked, val => ValueChanged?.Invoke(row, val))
                       .Build();
            else if (Select != null)
                Select.BuildCell(builder, Id, value?.ToString(), val => OnValueChanged(row, val));
            else
                builder.Field<Text>(Id).IsInput(true).Value(value?.ToString())
                       .ValueChanged(val => OnValueChanged(row, val)).Build();
        }
    }

    private void BuildQuery(RenderTreeBuilder builder, string name, string value, Action<string> valueChanged = null, BaseComponent grid = null)
    {
        if (Control != null)
        {
            builder.Component(Control, attr =>
            {
                attr.Add(nameof(Field.Id), Id)
                    .Add(nameof(Field.Label), name)
                    .Add(nameof(Field.IsInput), true)
                    .Add(nameof(Field.Value), value)
                    .Add(nameof(Field.ValueChanged), valueChanged);
            });
            return;
        }

        if (Type == ColumnType.Date || Type == ColumnType.DateTime)
        {
            builder.Field<DateRange>(name, Id).IsInput(true).Value(value).ValueChanged(valueChanged).Build();
        }
        else
        {
            if (Select != null)
                Select.BuildQuery(builder, this, name, value, valueChanged, grid != null ? grid.Refresh : null);
            else
                builder.Field<Text>(name, Id).IsInput(true).Value(value).ValueChanged(valueChanged).Build();
        }
    }

    private void OnValueChanged(T row, object value)
    {
        TypeHelper.SetValue(row, Id, value);
        ValueChanged?.Invoke(row, value);
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