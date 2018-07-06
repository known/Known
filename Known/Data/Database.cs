using System;
using System.Collections.Generic;
using System.Data;
using Known.Mapping;

namespace Known.Data
{
    public class Database : IDisposable
    {
        private IDbProvider provider;
        private List<Command> commands = new List<Command>();

        public Database(IDbProvider provider)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
            ConnectionString = provider.ConnectionString;
        }

        public string ConnectionString { get; }
        public string UserName { get; internal set; }

        public void Execute(string sql, dynamic param = null)
        {
            var command = CommandCache.GetCommand(sql, param);
            commands.Add(command);
        }

        public T Scalar<T>(string sql, dynamic param = null)
        {
            var command = CommandCache.GetCommand(sql, param);
            return (T)provider.Scalar(command);
        }

        public T Query<T>(string sql, dynamic param = null) where T : EntityBase
        {
            var row = QueryRow(sql, param);
            if (row == null)
                return default(T);

            return AutoMapper.GetBaseEntity<T>(row);
        }

        public List<T> QueryList<T>(string sql, dynamic param = null) where T : EntityBase
        {
            var data = QueryTable(sql, param);
            return AutoMapper.GetBaseEntities(data);
        }

        public PagingResult<T> QueryPage<T>(string sql, PagingCriteria criteria)
        {
            var cmd = CommandCache.GetCommand(sql, criteria.Parameters);
            if (cmd == null)
                return null;

            var sqlCount = CommandCache.GetCountSql(cmd.Text);
            var cmdCount = new Command(sqlCount, cmd.Parameters);
            var totalCount = (int)provider.Scalar(cmdCount);
            
            var sqlPage = CommandCache.GetPagingSql(cmd.Text, criteria);
            var cmdData = new Command(sqlPage, cmd.Parameters);
            var data = provider.Query(cmdData);
            var pageData = AutoMapper.GetEntities<T>(data);
            return new PagingResult<T>(totalCount, pageData);
        }

        public void Save<T>(T entity) where T : EntityBase
        {
            if (entity.IsNew)
            {
                entity.CreateBy = UserName;
                entity.CreateTime = DateTime.Now;
            }
            else
            {
                entity.ModifyBy = UserName;
                entity.ModifyTime = DateTime.Now;
            }

            var command = CommandCache.GetSaveCommand(entity);
            commands.Add(command);
        }

        public void Save<T>(List<T> entities) where T : EntityBase
        {
            foreach (var entity in entities)
            {
                Save(entity);
            }
        }

        public void Delete<T>(T entity) where T : EntityBase
        {
            var command = CommandCache.GetDeleteCommand(entity);
            commands.Add(command);
        }

        public void Delete<T>(List<T> entities) where T : EntityBase
        {
            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        public void SubmitChanges()
        {
            if (commands.Count > 1)
            {
                provider.Execute(commands);
            }
            else if (commands.Count > 0)
            {
                provider.Execute(commands[0]);
            }
            commands.Clear();
        }

        public void WriteTable(DataTable table)
        {
            provider.WriteTable(table);
        }

        public DataTable QueryTable(string sql, dynamic param = null)
        {
            var command = CommandCache.GetCommand(sql, param);
            return provider.Query(command);
        }

        public DataRow QueryRow(string sql, dynamic param = null)
        {
            var command = CommandCache.GetCommand(sql, param);
            var data = provider.Query(command);
            if (data == null || data.Rows.Count == 0)
                return null;

            return data.Rows[0];
        }

        public DataTable SelectTable(string tableName, Dictionary<string, object> parameters = null)
        {
            var command = CommandCache.GetSelectCommand(tableName, parameters);
            return provider.Query(command);
        }

        public DataRow SelectRow(string tableName, Dictionary<string, object> parameters)
        {
            var data = SelectTable(tableName, parameters);
            if (data == null || data.Rows.Count == 0)
                return null;

            return data.Rows[0];
        }

        public void Insert(string tableName, Dictionary<string, object> parameters)
        {
            var command = CommandCache.GetInsertCommand(tableName, parameters);
            commands.Add(command);
        }

        public void Update(string tableName, string keyFields, Dictionary<string, object> parameters)
        {
            var command = CommandCache.GetUpdateCommand(tableName, keyFields, parameters);
            commands.Add(command);
        }

        public void Delete(string tableName, Dictionary<string, object> parameters)
        {
            var command = CommandCache.GetDeleteCommand(tableName, parameters);
            commands.Add(command);
        }

        public void Dispose()
        {
            if (commands.Count > 0)
            {
                SubmitChanges();
            }
        }
    }
}
