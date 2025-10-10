namespace Known;

/// <summary>
/// 类型模型信息类。
/// </summary>
public class TypeModelInfo
{
    /// <summary>
    /// 构造函数。
    /// </summary>
    /// <param name="properties">类型属性集合。</param>
    public TypeModelInfo(PropertyInfo[] properties)
    {
        Properties = properties;
        Fields = [.. properties.Select(p => new TypeFieldInfo(p))];
        Dictionary = Fields.ToFrozenDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 取得类型属性集合。
    /// </summary>
    public PropertyInfo[] Properties { get; }

    /// <summary>
    /// 取得类型字段信息列表。
    /// </summary>
    public List<TypeFieldInfo> Fields { get; }

    /// <summary>
    /// 取得类型字段字典。
    /// </summary>
    public FrozenDictionary<string, TypeFieldInfo> Dictionary { get; }

    /// <summary>
    /// 根据属性名获取属性信息。
    /// </summary>
    /// <param name="name">属性名。</param>
    /// <returns></returns>
    public PropertyInfo GetProperty(string name) => Dictionary.GetValueOrDefault(name)?.Property;

    internal List<ColumnInfo> GetColumns(bool isAttr)
    {
        var columns = new List<ColumnInfo>();
        foreach (var item in Fields)
        {
            var column = item.GetColumn(isAttr);
            if (column != null)
                columns.Add(column);
        }
        return columns;
    }

    internal List<ColumnInfo> GetFormns()
    {
        var forms = new List<ColumnInfo>();
        foreach (var item in Fields)
        {
            var form = item.GetForm();
            if (form != null)
                forms.Add(form);
        }
        return forms;
    }
}

/// <summary>
/// 类型字段信息类。
/// </summary>
/// <param name="property">类型属性。</param>
public class TypeFieldInfo(PropertyInfo property)
{
    /// <summary>
    /// 取得字段属性名称。
    /// </summary>
    public string Name { get; } = property.Name;

    /// <summary>
    /// 取得字段属性信息。
    /// </summary>
    public PropertyInfo Property { get; } = property;

    /// <summary>
    /// 取得字段属性自定义特性集合。
    /// </summary>
    public object[] Attributes { get; } = property.GetCustomAttributes(false);

    /// <summary>
    /// 获取指定类型的特性列表。
    /// </summary>
    /// <typeparam name="T">特性类型。</typeparam>
    /// <returns></returns>
    public List<T> GetAttributes<T>() => Attributes?.OfType<T>().ToList();

    /// <summary>
    /// 获取指定类型的特性。
    /// </summary>
    /// <typeparam name="T">特性类型。</typeparam>
    /// <returns></returns>
    public T GetAttribute<T>() => Attributes == null ? default : Attributes.OfType<T>().FirstOrDefault();

    internal string DisplayName => GetAttribute<DisplayNameAttribute>()?.DisplayName;
    internal int? Length => GetAttribute<MaxLengthAttribute>()?.Length;
    internal bool Required => GetAttribute<RequiredAttribute>() is not null;
    internal bool IsKey => GetAttribute<KeyAttribute>() is not null;
    internal string Category => GetAttribute<CategoryAttribute>()?.Category;

    internal ColumnInfo GetColumn(bool isAttr = false)
    {
        var column = GetAttribute<ColumnAttribute>();
        if (isAttr && column == null) return null;

        var info = new ColumnInfo
        {
            Property = Property,
            Id = Name,
            Name = DisplayName,
            Required = Required,
            Category = Category,
        };
        if (column != null)
        {
            info.IsViewLink = column.IsViewLink;
            info.IsQuery = column.IsQuery;
            info.IsQueryAll = column.IsQueryAll;
            info.QueryValue = column.QueryValue;
            info.Ellipsis = column.Ellipsis;
            info.IsSum = column.IsSum;
            info.IsSort = column.IsSort;
            info.DefaultSort = column.DefaultSort;
            info.Fixed = column.Fixed;
            info.Width = column.Width;
            info.Align = column.Align;
            if (column.Type != FieldType.Text)
                info.Type = column.Type;
        }
        if (info.Type == FieldType.Text)
            info.Type = GetFieldType();
        return info;
    }

    internal ColumnInfo GetForm()
    {
        var form = GetAttribute<FormAttribute>();
        if (form == null) return null;

        var info = new ColumnInfo
        {
            IsForm = true,
            Property = Property,
            Id = Name,
            Name = DisplayName,
            Required = Required,
            Category = Category,
            Row = form.Row,
            Column = form.Column,
            FieldValue = form.FieldValue,
            ReadOnly = form.ReadOnly,
            Placeholder = form.Placeholder
        };
        if (string.IsNullOrWhiteSpace(info.Placeholder) && info.Type == FieldType.Select)
            info.Placeholder = "请选择";
        if (!string.IsNullOrWhiteSpace(form.Type))
            info.Type = Utils.ConvertTo<FieldType>(form.Type);
        if (info.Type == FieldType.Text)
            info.Type = GetFieldType();
        if (info.Type == FieldType.Custom)
            info.CustomField = form.CustomField;
        return info;
    }

    internal FieldInfo GetField()
    {
        var field = new FieldInfo { Id = Name, Name = DisplayName, Type = GetFieldType(), Length = Length?.ToString(), Required = Required, IsKey = IsKey };
        if (Property.PropertyType == typeof(bool))
            field.Required = true;
        return field;
    }

    internal PageColumnInfo GetPageColumn()
    {
        var column = GetAttribute<ColumnAttribute>();
        if (column == null) return null;

        var info = new PageColumnInfo { Id = Name, Name = DisplayName, Length = Length?.ToString(), Required = Required, Category = Category, Type = column.Type };
        if (info.Type == FieldType.Text)
            info.Type = GetFieldType();

        info.Width = column.Width > 0 ? column.Width : GetColumnWidth(info.Type);
        info.Ellipsis = column.Ellipsis;
        info.IsSum = column.IsSum;
        info.IsSort = column.IsSort;
        info.DefaultSort = column.DefaultSort;
        info.IsViewLink = column.IsViewLink;
        info.IsQuery = column.IsQuery;
        info.IsQueryAll = column.IsQueryAll;
        info.QueryValue = column.QueryValue;
        info.Fixed = column.Fixed;
        info.Align = column.Align;
        return info;
    }

    internal FormFieldInfo GetFormField()
    {
        var form = GetAttribute<FormAttribute>();
        if (form == null) return null;

        var info = new FormFieldInfo
        {
            Id = Name,
            Name = DisplayName,
            Category = Category,
            Length = Length?.ToString(),
            Required = Required,
            Row = form.Row,
            Column = form.Column,
            Type = FieldType.Text,
            CustomField = form.CustomField,
            ReadOnly = form.ReadOnly,
            Placeholder = form.Placeholder,
            FieldValue = form.FieldValue,
            Rows = form.Rows,
            Unit = form.Unit
        };
        if (!string.IsNullOrWhiteSpace(form.Type))
            info.Type = Utils.ConvertTo<FieldType>(form.Type);
        if (info.Type == FieldType.Text)
            info.Type = GetFieldType();
        return info;
    }

    private FieldType GetFieldType()
    {
        var form = GetAttribute<FormAttribute>();
        if (form != null && !string.IsNullOrWhiteSpace(form.Type))
            return Utils.ConvertTo<FieldType>(form.Type);

        return Property.PropertyType.GetFieldType();
    }

    private int? GetColumnWidth(FieldType type)
    {
        var width = type.GetColumnWidth();
        if (width > 0)
            return width;

        if (Length == null) return null;
        if (Length < 100) return Length * 2;
        return Length;
    }
}