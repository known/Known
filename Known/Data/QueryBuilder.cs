namespace Known.Data;

public class QueryBuilder<T>
{
    private readonly Database db;
    private readonly SqlBuilder builder;
    private readonly List<string> selects = [];
    private readonly List<string> groupBys = [];
    private readonly List<string> orderBys = [];

    internal QueryBuilder(Database db) : this(db.Builder)
    {
        this.db = db;
    }

    internal QueryBuilder(SqlBuilder builder)
    {
        this.builder = builder;
        orderBys = [];
        WhereSql = string.Empty;
        Parameters = [];
    }

    internal string SelectSql => string.Join(",", selects);
    internal string WhereSql { get; private set; }
    internal string GroupSql => string.Join(",", groupBys);
    internal string OrderSql => string.Join(",", orderBys);
    internal Dictionary<string, object> Parameters { get; private set; }

    public QueryBuilder<T> Select(params Expression<Func<T, object>>[] selectors)
    {
        foreach (var selector in selectors)
        {
            var pi = TypeHelper.Property(selector);
            var name = builder.FormatName(pi.Name);
            selects.Add(name);
        }
        return this;
    }

    public QueryBuilder<T> Select(Expression<Func<T, object>> selector, string nameAs)
    {
        var pi = TypeHelper.Property(selector);
        var name = builder.FormatName(pi.Name);
        var asName = builder.FormatName(nameAs);
        selects.Add($"{name} as {asName}");
        return this;
    }

    public QueryBuilder<T> SelectCount(string nameAs = null)
    {
        if (string.IsNullOrWhiteSpace(nameAs))
        {
            selects.Add("count(*)");
        }
        else
        {
            var asName = builder.FormatName(nameAs);
            selects.Add($"count(*) as {asName}");
        }
        return this;
    }

    public QueryBuilder<T> Where(Expression<Func<T, bool>> expression)
    {
        WhereSql = string.Empty;
        if (expression.Body is BinaryExpression be)
            RouteExpression(be.Left, be.Right, be.NodeType);
        else
            RouteExpression(expression.Body);
        return this;
    }

    public QueryBuilder<T> GroupBy(params Expression<Func<T, object>>[] selectors)
    {
        foreach (var selector in selectors)
        {
            var pi = TypeHelper.Property(selector);
            var name = builder.FormatName(pi.Name);
            groupBys.Add(name);
        }
        return this;
    }

    public QueryBuilder<T> OrderBy(params Expression<Func<T, object>>[] selectors)
    {
        foreach (var selector in selectors)
        {
            var pi = TypeHelper.Property(selector);
            var name = builder.FormatName(pi.Name);
            orderBys.Add(name);
        }
        return this;
    }

    public QueryBuilder<T> OrderByDescending(params Expression<Func<T, object>>[] selectors)
    {
        foreach (var selector in selectors)
        {
            var pi = TypeHelper.Property(selector);
            var name = builder.FormatName(pi.Name);
            orderBys.Add($"{name} desc");
        }
        return this;
    }

    public async Task<int> CountAsync()
    {
        var info = builder.GetCountCommand(this);
        return await db.ScalarAsync<int>(info);
    }

    public async Task<bool> ExistsAsync()
    {
        var info = builder.GetCountCommand(this);
        return await db.ScalarAsync<int>(info) > 0;
    }

    public Task<T> FirstAsync()
    {
        var info = builder.GetSelectCommand(this);
        return db.QueryAsync<T>(info);
    }

    public Task<TItem> FirstAsync<TItem>()
    {
        var info = builder.GetSelectCommand(this);
        return db.QueryAsync<TItem>(info);
    }

    public Task<List<T>> ToListAsync()
    {
        var info = builder.GetSelectCommand(this);
        return db.QueryListAsync<T>(info);
    }

    public Task<List<TItem>> ToListAsync<TItem>()
    {
        var info = builder.GetSelectCommand(this);
        return db.QueryListAsync<TItem>(info);
    }

    private void RouteExpression(Expression left, Expression right, ExpressionType type)
    {
        var field = RouteExpression(left);
        if (field != null)
            WhereSql += builder.FormatName($"{field}");
        WhereSql += ExpressionHelper.TypeCast(type);
        var value = RouteExpression(right);
        if (value == null)
        {
            if (WhereSql.EndsWith("="))
                WhereSql = WhereSql[..^1] + " is null";
            else if (WhereSql.EndsWith("<>"))
                WhereSql = WhereSql[..^2] + " is not null";
        }
        else if (field != null)
        {
            WhereSql += $"@{field}";
            Parameters[$"{field}"] = value;
        }
    }

    private object RouteExpression(Expression exp, object fieldName = null)
    {
        if (exp is NewArrayExpression ae)
            return RouteExpression(ae);

        if (exp is ConstantExpression ce)
            return ce.Value;

        if (exp is MemberExpression me) //d.Enabled
        {
            if (!exp.ToString().StartsWith("value"))
                return RouteExpression(me);
            return RouteExpressionValue(exp, fieldName);
        }

        if (exp is BinaryExpression be)
        {
            RouteExpression(be.Left, be.Right, be.NodeType);
        }
        else if (exp is MethodCallExpression mce)
        {
            return RouteExpression(mce);
        }
        else if (exp is UnaryExpression ue) //!d.Enabled  ue.NodeType=Not  ue.Operand =d.Enabled
        {
            if (ue.NodeType == ExpressionType.Not)
                WhereSql += "Not";
            RouteExpression(ue.Operand);
        }
        return null;
    }

    private string RouteExpression(NewArrayExpression ae)
    {
        var sb = new StringBuilder();
        //var index = 0;
        foreach (Expression ex in ae.Expressions)
        {
            //var key = $"{fieldName}{index++}";
            //Parameters[key] = ExpressionRouter(ex);
            //sb.Append($"@{key},");
            sb.Append(RouteExpression(ex));
            sb.Append(',');
        }
        return sb.ToString(0, sb.Length - 1);
    }

    private object RouteExpression(MethodCallExpression mce)
    {
        if (mce.Method.Name == nameof(string.Contains))
            return SetLikeWhere(mce, "%{0}%");
        else if (mce.Method.Name == nameof(string.StartsWith))
            return SetLikeWhere(mce, "{0}%");
        else if (mce.Method.Name == nameof(string.EndsWith))
            return SetLikeWhere(mce, "%{0}");
        else if (mce.Method.Name == nameof(WhereExtension.In))
            return SetArrayWhere(mce, "in");
        else if (mce.Method.Name == nameof(WhereExtension.NotIn))
            return SetArrayWhere(mce, "not in");
        else if (mce.Method.Name == nameof(WhereExtension.Between))
            return SetArrayWhere(mce, "between");
        else
            return RouteExpressionValue(mce);
    }

    private string RouteExpression(MemberExpression me)
    {
        if (WhereSql.EndsWith("Not"))
        {
            WhereSql = WhereSql[..^3] + builder.FormatName(me.Member.Name) + "='False'";
            return null;
        }
        else if (me.Member.ToString().Contains("Boolean"))
        {
            WhereSql = WhereSql + builder.FormatName(me.Member.Name) + "='True'";
            return null;
        }
        return me.Member.Name;
    }

    private object RouteExpressionValue(Expression exp, object fieldName = null)
    {
        var value = Expression.Lambda(exp).Compile().DynamicInvoke();
        if (value == null)
            return null;

        if (value is int[] ints)
            return GetArrayWhere(fieldName, ints);

        if (value is string[] values)
            return GetArrayWhere(fieldName, values);

        return value;
    }

    private object GetArrayWhere<TItem>(object fieldName, TItem[] values)
    {
        var sb = new StringBuilder();
        var index = 0;
        foreach (var item in values)
        {
            var key = $"{fieldName}{index++}";
            Parameters[key] = item;
            sb.Append($"@{key},");
        }
        return sb.ToString(0, sb.Length - 1);
    }

    private object SetLikeWhere(MethodCallExpression mce, string format)
    {
        var isNot = WhereSql.EndsWith("Not");
        if (isNot)
            WhereSql = WhereSql[..^3];
        if (mce.Object == null)
        {
            var field = RouteExpression(mce.Arguments[0]);
            var fieldName = builder.FormatName($"{field}");
            var arg1 = RouteExpression(mce.Arguments[1]);
            var operate = isNot ? "not in" : "in";
            WhereSql += $"{fieldName} {operate} ({arg1})";
        }
        else if (mce.Object.NodeType == ExpressionType.MemberAccess)
        {
            var field = RouteExpression(mce.Object);
            var fieldName = builder.FormatName($"{field}");
            var value = RouteExpression(mce.Arguments[0]);
            Parameters[$"{field}"] = string.Format(format, value);
            var operate = isNot ? "not like" : "like";
            WhereSql += $"{fieldName} {operate} @{field}";
        }
        return null;
    }

    private object SetArrayWhere(MethodCallExpression mce, string format)
    {
        if (mce.Object == null)
        {
            var field = RouteExpression(mce.Arguments[0]);
            var fieldName = builder.FormatName($"{field}");
            if (format == "between")
            {
                var arg1 = RouteExpression(mce.Arguments[1]);
                var arg2 = RouteExpression(mce.Arguments[2]);
                if (arg1 is DateTime && db.DatabaseType == DatabaseType.Access)
                    WhereSql += $"{fieldName} between #{arg1}# and #{arg2}#";
                else
                    WhereSql += $"{fieldName} between '{arg1}' and '{arg2}'";
            }
            else
            {
                var arg1 = RouteExpression(mce.Arguments[1], field);
                WhereSql += $"{fieldName} {format} ({arg1})";
            }
        }
        else if (mce.Object.NodeType == ExpressionType.MemberAccess)
        {
            var field = RouteExpression(mce.Object);
            var fieldName = builder.FormatName($"{field}");
            var value = RouteExpression(mce.Arguments[0]);
            Parameters[$"{field}"] = string.Format(format, value);
            WhereSql += $"{fieldName} {format} (@{field})";
        }
        return null;
    }
}