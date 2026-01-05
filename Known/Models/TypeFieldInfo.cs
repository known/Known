namespace Known.Models;

/// <summary>
/// 类型字段信息类。
/// </summary>
/// <param name="property">类型属性。</param>
public class TypeFieldInfo(PropertyInfo property)
{
    private readonly Func<object, object> _getter = CompileGetter(property);
    private readonly Action<object, object> _setter = property.CanWrite ? CompileSetter(property) : null;

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

    /// <summary>
    /// 获取属性值。
    /// </summary>
    /// <param name="instance">对象实例。</param>
    /// <returns>属性值。</returns>
    public object GetValue(object instance) => _getter?.Invoke(instance);

    /// <summary>
    /// 设置属性值。
    /// </summary>
    /// <param name="instance">对象实例。</param>
    /// <param name="value">属性值。</param>
    public void SetValue(object instance, object value) => _setter?.Invoke(instance, value);

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
            info.IsVisible = column.IsVisible;
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
        var field = new FieldInfo
        {
            Id = Name,
            Name = DisplayName,
            Type = GetFieldType(),
            Length = Length?.ToString(),
            Required = Required,
            IsKey = IsKey
        };
        if (field.IsKey || Property.PropertyType == typeof(bool))
            field.Required = true;
        return field;
    }

    internal PageColumnInfo GetPageColumn()
    {
        var column = GetAttribute<ColumnAttribute>();
        if (column == null) return null;

        var info = new PageColumnInfo
        {
            Id = Name,
            Name = DisplayName,
            Length = Length?.ToString(),
            Required = Required,
            Category = Category,
            Type = column.Type
        };
        if (info.Type == FieldType.Text)
            info.Type = GetFieldType();

        info.Width = column.Width > 0 ? column.Width : GetColumnWidth(info.Type);
        info.Ellipsis = column.Ellipsis;
        info.IsVisible = column.IsVisible;
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

    private static Func<object, object> CompileGetter(PropertyInfo property)
    {
        var instance = Expression.Parameter(typeof(object), "instance");
        var instanceCast = Expression.Convert(instance, property.DeclaringType!);
        var propertyAccess = Expression.Property(instanceCast, property);
        var castPropertyValue = Expression.Convert(propertyAccess, typeof(object));
        return Expression.Lambda<Func<object, object>>(castPropertyValue, instance).Compile();
    }

    private static Action<object, object> CompileSetter(PropertyInfo property)
    {
        var setMethod = property.GetSetMethod(true);
        if (setMethod == null) return null;

        var instance = Expression.Parameter(typeof(object), "instance");
        var value = Expression.Parameter(typeof(object), "value");
        var instanceCast = Expression.Convert(instance, property.DeclaringType!);

        var convertedValue = Expression.Call(
            typeof(TypeCache).GetMethod(nameof(TypeCache.ConvertTo))!,
            Expression.Constant(property.PropertyType),
            value
        );
        var valueCast = Expression.Convert(convertedValue, property.PropertyType);
        var setterCall = Expression.Call(instanceCast, setMethod, valueCast);
        return Expression.Lambda<Action<object, object>>(setterCall, instance, value).Compile();

        //Expression convertedValue;
        //if (property.PropertyType.IsValueType)
        //    convertedValue = Expression.Convert(value, property.PropertyType);
        //else
        //    convertedValue = Expression.TypeAs(value, property.PropertyType);
        //var setterCall = Expression.Call(instanceCast, setMethod, convertedValue);
        //return Expression.Lambda<Action<object, object>>(setterCall, instance, value).Compile();
    }
}