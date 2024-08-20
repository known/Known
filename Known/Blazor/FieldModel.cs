namespace Known.Blazor;

public class FieldModel<TItem> : BaseModel where TItem : class, new()
{
    internal FieldModel(FormModel<TItem> form, ColumnInfo column) : base(form.Context)
    {
        if (!string.IsNullOrWhiteSpace(column.Id))
            form.Fields[column.Id] = this;
        Form = form;
        Column = column;
    }

    public FormModel<TItem> Form { get; }
    public ColumnInfo Column { get; }
    public bool IsReadOnly => Form.IsView || Column.ReadOnly;
    internal PropertyInfo Property => Column.Property;

    public object Value
    {
        get
        {
            if (Form.Data == null)
                return null;

            if (Form.IsDictionary)
                return Column.GetDictionaryValue(Form.Data as Dictionary<string, object>);

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

    public Type GetPropertyType()
    {
        if (Form.IsDictionary)
        {
            var value = Value;
            return Column.GetPropertyType(value);
        }

        return Property?.PropertyType;
    }

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
            //TODO：动态数据Value表达式
            //var delegateType = typeof(Func<>).MakeGenericType(typeof(Dictionary<string, object>), propertyType);
            //var dicParam = Expression.Parameter(typeof(Dictionary<string, object>), "dic");
            //var keyParam = Expression.Parameter(typeof(string), model.Column.Id);
            //var methodCall = Expression.Call(typeof(CommonExtension), nameof(CommonExtension.GetValue), [propertyType], dicParam, keyParam);
            //lambda = Expression.Lambda(delegateType, methodCall, dicParam);
            try
            {
                //var property = Expression.Property(Expression.Constant(model), nameof(model.Value));
                //var access = Expression.Convert(property, propertyType);
                //lambda = Expression.Lambda(typeof(Func<>).MakeGenericType(propertyType), access);
                //lambda = Expression.Lambda<Func<object>>(access);
                //lambda = () => (model.Data as Dictionary<string, object>).GetValue(propertyType, model.Column.Id);
                //Console.WriteLine(lambda);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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