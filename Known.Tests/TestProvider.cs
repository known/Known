using System;
using System.Collections.Generic;
using System.Data;
using Known.Data;
using Known.Mapping;

namespace Known.Tests
{
    class TestDbProvider : IDbProvider
    {
        public string ProviderName { get; }
        public string ConnectionString { get; }

        public void BeginTrans()
        {
        }

        public void Commit()
        {
        }

        public void Rollback()
        {
        }

        public void Execute(Command command)
        {
        }

        public object Scalar(Command command)
        {
            return "";
        }

        public DataTable Query(Command command)
        {
            return new DataTable();
        }

        public void WriteTable(DataTable table)
        {
        }

        public void Dispose()
        {
        }
    }

    class TestRepository : IRepository
    {
        public TestRepository() { }

        public TestRepository(Database database)
        {
        }

        public T QueryById<T>(string id) where T : EntityBase
        {
            return default(T);
        }

        public List<T> QueryList<T>() where T : EntityBase
        {
            return new List<T>();
        }

        public List<T> QueryListById<T>(string[] ids) where T : EntityBase
        {
            return new List<T>();
        }

        public void Save<T>(T entity) where T : EntityBase
        {
        }

        public void Update<T>(T entity) where T : EntityBase
        {
        }

        public void Delete<T>(T entity) where T : EntityBase
        {
        }

        public Result Transaction(Action<IRepository> action, object data = null)
        {
            return Result.Success("操作成功！", data);
        }
    }
}
