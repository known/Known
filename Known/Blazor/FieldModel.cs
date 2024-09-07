namespace Known.Blazor;

/// <summary>
/// 字段组件模型类。
/// </summary>
/// <typeparam name="TItem">表单数据类型。</typeparam>
public class FieldModel<TItem> : BaseModel where TItem : class, new()
{
    internal FieldModel(FormModel<TItem> form, ColumnInfo column) : base(form.Context)
    {
        if (!string.IsNullOrWhiteSpace(column.Id))
            form.Fields[column.Id] = this;
        Form = form;
        Column = column;
    }

    /// <summary>
    /// 取得字段模型对应的表单模型。
    /// </summary>
    public FormModel<TItem> Form { get; }

    /// <summary>
    /// 取得字段关联的栏位配置信息。
    /// </summary>
    public ColumnInfo Column { get; }

    /// <summary>
    /// 取得字段是否为只读（表单只读或字段只读）。
    /// </summary>
    public bool IsReadOnly => Form.IsView || Column.ReadOnly;

    internal PropertyInfo Property => Column.Property;

    /// <summary>
    /// 取得或设置字段值。
    /// </summary>
    public object Value
    {
        get
        {
            if (Form.Data == null)
                return null;

            if (Form.IsDictionary)
                return GetDictionaryValue(Column, Form.Data as Dictionary<string, object>);

            return Property?.GetValue(Form.Data);
        }
        set
        {
            if (!Equals(Value, value) && Form.Data != null)
            {
                if (Form.IsDictionary)
                    (Form.Data as Dictionary<string, object>)[Column.Id] = value;
                else if (Property?.SetMethod is not null)
                    Property?.SetValue(Form.Data, value);
                Form.OnFieldChanged?.Invoke(Column.Id);
            }
        }
    }

    /// <summary>
    /// 获取字段对应属性的类型。
    /// </summary>
    /// <returns></returns>
    public Type GetPropertyType()
    {
        if (Form.IsDictionary)
            return GetPropertyType(Column, Value);

        return Property?.PropertyType;
    }

    /// <summary>
    /// 获取下拉框代码表列表。
    /// </summary>
    /// <param name="emptyText">空值占位符提示，默认请选择。</param>
    /// <returns>代码表列表。</returns>
    public List<CodeInfo> GetCodes(string emptyText = "Please select")
    {
        if (!string.IsNullOrWhiteSpace(emptyText))
            emptyText = Language?["PleaseSelect"];

        var codes = Form.GetCodes(Column);
        if (codes != null)
            return codes.ToCodes(emptyText);

        if (Column.Codes != null)
            return Column.Codes.ToCodes(emptyText);

        codes = Cache.GetCodes(Column.Category);
        foreach (var item in codes)
        {
            var name = Language?.GetString(item);
            if (!string.IsNullOrWhiteSpace(name))
                item.Name = name;
        }
        return codes.ToCodes(emptyText);
    }

    internal IDictionary<string, object> InputAttributes
    {
        get
        {
            var attributes = new Dictionary<string, object>
            {
                { "id", Column.Id },
                { "autofocus", true },
                { "readonly", IsReadOnly },
                { "required", Column.Required },
                { "placeholder", Column.Placeholder }
            };
            if (Column.Type != FieldType.Date && Column.Type != FieldType.DateTime)
                attributes["disabled"] = IsReadOnly;
            UI.AddInputAttributes(attributes, this);

            var expression = InputExpression.Create(this);
            attributes["Value"] = Value;
            if (!IsReadOnly)
                attributes["ValueChanged"] = expression?.ValueChanged;
            attributes["ValueExpression"] = expression?.ValueExpression;
            return attributes;
        }
    }

    internal DateTime? ValueAsDateTime => Value as DateTime?;
    internal int? ValueAsInt => Value as int?;
    internal decimal? ValueAsDecimal => Value as decimal?;
    internal bool ValueAsBool => (bool)Value;
    internal string ValueAsString => Value as string;

    private static object GetDictionaryValue(ColumnInfo column, Dictionary<string, object> data)
    {
        if (data == null)
            return null;

        data.TryGetValue(column.Id, out object value);
        switch (column.Type)
        {
            case FieldType.Date:
            case FieldType.DateTime:
                return Utils.ConvertTo<DateTime?>(value);
            case FieldType.Number:
                return Utils.ConvertTo<decimal?>(value);
            case FieldType.Switch:
            case FieldType.CheckBox:
                return Utils.ConvertTo<bool>(value);
            default:
                return value?.ToString();
        }
    }

    private static Type GetPropertyType(ColumnInfo column, object value)
    {
        switch (column.Type)
        {
            case FieldType.Date:
            case FieldType.DateTime:
                return value != null ? value.GetType() : typeof(DateTime?);
            case FieldType.Number:
                return value != null ? value.GetType() : typeof(decimal?);
            case FieldType.Switch:
            case FieldType.CheckBox:
                return typeof(bool);
            case FieldType.CheckList:
                return typeof(string[]);
            default:
                return typeof(string);
        }
    }
}

record InputExpression(LambdaExpression ValueExpression, object ValueChanged)
{
    private static readonly MethodInfo EventCallbackFactory = GetEventCallbackFactory();

    private static MethodInfo GetEventCallbackFactory()
    {
        return typeof(EventCallbackFactory)
            .GetMethods()
            .Single(m =>
            {
                if (m.Name != "Create" || !m.IsPublic || m.IsStatic || !m.IsGenericMethod)
                    return false;

                var generic = m.GetGenericArguments();
                if (generic.Length != 1)
                    return false;

                var args = m.GetParameters();
                return args.Length == 2 &&
                       args[0].ParameterType == typeof(object) &&
                       args[1].ParameterType.IsGenericType &&
                       args[1].ParameterType.GetGenericTypeDefinition() == typeof(Action<>);
            });
    }

    public static InputExpression Create<TItem>(FieldModel<TItem> model) where TItem : class, new()
    {
        var propertyType = model.GetPropertyType();
        if (propertyType == null)
            return null;

        LambdaExpression lambda = null;
        if (model.Form.IsDictionary)
        {
            try
            {
                MemberExpression access = null;
                switch (model.Column.Type)
                {
                    case FieldType.Date:
                    case FieldType.DateTime:
                        access = Expression.Property(Expression.Constant(model), nameof(model.ValueAsDateTime));
                        break;
                    case FieldType.Number:
                        access = Expression.Property(Expression.Constant(model), nameof(model.ValueAsDecimal));
                        break;
                    case FieldType.Switch:
                    case FieldType.CheckBox:
                        access = Expression.Property(Expression.Constant(model), nameof(model.ValueAsBool));
                        break;
                    //case FieldType.CheckList: //return typeof(string[]);
                    default:
                        access = Expression.Property(Expression.Constant(model), nameof(model.ValueAsString));
                        break;
                }
                lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(propertyType), access);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
            }
        }
        else if (model.Property != null)
        {
            // () => Owner.Property
            var access = Expression.Property(Expression.Constant(model.Form.Data, typeof(TItem)), model.Property);
            lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(propertyType), access);
        }

        // Create(object receiver, Action<object>) callback
        var method = EventCallbackFactory.MakeGenericMethod(propertyType);

        // value => Field.Value = value;
        var changeHandlerParameter = Expression.Parameter(propertyType);

        var body = Expression.Assign(
            Expression.Property(Expression.Constant(model), nameof(model.Value)),
            Expression.Convert(changeHandlerParameter, typeof(object)));

        var changeHandlerLambda = Expression.Lambda(
            typeof(Action<>).MakeGenericType(propertyType),
            body,
            changeHandlerParameter);

        var changeHandler = method.Invoke(
            EventCallback.Factory,
            [model, changeHandlerLambda.Compile()]);

        return new InputExpression(lambda, changeHandler);
    }
}