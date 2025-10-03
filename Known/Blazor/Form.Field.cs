namespace Known.Blazor;

/// <summary>
/// 字段组件模型类。
/// </summary>
/// <typeparam name="TItem">表单数据类型。</typeparam>
public class FieldModel<TItem> : BaseModel where TItem : class, new()
{
    internal FieldModel(FormModel<TItem> form, ColumnInfo column) : base(form.Component)
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
                return GetDictionaryValue(Form.Data as Dictionary<string, object>, Column);

            return TypeHelper.GetPropertyValue(Form.Data, Column.Id);
        }
        set
        {
            if (!Equals(Value, value) && Form.Data != null)
            {
                if (Form.IsDictionary)
                    (Form.Data as Dictionary<string, object>).SetValue(Column.Id, value);
                else if (Property?.SetMethod is not null)
                    TypeHelper.SetPropertyValue(Form.Data, Column.Id, value);
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
            return GetPropertyType(Column);

        return Property?.GetFieldPropertyType();
    }

    /// <summary>
    /// 获取下拉框代码表列表。
    /// </summary>
    /// <param name="emptyText">空值占位符提示，默认请选择。</param>
    /// <returns>代码表列表。</returns>
    public List<CodeInfo> GetCodes(string emptyText = Language.PleaseSelect)
    {
        if (!string.IsNullOrWhiteSpace(emptyText))
            emptyText = Language?[Language.PleaseSelect];

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
            else
                attributes[nameof(AntDatePicker.Disabled)] = IsReadOnly;
            UI.AddInputAttributes(attributes, this);

            if (Column.Attributes.Count > 0)
            {
                foreach (var attr in Column.Attributes)
                {
                    attributes[attr.Key] = attr.Value;
                }
            }

            attributes["Value"] = Value;
            var expression = InputExpression.Create(this);
            if (expression != null)
            {
                if (!IsReadOnly)
                    attributes["ValueChanged"] = expression.ValueChanged;
                //该表达式用于AntDesign控件验证
                attributes["ValueExpression"] = expression.ValueExpression;
            }
            return attributes;
        }
    }

    internal DateTime? ValueAsDateTime => Value as DateTime?;
    internal int? ValueAsInt => Value as int?;
    internal decimal? ValueAsDecimal => Value as decimal?;
    internal bool ValueAsBool => (bool)Value;
    internal string ValueAsString => Value as string;

    private static object GetDictionaryValue(Dictionary<string, object> data, ColumnInfo column)
    {
        if (data == null)
            return null;

        var value = data.GetValue(column.Id);
        return column.Type switch
        {
            FieldType.Date or FieldType.DateTime => Utils.ConvertTo<DateTime?>(value),
            FieldType.Integer => Utils.ConvertTo<int?>(value),
            FieldType.Number => Utils.ConvertTo<decimal?>(value),
            FieldType.Switch or FieldType.CheckBox => Utils.ConvertTo<bool>(value),
            _ => value?.ToString(),
        };
    }

    private static Type GetPropertyType(ColumnInfo column)
    {
        return column.Type switch
        {
            FieldType.Date or FieldType.DateTime => typeof(DateTime?),
            FieldType.Integer => typeof(int?),
            FieldType.Number => typeof(decimal?),
            FieldType.Switch or FieldType.CheckBox => typeof(bool),
            FieldType.CheckList => typeof(string[]),
            _ => typeof(string),
        };
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
                access = model.Column.Type switch
                {
                    FieldType.Date or FieldType.DateTime => Expression.Property(Expression.Constant(model), nameof(model.ValueAsDateTime)),
                    FieldType.Integer => Expression.Property(Expression.Constant(model), nameof(model.ValueAsInt)),
                    FieldType.Number => Expression.Property(Expression.Constant(model), nameof(model.ValueAsDecimal)),
                    FieldType.Switch or FieldType.CheckBox => Expression.Property(Expression.Constant(model), nameof(model.ValueAsBool)),
                    //case FieldType.CheckList: //return typeof(string[]);
                    _ => Expression.Property(Expression.Constant(model), nameof(model.ValueAsString)),
                };
                lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(propertyType), access);
            }
            catch //(Exception ex)
            {
                //Logger.Exception(LogTarget.FrontEnd, user, ex);
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