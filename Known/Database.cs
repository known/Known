using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Newtonsoft.Json;

namespace Known
{
    public class Database : IDisposable
    {
        private readonly string prefix;
        private readonly DbConnection conn;
        private DbTransaction trans;

        public Database(string name = "Default", UserInfo user = null)
        {
            var setting = ConfigurationManager.ConnectionStrings[name];
            ProviderName = setting.ProviderName;
            ConnectionString = setting.ConnectionString;
            User = user ?? UserInfo.CreateAnonymousUser();

            var factory = DbProviderFactories.GetFactory(ProviderName);
            conn = factory.CreateConnection();
            conn.ConnectionString = ConnectionString;
            prefix = ProviderName.Contains("Oracle") ? ":" : "@";
        }

        private Database(string providerName, string connectionString, UserInfo user)
        {
            ProviderName = providerName;
            ConnectionString = connectionString;
            User = user;

            var factory = DbProviderFactories.GetFactory(ProviderName);
            conn = factory.CreateConnection();
            conn.ConnectionString = ConnectionString;
            prefix = ProviderName.Contains("Oracle") ? ":" : "@";
        }

        public string ProviderName { get; }
        public string ConnectionString { get; }
        public UserInfo User { get; set; }

        public void Dispose()
        {
            if (trans != null)
                trans.Dispose();
            trans = null;

            if (conn.State != ConnectionState.Closed)
                conn.Close();
            conn.Dispose();
        }

        public Result Transaction(string name, Action<Database> action, object data = null)
        {
            using (var db = new Database(ProviderName, ConnectionString, User))
            {
                try
                {
                    db.BeginTrans();
                    action(db);
                    db.Commit();
                    return Result.Success($"{name}成功！", data);
                }
                catch
                {
                    db.Rollback();
                    throw;
                }
            }
        }

        public int Execute(string sql, object param = null)
        {
            var info = new CommandInfo(prefix, sql, param);
            return ExecuteNonQuery(info);
        }

        public T Scalar<T>(string sql, object param = null)
        {
            var cmd = conn.CreateCommand();
            var info = new CommandInfo(prefix, sql, param);
            PrepareCommand(conn, cmd, trans, info, out bool close);
            var scalar = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            if (close)
                conn.Close();

            return (T)scalar;
        }

        public T QuerySingle<T>(string sql, object param = null)
        {
            var info = new CommandInfo(prefix, sql, param);
            return QuerySingle<T>(info);
        }

        public List<T> QueryList<T>(string sql, object param = null)
        {
            var info = new CommandInfo(prefix, sql, param);
            return QueryList<T>(info);
        }

        public PagingResult<T> QueryPage<T>(string sql, PagingCriteria criteria)
        {
            conn.Open();

            var data = new List<T>();
            var cmd = conn.CreateCommand();
            var info = new CommandInfo(prefix, sql, criteria.Parameter);
            PrepareCommand(conn, cmd, trans, info, out _);
            cmd.CommandText = info.CountSql;
            var total = (int)cmd.ExecuteScalar();
            if (total > 0)
            {
                cmd.CommandText = info.GetPagingSql(ProviderName, criteria);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var obj = ConvertTo<T>(reader);
                        data.Add(obj);
                    }
                }
            }

            cmd.Parameters.Clear();
            conn.Close();

            return new PagingResult<T>
            {
                TotalCount = total,
                PageData = data
            };
        }

        public bool Exists<T>(Expression<Func<T, bool>> func) where T:EntityBase
        {
            var where = ExpressionHelper.Route(func);
            return true;
        }

        public T QueryById<T>(string id) where T : EntityBase
        {
            if (string.IsNullOrWhiteSpace(id))
                return default;

            var tableName = typeof(T).Name;
            var sql = $"select * from {tableName} where id=@id";
            return QuerySingle<T>(sql, new { id });
        }

        public List<T> QueryListById<T>(string[] ids) where T : EntityBase
        {
            if (ids == null || ids.Length == 0)
                return null;

            var idTexts = new List<string>();
            var paramters = new Dictionary<string, object>();
            for (int i = 0; i < ids.Length; i++)
            {
                idTexts.Add($"id=@id{i}");
                paramters.Add($"id{i}", ids[i]);
            }

            var tableName = typeof(T).Name;
            var idText = string.Join(" or ", idTexts);
            var sql = $"select * from {tableName} where {idText}";
            var info = new CommandInfo(prefix, sql) { Params = paramters };

            return QueryList<T>(info);
        }

        public void Delete<T>(T entity) where T : EntityBase
        {
            var info = CommandInfo.GetDeleteCommand(prefix, entity);
            ExecuteNonQuery(info);
        }

        public void Insert<T>(T entity) where T : EntityBase
        {
            if (entity == null)
                return;

            entity.Id = Utils.GetGuid();
            entity.IsNew = true;
            entity.ModifyBy = null;
            entity.ModifyTime = null;
            entity.Version = 1;
            Save(entity);
        }

        public void Save<T>(T entity) where T : EntityBase
        {
            if (entity == null)
                return;

            if (entity.IsNew)
            {
                entity.CreateBy = User.UserName;
                entity.CreateTime = DateTime.Now;
                entity.CompNo = User.CompNo;
            }
            else
            {
                entity.ModifyBy = User.UserName;
                entity.ModifyTime = DateTime.Now;
                entity.Version += 1;
            }

            var info = CommandInfo.GetSaveCommand(prefix, entity);
            ExecuteNonQuery(info);
        }

        #region Trans
        private void BeginTrans()
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            trans = conn.BeginTransaction();
        }

        private void Commit()
        {
            if (trans != null)
                trans.Commit();
        }

        private void Rollback()
        {
            if (trans != null)
                trans.Rollback();
        }
        #endregion

        #region Private
        private int ExecuteNonQuery(CommandInfo info)
        {
            var cmd = conn.CreateCommand();
            PrepareCommand(conn, cmd, trans, info, out bool close);
            var value = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            if (close)
                conn.Close();

            return value;
        }

        private DbDataReader ExecuteReader(CommandInfo info, out bool close)
        {
            close = false;
            var cmd = conn.CreateCommand();

            try
            {
                PrepareCommand(conn, cmd, trans, info, out close);
                var reader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                return reader;
            }
            catch
            {
                if (close)
                    conn.Close();
                throw;
            }
        }

        private T QuerySingle<T>(CommandInfo info)
        {
            bool close;
            T obj;

            using (var reader = ExecuteReader(info, out close))
            {
                reader.Read();
                obj = ConvertTo<T>(reader);
            }

            if (close)
                conn.Close();

            return obj;
        }

        private List<T> QueryList<T>(CommandInfo info)
        {
            bool close;
            var lists = new List<T>();

            using (var reader = ExecuteReader(info, out close))
            {
                while (reader.Read())
                {
                    var obj = ConvertTo<T>(reader);
                    lists.Add(obj);
                }
            }

            if (close)
                conn.Close();

            return lists;
        }

        private static T ConvertTo<T>(DbDataReader reader)
        {
            var dic = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i).Replace("_", "");
                dic.Add(name, reader[i]);
            }

            var json = JsonConvert.SerializeObject(dic);
            var obj = JsonConvert.DeserializeObject<T>(json);
            if (obj is EntityBase)
            {
                (obj as EntityBase).IsNew = false;
                (obj as EntityBase).Original = dic;
            }

            return obj;
        }

        private static void PrepareCommand(DbConnection conn, DbCommand cmd, DbTransaction trans, CommandInfo info, out bool close)
        {
            if (conn.State != ConnectionState.Open)
            {
                close = true;
                conn.Open();
            }
            else
            {
                close = false;
            }

            cmd.Connection = conn;
            cmd.CommandText = info.Text;

            if (trans != null)
            {
                if (trans.Connection == null)
                    throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", nameof(trans));
                cmd.Transaction = trans;
            }

            if (info.Params != null && info.Params.Count > 0)
            {
                foreach (var item in info.Params)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = $"{info.Prefix}{item.Key}";
                    p.Value = item.Value ?? DBNull.Value;
                    cmd.Parameters.Add(p);
                }
            }
        }
        #endregion
    }

    class ExpressionHelper
    {
        internal static string Route(Expression exp)
        {
            if (exp is BinaryExpression be)
            {
                return BinaryRoute(be.Left, be.Right, be.NodeType);
            }
            else if (exp is MemberExpression me)
            {
                if (!exp.ToString().StartsWith("value("))
                    return me.Member.Name;

                var result = Expression.Lambda(exp).Compile().DynamicInvoke();
                if (result == null)
                    return "null";
                if (result is ValueType)
                    return result.ToString();
                else if (result is string || result is DateTime || result is char)
                    return string.Format("'{0}'", result.ToString());
            }
            else if (exp is NewArrayExpression ae)
            {
                var tmpstr = new StringBuilder();
                foreach (Expression ex in ae.Expressions)
                {
                    tmpstr.Append(Route(ex));
                    tmpstr.Append(",");
                }
                return tmpstr.ToString(0, tmpstr.Length - 1);
            }
            else if (exp is MethodCallExpression mce)
            {
                if (mce.Method.Name == "Like")
                    return string.Format("({0} like {1})", Route(mce.Arguments[0]), Route(mce.Arguments[1]));
                else if (mce.Method.Name == "NotLike")
                    return string.Format("({0} not like {1})", Route(mce.Arguments[0]), Route(mce.Arguments[1]));
                else if (mce.Method.Name == "In")
                    return string.Format("{0} in ({1})", Route(mce.Arguments[0]), Route(mce.Arguments[1]));
                else if (mce.Method.Name == "NotIn")
                    return string.Format("{0} not In ({1})", Route(mce.Arguments[0]), Route(mce.Arguments[1]));
            }
            else if (exp is ConstantExpression ce)
            {
                if (ce.Value == null)
                    return "null";
                else if (ce.Value is ValueType)
                    return ce.Value.ToString();
                else if (ce.Value is string || ce.Value is DateTime || ce.Value is char)
                    return string.Format("'{0}'", ce.Value.ToString());
            }
            else if (exp is UnaryExpression ue)
            {
                return Route(ue.Operand);
            }
            return null;
        }

        static string BinaryRoute(Expression left, Expression right, ExpressionType type)
        {
            string sb = "(";
            //先处理左边
            sb += Route(left);
            sb += CastType(type);
            //再处理右边
            string tmpStr = Route(right);
            if (tmpStr == "null")
            {
                if (sb.EndsWith(" ="))
                    sb = sb.Substring(0, sb.Length - 2) + " is null";
                else if (sb.EndsWith("<>"))
                    sb = sb.Substring(0, sb.Length - 2) + " is not null";
            }
            else
                sb += tmpStr;
            return sb += ")";
        }

        static string CastType(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return " and ";
                case ExpressionType.Equal:
                    return " =";
                case ExpressionType.GreaterThan:
                    return " >";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return " or ";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                default:
                    return null;
            }
        }
    }

    class CommandInfo
    {
        internal CommandInfo(string prefix, string text, dynamic param = null)
        {
            Prefix = prefix;
            Text = text.Replace("@", prefix);
            if (param != null)
                Params = ToDictionary(param);
        }

        internal string Prefix { get; set; }
        internal string Text { get; set; }
        internal Dictionary<string, object> Params { get; set; }

        internal string CountSql
        {
            get { return $"select count(*) from ({Text}) t"; }
        }

        internal string GetPagingSql(string providerName, PagingCriteria criteria)
        {
            var orderBy = string.Join(",", criteria.OrderBys.Select(f => string.Format("t1.{0}", f)));
            var startNo = criteria.PageSize * (criteria.PageIndex - 1);
            var endNo = startNo + criteria.PageSize;

            if (string.IsNullOrWhiteSpace(orderBy))
            {
                orderBy = "t1.CreateTime";
            }

            if (providerName.Contains("MySql"))
            {
                return $@"
select t1.* from (
    {Text}
) t1 
order by {orderBy} 
limit {startNo}, {endNo}";
            }

            return $@"
select t.* from (
    select t1.*,row_number() over (order by {orderBy}) row_no 
    from (
        {Text}
    ) t1
) t where t.row_no>{startNo} and t.row_no<={endNo}";
        }

        internal static CommandInfo GetSaveCommand<T>(string prefix, T entity) where T : EntityBase
        {
            var cmdParams = ToDictionary(entity);
            var orgParams = ToDictionary(entity.Original);

            var sql = string.Empty;
            var tableName = typeof(T).Name;
            if (entity.IsNew)
            {
                var cloumn = string.Join(",", cmdParams.Keys);
                var value = string.Join(",", cmdParams.Keys.Select(k => $"@{k}"));
                sql = $"insert into {tableName}({cloumn}) values({value})";
            }
            else
            {
                var column = string.Join(",", cmdParams.Keys.Select(k => $"{k}=@{k}"));
                sql = $"update {tableName} set {column} where Id=@Id";
            }

            return new CommandInfo(prefix, sql) { Params = cmdParams };
        }

        internal static CommandInfo GetDeleteCommand<T>(string prefix, T entity) where T : EntityBase
        {
            var tableName = typeof(T).Name;
            var sql = $"delete from {tableName} where id=@id";
            return new CommandInfo(prefix, sql, new { id = entity.Id });
        }

        private static Dictionary<string, object> ToDictionary(object value)
        {
            if (value == null)
                return null;

            var json = JsonConvert.SerializeObject(value);
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }
    }

    public class UserInfo
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string EnglishName { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public DateTime? FirstLoginTime { get; set; }
        public string FirstLoginIP { get; set; }
        public DateTime? LastLoginTime { get; set; }
        public string LastLoginIP { get; set; }
        public string CompNo { get; set; }
        public string CompName { get; set; }
        public string DeptNo { get; set; }
        public string DeptName { get; set; }

        internal static UserInfo CreateAnonymousUser()
        {
            var userName = "Anonymous";
            return new UserInfo
            {
                Id = userName,
                UserName = userName,
                Name = "匿名用户",
                EnglishName = userName,
                CompNo = Config.AppId
            };
        }
    }

    public class Result
    {
        private Result(bool isValid, string message, object data)
        {
            IsValid = isValid;
            Message = message;
            Data = data;
        }

        public bool IsValid { get; }
        public string Message { get; }
        public object Data { get; }

        public static Result Error(string message, object data = null)
        {
            return new Result(false, message, data);
        }

        public static Result Success(string message, object data = null)
        {
            return new Result(true, message, data);
        }
    }

    public class PagingResult<T>
    {
        public int TotalCount { get; set; }
        public List<T> PageData { get; set; }
        public object Summary { get; set; }
    }

    public class PagingCriteria
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string[] OrderBys { get; set; }
        public dynamic Parameter { get; set; }
    }

    public class EntityBase
    {
        public EntityBase()
        {
            Id = Utils.GetGuid();
            CreateBy = "temp";
            CreateTime = DateTime.Now;
            Version = 1;
            IsNew = true;
        }

        internal virtual bool IsNew { get; set; }
        internal Dictionary<string, object> Original { get; set; }

        public string Id { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateTime { get; set; }
        public string ModifyBy { get; set; }
        public DateTime? ModifyTime { get; set; }
        public int Version { get; set; }
        public string Extension { get; set; }
        public string CompNo { get; set; }

        public void FillModel(dynamic model)
        {
            var properties = GetType().GetProperties();
            var pis = model.Properties();
            foreach (var pi in pis)
            {
                var name = (string)pi.Name;
                if (name == "Id")
                    continue;

                var value = (object)pi.Value.Value;
                var property = properties.FirstOrDefault(p => p.Name == name);
                if (property != null)
                {
                    value = Utils.ConvertTo(property.PropertyType, value);
                    property.SetValue(this, value);
                }
            }
        }

        public Result Validate()
        {
            return Result.Success("");
        }
    }
}