﻿using System;
using System.Collections.Generic;
using System.Data;
using Known.Mapping;

namespace Known.Data
{
    public class Database
    {
        public Database() : this("Default") { }

        public Database(string name)
        {
            Provider = new DbProvider(name);
        }

        public Database(IDbProvider provider)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        internal IDbProvider Provider { get; }
        public string ConnectionString
        {
            get { return Provider.ConnectionString; }
        }

        public string UserName { get; internal set; }

        public Result Transaction(Action<Database> action)
        {
            var db = new Database(Provider);

            try
            {
                db.Provider.BeginTrans();
                action(db);
                db.Provider.Commit();
                return Result.Success("");
            }
            catch (Exception ex)
            {
                db.Provider.Rollback();
                throw ex;
            }
            finally
            {
                db.Provider.Dispose();
            }
        }

        public void Execute(string sql, object param = null)
        {
            var command = CommandCache.GetCommand(sql, param);
            Provider.Execute(command);
        }

        public T Scalar<T>(string sql, object param = null)
        {
            var command = CommandCache.GetCommand(sql, param);
            return (T)Provider.Scalar(command);
        }

        public T QueryById<T>(string id) where T : BaseEntity
        {
            var sql = CommandCache.GetQueryByIdSql<T>();
            return Query<T>(sql, new { id });
        }

        public T Query<T>(string sql, object param = null) where T : BaseEntity
        {
            var row = QueryRow(sql, param);
            if (row == null)
                return default(T);

            return AutoMapper.GetBaseEntity<T>(row);
        }

        public List<T> QueryList<T>() where T : BaseEntity
        {
            var sql = CommandCache.GetQueryListSql<T>();
            var data = QueryTable(sql);
            return AutoMapper.GetBaseEntities<T>(data);
        }

        public List<T> QueryList<T>(string sql, object param = null) where T : BaseEntity
        {
            var data = QueryTable(sql, param);
            return AutoMapper.GetBaseEntities<T>(data);
        }

        public PagingResult<T> QueryPage<T>(string sql, PagingCriteria criteria)
        {
            var result = QueryPageTable(sql, criteria);
            if (result == null)
                return null;

            var pageData = AutoMapper.GetEntities<T>(result.PageData);
            return new PagingResult<T>(result.TotalCount, pageData);
        }

        public void Save<T>(T entity) where T : BaseEntity
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
            Provider.Execute(command);
        }

        public void Save<T>(List<T> entities) where T : BaseEntity
        {
            foreach (var entity in entities)
            {
                Save(entity);
            }
        }

        public void Delete<T>(T entity) where T : BaseEntity
        {
            var command = CommandCache.GetDeleteCommand(entity);
            Provider.Execute(command);
        }

        public void Delete<T>(List<T> entities) where T : BaseEntity
        {
            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        public void WriteTable(DataTable table)
        {
            Provider.WriteTable(table);
        }

        public DataTable QueryTable(string sql, object param = null)
        {
            var command = CommandCache.GetCommand(sql, param);
            return Provider.Query(command);
        }

        public PagingResult QueryPageTable(string sql, PagingCriteria criteria)
        {
            var cmd = CommandCache.GetCommand(sql, criteria.Parameters);
            if (cmd == null)
                return null;

            var sqlCount = CommandCache.GetCountSql(cmd.Text);
            var cmdCount = new Command(sqlCount, cmd.Parameters);
            var totalCount = (int)Provider.Scalar(cmdCount);

            var sqlPage = CommandCache.GetPagingSql(cmd.Text, criteria);
            var cmdData = new Command(sqlPage, cmd.Parameters);
            var pageData = Provider.Query(cmdData);
            return new PagingResult(totalCount, pageData);
        }

        public DataRow QueryRow(string sql, object param = null)
        {
            var command = CommandCache.GetCommand(sql, param);
            var data = Provider.Query(command);
            if (data == null || data.Rows.Count == 0)
                return null;

            return data.Rows[0];
        }
    }
}
