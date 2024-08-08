namespace Known.Data;

public class QueryBuilder<T>
{
    private readonly Database db;
    private readonly SqlBuilder builder;
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

    internal string WhereSql { get; private set; }
    internal string OrderSql => string.Join(",", orderBys);
    internal Dictionary<string, object> Parameters { get; private set; }

    public QueryBuilder<T> Where(Expression<Func<T, bool>> expression)
    {
        if (expression.Body is BinaryExpression be)
            ExpressionRouter(be.Left, be.Right, be.NodeType);
        else
            ExpressionRouter(expression.Body);
        return this;
    }

    public QueryBuilder<T> OrderBy(Expression<Func<T, object>> selector)
    {
        var pi = TypeHelper.Property(selector);
        var name = builder.FormatName(pi.Name);
        orderBys.Add(name);
        return this;
    }

    public QueryBuilder<T> OrderByDescending(Expression<Func<T, object>> selector)
    {
        var pi = TypeHelper.Property(selector);
        var name = builder.FormatName(pi.Name);
        orderBys.Add($"{name} desc");
        return this;
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

    public Task<List<T>> ToListAsync()
    {
        var info = builder.GetSelectCommand(this);
        return db.QueryListAsync<T>(info);
    }

    private void ExpressionRouter(Expression left, Expression right, ExpressionType type)
    {
        var field = ExpressionRouter(left);
        if (field != null)
            WhereSql += builder.FormatName($"{field}");
        WhereSql += ExpressionHelper.TypeCast(type);
        var value = ExpressionRouter(right);
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

    private object ExpressionRouter(Expression exp, object fieldName = null)
    {
        if (exp is BinaryExpression be)
        {
            ExpressionRouter(be.Left, be.Right, be.NodeType);
        }
        else if (exp is NewArrayExpression ae)
        {
            var sb = new StringBuilder();
            //var index = 0;
            foreach (Expression ex in ae.Expressions)
            {
                //var key = $"@{fieldName}{index++}";
                //Parameters[key] = ExpressionRouter(ex);
                //sb.Append($"{key},");
                sb.Append(ExpressionRouter(ex));
                sb.Append(',');
            }
            return sb.ToString(0, sb.Length - 1);
        }
        else if (exp is MethodCallExpression mce)
        {
            var field = ExpressionRouter(mce.Arguments[0]);
            var name = builder.FormatName($"{field}");
            if (mce.Method.Name == nameof(WhereExtension.Like))
                WhereSql += string.Format("{0} like {1}", name, ExpressionRouter(mce.Arguments[1]));
            else if (mce.Method.Name == nameof(WhereExtension.NotLike))
                WhereSql += string.Format("{0} not like {1}", name, ExpressionRouter(mce.Arguments[1]));
            else if (mce.Method.Name == nameof(WhereExtension.In))
                WhereSql += string.Format("{0} in ({1})", name, ExpressionRouter(mce.Arguments[1]));
            else if (mce.Method.Name == nameof(WhereExtension.NotIn))
                WhereSql += string.Format("{0} not in ({1})", name, ExpressionRouter(mce.Arguments[1]));
        }
        else if (exp is UnaryExpression ue)
        {
            //!d.Enabled
            //ue.NodeType=Not
            //ue.Operand =d.Enabled
            if (ue.NodeType == ExpressionType.Not)
                WhereSql += "Not";
            ExpressionRouter(ue.Operand);
        }
        else if (exp is MemberExpression me)
        {
            //d.Enabled
            if (!exp.ToString().StartsWith("value"))
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

            var value = Expression.Lambda(exp).Compile().DynamicInvoke();
            if (value.ToString().Contains("[]"))
            {

            }
            return value;
        }
        else if (exp is ConstantExpression ce)
        {
            return ce.Value;
        }
        return null;
    }
}