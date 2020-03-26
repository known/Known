using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Known
{
    public class Database : IDisposable
    {
        private readonly DbProviderFactory factory;
        private readonly DbConnection conn;
        private DbTransaction trans;

        public Database(string name = "Default")
        {
            var setting = ConfigurationManager.ConnectionStrings[name];
            factory = DbProviderFactories.GetFactory(setting.ProviderName);
            conn = factory.CreateConnection();
            conn.ConnectionString = setting.ConnectionString;
        }

        #region Trans
        private Database(DbConnection conn, string userName)
        {
            this.conn = conn;
            UserName = userName;
        }

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

        public string UserName { get; set; }

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
            var conn = factory.CreateConnection();
            conn.ConnectionString = this.conn.ConnectionString;
            using (var db = new Database(conn, UserName))
            {
                try
                {
                    db.BeginTrans();
                    action(db);
                    db.Commit();
                    return Result.Success($"{name}成功！", data);
                }
                catch (Exception ex)
                {
                    db.Rollback();
                    throw ex;
                }
            }
        }

        #region SQL
        public int Execute(string sql, object param = null)
        {
            var info = new CommandInfo(sql, param);
            return Execute(info);
        }

        public T Scalar<T>(string sql, object param = null)
        {
            var cmd = conn.CreateCommand();
            var info = new CommandInfo(sql, param);
            PrepareCommand(conn, cmd, trans, info, out bool close);
            var scalar = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            if (close)
                conn.Close();

            return Utils.ConvertTo<T>(scalar);
        }
        #endregion

        #region Type
        public T Query<T>(string sql, object param = null)
        {
            bool close;
            Dictionary<string, object> dic;
            using (var reader = Reader(sql, param, out close))
            {
                reader.Read();
                dic = ConvertToDictionary(reader);
            }

            if (close)
                conn.Close();

            return Utils.FromDictionary<T>(dic);
        }

        public List<T> QueryList<T>(string sql, object param = null)
        {
            bool close;
            var lists = new List<T>();
            using (var reader = Reader(sql, param, out close))
            {
                while (reader.Read())
                {
                    var dic = ConvertToDictionary(reader);
                    var obj = Utils.FromDictionary<T>(dic);
                    lists.Add(obj);
                }
            }

            if (close)
                conn.Close();

            return lists;
        }

        public void Save<T>(T entity) where T : EntityBase
        {
            if (entity == null)
                return;

            if (entity.IsNew)
            {
                entity.CreateBy = UserName;
                entity.CreateTime = DateTime.Now;
            }
            else
            {
                entity.ModifyBy = UserName;
                entity.ModifyTime = DateTime.Now;
                entity.Version += 1;
            }

            var info = CommandInfo.GetSaveCommand(entity);
            Execute(info);
        }
        #endregion

        #region Private
        private int Execute(CommandInfo info)
        {
            var cmd = conn.CreateCommand();
            PrepareCommand(conn, cmd, trans, info, out bool close);
            var value = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            if (close)
                conn.Close();

            return value;
        }

        private DbDataReader Reader(string sql, object param, out bool close)
        {
            close = false;
            var cmd = conn.CreateCommand();

            try
            {
                var info = new CommandInfo(sql, param);
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

        private static Dictionary<string, object> ConvertToDictionary(DbDataReader reader)
        {
            var dic = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i).Replace("_", "");
                dic.Add(name, reader[i]);
            }

            return dic;
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

            var isOracle = conn.GetType().Name.Contains("Oracle");
            var prefix = isOracle ? ":" : "@";

            cmd.Connection = conn;
            cmd.CommandText = isOracle ? info.Text.Replace("@", ":") : info.Text;

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
                    p.ParameterName = $"{prefix}{item.Key}";
                    p.Value = item.Value;
                    cmd.Parameters.Add(p);
                }
            }
        }

        private class CommandInfo
        {
            private CommandInfo() { }

            internal CommandInfo(string text, object param)
            {
                Text = text;
                if (param != null)
                    Params = Utils.ToDictionary(param);
            }

            internal string Text { get; set; }
            internal Dictionary<string, object> Params { get; set; }

            internal static CommandInfo GetSaveCommand<T>(T entity) where T : EntityBase
            {
                var info = new CommandInfo();
                info.Params = Utils.ToDictionary(entity);
                var orgParams = Utils.ToDictionary(entity.Original);

                if (entity.IsNew)
                {

                }
                else
                {

                }

                return info;
            }
        }
        #endregion
    }
}